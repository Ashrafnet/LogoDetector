using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogoDetector
{
    public partial class Form1 : Form
    {
       // List<ImageLogoInfo> processedImages = new List<ImageLogoInfo>();
       // List<ImageLogoInfo> listviewItems = new List<ImageLogoInfo>();
        string[] imgExts = new string[] { "*.jpeg", "*.jpg", "*.png", "*.BMP", "*.GIF", "*.TIFF", "*.Exif", "*.WMF", "*.EMF", "*.ppm", "*.pgm", "*.pbm" };
      

        CancellationTokenSource cancellationTokenSource;
        public Form1()
        {
            InitializeComponent();
        }
      //  double total_process_time = 0;
       // long withLogoCount = 0;
        Stopwatch processStopwatch;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (backgroundWorker1.IsBusy)
                {
                    if (MessageBox.Show(this, "Do you want to cancel the process?", "Cancel process", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                    button1.Text = "Process";
                    backgroundWorker1.CancelAsync();
                    cancellationTokenSource.Cancel();
                    return;
                }
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("You have to set the images folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }
                if (!Directory.Exists(textBox1.Text))
                {
                    MessageBox.Show("This directory is not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txt_auto_csv_file.Text) && chk_auto_csv_file.Checked)
                {
                    MessageBox.Show("You have to set the csv file to save results automatically!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_auto_csv_file.Focus();
                    txt_auto_csv_file.SelectAll();
                    return;
                }
                if (!txt_auto_csv_file.Text.ToLower().EndsWith(".csv") && chk_auto_csv_file.Checked)
                {
                    MessageBox.Show("csv file must have .csv extension !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_auto_csv_file.Focus();
                    txt_auto_csv_file.SelectAll();
                    return;
                }

                if (File.Exists(txt_auto_csv_file.Text) && chk_auto_csv_file.Checked)
                {
                    var result = MessageBox.Show(string.Format("This csv file already exist.{0}What do you want to do:{0}{0}Retry: Resume previous operation.{0}Ignore: Start over and clean csv file.{0}Abort: Cancel the operation and do nothing.{0}{0}CSV File:{0}{1}", Environment.NewLine, txt_auto_csv_file.Text), "csv file exist", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel || result == DialogResult.Abort)
                    {
                        txt_auto_csv_file.Focus();
                        txt_auto_csv_file.SelectAll();
                        return;
                    }
                    else if (result == DialogResult.Retry)
                    {
                        Stat_info = new Stat_Info();
                        previousLogs.Clear();
                        ReadPreviuseLogFile(txt_auto_csv_file.Text);
                    }
                    else if (result == DialogResult.Ignore)
                    {
                        var f = new FileInfo(txt_auto_csv_file.Text);
                        if (f.Exists)
                        {
                            File.Create(f.FullName).Dispose();//make sure can write to file
                        }
                    }
                }
                if (!File.Exists(txt_auto_csv_file.Text) && chk_auto_csv_file.Checked)
                {
                    try
                    {

                        var f = new FileInfo(txt_auto_csv_file.Text);
                        if (f.Exists)
                        {
                            File.Create(f.FullName).Dispose();//make sure can write to file
                        }
                        else
                        {
                            File.Create(f.FullName).Dispose();//make sure can write to file
                            f.Delete();
                        }
                    }
                    catch (Exception er)
                    {

                        MessageBox.Show("This csv file is not valid!" + Environment.NewLine + er.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txt_auto_csv_file.Focus();
                        txt_auto_csv_file.SelectAll();
                        return;
                    }


                }

                if (CSV_Autofile != null)
                    CSV_Autofile.Dispose();
                if (previousLogs == null || previousLogs.Count < 1)
                    Stat_info = new Stat_Info();
                button1.Text = "Stop";
                buttonPause.Text = "Pause";
                buttonPause.Enabled = txt_auto_csv_file.ReadOnly = true;
                btn_imags_cnt.Enabled = false;
                backgroundWorker1.RunWorkerAsync(textBox1.Text);

            }
            catch (Exception er)
            {
                MessageBox.Show(er.FullErrorMessage(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        Dictionary<string, int> previousLogs = new Dictionary<string, int>();
        List<string> previousLogs_errors = new List<string>();
        Stat_Info Stat_info = new Stat_Info();
        void ReadPreviuseLogFile(string logfile)
        {
            try
            {
                SetStatusInfo("Reading previous logs..");
                Cursor = Cursors.WaitCursor;

                if (!File.Exists(logfile)) return;

                Stopwatch sw = Stopwatch.StartNew();
                
                foreach (var item in File.ReadLines(logfile))
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    if (!item.EndsWith(","))//failed image
                    {
                        if (item.EndsWith("Error")) continue;
                        if (item.EndsWith("Error reading the stream! ")) continue;
                        if (item.EndsWith("Binary .pbm (Magic Number P4) is not supported at this time.")) continue;
                        if (item.StartsWith("Parameter name: chars")) continue;
                        if (item.StartsWith("The output char buffer is too small to contain the decoded characters, ")) continue;

                        previousLogs_errors.Add(item);
                        Stat_info.Failed_logos++;
                        Calc_Groups_Counts();
                        continue; 
                    }
                    var i = item.Split(',');
                    if (string.IsNullOrWhiteSpace(i[3] + "")) continue;
                    if (previousLogs.ContainsKey(i[0] + "")) continue;
                    int? confidence = (i[3] + "").Replace("%","").ToIntOrNull();
                    if (!confidence.HasValue ) continue;
                    previousLogs.Add(i[0] + "", confidence.Value );
                    if (confidence.Value  > 50)
                        Stat_info.Has_Logos++;
                    else if (confidence.Value  > 45 && confidence.Value  < 50)
                        Stat_info.Confused_Logos++;
                    else if (confidence.Value  < 50)
                        Stat_info.Has_noLogos++;
                }
                sw.Stop();
                SetStatusInfo("Finished read previous logs in " + sw.ElapsedMilliseconds + " Milliseconds");
            }
           
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        bool processPaused;
        private void buttonPause_Click(object sender, EventArgs e)
        {
            processPaused = !processPaused;
            btn_imags_cnt.Enabled =  processPaused;
            buttonPause.Text = processPaused ? "Resume" : "Pause";
            if (processPaused) processStopwatch.Stop();
            else processStopwatch.Start();
        }
        StreamWriter CSV_Autofile = null;
        string  auto_csv_file = "";
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //processedImages.Clear();
            string folderPath = e.Argument+"";
           
           
            processPaused = false;
            processStopwatch = Stopwatch.StartNew();
            cancellationTokenSource = new CancellationTokenSource();
           // bool export_csv_auto = chk_auto_csv_file.Checked;
             auto_csv_file = txt_auto_csv_file.Text;
           // if (export_csv_auto)
           // {
                if (CSV_Autofile != null)
                    CSV_Autofile.Dispose();
                CSV_Autofile = new StreamWriter(auto_csv_file,previousLogs.Count >0);
            if (previousLogs.Count == 0)
                WriteToCSV_Auto(CSV_Autofile, "Image Path,Has Logo,Processing Time,Confidence,Error");
            //}
            Parallel.ForEach(MyDirectory.GetFiles(folderPath, imgExts, previousLogs, SearchOption.AllDirectories), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = cancellationTokenSource.Token }, (item) =>
            {
                try
                {


                    if (backgroundWorker1.CancellationPending)
                        return;
                    while (processPaused && !backgroundWorker1.CancellationPending)
                        Thread.Sleep(1000);
                    var info = ImageLogoInfo.ProccessImage(item, false);
                    Stat_info.Total_process_time += info.ProcessingTime;




                   // lock (processedImages) processedImages.Add(info);
                   WriteToCSV_Auto(CSV_Autofile, info.ImagePath + "," + (info.ConfusedImage == true ? "Maybe" : info.HasLogo + "") + "," + info.ProcessingTime + "ms," + info.Confidence + "%" + "," + info.Error);
                    
                    if(!string.IsNullOrWhiteSpace(info.Error) )
                        Stat_info.Failed_logos++;
                    if (info.HasLogo)
                        Stat_info.Has_Logos++;
                    else if (info.ConfusedImage)
                        Stat_info.Confused_Logos++;
                    else if (!info.HasLogo)
                        Stat_info.Has_noLogos++;
                }
                catch (Exception er)
                {
                    Stat_info.Failed_logos++;
                    WriteToCSV_Auto(CSV_Autofile, "{ERROR},,,," + er.FullErrorMessage());

                }
            });


        }
        void WriteToCSV_Auto(StreamWriter outfile, string csv_row)
        {
            try
            {

                lock (outfile)
                {

                    outfile.WriteLine(csv_row);
                }

                //outfile.Flush(); 
            }
            catch (Exception er)
            {
                try
                {
                    if (outfile != null)
                        outfile.Dispose();
                    outfile = new StreamWriter(auto_csv_file, true);
                    outfile.WriteLine(csv_row + "" + er.FullErrorMessage());
                }
                catch
                {


                }

            }

        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            buttonPause.Enabled = false;
            processPaused = false;
            txt_auto_csv_file.ReadOnly = false ;
             btn_imags_cnt.Enabled= true ;
            processStopwatch.Stop();
            timerRefreshlistview_Tick(null, null);
            button1.Text = "Process";
            Calc_Groups_Counts();
            if (CSV_Autofile != null)
                CSV_Autofile.Dispose();
            if (e.Error != null&&!(e.Error is OperationCanceledException))
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Error == null)
            {

                if (chk_auto_csv_file.Checked)
                {
                    var fpath = txt_auto_csv_file.Text;
                    CSV_Autofile.Close();
                    CSV_Autofile.Dispose();
                    if (MessageBox.Show(
                            string.Format("Process completed!{0}Auto export to the following csv file was success.{0}Do you want to open the exported File?{0}{0}Exported File:{0}{1}", Environment.NewLine, fpath), "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Process.Start(fpath);
                    }
                }else

                MessageBox.Show("Process completed", "Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {/*
            var selectedIndexes = listView1.SelectedIndices;
            if (selectedIndexes.Count == 1)
            {

                var info = listviewItems[selectedIndexes[0]];
                labelFailImage.Visible = false;
                pictureBox1.Image =pictureBox2.Image = null;
                if (info == null) { }
                else if (info.Error != null)
                {
                    labelFailImage.Text = info.Error;
                    labelFailImage.Visible = true;
                }
                else
                {
                    Bitmap source =ImageLogoInfo. GetBitmap(info.ImagePath);
                    pictureBox1.Image = source;
                    if (info.ProcessedImage == null)
                    {
                          ImageLogoInfo info1 = ImageLogoInfo.ProccessImage(info.ImagePath,true );
                        pictureBox2.Image = info1.ProcessedImage ?? source.Crop(65, 65);
                    }
                }

            }
            */
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            listView1.SelectedIndices.Clear();
            timerRefreshlistview_Tick(null, null) ;
        }

        void SetStatusInfo(string info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatusInfo), info);
            }
            else
            {
                status_info.Text = info;              
                Refresh();
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {/*
            try
            {
                if (listviewItems == null || listviewItems.Count < 1)
                {
                    MessageBox.Show("No items in list to export!", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (DialogResult.OK != saveFileDialog1.ShowDialog(this))
                    return;
                Cursor = Cursors.WaitCursor;
                //  Enabled = false;
                SetStatusInfo( "Exporting..");

                statusStrip1.Refresh();
                Application.DoEvents();
                var fpath = saveFileDialog1.FileName;
                StringBuilder txt = new StringBuilder();

                txt.AppendLine("Image Path,Has Logo,Processing Time,Confidence,Error");
                using (StreamWriter outfile = new StreamWriter(fpath))
                {
                    long i = 0; long maxstep = listviewItems.Count / 50;
                    int perc = 0;
                    foreach (var item in listviewItems)
                    {
                        i++;
                        if (i % maxstep == 0 && i != 0)
                        {
                            perc++; perc++;
                            status_info.Text = "Exporting.. (" + perc + "%)";
                            Refresh();



                        }

                        txt.AppendLine(item.ImagePath + "," + (item.ConfusedImage == true ? "Maybe" : item.HasLogo + "") + "," + item.ProcessingTime + "ms," + item.Confidence + "%");
                        outfile.Write(txt.ToString());


                        txt.Clear();
                    }
                    outfile.Close();


                }
                if (MessageBox.Show(
                        string.Format("Export to the following file was success.{0}Do you want to open the exported File?{0}{0}Exported File:{0}{1}", Environment.NewLine, fpath), "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Process.Start(fpath);
                }


                // File.WriteAllText(fpath, txt.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
                status_info.Text = "Exporting finished";
                Enabled = true;
            }
            */
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           pic_haslogs.Image= imageList1.Images[0];
            pic_hasnologos.Image = imageList1.Images[1];
            pic_confusedlogos.Image = imageList1.Images[2];
            pic_failedlogos.Image = imageList1.Images[3];


#if DEBUG
#else
            textBox1.Text = "";
            txt_auto_csv_file.Text = "";
#endif
        }
        private int sortColumn = -1;

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }
            timerRefreshlistview_Tick(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("You have to set the images folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }
                if (!Directory.Exists(textBox1.Text))
                {
                    MessageBox.Show("This directory is not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }
                Cursor = Cursors.WaitCursor;
                Stopwatch sw = Stopwatch.StartNew();
                SetStatusInfo("Counting images files..");
                var cnt = MyDirectory.GetFiles(textBox1.Text, imgExts, previousLogs, SearchOption.AllDirectories).LongCount(x => !string.IsNullOrWhiteSpace(x));
                sw.Stop();
                Text = sw.ElapsedMilliseconds + " ms";
                SetStatusInfo("Number of images= " + cnt + " images");
                MessageBox.Show("Number of images= " + cnt + " images" + Environment.NewLine + "Images supported are:" + Environment.NewLine + string.Join(" , ", imgExts) + "");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {/*
            var info = listviewItems[e.ItemIndex];
            var lvi = new ListViewItem(info.ImageName, info.HasLogo ? 0 : 1);

            if (info.ConfusedImage)
            {
                lvi.ForeColor = Color.Orange;
                lvi.ImageIndex = 2;
            }
            else if (info.Error!=null)
            {
                lvi.ForeColor = Color.Red;
                lvi.ImageIndex = 3;
            }
            lvi.SubItems.Add(info.ConfusedImage == true ? "Maybe" : info.HasLogo ? "Yes" : "No");
            lvi.SubItems.Add(info.ProcessingTime + " ms");
            lvi.SubItems.Add(info.Confidence + " %");
            lvi.Tag = info;
            e.Item = lvi;*/
        }

        private void timerRefreshlistview_Tick(object sender, EventArgs e)
        {

            if (!backgroundWorker1.IsBusy && sender == timerRefreshlistview) return;

            status_info.Text = Stat_info.TotalImages + " Items";
            if (processStopwatch != null)
                stat_time.Text = processStopwatch.Elapsed.TotalSeconds + " Seconds" + " [Total Process Time: " + Stat_info.Total_process_time / 1000 + " Seconds]" + " (" + Stat_info.TotalImages  + " Items, True=" + Stat_info.Has_Logos + " False=" + (Stat_info.Has_noLogos + Stat_info.Failed_logos + Stat_info.Confused_Logos) + ")";
            if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                status_info.Text += " (User canceled the process)";
            else if (!backgroundWorker1.IsBusy)
                status_info.Text += " (Process completed)";

           /* var items = processedImages.FindAll(info => ((checkBox1.Checked && info.HasLogo && info.Error == null) || (checkBox2.Checked && !info.HasLogo && !info.ConfusedImage && info.Error == null) || (checkBox3.Checked && info.ConfusedImage && info.Error == null) || (checkBoxShowErrors.Checked && info.Error != null)));

            if (sortColumn != -1 && listView1.Sorting != SortOrder.None)
                items.Sort(new ListViewItemComparer(sortColumn, listView1.Sorting));

            listviewItems = items;
            listView1.VirtualListSize = listviewItems.Count;
            buttonCopyImages.Enabled = buttonExportMatches.Enabled = listviewItems != null && listviewItems.Count > 0;
            */
            Calc_Groups_Counts();

        }

        private void Calc_Groups_Counts()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Calc_Groups_Counts));
            }
            else
            {
                

                lbl_HasLogo.Text = "Images with logo (" + Stat_info.Has_Logos + ")";
                lbl_hasnologos.Text = "Images with No logo (" + Stat_info.Has_noLogos + ")";
                lbl_confusedlogos.Text = "Confused images (" + Stat_info.Confused_Logos + ")";
                lbl_failedlogos.Text = "Failed images (" + Stat_info.Failed_logos + ")";
                Refresh();
            }
            /*
            try
            {

          
            lock (processedImages)
            {


                var g = processedImages.GroupBy(x => x.Status).Select(group => new
                {
                    Status = group.Key,
                    Count = group.Count()
                }).OrderBy(x => x.Status).ToList();
                if (g != null)
                {
                    if (g.Count > 0)
                    {
                        var haslogo_cnt = g.FirstOrDefault(x => x.Status == "Has Logo");
                        var hasnologo_cnt = g.FirstOrDefault(x => x.Status == "Has No Logo");
                        var maybe_cnt = g.FirstOrDefault(x => x.Status == "Maybe");
                        var error_cnt = g.FirstOrDefault(x => x.Status == "Error");


                        if (haslogo_cnt != null)
                            checkBox1.Text = "Show images with logo (" + haslogo_cnt.Count + ")";

                        if (hasnologo_cnt != null)
                            checkBox2.Text = "Show images without logo (" + hasnologo_cnt.Count + ")";

                        if (maybe_cnt != null)
                            checkBox3.Text = "Show confused images (" + maybe_cnt.Count + ")";

                        if (error_cnt != null)
                            checkBoxShowErrors.Text = "Show failed images (" + error_cnt.Count + ")";
                    }
                }
            }
            }
            catch
            {
                
            }*/
        }

        private void buttonCopyImages_Click(object sender, EventArgs e)
        {/*
            if(listviewItems==null || listviewItems.Count <1)
            {
                MessageBox.Show("No items in list to copy!", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (DialogResult.OK != folderBrowserDialog1.ShowDialog(this))
                return;
            var files = listviewItems.ConvertAll(c => c.ImagePath);
            new CopyFiles.CopyFiles(files, folderBrowserDialog1.SelectedPath).CopyAsync(new CopyFiles.DIA_CopyFiles() {  SynchronizationObject=this});
            */
        }

        private void chk_auto_csv_file_CheckedChanged(object sender, EventArgs e)
        {
            txt_auto_csv_file.Enabled = chk_auto_csv_file.Checked;
            textBox1_TextChanged(sender, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_auto_csv_file.Text) || !txt_auto_csv_file.Text.ToLower().EndsWith(".csv"))
                txt_auto_csv_file.Text = Path.Combine(textBox1.Text, "Results.csv");
            try
            {
                if (!File.Exists(txt_auto_csv_file.Text))
                    File.CreateText(txt_auto_csv_file.Text).Close();
            }
            catch (Exception er)
            {
                txt_auto_csv_file.Text = Path.Combine(textBox1.Text, "Results.csv");
              
            }

         
        }

        private void lbl_HasLogo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if((((LinkLabel)sender).Tag +"") == "failed")
            {
                StringBuilder str_err_items = new StringBuilder();
                foreach (var item in previousLogs_errors)
                {
                    str_err_items.AppendLine(item);
                }
                frmTextViewer frmt = new frmTextViewer(str_err_items.ToString());
                frmt.ShowDialog();
                return;
            }
            MessageBox.Show("Images logs and details are under construction!", "Under Construction!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btn_Pre_logs_Click(object sender, EventArgs e)
        {
            if(previousLogs !=null && previousLogs.Count > 0) // clear
            {
               if( MessageBox.Show("Are you sure you want to clear current Stats from memory","Clear Memory!", MessageBoxButtons.YesNo , MessageBoxIcon.Question)== DialogResult.Yes)
                {
                    previousLogs.Clear();
                    previousLogs_errors.Clear();
                    Stat_info = new Stat_Info();
                    Calc_Groups_Counts();
                }
                return;
            }

            var file_csv = txt_auto_csv_file.Text;
            if (File.Exists(file_csv))
            {
                if (MessageBox.Show(string.Format("Are you sure you want to read previous logs from this csv file.{0}{0}CSV File:{0}{1}", Environment.NewLine , file_csv), "previous logs!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ReadPreviuseLogFile(file_csv);
                    Calc_Groups_Counts();
                }

            }
            else
            {
                MessageBox.Show(string.Format("This csv file does not exist{0}{0}CSV File:{0}{1}", Environment.NewLine, file_csv), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                txt_auto_csv_file.Focus();
                txt_auto_csv_file.SelectAll();
            }
        }
    }




    public static class BitmapProcess
    {
        public static KeyValuePair<Bitmap, int> HasLogo(Bitmap source, int minShapes, int MinPixels = 30, int MaxPixels = 150)
        {
            var sourceData = new LockBitmap(source);
            var target = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            var targetData = new LockBitmap(target);
            sourceData.LockBits();
            targetData.LockBits();
            try
            {
                //Filter the shapes similar to logo
                var closedPaths = sourceData.FindClosedAreas(MinPixels, MaxPixels);
                var shapesFopund = closedPaths.Count;
                if (closedPaths.Count >= minShapes)
                {
                    closedPaths = closedPaths.FindShapesInCirclesBorder(minShapes);
                    foreach (var item in closedPaths)
                    {
                        // var cmykColors= item.ConvertAll(c => ColorSpaceHelper.RGBtoCMYK(sourceData[c.X, c.Y]));
                        targetData.ChangeColor(item, Color.Red);
                    }
                }
                var conf = closedPaths.Count < minShapes ? 0 : 100 - Math.Abs(closedPaths.Count - 5) * 20;
                return new KeyValuePair<Bitmap, int>(target, conf);
            }
            finally
            {
                sourceData.UnlockBits();
                targetData.UnlockBits();
                //bitmap.Save("c:\\d\\logo.png");
            }
        }




    }

    public static class MyDirectory
    {   // Regex version
        public static IEnumerable<string> GetFiles(string path,
                            string searchPatternExpression = "",
                            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file =>
                                     reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path,
                            string[] searchPatterns, Dictionary<string, int> previuseLogs,
                            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {

             var v=  searchPatterns.AsParallel()
                     .SelectMany(searchPattern =>
                            Alphaleonis.Win32.Filesystem.Directory.EnumerateFiles(path, searchPattern, Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.ContinueOnException| Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Files | Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Recursive).Where(x=> !previuseLogs.ContainsKey(x) ));
           
            
          /*  var v = searchPatterns.AsParallel()
       .SelectMany(searchPattern =>
              Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories ).Where(x => !previuseLogs.ContainsKey(x)));
              */

            return v;
        }
    }
   public  struct Stat_Info
    {
       public  int Has_Logos;
       public  int Has_noLogos;
       public  int Confused_Logos;
       public  int Failed_logos;
        public double Total_process_time;
        public long TotalImages
        {
            get
            {
                return Has_Logos + Has_noLogos + Confused_Logos + Failed_logos;
            }
        }
    }
    /// <summary>
    /// This class just holds the image info after we process it.
    /// </summary>
   public  class ImageLogoInfo
    {
        public long ProcessingTime { get; set; }
        public string ImagePath { get; set; }
        public string ImageName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImagePath))
                    return null;
                else
                    return Path.GetFileName(ImagePath);
            }
        }
        public bool HasLogo { get; set; }
        public int Confidence { get; set; }
        public bool ConfusedImage { get { return Confidence < 50 && Confidence > 45; } }
        public string Status { get
            {
                if (!string.IsNullOrWhiteSpace(Error))
                    return "Error";

                return ConfusedImage == true ? "Maybe" : HasLogo ? "Has Logo" : "Has No Logo";
           
            }
        }
        public Bitmap ProcessedImage { get; set; }

        public string  Error { get;  set; }
        public static ImageLogoInfo ProccessImage(string imgPath, bool includeprocessedimage)
        {
            ImageLogoInfo info = new ImageLogoInfo();
            info.ImagePath = imgPath;
            var sw = Stopwatch.StartNew();
            try
            {
                Bitmap source = GetBitmap(imgPath);
                var min = Math.Min(source.Width, source.Height);
                var scales = min > 500 ? new float[] { 1 } : (min > 400 ? new float[] { 1, 1.5f } : new float[] { 1, 1.5f, 2f });
                foreach (var scale in scales)
                {
                    var image = source.Crop(65, 65, scale);
                    var firstCheck = MyTemplateMatching.DetectLogo(image);
                    info.HasLogo = firstCheck > 50;
                    info.Confidence = (int)firstCheck;
                    if (includeprocessedimage)
                        info.ProcessedImage = image;

                    if (info.HasLogo) break;
                }
            }
            catch (Exception ex) { info.Error = ex.FullErrorMessage (); }
            sw.Stop();
            info.ProcessingTime = sw.ElapsedMilliseconds;

            return info;
        }

        internal  static Bitmap GetBitmap(string imgPath)
        {
            string[] imgExts_ppm = new string[] { ".ppm", ".pgm", ".pbm" };
            Bitmap source = null;

            if (imgExts_ppm.Contains((Path.GetExtension(imgPath) + "").ToLower()))
            {

                source = DmitryBrant.ImageFormats.Picture.Load(imgPath);

                if (source == null)
                {
                }
            }

            else
                source = (Bitmap)Bitmap.FromStream(new MemoryStream(File.ReadAllBytes(imgPath)));
            return source;
        }
        internal ImageLogoInfo Clone()
        {
            var i = new ImageLogoInfo();
            i.Confidence = Confidence;
           
            i.Error = Error;
            i.HasLogo = HasLogo;
            i.ImagePath = ImagePath;
            if (ProcessedImage != null)
                i.ProcessedImage = (Bitmap)ProcessedImage.Clone();
            i.ProcessingTime = ProcessingTime;

            return i;

        }
    }
}
