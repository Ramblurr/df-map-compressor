namespace DwarfFortressMapCompressor {
    partial class SimpleMainMenuForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleMainMenuForm));
            this.VisitArchiveButton = new System.Windows.Forms.Button();
            this.OutputFlashFilesButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SwitchToAdvancedInterfaceButton = new System.Windows.Forms.Button();
            this.AutoCompressButton = new System.Windows.Forms.Button();
            this.ViewFortressMapButton = new System.Windows.Forms.Button();
            this.deleteImagesCheckbox = new System.Windows.Forms.CheckBox();
            this.doNotAnalyzeCheckbox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.colorMatchSensitivityTextBox = new System.Windows.Forms.TextBox();
            this.eraseDarkGreyGroundCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // VisitArchiveButton
            // 
            this.VisitArchiveButton.Location = new System.Drawing.Point(240, 82);
            this.VisitArchiveButton.Name = "VisitArchiveButton";
            this.VisitArchiveButton.Size = new System.Drawing.Size(176, 38);
            this.VisitArchiveButton.TabIndex = 8;
            this.VisitArchiveButton.Text = "&Visit Markavian\'s DF Map Archive";
            this.VisitArchiveButton.UseVisualStyleBackColor = true;
            this.VisitArchiveButton.Click += new System.EventHandler(this.VisitArchiveButton_Click);
            // 
            // OutputFlashFilesButton
            // 
            this.OutputFlashFilesButton.Location = new System.Drawing.Point(240, 51);
            this.OutputFlashFilesButton.Name = "OutputFlashFilesButton";
            this.OutputFlashFilesButton.Size = new System.Drawing.Size(176, 25);
            this.OutputFlashFilesButton.TabIndex = 7;
            this.OutputFlashFilesButton.Text = "&Compress image(s) to .fdf-map file";
            this.OutputFlashFilesButton.UseVisualStyleBackColor = true;
            this.OutputFlashFilesButton.Click += new System.EventHandler(this.OutputFlashFilesButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 312);
            this.label1.TabIndex = 10;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // SwitchToAdvancedInterfaceButton
            // 
            this.SwitchToAdvancedInterfaceButton.Location = new System.Drawing.Point(240, 296);
            this.SwitchToAdvancedInterfaceButton.Name = "SwitchToAdvancedInterfaceButton";
            this.SwitchToAdvancedInterfaceButton.Size = new System.Drawing.Size(176, 23);
            this.SwitchToAdvancedInterfaceButton.TabIndex = 11;
            this.SwitchToAdvancedInterfaceButton.Text = "Switch to Advanced Interface";
            this.SwitchToAdvancedInterfaceButton.UseVisualStyleBackColor = true;
            this.SwitchToAdvancedInterfaceButton.Click += new System.EventHandler(this.SwitchToAdvancedInterfaceButton_Click);
            // 
            // AutoCompressButton
            // 
            this.AutoCompressButton.Location = new System.Drawing.Point(240, 254);
            this.AutoCompressButton.Name = "AutoCompressButton";
            this.AutoCompressButton.Size = new System.Drawing.Size(176, 36);
            this.AutoCompressButton.TabIndex = 12;
            this.AutoCompressButton.Text = "Auto-Compress (compresses map bmps without fdf-maps)";
            this.AutoCompressButton.UseVisualStyleBackColor = true;
            this.AutoCompressButton.Visible = false;
            // 
            // ViewFortressMapButton
            // 
            this.ViewFortressMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ViewFortressMapButton.Location = new System.Drawing.Point(240, 126);
            this.ViewFortressMapButton.Name = "ViewFortressMapButton";
            this.ViewFortressMapButton.Size = new System.Drawing.Size(176, 38);
            this.ViewFortressMapButton.TabIndex = 15;
            this.ViewFortressMapButton.Text = "&Preview .fdf-map file (before uploading)";
            this.ViewFortressMapButton.UseVisualStyleBackColor = true;
            this.ViewFortressMapButton.Click += new System.EventHandler(this.ViewFortressMapButton_Click);
            // 
            // deleteImagesCheckbox
            // 
            this.deleteImagesCheckbox.AutoSize = false;
            this.deleteImagesCheckbox.Location = new System.Drawing.Point(240, 180);
            this.deleteImagesCheckbox.Name = "deleteImagesCheckbox";
            this.deleteImagesCheckbox.Size = new System.Drawing.Size(179, 17);
            this.deleteImagesCheckbox.TabIndex = 16;
            this.deleteImagesCheckbox.Text = "Delete images after compressing";
            this.deleteImagesCheckbox.UseVisualStyleBackColor = true;
            // 
            // doNotAnalyzeCheckbox
            // 
            this.doNotAnalyzeCheckbox.AutoSize = false;
            this.doNotAnalyzeCheckbox.Location = new System.Drawing.Point(240, 203);
            this.doNotAnalyzeCheckbox.Name = "doNotAnalyzeCheckbox";
            this.doNotAnalyzeCheckbox.Size = new System.Drawing.Size(141, 17);
            this.doNotAnalyzeCheckbox.TabIndex = 17;
            this.doNotAnalyzeCheckbox.Text = "Do not try to identify tiles";
            this.doNotAnalyzeCheckbox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(232, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "(∆R² + ∆G² + ∆B²)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Color match sensitivity (max allowed match):";
            // 
            // colorMatchSensitivityTextBox
            // 
            this.colorMatchSensitivityTextBox.Location = new System.Drawing.Point(328, 25);
            this.colorMatchSensitivityTextBox.Name = "colorMatchSensitivityTextBox";
            this.colorMatchSensitivityTextBox.Size = new System.Drawing.Size(83, 20);
            this.colorMatchSensitivityTextBox.TabIndex = 23;
            // 
            // eraseDarkGreyGroundCheckbox
            // 
            this.eraseDarkGreyGroundCheckbox.AutoSize = false;
            this.eraseDarkGreyGroundCheckbox.Location = new System.Drawing.Point(240, 226);
            this.eraseDarkGreyGroundCheckbox.Name = "eraseDarkGreyGroundCheckbox";
            this.eraseDarkGreyGroundCheckbox.Size = new System.Drawing.Size(136, 17);
            this.eraseDarkGreyGroundCheckbox.TabIndex = 26;
            this.eraseDarkGreyGroundCheckbox.Text = "Erase dark grey ground";
            this.eraseDarkGreyGroundCheckbox.UseVisualStyleBackColor = true;
            this.eraseDarkGreyGroundCheckbox.Visible = false;
            // 
            // SimpleMainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 331);
            this.Controls.Add(this.eraseDarkGreyGroundCheckbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.colorMatchSensitivityTextBox);
            this.Controls.Add(this.doNotAnalyzeCheckbox);
            this.Controls.Add(this.deleteImagesCheckbox);
            this.Controls.Add(this.ViewFortressMapButton);
            this.Controls.Add(this.AutoCompressButton);
            this.Controls.Add(this.SwitchToAdvancedInterfaceButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VisitArchiveButton);
            this.Controls.Add(this.OutputFlashFilesButton);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "SimpleMainMenuForm";
            this.Text = "Dwarf Fortress Map Compressor - version 3.3.4";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button VisitArchiveButton;
        private System.Windows.Forms.Button OutputFlashFilesButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SwitchToAdvancedInterfaceButton;
        private System.Windows.Forms.Button AutoCompressButton;
        private System.Windows.Forms.Button ViewFortressMapButton;
        private System.Windows.Forms.CheckBox deleteImagesCheckbox;
        private System.Windows.Forms.CheckBox doNotAnalyzeCheckbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox colorMatchSensitivityTextBox;
        private System.Windows.Forms.CheckBox eraseDarkGreyGroundCheckbox;

    }
}