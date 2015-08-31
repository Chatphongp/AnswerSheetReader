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
using System.IO;

namespace AnswerSheetReader
{
    public partial class FormMain : Form
    {
        FormLogger logger;
        FormSheet sheets;
        FormConfig config;
        FormResult result;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            logger = new FormLogger();
            logger.MdiParent = this;
            logger.Show();
            logger.Location = new Point(this.Width - logger.Width - 30, this.Height - logger.Height - 70);

            sheets = new FormSheet();
            sheets.MdiParent = this;
            sheets.Show();
            sheets.Location = new Point(this.Width - sheets.Width - 30, this.Height - logger.Height - sheets.Height - 90);

            result = new FormResult();
            result.MdiParent = this;
            result.Show();
            result.Location = new Point(10, 20);

            logger.addLog("Initialized.");
        }

        private void executeMenuStrip_Click(object sender, EventArgs e)
        {
            List<String> files = sheets.getFiles();
            if (files.Count > 0)
            {
                logger.addLog("Start Processing " + files.Count + " file(s).");
                Thread thread = new Thread(() => LongRun(files));
                thread.Start();
            }
            else
            {
                logger.addError("No file selected.");
            }
        }

        private void LongRun(List<String> files)
        {            
            Stopwatch w = new Stopwatch();
            w.Start();

            int count = 0;            
            foreach (String file in files)
            {
                ImageUtils imgUtils = new ImageUtils(new Image<Bgr, byte>(file));

                int corner = imgUtils.FindCorner();
                imgUtils.PerspectiveTransform("C:\\Users\\Chadpong\\Desktop\\output\\" + count + ".jpg");
                //imgUtils.PerspectiveTransform("C:\\Users\\Chadpong\\Desktop\\output\\" + count + ".jpg");
                //imgUtils.DeSkew();
                //imgUtils.Validate();

                string appId = Utils.GetApplicantID(imgUtils.ReadAppicantId());
                
                this.Invoke((MethodInvoker)delegate
                {
                    if (appId.Contains('X'))
                    {
                        result.addErrorResult(Path.GetFileName(file), appId);
                    }
                    else
                    {
                        result.addResult(Path.GetFileName(file), appId);
                    }

                    logger.addLog(corner.ToString());

                });
                count++;
                imgUtils = null;
            }

            this.Invoke((MethodInvoker)delegate
            {
                logger.addLog("Process completed in " + Utils.toReadableString(w.ElapsedMilliseconds));
            });
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (config != null)
            {
                if (!config.Visible)
                {
                    config.Show();
                }
            }
            else
            {
                config = new FormConfig();
                config.MdiParent = this;
                config.Show();
            }
        }
    }
}
