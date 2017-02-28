using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CopyFiles
{

    public partial class DIA_CopyFiles : Form, ICopyFilesDiag
    {

        // Properties
        public System.ComponentModel.ISynchronizeInvoke SynchronizationObject { get; set; }

        // Constructors
        public DIA_CopyFiles()
        {
            InitializeComponent();
        }

        // Methods
        public void update(Int32 totalFiles, Int32 copiedFiles, Int64 totalBytes, Int64 copiedBytes, String currentFilename)
        {
            Prog_TotalFiles.Maximum = totalFiles;
            Prog_TotalFiles.Value = copiedFiles;
            Prog_CurrentFile.Maximum = 100;
            if (totalBytes != 0)
            {
                Prog_CurrentFile.Value = Convert.ToInt32((100f / (totalBytes / 1024f)) * (copiedBytes / 1024f));
            }

            Lab_TotalFiles.Text = "Total files (" + copiedFiles + "/" + totalFiles + ")";
            Lab_CurrentFile.Text = currentFilename;
        }
        private void But_Cancel_Click(object sender, EventArgs e)
        {
            RaiseCancel();
        }
        private void DIA_CopyFiles_Closed(object sender, System.EventArgs e)
        {
            RaiseCancel();
        }
        private void RaiseCancel()
        {
            if (EN_cancelCopy != null)
            {
                EN_cancelCopy();
            }
        }

        //Events
        public event CopyFiles.DEL_cancelCopy EN_cancelCopy;


    }

}
