using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AnswerSheetReader.Class
{
    public partial class FormResult : Form
    {
        public FormResult()
        {
            InitializeComponent();
        }

        public void addResult(string file, string m)
        {
            string[] row = { (result.Items.Count + 1).ToString(), file, m };
            result.Items.Add(new ListViewItem(row));
            result.Items[result.Items.Count - 1].EnsureVisible();
        }

        public void addErrorResult(string file, string m)
        {
            string[] row = { (result.Items.Count + 1).ToString() ,file , m };
            ListViewItem li = new ListViewItem(row);
            li.ForeColor = Color.Red;
            result.Items.Add(li);
            result.Items[result.Items.Count - 1].EnsureVisible();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            result.Items.Clear();
        }
    }
}
