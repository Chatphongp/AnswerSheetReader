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
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
        }


        private void FormClosing_close(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
        }

        public void GetConfig()
        {

        }
    }
}
