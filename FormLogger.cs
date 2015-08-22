using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AnswerSheetReader
{
    public partial class FormLogger : Form
    {
        public FormLogger()
        {
            InitializeComponent();
        }


        public void addLog(string m)
        {
            string[] row = { DateTime.Now.ToString("HH:mm:ss tt"), m };
            log.Items.Add(new ListViewItem(row));
            log.Items[log.Items.Count - 1].EnsureVisible();
        }
    }
}
