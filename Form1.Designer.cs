﻿namespace LogoDetector
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
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.labelFailImage = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lbl_confusedlogos = new System.Windows.Forms.LinkLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status_info = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.stat_time = new System.Windows.Forms.ToolStripStatusLabel();
            this.btn_imags_cnt = new System.Windows.Forms.Button();
            this.timerRefreshlistview = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonPause = new System.Windows.Forms.Button();
            this.chk_auto_csv_file = new System.Windows.Forms.CheckBox();
            this.txt_auto_csv_file = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_HasLogo = new System.Windows.Forms.LinkLabel();
            this.lbl_failedlogos = new System.Windows.Forms.LinkLabel();
            this.lbl_hasnologos = new System.Windows.Forms.LinkLabel();
            this.btn_Pre_logs = new System.Windows.Forms.Button();
            this.pic_failedlogos = new System.Windows.Forms.PictureBox();
            this.pic_confusedlogos = new System.Windows.Forms.PictureBox();
            this.pic_hasnologos = new System.Windows.Forms.PictureBox();
            this.pic_haslogs = new System.Windows.Forms.PictureBox();
            this.buttonExportMatches = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_failedlogos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_confusedlogos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_hasnologos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_haslogs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
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
            this.textBox1.Size = new System.Drawing.Size(481, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "C:\\D\\Ken\\LogoDetector\\Photos\\New folder\\New folder";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(757, 4);
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
            this.listView1.Size = new System.Drawing.Size(376, 42);
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
            this.splitContainer1.Location = new System.Drawing.Point(811, 83);
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
            this.splitContainer1.Size = new System.Drawing.Size(820, 119);
            this.splitContainer1.SplitterDistance = 376;
            this.splitContainer1.TabIndex = 6;
            this.splitContainer1.Visible = false;
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
            this.buttonCopyImages.Location = new System.Drawing.Point(252, 25);
            this.buttonCopyImages.Name = "buttonCopyImages";
            this.buttonCopyImages.Size = new System.Drawing.Size(124, 24);
            this.buttonCopyImages.TabIndex = 8;
            this.buttonCopyImages.Text = "Copy Images...";
            this.buttonCopyImages.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCopyImages.UseVisualStyleBackColor = true;
            this.buttonCopyImages.Click += new System.EventHandler(this.buttonCopyImages_Click);
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
            this.splitContainer2.Size = new System.Drawing.Size(440, 119);
            this.splitContainer2.SplitterDistance = 82;
            this.splitContainer2.TabIndex = 5;
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
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSV Files |*.csv";
            // 
            // lbl_confusedlogos
            // 
            this.lbl_confusedlogos.AutoSize = true;
            this.lbl_confusedlogos.Location = new System.Drawing.Point(46, 85);
            this.lbl_confusedlogos.Name = "lbl_confusedlogos";
            this.lbl_confusedlogos.Size = new System.Drawing.Size(141, 13);
            this.lbl_confusedlogos.TabIndex = 14;
            this.lbl_confusedlogos.TabStop = true;
            this.lbl_confusedlogos.Text = "images with low confedance";
            this.toolTip1.SetToolTip(this.lbl_confusedlogos, "images with low confedance (<%50 and > %45)");
            this.lbl_confusedlogos.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_HasLogo_LinkClicked);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status_info,
            this.toolStripStatusLabel1,
            this.stat_time});
            this.statusStrip1.Location = new System.Drawing.Point(0, 217);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(844, 22);
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
            // btn_imags_cnt
            // 
            this.btn_imags_cnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_imags_cnt.Location = new System.Drawing.Point(587, 4);
            this.btn_imags_cnt.Name = "btn_imags_cnt";
            this.btn_imags_cnt.Size = new System.Drawing.Size(87, 23);
            this.btn_imags_cnt.TabIndex = 2;
            this.btn_imags_cnt.Text = "Images Count";
            this.btn_imags_cnt.UseVisualStyleBackColor = true;
            this.btn_imags_cnt.Click += new System.EventHandler(this.button2_Click);
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
            this.buttonPause.Location = new System.Drawing.Point(678, 4);
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
            this.chk_auto_csv_file.Enabled = false;
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
            this.txt_auto_csv_file.Size = new System.Drawing.Size(481, 20);
            this.txt_auto_csv_file.TabIndex = 1;
            this.txt_auto_csv_file.Text = "C:\\D\\Ken\\LogoDetector\\PDF_Images_Results_04102017.csv";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbl_HasLogo);
            this.groupBox1.Controls.Add(this.pic_failedlogos);
            this.groupBox1.Controls.Add(this.lbl_failedlogos);
            this.groupBox1.Controls.Add(this.pic_confusedlogos);
            this.groupBox1.Controls.Add(this.lbl_confusedlogos);
            this.groupBox1.Controls.Add(this.pic_hasnologos);
            this.groupBox1.Controls.Add(this.lbl_hasnologos);
            this.groupBox1.Controls.Add(this.pic_haslogs);
            this.groupBox1.Location = new System.Drawing.Point(15, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(825, 151);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Statistics Info";
            // 
            // lbl_HasLogo
            // 
            this.lbl_HasLogo.AutoSize = true;
            this.lbl_HasLogo.Location = new System.Drawing.Point(48, 25);
            this.lbl_HasLogo.Name = "lbl_HasLogo";
            this.lbl_HasLogo.Size = new System.Drawing.Size(88, 13);
            this.lbl_HasLogo.TabIndex = 18;
            this.lbl_HasLogo.TabStop = true;
            this.lbl_HasLogo.Text = "Images with logo";
            this.lbl_HasLogo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_HasLogo_LinkClicked);
            // 
            // lbl_failedlogos
            // 
            this.lbl_failedlogos.AutoSize = true;
            this.lbl_failedlogos.Location = new System.Drawing.Point(46, 115);
            this.lbl_failedlogos.Name = "lbl_failedlogos";
            this.lbl_failedlogos.Size = new System.Drawing.Size(71, 13);
            this.lbl_failedlogos.TabIndex = 16;
            this.lbl_failedlogos.TabStop = true;
            this.lbl_failedlogos.Tag = "failed";
            this.lbl_failedlogos.Text = "Failed images";
            this.lbl_failedlogos.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_HasLogo_LinkClicked);
            // 
            // lbl_hasnologos
            // 
            this.lbl_hasnologos.AutoSize = true;
            this.lbl_hasnologos.Location = new System.Drawing.Point(46, 55);
            this.lbl_hasnologos.Name = "lbl_hasnologos";
            this.lbl_hasnologos.Size = new System.Drawing.Size(103, 13);
            this.lbl_hasnologos.TabIndex = 12;
            this.lbl_hasnologos.TabStop = true;
            this.lbl_hasnologos.Text = "Images with no logo";
            this.lbl_hasnologos.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_HasLogo_LinkClicked);
            // 
            // btn_Pre_logs
            // 
            this.btn_Pre_logs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Pre_logs.Image = global::LogoDetector.Properties.Resources._1492455014_67;
            this.btn_Pre_logs.Location = new System.Drawing.Point(547, 33);
            this.btn_Pre_logs.Name = "btn_Pre_logs";
            this.btn_Pre_logs.Size = new System.Drawing.Size(33, 18);
            this.btn_Pre_logs.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btn_Pre_logs, "Read Logs from csv file");
            this.btn_Pre_logs.UseVisualStyleBackColor = true;
            this.btn_Pre_logs.Click += new System.EventHandler(this.btn_Pre_logs_Click);
            // 
            // pic_failedlogos
            // 
            this.pic_failedlogos.Location = new System.Drawing.Point(18, 109);
            this.pic_failedlogos.Name = "pic_failedlogos";
            this.pic_failedlogos.Size = new System.Drawing.Size(24, 24);
            this.pic_failedlogos.TabIndex = 17;
            this.pic_failedlogos.TabStop = false;
            // 
            // pic_confusedlogos
            // 
            this.pic_confusedlogos.Location = new System.Drawing.Point(18, 79);
            this.pic_confusedlogos.Name = "pic_confusedlogos";
            this.pic_confusedlogos.Size = new System.Drawing.Size(24, 24);
            this.pic_confusedlogos.TabIndex = 15;
            this.pic_confusedlogos.TabStop = false;
            // 
            // pic_hasnologos
            // 
            this.pic_hasnologos.Location = new System.Drawing.Point(18, 49);
            this.pic_hasnologos.Name = "pic_hasnologos";
            this.pic_hasnologos.Size = new System.Drawing.Size(24, 24);
            this.pic_hasnologos.TabIndex = 13;
            this.pic_hasnologos.TabStop = false;
            // 
            // pic_haslogs
            // 
            this.pic_haslogs.Location = new System.Drawing.Point(18, 19);
            this.pic_haslogs.Name = "pic_haslogs";
            this.pic_haslogs.Size = new System.Drawing.Size(24, 24);
            this.pic_haslogs.TabIndex = 11;
            this.pic_haslogs.TabStop = false;
            // 
            // buttonExportMatches
            // 
            this.buttonExportMatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExportMatches.Enabled = false;
            this.buttonExportMatches.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.buttonExportMatches.ForeColor = System.Drawing.Color.Green;
            this.buttonExportMatches.Image = global::LogoDetector.Properties.Resources._1487537573_document_excel_csv;
            this.buttonExportMatches.Location = new System.Drawing.Point(252, 50);
            this.buttonExportMatches.Name = "buttonExportMatches";
            this.buttonExportMatches.Size = new System.Drawing.Size(124, 24);
            this.buttonExportMatches.TabIndex = 7;
            this.buttonExportMatches.Text = "Export to csv..";
            this.buttonExportMatches.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonExportMatches.UseVisualStyleBackColor = true;
            this.buttonExportMatches.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(440, 82);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(440, 33);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 239);
            this.Controls.Add(this.btn_Pre_logs);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chk_auto_csv_file);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btn_imags_cnt);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_failedlogos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_confusedlogos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_hasnologos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_haslogs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
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
        private System.Windows.Forms.Button btn_imags_cnt;
        private System.Windows.Forms.Timer timerRefreshlistview;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label labelFailImage;
        private System.Windows.Forms.Button buttonCopyImages;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.CheckBox checkBoxShowErrors;
        private System.Windows.Forms.CheckBox chk_auto_csv_file;
        private System.Windows.Forms.TextBox txt_auto_csv_file;
        private System.Windows.Forms.PictureBox pic_haslogs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pic_failedlogos;
        private System.Windows.Forms.LinkLabel lbl_failedlogos;
        private System.Windows.Forms.PictureBox pic_confusedlogos;
        private System.Windows.Forms.LinkLabel lbl_confusedlogos;
        private System.Windows.Forms.PictureBox pic_hasnologos;
        private System.Windows.Forms.LinkLabel lbl_hasnologos;
        private System.Windows.Forms.LinkLabel lbl_HasLogo;
        private System.Windows.Forms.Button btn_Pre_logs;
    }
}

