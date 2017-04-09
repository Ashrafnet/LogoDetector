namespace LogoDetector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBoxShowErrors = new System.Windows.Forms.CheckBox();
            this.buttonCopyImages = new System.Windows.Forms.Button();
            this.buttonExportMatches = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelFailImage = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status_info = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.stat_time = new System.Windows.Forms.ToolStripStatusLabel();
            this.button2 = new System.Windows.Forms.Button();
            this.timerRefreshlistview = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonPause = new System.Windows.Forms.Button();
            this.chk_auto_csv_file = new System.Windows.Forms.CheckBox();
            this.txt_auto_csv_file = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Images folder";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.textBox1.Location = new System.Drawing.Point(100, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(454, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "C:\\D\\Ken\\LogoDetector\\Photos\\New folder";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(730, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Proccess";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(0, 77);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(364, 197);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.VirtualMode = true;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView1_RetrieveVirtualItem);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Image";
            this.columnHeader1.Width = 90;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Has Logo";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Process Time";
            this.columnHeader3.Width = 72;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Confidence";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "OK.png");
            this.imageList1.Images.SetKeyName(1, "error.png");
            this.imageList1.Images.SetKeyName(2, "warning.png");
            this.imageList1.Images.SetKeyName(3, "fail.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 58);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxShowErrors);
            this.splitContainer1.Panel1.Controls.Add(this.buttonCopyImages);
            this.splitContainer1.Panel1.Controls.Add(this.buttonExportMatches);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox3);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox2);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox1);
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(793, 274);
            this.splitContainer1.SplitterDistance = 364;
            this.splitContainer1.TabIndex = 6;
            // 
            // checkBoxShowErrors
            // 
            this.checkBoxShowErrors.AutoSize = true;
            this.checkBoxShowErrors.Location = new System.Drawing.Point(3, 58);
            this.checkBoxShowErrors.Name = "checkBoxShowErrors";
            this.checkBoxShowErrors.Size = new System.Drawing.Size(117, 17);
            this.checkBoxShowErrors.TabIndex = 9;
            this.checkBoxShowErrors.Text = "Show failed images";
            this.toolTip1.SetToolTip(this.checkBoxShowErrors, "Show failed images");
            this.checkBoxShowErrors.UseVisualStyleBackColor = true;
            this.checkBoxShowErrors.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonCopyImages
            // 
            this.buttonCopyImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopyImages.Enabled = false;
            this.buttonCopyImages.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.buttonCopyImages.ForeColor = System.Drawing.Color.Green;
            this.buttonCopyImages.Location = new System.Drawing.Point(240, 25);
            this.buttonCopyImages.Name = "buttonCopyImages";
            this.buttonCopyImages.Size = new System.Drawing.Size(124, 24);
            this.buttonCopyImages.TabIndex = 8;
            this.buttonCopyImages.Text = "Copy Images...";
            this.buttonCopyImages.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCopyImages.UseVisualStyleBackColor = true;
            this.buttonCopyImages.Click += new System.EventHandler(this.buttonCopyImages_Click);
            // 
            // buttonExportMatches
            // 
            this.buttonExportMatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExportMatches.Enabled = false;
            this.buttonExportMatches.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.buttonExportMatches.ForeColor = System.Drawing.Color.Green;
            this.buttonExportMatches.Image = global::LogoDetector.Properties.Resources._1487537573_document_excel_csv;
            this.buttonExportMatches.Location = new System.Drawing.Point(240, 50);
            this.buttonExportMatches.Name = "buttonExportMatches";
            this.buttonExportMatches.Size = new System.Drawing.Size(124, 24);
            this.buttonExportMatches.TabIndex = 7;
            this.buttonExportMatches.Text = "Export to csv..";
            this.buttonExportMatches.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonExportMatches.UseVisualStyleBackColor = true;
            this.buttonExportMatches.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(3, 39);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(135, 17);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.Text = "Show confused images";
            this.toolTip1.SetToolTip(this.checkBox3, "Show images with low confedance (<%50 and > %45)");
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(3, 21);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(150, 17);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.Text = "Show images without logo";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(134, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Show images with logo";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.labelFailImage);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer2.Size = new System.Drawing.Size(425, 274);
            this.splitContainer2.SplitterDistance = 189;
            this.splitContainer2.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(425, 189);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // labelFailImage
            // 
            this.labelFailImage.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.labelFailImage.ForeColor = System.Drawing.Color.Red;
            this.labelFailImage.Location = new System.Drawing.Point(3, 1);
            this.labelFailImage.Name = "labelFailImage";
            this.labelFailImage.Size = new System.Drawing.Size(376, 87);
            this.labelFailImage.TabIndex = 6;
            this.labelFailImage.Text = "Fail Image";
            this.labelFailImage.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(425, 81);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSV Files |*.csv";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status_info,
            this.toolStripStatusLabel1,
            this.stat_time});
            this.statusStrip1.Location = new System.Drawing.Point(0, 335);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(817, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status_info
            // 
            this.status_info.Name = "status_info";
            this.status_info.Size = new System.Drawing.Size(39, 17);
            this.status_info.Text = "Ready";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Enabled = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // stat_time
            // 
            this.stat_time.Name = "stat_time";
            this.stat_time.Size = new System.Drawing.Size(0, 17);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(560, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Images Count";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timerRefreshlistview
            // 
            this.timerRefreshlistview.Enabled = true;
            this.timerRefreshlistview.Interval = 300;
            this.timerRefreshlistview.Tick += new System.EventHandler(this.timerRefreshlistview_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // buttonPause
            // 
            this.buttonPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPause.Enabled = false;
            this.buttonPause.Location = new System.Drawing.Point(651, 4);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 23);
            this.buttonPause.TabIndex = 8;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // chk_auto_csv_file
            // 
            this.chk_auto_csv_file.AutoSize = true;
            this.chk_auto_csv_file.Checked = true;
            this.chk_auto_csv_file.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_auto_csv_file.Location = new System.Drawing.Point(15, 32);
            this.chk_auto_csv_file.Name = "chk_auto_csv_file";
            this.chk_auto_csv_file.Size = new System.Drawing.Size(77, 17);
            this.chk_auto_csv_file.TabIndex = 9;
            this.chk_auto_csv_file.Text = "csv export";
            this.chk_auto_csv_file.UseVisualStyleBackColor = true;
            this.chk_auto_csv_file.CheckedChanged += new System.EventHandler(this.chk_auto_csv_file_CheckedChanged);
            // 
            // txt_auto_csv_file
            // 
            this.txt_auto_csv_file.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_auto_csv_file.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txt_auto_csv_file.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.txt_auto_csv_file.Location = new System.Drawing.Point(100, 32);
            this.txt_auto_csv_file.Name = "txt_auto_csv_file";
            this.txt_auto_csv_file.Size = new System.Drawing.Size(454, 20);
            this.txt_auto_csv_file.TabIndex = 1;
            this.txt_auto_csv_file.Text = "C:\\D\\Ken\\LogoDetector\\Photos\\New folder";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 357);
            this.Controls.Add(this.chk_auto_csv_file);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_auto_csv_file);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Logo Detector";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button buttonExportMatches;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status_info;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel stat_time;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timerRefreshlistview;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label labelFailImage;
        private System.Windows.Forms.Button buttonCopyImages;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.CheckBox checkBoxShowErrors;
        private System.Windows.Forms.CheckBox chk_auto_csv_file;
        private System.Windows.Forms.TextBox txt_auto_csv_file;
    }
}

