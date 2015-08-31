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
        public string dirPath { get; set; }
        private string path = "C:\\Users\\Chadpong\\Desktop\\5635_Answer Sheet - Copy\\Answer Sheet\\100\\JPEG File";

        public FormSheet()
        {
            InitializeComponent();        
            
            string[] files = Directory.GetFiles(path, "*.jpg");
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
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            fileList.Items.Clear();

            if (folder.ShowDialog() == DialogResult.OK)
            {
                dirPath = folder.SelectedPath;

                string[] files = Directory.GetFiles(folder.SelectedPath, "*.jpg");

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

        public List<String> getFiles()
        {
            List<String> l = new List<String>();
            foreach (ListViewItem item in fileList.Items)
            {
                l.Add(item.Text);
            }
            return l;
        }
    }
}
