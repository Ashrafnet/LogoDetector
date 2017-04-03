using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetector
{
   public  class ProcessedItems:IDisposable
    {

        SQLiteConnection conn = null;
        SQLiteTable tb = null;
        public string DbFileName { get
            {
               return  conn.FileName;
            }
        }
        public ProcessedItems(bool  CreateNewDB)
        {
            var db_path = Path.GetTempFileName();
            conn = new SQLiteConnection(string.Format("data source={0}; New=True", db_path));
            conn.Open();
            CreateTable();

        }
       
       private  void _OpenDBConn()
        {
            try
            {
                if (conn.State != System.Data.ConnectionState.Open )
                    conn.Open();
            }
            catch (Exception er)
            {

               
            }
        }
        private void  CreateTable()
        {
            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                cmd.Connection = conn;
                _OpenDBConn();

                SQLiteHelper sh = new SQLiteHelper(cmd);


                

                tb = new SQLiteTable("ProcessedItems");

                tb.Columns.Add(new SQLiteColumn("id", true));
                tb.Columns.Add(new SQLiteColumn("ImagePath"));
                tb.Columns.Add(new SQLiteColumn("HasLogo", ColType.Integer));
                tb.Columns.Add(new SQLiteColumn("Confidence", ColType.Integer));
                tb.Columns.Add(new SQLiteColumn("ConfusedImage", ColType.Integer));
                tb.Columns.Add(new SQLiteColumn("Error"));
                tb.Columns.Add(new SQLiteColumn("ProcessingTime", ColType.Decimal));
                

                sh.CreateTable(tb);
              

            }

        }

      
        /// <summary>
        /// Insert processedimage to db and get its ID
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public  long  InsertItemToSqlite(ImageLogoInfo info)
        {

            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                cmd.Connection = conn;
                _OpenDBConn();

                SQLiteHelper sh = new SQLiteHelper(cmd);
                
                var dic = new Dictionary<string, object>();
                dic["ImagePath"] = info.ImagePath;
                dic["HasLogo"] = info.HasLogo ?1:0;
                dic["Confidence"] = info.Confidence ;
                dic["ConfusedImage"] = info.ConfusedImage ?1:0;
                dic["Error"] = info.Error?.Message ;
                dic["ProcessingTime"] = info.ProcessingTime ;


                sh.Insert(tb.TableName, dic);
               return  sh.LastInsertRowId();

            }

        }

  internal List<ImageLogoInfo> FindAll(bool haslogo, bool hasnologo, bool hasconfusedimages, bool  hasError)
        {
            
            List<ImageLogoInfo> items = new List<ImageLogoInfo>();
            if (!haslogo && !hasnologo & !hasError)
                return items;
            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                cmd.Connection = conn;
                _OpenDBConn();

                SQLiteHelper sh = new SQLiteHelper(cmd);


                var sql = "select * from " + tb.TableName + " where ";
                if (hasError)
                    sql += " Error is not null";
                else
                    sql += " Error is  null";

                if (!haslogo || !hasnologo)
                {
                    if (hasError && haslogo)
                        sql += " or HasLogo=1 ";
                    else if (!hasError && hasnologo)
                        sql += " and HasLogo=0 ";

                    if (hasError && hasnologo)
                        sql += " or HasLogo=0 ";
                     else if (!hasError && hasnologo)
                        sql += " and HasLogo=0 ";

                }
                else if (hasError && haslogo && hasnologo)
                    sql += " or HasLogo=0 or haslogo=1 ";
                else if (haslogo && hasnologo)
                    sql += " and (HasLogo=0 or haslogo=1 )";





                var dt= sh.Select(sql );
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var info = new ImageLogoInfo
                        {
                            Confidence = int.Parse(row["Confidence"] + ""), 
                            ImagePath = row["ImagePath"] + "",
                            ProcessingTime = long.Parse(row["ProcessingTime"] + ""),
                            HasLogo = row["HasLogo"].ToString() == "1" ? true : false
                        };
                        if (info.ConfusedImage && !hasconfusedimages) continue;
                        if (!string.IsNullOrWhiteSpace(row["Error"] + ""))
                            info.Error = new Exception(row["Error"] + "");
                        items.Add(info);
                    }

                }

                return items;
            }
        }
        public void Dispose()
        {
            if (conn != null)
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    var fname = DbFileName;
                    conn.Close();
                    if (File.Exists(fname))
                        File.Delete(fname);
                    
                }
                conn.Dispose();
            }
        }

      
    }
}
