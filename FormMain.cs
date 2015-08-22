using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using AnswerSheetReader.Class;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;

namespace AnswerSheetReader
{
    public partial class FormMain : Form
    {
        FormLogger logger;
        FormSheet sheets;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            logger = new FormLogger();
            logger.MdiParent = this;
            logger.Show();
            logger.Location = new Point(this.Width - logger.Width - 30, this.Height - logger.Height - 50);

            sheets = new FormSheet();
            sheets.MdiParent = this;
            sheets.Show();
            sheets.Location = new Point(this.Width - sheets.Width - 30, this.Height - logger.Height - sheets.Height - 70);

            logger.addLog("Started.");
        }

        private void test()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                this.Invoke((MethodInvoker)delegate
                {
                    logger.addLog(i.ToString());
                });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImageUtils utils = new ImageUtils();
            Thread thread = new Thread(LongRun);

            thread.Start();
        }

        private void LongRun()
        {
            ImageUtils utils = new ImageUtils();
            Stopwatch w = new Stopwatch();
            w.Start();
            for (int i = 0; i < 100; i++)
            {

                utils.originalImage = new Image<Bgr, byte>("C:\\Users\\Chadpong\\Desktop\\Answer sheet\\max.jpg");
                utils.DeSkew(i);
                this.Invoke((MethodInvoker)delegate
                {
                    logger.addLog(i.ToString());
                });

            }

            this.Invoke((MethodInvoker)delegate
            {
                logger.addLog("Process completed in " + Utils.toReadableString(w.ElapsedMilliseconds));
            });
            
        }

    }
}
