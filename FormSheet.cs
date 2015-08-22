using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AnswerSheetReader
{
    public partial class FormSheet : Form
    {
        public Boolean isLoaded { get; set; } 

        public FormSheet()
        {
            InitializeComponent();            
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            fileList.Items.Clear();

            if (folder.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folder.SelectedPath);

                if (files.Length > 0)
                {
                    foreach (string s in files)
                    {
                        fileList.Items.Add(new ListViewItem(s));
                    }
                    fileList.Items[fileList.Items.Count - 1].EnsureVisible();

                    this.sheetsStatus.Text = files.Length + " sheet(s) loaded.";
                    this.isLoaded = true;
                }
                else
                {
                    MessageBox.Show("No file in such folder", "Error");
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileList.Items.Clear();
            this.sheetsStatus.Text = "0 file(s) loaded.";
        }
    }
}
