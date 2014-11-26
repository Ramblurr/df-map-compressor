namespace DwarfFortressMapCompressor {
    partial class FastMapViewer {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FastMapViewer));
            this.label1 = new System.Windows.Forms.Label();
            this.actionTimer = new System.Windows.Forms.Timer(this.components);
            this.flashControl = new AxShockwaveFlashObjects.AxShockwaveFlash();
            ((System.ComponentModel.ISupportInitialize) (this.flashControl)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Location = new System.Drawing.Point(12, 597);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(358, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "Use the - and + keys to zoom, and click and drag to scroll around the map.\r\n(The " +
    "keys are the same as on the DF Map Archive)";
            // 
            // flashControl
            // 
            this.flashControl.Enabled = true;
            this.flashControl.Location = new System.Drawing.Point(11, 11);
            this.flashControl.Name = "flashControl";
            this.flashControl.OcxState = ((System.Windows.Forms.AxHost.State) (resources.GetObject("flashControl.OcxState")));
            this.flashControl.Size = new System.Drawing.Size(914, 583);
            this.flashControl.TabIndex = 3;
            // 
            // FastMapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 648);
            this.Controls.Add(this.flashControl);
            this.Controls.Add(this.label1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "FastMapViewer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "FastMapViewer";
            this.Resize += new System.EventHandler(this.FastMapViewer_Resize);
            ((System.ComponentModel.ISupportInitialize) (this.flashControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer actionTimer;
        private AxShockwaveFlashObjects.AxShockwaveFlash flashControl;

    }
}