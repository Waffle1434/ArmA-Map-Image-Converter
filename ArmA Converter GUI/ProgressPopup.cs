using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArmA_Converter_GUI {
    public partial class ProgressPopup : Form {
        //Action act;
        public BackgroundWorker bw;

        public ProgressPopup() {
            InitializeComponent();
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            //this.act = act;

            bw = backgroundWorker1;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            //backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) => Close();
        void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) => progressBar1.SetProgressNoAnimation(e.ProgressPercentage);

        void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            for (int i = 0; i < 1000; i++) {
                Thread.Sleep(10);
                backgroundWorker1.ReportProgress((int)(i / 10f));
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            backgroundWorker1.RunWorkerAsync();
        }
    }
    public static class Ext {
        public static void SetProgressNoAnimation(this ProgressBar pb, int value) {
            if (value == pb.Maximum) {
                pb.Value = pb.Maximum = value + 1;
                pb.Maximum = value;
            }
            else pb.Value = value + 1;
            pb.Value = value;
        }
    }
}
