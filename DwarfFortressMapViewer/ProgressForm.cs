using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DwarfFortressMapCompressor {
    public partial class ProgressForm : Form {
        string prefix = "";

        public ProgressForm() {
            InitializeComponent();
        }

        public void SetPrefix(string prefix) {
            this.prefix = prefix;
        }

        public void SetStatus(string status) {
            statusLine.Text = prefix + status;
            progressBar.Value = 0;
            progressBar.Maximum = 100;
            this.Update();
            Application.DoEvents();
        }

        public void SetProgress(int progress) {
            progressBar.Value = progress;
            this.Update();
            Application.DoEvents();
        }

        public void SetMaximum(int maximum) {
            progressBar.Maximum = maximum;
        }
    }
}