using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dapper;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageSimilarFinder
{
    public partial class ReviewImageSimilarity : Form
    {
        public static Func<DbConnection> ConnectionFactory = () => new SqlConnection(ConnectionString.GetConnectionString());
      // static  string  _table_name = "[ManaulReview_DataSet_20170719]";
        public static string _table_name { get {

                Properties.Settings.Default.Reload();
              var t=  Properties.Settings.Default.ManaulReview_SqlTableName;
                if (string.IsNullOrWhiteSpace(t))
                    t = "ManaulReview_DataSet_20170719";

                return t;
            }
        }
        string _sql_listOrg_images = $"select [ID],[Cropped],[logo],[Costar_controlnumber],[costarpath],[xceligentpath],[Similar],[Manual_QC_Similar],[Manual_Comments],[Manual_QC_By],[DateLogged_QC] from {_table_name}";
        string _sql_listCostar_images = $@"select [costarpath],
                                            count(0) total,
                                            (select count(0) from {_table_name} where x.[costarpath]=[costarpath] and [DateLogged_QC] is not null) processed
                                             from {_table_name} x
                                               %where%-- where [DateLogged_QC] is null
                                                 group by[costarpath]
                                                 order by [costarpath]";

        string _sql_update_comment = $@"update {_table_name} set [Manual_Comments]=@Manual_Comments , [DateLogged_QC]=getdate() ,[Manual_QC_By]=@Manual_QC_By where [id]=@id ";
        string _sql_update_match = $@"update {_table_name} set  [Manual_QC_Similar]=@Manual_QC_Similar , [DateLogged_QC]=getdate() ,[Manual_QC_By]=@Manual_QC_By where [id]=@id ";
        string _sql_update_match_crooped = $@"update {_table_name} set [Cropped]=@Cropped, [DateLogged_QC]=getdate() ,[Manual_QC_By]=@Manual_QC_By where [id]=@id ";
        string _sql_update_match_logo = $@"update {_table_name} set [logo]=@Logo , [DateLogged_QC]=getdate() ,[Manual_QC_By]=@Manual_QC_By where [id]=@id ";

        public ReviewImageSimilarity()
        {
            InitializeComponent();
        }

        private void ReviewImageSimilarity_Load(object sender, EventArgs e)
        {
            try
            {
                Text += " v" + Application.ProductVersion;
                Icon =Program. GetExecutableIcon();// Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            catch (Exception er)
            {

            }
            LoadMainItems();

        }
      
        void LoadMainItems()

        {
            var sql = _sql_listCostar_images;
           sql = sql.Replace("%where%", "");
            var results= ExecuteQuery_list(sql);
            listView1.Items.Clear();
            listView1.SuspendLayout();
            foreach (var item in results)
            {
                if(_hideProcessedItems && item.total == item.processed)
                {
                    continue;
                }
                ListViewItem lvi = new ListViewItem(Path.GetFileName( item.costarpath));
                lvi.Tag = item.costarpath;
                lvi.SubItems.Add(item.total+"");
                lvi.SubItems.Add(item.processed  + "");

                if (item.total == item.processed)//full
                    lvi.ImageIndex = 0;
                else if (item.total > item.processed && item.processed > 0)
                    lvi.ImageIndex = 1;
                else
                    lvi.ImageIndex = 2;
                listView1.Items.Add(lvi);
              
            }
            listView1.ResumeLayout();
            if (listView1.Items != null && listView1.Items.Count > 0)
                listView1.Items[0].Selected = true;


        }

        void ExecuteQuery(string sql , object param = null)
        {
            using (var connection = ConnectionFactory())
            {
                connection.Open();

                var affectedRows = connection.Execute(sql, param);
                

            }
        }

        IEnumerable<reviewEntity> ExecuteQuery_list(string sql, object param = null)
        {
            using (var connection = ConnectionFactory())
            {
                connection.Open();
               
                var result = connection.Query<reviewEntity>(sql,param ); 
                return result;


            }
        }

        List<reviewEntity> results = null;
        int _page = 1;
        int _page_mx = 1;
        int _page_size = 3;
        int _items_max;
        int _items_cnt;
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0) return;
            pictureBox1.Image= loadImage(listView1.SelectedItems[0].Tag+"").Item1;
            GetNextResults(1);

            if (results == null || results.Count < 1)
            {
               // pictureBox1.Image = null;
                txtControlNumber.Text =
                txtQc_Date.Text =
                txtQC_BY.Text = "";
                return;
            }
            reviewEntity r = results[0];
            var imageloader = loadImage(r.costarpath);
            pictureBox1.Image = imageloader.Item1;
            txtControlNumber.Text = r.Costar_controlnumber;
            txtQc_Date.Text = r.DateLogged_QC.HasValue ? r.DateLogged_QC.Value + "" : "";
            txtQC_BY.Text = r.Manual_QC_By;

            updateUI_buttons();


        }

        private void GetNextResults(int page)
        {
            if (listView1.SelectedIndices.Count <= 0) return;
            var costarpath = listView1.SelectedItems[0].Tag + "";
            panel1.Controls.Clear();
            _page = page;
            var sql = _sql_listOrg_images + " where [costarpath] = @costarpath";
            if (chk_uncheckeditems.Checked)
                sql += " and [DateLogged_QC] is null";
            var items = ExecuteQuery_list(sql, new { costarpath });
            _items_max = items.Count();
           // _page_mx = (int)Math.Ceiling((double)_items_max / _page_size);
            results = items.ToList();// items.Skip((_page - 1) * _page_size)
                                    //.Take(_page_size).ToList();
          
            SuspendLayout();
            var p1 = new Point(7, 2);
            var p2 = new Point(340, 192);
            var p3 = new Point(400, 192);
            var p4 = new Point(480, 174);
            var p5 = new Point(7, 310);
            var p_width = panel1.Width;
            var _p_y = 320;
            _items_cnt = results.Count;
            foreach (var item in results)
            {
               
                // 
                // pic1
                // 
                var pic1 = new PictureBox();
                var imageloader = loadImage(item.xceligentpath);
               
                pic1.Location = p1;
                pic1.Size = new Size(320, 300);
                if (imageloader.Item2 == "error")
                {
                    ToolTip t = new ToolTip();
                    t.SetToolTip(pic1, "File does not exist or inaccessable: " + Environment.NewLine + item.xceligentpath);
                    pic1.SizeMode = PictureBoxSizeMode.CenterImage;



                }
                
                
                   
                
                else
                {
                   
                    pic1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic1.Click += this.pictureBox1_Click;
                    pic1.Cursor = Cursors.Hand;
                    ToolTip t = new ToolTip();

                    if (imageloader.Item2 == "pdf")
                    { pic1.Tag = item.xceligentpath;
                        t.SetToolTip(pic1, "Pdf Path: " + Environment.NewLine + item.xceligentpath);

                    }
                    else
                        t.SetToolTip(pic1, "Image Path: " + Environment.NewLine + item.xceligentpath);

                }
                p1.Y = p1.Y + _p_y;
                
                pic1.Image = imageloader.Item1;
                

                // 
                // chk_app1
                // 
                var chk_app1 = new CheckBox();
               
                chk_app1.AutoSize = true;
                chk_app1.Enabled = false;
                chk_app1.Location = p2;
                chk_app1.Size = new System.Drawing.Size(55, 17);
                chk_app1.TabIndex = 6;
                chk_app1.Text = "Match";
                chk_app1.UseVisualStyleBackColor = true;
                p2.Y = p2.Y + _p_y;
                chk_app1.Checked = (item.Similar + "").ToLower().Trim() == "yes" || (item.Similar + "").ToLower().Trim() == "true"  || (item.Similar + "").ToLower().Trim() == "1" ? true : false;

                // 
                // chk_qc
                // 
                var chk_qc = new CheckBox();
                chk_qc.Tag = item;
                chk_qc.AutoSize = true;
                chk_qc.Location = p3;// new System.Drawing.Point(267, 92);
                chk_qc.Size = new System.Drawing.Size(55, 17);
                chk_qc.TabIndex = 6;
                chk_qc.Text = "Match";
                chk_qc.UseVisualStyleBackColor = true;
                p3.Y = p3.Y + _p_y;
                chk_qc.Checked = item.Manual_QC_Similar.HasValue  ? item.Manual_QC_Similar.Value  : false;
             //   chk_qc.CheckedChanged += Chk_qc_CheckedChanged;

                // 
                // chk_qc [Cropped]
                // 
                var chk_Cropped = new CheckBox();
                chk_Cropped.Tag = item;
                chk_Cropped.AutoSize = true;
                chk_Cropped.Location =new Point( chk_qc.Location.X , chk_qc.Location.Y + 23) ;// new System.Drawing.Point(267, 92);
                chk_Cropped.Size = new System.Drawing.Size(55, 17);
                chk_Cropped.TabIndex = 6;
                chk_Cropped.Text = "Cropped";
                
                chk_Cropped.UseVisualStyleBackColor = true;
               // p3.Y = p3.Y + _p_y;
                chk_Cropped.Checked = item.Cropped.HasValue  ? item.Cropped.Value  : false;
              //  chk_Cropped.CheckedChanged += Chk_Cropped_CheckedChanged;

                // 
                // chk_qc [Logo]
                // 
                var chk_Logo = new CheckBox();
                chk_Logo.Tag = item;
                chk_Logo.AutoSize = true;
                chk_Logo.Location = new Point(chk_Cropped.Location.X, chk_Cropped.Location.Y + 23);// new System.Drawing.Point(267, 92);
                chk_Logo.Size = new System.Drawing.Size(55, 17);
                chk_Logo.TabIndex = 6;
                chk_Logo.Text = "Has Logo";
                chk_Logo.UseVisualStyleBackColor = true;
               // p3.Y = p3.Y + _p_y;
                chk_Logo.Checked = item.Logo.HasValue ? item.Logo.Value : false;
             //   chk_Logo.CheckedChanged += Chk_Logo_CheckedChanged;


                // 
                // txtComment1
                // 
                var txtComment = new TextBox();
              //  txtComment.Leave += TxtComment_Leave;
                txtComment.Tag = item;
                txtComment.Location = p4;// new System.Drawing.Point(364, 74);
                txtComment.Multiline = true;
                txtComment.Size = new System.Drawing.Size(p_width - 500, 40);//596  - 224
                txtComment.TabIndex = 5;
                txtComment.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top ;
                p4.Y = p4.Y + _p_y;
                txtComment.Text = item.Manual_Comments;
                if (item.DateLogged_QC .HasValue || !string.IsNullOrWhiteSpace(item.Manual_QC_By ))
                {
                    var lbl_info = new Label();
                    lbl_info.AutoSize = true;
                    if (!string.IsNullOrWhiteSpace(item.Manual_QC_By))
                        lbl_info.Text = "QC By: " + item.Manual_QC_By + " " + Environment.NewLine ;
                    if (item.DateLogged_QC.HasValue)
                        lbl_info.Text += "QC Date: " + item.DateLogged_QC.Value + " ";
                    lbl_info.ForeColor = Color.Gray;
                    lbl_info.Font = new Font(lbl_info.Font, FontStyle.Bold | FontStyle.Italic);
                    lbl_info.Location  =new Point( txtComment.Location.X , txtComment.Location.Y + 0 + txtComment.Height );
                    panel1.Controls.Add(lbl_info);

                }
                // 
                // lblLine
                // 
                var lblLine = new Label();                              
                lblLine.BackColor = System.Drawing.Color.Silver;
                lblLine.Location = p5;// new System.Drawing.Point(15, 169);
                lblLine.Size = new System.Drawing.Size(p_width - 20, 1);
                lblLine.Anchor = AnchorStyles.Left | AnchorStyles.Right| AnchorStyles.Top ;
                p5.Y = p5.Y + _p_y;





                panel1.Controls.Add(pic1);
                panel1.Controls.Add(chk_app1);
                panel1.Controls.Add(chk_qc);
                panel1.Controls.Add(chk_Cropped);
                panel1.Controls.Add(chk_Logo);
                panel1.Controls.Add(txtComment);
                panel1.Controls.Add(lblLine);

            }
            ResumeLayout();
        }

        private void Chk_qc_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk == null) return;
           // chk.CheckState 
            var item =(reviewEntity) chk.Tag ;
            ExecuteQuery(_sql_update_match, new {  Manual_QC_Similar = chk.Checked , Manual_QC_By = CurrentUserName(), ID = item.ID  });

        }
        private void Chk_Cropped_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk == null) return;
            // chk.CheckState 
            var item = (reviewEntity)chk.Tag;
            ExecuteQuery(_sql_update_match_crooped, new { Cropped = chk.Checked, Manual_QC_By = CurrentUserName(), ID = item.ID });

        }
          private void Chk_Logo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk == null) return;
           // chk.CheckState 
            var item =(reviewEntity) chk.Tag ;
            ExecuteQuery(_sql_update_match_logo, new { Logo= chk.Checked , Manual_QC_By = CurrentUserName(), ID = item.ID  });

        }

        private void TxtComment_Leave(object sender, EventArgs e)
        {
           TextBox txt = (TextBox)sender ;
            if (txt == null) return;
           // if (txt.Text == "") return;
            if (!(txt.Tag is reviewEntity)) return;
            var item = (reviewEntity) txt.Tag ;
            if ((item.Manual_Comments + "").Trim() == txt.Text.Trim())
                return;
            ExecuteQuery(_sql_update_comment, new { Manual_Comments=txt.Text , Manual_QC_By = CurrentUserName(), ID= item.ID  });
        }

        string CurrentUserName()
        {
           return  System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
        void updateUI_buttons()
        {
           // btn_save.Enabled = _items_cnt > 0;
            btn_back.Enabled = _page > 1;
            btn_next.Enabled = _page_mx > _page;
            label12.Text = $"Page {_page} of {_page_mx} ({_items_cnt} items of { _items_max})";

        
        }
        Tuple<Bitmap, string> loadImage(string imagepath)
        {
            if (string.IsNullOrWhiteSpace(imagepath)) return null;
            if (!File.Exists(imagepath)) return Tuple.Create(Properties.Resources.img_error, "error");
            if ((Path.GetExtension(imagepath) + "").Trim().ToLower() == ".pdf")
                return Tuple.Create(Properties.Resources.pdf2, "pdf");

            string[] imgExts_ppm = new string[] { ".ppm", ".pgm", ".pbm" };
            Bitmap source = null;

            if (imgExts_ppm.Contains((Path.GetExtension(imagepath) + "").ToLower()))            
                source = DmitryBrant.ImageFormats.Picture.Load(imagepath);                          
            else
                source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imagepath)));
            return Tuple.Create( source, "bitmap");

          //  return Tuple.Create(new Bitmap((Bitmap)new Mat(imagepath).Bitmap.Clone()), "bitmap");

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                var pic = ((PictureBox)sender);
                if(!string.IsNullOrWhiteSpace( pic.Tag +""))
                {
                    var pdf_path = pic.Tag + "";
                    if (File.Exists(pdf_path))
                        System.Diagnostics.Process.Start(pdf_path);
                    return;
                }
                var img = (Image)((PictureBox)sender).Image.Clone();
                Imageviewer i = new Imageviewer(new Bitmap(img));
                i.WindowState = FormWindowState.Maximized;
                i.ShowDialog();
            }
            catch (Exception errr)
            {


            }
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            _page++;
            GetNextResults(_page);
            updateUI_buttons();

        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            _page--;
            GetNextResults(_page);
            updateUI_buttons();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            foreach (var control in panel1.Controls)
            {
                var c = control is CheckBox;
                if (!c) continue;

                CheckBox chk = (CheckBox)control;
                if (!chk.Enabled) continue;
                var item = (reviewEntity)chk.Tag;
                if (item == null) continue;

                if (!item.DateLogged_QC.HasValue)
                {
                   var sql = _sql_update_match + " and DateLogged_QC is null";
                    ExecuteQuery(sql, new {Logo =false , Cropped=false ,  Manual_QC_Similar = false, Manual_QC_By = CurrentUserName(), ID = item.ID });
                }
            }


            if (listView1.SelectedItems.Count > 0)//we need to update listview values
            {
                var lvi = listView1.SelectedItems[0];
                var costarpath = lvi.Tag + "";

                var sql = _sql_listCostar_images;
                sql = sql.Replace("%where%", "  where [costarpath] =@costarpath");
                var results = ExecuteQuery_list(sql, new { costarpath = costarpath });
                if (results == null || results.Count() < 1) return;
                var item = results.FirstOrDefault();
                if (item == null) return;

                lvi.SubItems[1].Text = item.total + "";
                lvi.SubItems[2].Text = item.processed + "";

                if (item.total == item.processed)//full
                    lvi.ImageIndex = 0;
                else if (item.total > item.processed && item.processed > 0)
                    lvi.ImageIndex = 1;
                else
                    lvi.ImageIndex = 2;

            }

            var _sel_no = 0;
            if (listView1.SelectedItems == null || listView1.SelectedItems.Count < 1) // not selected
                _sel_no = 0;
            else
                _sel_no = listView1.SelectedItems[0].Index+1;

            if (_sel_no >= listView1.Items.Count) return;
            listView1.Items[_sel_no].Selected = true;
      


            updateUI_buttons();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _page_size =(int) numericUpDown1.Value;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadMainItems();
        }
        bool _hideProcessedItems = false;
        private void hideProcessedItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _hideProcessedItems = !_hideProcessedItems;
            hideProcessedItemsToolStripMenuItem.Checked = _hideProcessedItems;
            LoadMainItems();
        }

        private void chk_uncheckeditems_CheckedChanged(object sender, EventArgs e)
        {
            GetNextResults(1);
            updateUI_buttons();
        }

      
    }

    internal class reviewEntity
    {
        /*
         *   [ID] [int] IDENTITY(1,1) NOT NULL,
             [Costar_controlnumber] [varchar](50) NULL,
             [costarpath] [varchar](max) NULL,
             [xceligentpath] [varchar](500) NULL,
             [Similar] [varchar](500) NULL,
             [Manual_QC_Similar] [bit] NULL,
             [Manual_Comments] [varchar](max) NULL,
             [Manual_QC_By] [varchar](50) NULL,
             [DateLogged_QC] [datetime] NULL
         * */

        public int processed { get; set; }
        public int total { get; set; }

        public int ID { get; set; }
        public string Costar_controlnumber { get; set; }
        public string costarpath { get; set; }
        public string xceligentpath { get; set; }
        public string Similar { get; set; }

        public bool? Manual_QC_Similar { get; set; }

        public string Manual_Comments { get; set; }
        public string Manual_QC_By { get; set; }
        public DateTime? DateLogged_QC { get; set; }
        public bool? Cropped { get;  set; }
        public bool? Logo { get; set; }
    }

    public static class ConnectionString
    {
        public static string GetConnectionString()
        {
            Properties.Settings.Default.Reload();
            return Properties.Settings.Default.ConnectionString;
        }
     
    }
}
