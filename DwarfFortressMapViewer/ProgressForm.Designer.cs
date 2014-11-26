namespace DwarfFortressMapCompressor {
    partial class ProgressForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusLine = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 52);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(338, 32);
            this.progressBar.TabIndex = 0;
            //this.progressBar.UseWaitCursor = true;
            // 
            // statusLine
            // 
            this.statusLine.Cursor = System.Windows.Forms.Cursors.Default;
            this.statusLine.Location = new System.Drawing.Point(12, 12);
            this.statusLine.Name = "statusLine";
            this.statusLine.ReadOnly = true;
            this.statusLine.Size = new System.Drawing.Size(338, 20);
            this.statusLine.TabIndex = 1;
            this.statusLine.TabStop = false;
            this.statusLine.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 96);
            this.Controls.Add(this.statusLine);
            this.Controls.Add(this.progressBar);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "ProgressForm";
            this.Text = "DF Map Compressor - Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox statusLine;
    }
}