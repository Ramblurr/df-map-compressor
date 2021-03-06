namespace DwarfFortressMapCompressor {
    partial class MainMenuForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenuForm));
            this.DecodeFortressMapButton = new System.Windows.Forms.Button();
            this.EncodeFortressMapButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.selectMapFileToEncodeOFD = new System.Windows.Forms.OpenFileDialog();
            this.selectEncodedFortressMapToViewOFD = new System.Windows.Forms.OpenFileDialog();
            this.encodeFortressMapSFD = new System.Windows.Forms.SaveFileDialog();
            this.decodeFortressMapSFD = new System.Windows.Forms.SaveFileDialog();
            this.ViewFortressMapButton = new System.Windows.Forms.Button();
            this.viewFortressMapOFD = new System.Windows.Forms.OpenFileDialog();
            this.OutputFlashFilesButton = new System.Windows.Forms.Button();
            this.selectMapFileToEncodeOFD2 = new System.Windows.Forms.OpenFileDialog();
            this.encodeFortressMapSFD2 = new System.Windows.Forms.SaveFileDialog();
            this.VisitArchiveButton = new System.Windows.Forms.Button();
            this.SwitchToSimpleInterfaceButton = new System.Windows.Forms.Button();
            this.decodeFortressMapToZipSFD = new System.Windows.Forms.SaveFileDialog();
            this.encodeCMVSFD = new System.Windows.Forms.SaveFileDialog();
            this.selectCMVFileToEncodeOFD = new System.Windows.Forms.OpenFileDialog();
            this.encodeFCMVSFD = new System.Windows.Forms.SaveFileDialog();
            this.compressCmvButton2 = new System.Windows.Forms.Button();
            this.compressCmvButton = new System.Windows.Forms.Button();
            this.compressTilesetButton = new System.Windows.Forms.Button();
            this.deleteImagesCheckbox = new System.Windows.Forms.CheckBox();
            this.doNotAnalyzeCheckbox = new System.Windows.Forms.CheckBox();
            this.colorMatchSensitivityTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.eraseDarkGreyGroundCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DecodeFortressMapButton
            // 
            this.DecodeFortressMapButton.Location = new System.Drawing.Point(223, 215);
            this.DecodeFortressMapButton.Name = "DecodeFortressMapButton";
            this.DecodeFortressMapButton.Size = new System.Drawing.Size(176, 41);
            this.DecodeFortressMapButton.TabIndex = 1;
            this.DecodeFortressMapButton.Text = "&Decode: .df-map to .png\r\n(Can also decode .fdf-map)";
            this.DecodeFortressMapButton.UseVisualStyleBackColor = true;
            this.DecodeFortressMapButton.Click += new System.EventHandler(this.DecodeFortressMapButton_Click);
            // 
            // EncodeFortressMapButton
            // 
            this.EncodeFortressMapButton.Location = new System.Drawing.Point(223, 61);
            this.EncodeFortressMapButton.Name = "EncodeFortressMapButton";
            this.EncodeFortressMapButton.Size = new System.Drawing.Size(176, 36);
            this.EncodeFortressMapButton.TabIndex = 0;
            this.EncodeFortressMapButton.Text = "&Encode: .bmp to .df-map\r\n(Also can read .fdf-map or .png)";
            this.EncodeFortressMapButton.UseVisualStyleBackColor = true;
            this.EncodeFortressMapButton.Click += new System.EventHandler(this.EncodeFortressMapButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 117);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // selectMapFileToEncodeOFD
            // 
            this.selectMapFileToEncodeOFD.Filter = "Bitmap Images (*.bmp)|*.bmp|Fdf-map (*.fdf-map)|*.fdf-map|PNG Images (*.png)|*.pn" +
    "g|Zipped multi-level images (*.zip)|*.zip";
            this.selectMapFileToEncodeOFD.Title = "Select the local map bmp";
            // 
            // selectEncodedFortressMapToViewOFD
            // 
            this.selectEncodedFortressMapToViewOFD.Filter = "Encoded Fortress Map (*.df-map, *.fdf-map)|*.df-map;*.fdf-map";
            this.selectEncodedFortressMapToViewOFD.Title = "Select the encoded map";
            // 
            // encodeFortressMapSFD
            // 
            this.encodeFortressMapSFD.DefaultExt = "df-map";
            this.encodeFortressMapSFD.Filter = "Encoded Fortress Map (*.df-map)|*.df-map";
            this.encodeFortressMapSFD.Title = "Where do you want to save the encoded map?";
            // 
            // decodeFortressMapSFD
            // 
            this.decodeFortressMapSFD.DefaultExt = "png";
            this.decodeFortressMapSFD.FileName = "map.png";
            this.decodeFortressMapSFD.Filter = "PNG image|*.png";
            this.decodeFortressMapSFD.Title = "Where do you want to save the fortress map image?";
            // 
            // ViewFortressMapButton
            // 
            this.ViewFortressMapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ViewFortressMapButton.Location = new System.Drawing.Point(223, 262);
            this.ViewFortressMapButton.Name = "ViewFortressMapButton";
            this.ViewFortressMapButton.Size = new System.Drawing.Size(176, 23);
            this.ViewFortressMapButton.TabIndex = 2;
            this.ViewFortressMapButton.Text = "&View: (.df-map, .png, .fdf-map)";
            this.ViewFortressMapButton.UseVisualStyleBackColor = true;
            this.ViewFortressMapButton.Click += new System.EventHandler(this.ViewFortressMapButton_Click);
            // 
            // viewFortressMapOFD
            // 
            this.viewFortressMapOFD.Filter = "Maps (*.png, *.df-map, *.fdf-map)|*.png;*.df-map;*.fdf-map";
            // 
            // OutputFlashFilesButton
            // 
            this.OutputFlashFilesButton.Location = new System.Drawing.Point(223, 103);
            this.OutputFlashFilesButton.Name = "OutputFlashFilesButton";
            this.OutputFlashFilesButton.Size = new System.Drawing.Size(176, 39);
            this.OutputFlashFilesButton.TabIndex = 5;
            this.OutputFlashFilesButton.Text = "Make .fdf-map for &flash viewer\r\n(Slightly larger than .df-map)";
            this.OutputFlashFilesButton.UseVisualStyleBackColor = true;
            this.OutputFlashFilesButton.Click += new System.EventHandler(this.OutputFlashFilesButton_Click);
            // 
            // selectMapFileToEncodeOFD2
            // 
            this.selectMapFileToEncodeOFD2.Filter = "Valid Images (*.bmp, *.df-map, *.png, *.zip)|*.bmp;*.df-map;*.png;*.zip";
            this.selectMapFileToEncodeOFD2.Title = "Select the map bmp, df-map, or png (or zip containing images, or cmv).";
            // 
            // encodeFortressMapSFD2
            // 
            this.encodeFortressMapSFD2.DefaultExt = "fdf-map";
            this.encodeFortressMapSFD2.Filter = "Encoded zlib\'d Fortress Map (*.fdf-map)|*.fdf-map";
            this.encodeFortressMapSFD2.Title = "Where do you want to save the encoded map?";
            // 
            // VisitArchiveButton
            // 
            this.VisitArchiveButton.Location = new System.Drawing.Point(223, 148);
            this.VisitArchiveButton.Name = "VisitArchiveButton";
            this.VisitArchiveButton.Size = new System.Drawing.Size(176, 62);
            this.VisitArchiveButton.TabIndex = 6;
            this.VisitArchiveButton.Text = "&Visit Markavian\'s DF Map Archive to upload your .fdf-map file(s) to share your D" +
    "F map images";
            this.VisitArchiveButton.UseVisualStyleBackColor = true;
            this.VisitArchiveButton.Click += new System.EventHandler(this.VisitArchiveButton_Click);
            // 
            // SwitchToSimpleInterfaceButton
            // 
            this.SwitchToSimpleInterfaceButton.Location = new System.Drawing.Point(223, 291);
            this.SwitchToSimpleInterfaceButton.Name = "SwitchToSimpleInterfaceButton";
            this.SwitchToSimpleInterfaceButton.Size = new System.Drawing.Size(176, 23);
            this.SwitchToSimpleInterfaceButton.TabIndex = 12;
            this.SwitchToSimpleInterfaceButton.Text = "Switch to Simple Interface";
            this.SwitchToSimpleInterfaceButton.UseVisualStyleBackColor = true;
            this.SwitchToSimpleInterfaceButton.Click += new System.EventHandler(this.SwitchToSimpleInterfaceButton_Click);
            // 
            // decodeFortressMapToZipSFD
            // 
            this.decodeFortressMapToZipSFD.DefaultExt = "zip";
            this.decodeFortressMapToZipSFD.FileName = "map.zip";
            this.decodeFortressMapToZipSFD.Filter = "ZIP archive|*.zip";
            this.decodeFortressMapToZipSFD.Title = "Where do you want to save the fortress map image(s) zip?";
            // 
            // encodeCMVSFD
            // 
            this.encodeCMVSFD.DefaultExt = "ccmv";
            this.encodeCMVSFD.Filter = "Compressed CMV (*.ccmv)|*.ccmv";
            this.encodeCMVSFD.Title = "Where do you want to save the encoded map?";
            // 
            // selectCMVFileToEncodeOFD
            // 
            this.selectCMVFileToEncodeOFD.Filter = "CMV Movie (*.cmv)|*.cmv";
            this.selectCMVFileToEncodeOFD.Title = "Select the CMV file to compress.";
            // 
            // encodeFCMVSFD
            // 
            this.encodeFCMVSFD.DefaultExt = "ccmv";
            this.encodeFCMVSFD.Filter = "Compressed CMV 2 (*.fcmv)|*.fcmv";
            this.encodeFCMVSFD.Title = "Where do you want to save the encoded map?";
            // 
            // compressCmvButton2
            // 
            this.compressCmvButton2.Location = new System.Drawing.Point(12, 180);
            this.compressCmvButton2.Name = "compressCmvButton2";
            this.compressCmvButton2.Size = new System.Drawing.Size(176, 26);
            this.compressCmvButton2.TabIndex = 16;
            this.compressCmvButton2.Text = "Compress .cmv file to .fcmv";
            this.compressCmvButton2.UseVisualStyleBackColor = true;
            this.compressCmvButton2.Click += new System.EventHandler(this.compressCmvButton2_Click);
            // 
            // compressCmvButton
            // 
            this.compressCmvButton.Location = new System.Drawing.Point(11, 148);
            this.compressCmvButton.Name = "compressCmvButton";
            this.compressCmvButton.Size = new System.Drawing.Size(176, 26);
            this.compressCmvButton.TabIndex = 15;
            this.compressCmvButton.Text = "Compress .cmv file to .ccmv";
            this.compressCmvButton.UseVisualStyleBackColor = true;
            this.compressCmvButton.Click += new System.EventHandler(this.compressCmvButton_Click);
            // 
            // compressTilesetButton
            // 
            this.compressTilesetButton.Location = new System.Drawing.Point(15, 212);
            this.compressTilesetButton.Name = "compressTilesetButton";
            this.compressTilesetButton.Size = new System.Drawing.Size(173, 23);
            this.compressTilesetButton.TabIndex = 17;
            this.compressTilesetButton.Text = "Compress tileset";
            this.compressTilesetButton.UseVisualStyleBackColor = true;
            this.compressTilesetButton.Visible = false;
            this.compressTilesetButton.Click += new System.EventHandler(this.compressTilesetButton_Click);
            // 
            // deleteImagesCheckbox
            // 
            this.deleteImagesCheckbox.AutoSize = false;
            this.deleteImagesCheckbox.Location = new System.Drawing.Point(15, 241);
            this.deleteImagesCheckbox.Name = "deleteImagesCheckbox";
            this.deleteImagesCheckbox.Size = new System.Drawing.Size(179, 17);
            this.deleteImagesCheckbox.TabIndex = 18;
            this.deleteImagesCheckbox.Text = "Delete images after compressing";
            this.deleteImagesCheckbox.UseVisualStyleBackColor = true;
            // 
            // doNotAnalyzeCheckbox
            // 
            this.doNotAnalyzeCheckbox.AutoSize = false;
            this.doNotAnalyzeCheckbox.Location = new System.Drawing.Point(15, 262);
            this.doNotAnalyzeCheckbox.Name = "doNotAnalyzeCheckbox";
            this.doNotAnalyzeCheckbox.Size = new System.Drawing.Size(141, 17);
            this.doNotAnalyzeCheckbox.TabIndex = 19;
            this.doNotAnalyzeCheckbox.Text = "Do not try to identify tiles";
            this.doNotAnalyzeCheckbox.UseVisualStyleBackColor = true;
            // 
            // colorMatchSensitivityTextBox
            // 
            this.colorMatchSensitivityTextBox.Location = new System.Drawing.Point(316, 25);
            this.colorMatchSensitivityTextBox.Name = "colorMatchSensitivityTextBox";
            this.colorMatchSensitivityTextBox.Size = new System.Drawing.Size(83, 20);
            this.colorMatchSensitivityTextBox.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Color match sensitivity (max allowed match):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(220, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "(∆R² + ∆G² + ∆B²)";
            // 
            // eraseDarkGreyGroundCheckbox
            // 
            this.eraseDarkGreyGroundCheckbox.AutoSize = false;
            this.eraseDarkGreyGroundCheckbox.Location = new System.Drawing.Point(15, 285);
            this.eraseDarkGreyGroundCheckbox.Name = "eraseDarkGreyGroundCheckbox";
            this.eraseDarkGreyGroundCheckbox.Size = new System.Drawing.Size(136, 17);
            this.eraseDarkGreyGroundCheckbox.TabIndex = 23;
            this.eraseDarkGreyGroundCheckbox.Text = "Erase dark grey ground";
            this.eraseDarkGreyGroundCheckbox.UseVisualStyleBackColor = true;
            this.eraseDarkGreyGroundCheckbox.Visible = false;
            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 331);
            this.Controls.Add(this.eraseDarkGreyGroundCheckbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.colorMatchSensitivityTextBox);
            this.Controls.Add(this.doNotAnalyzeCheckbox);
            this.Controls.Add(this.deleteImagesCheckbox);
            this.Controls.Add(this.compressTilesetButton);
            this.Controls.Add(this.compressCmvButton2);
            this.Controls.Add(this.compressCmvButton);
            this.Controls.Add(this.SwitchToSimpleInterfaceButton);
            this.Controls.Add(this.VisitArchiveButton);
            this.Controls.Add(this.OutputFlashFilesButton);
            this.Controls.Add(this.ViewFortressMapButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EncodeFortressMapButton);
            this.Controls.Add(this.DecodeFortressMapButton);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "MainMenuForm";
            this.Text = "Dwarf Fortress Map Compressor - version 3.3.4";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainMenuForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button DecodeFortressMapButton;
        private System.Windows.Forms.Button EncodeFortressMapButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog selectMapFileToEncodeOFD;
        private System.Windows.Forms.OpenFileDialog selectEncodedFortressMapToViewOFD;
        private System.Windows.Forms.SaveFileDialog encodeFortressMapSFD;
        private System.Windows.Forms.SaveFileDialog decodeFortressMapSFD;
        private System.Windows.Forms.Button ViewFortressMapButton;
        private System.Windows.Forms.OpenFileDialog viewFortressMapOFD;
        private System.Windows.Forms.Button OutputFlashFilesButton;
        private System.Windows.Forms.OpenFileDialog selectMapFileToEncodeOFD2;
        private System.Windows.Forms.SaveFileDialog encodeFortressMapSFD2;
        private System.Windows.Forms.Button VisitArchiveButton;
        private System.Windows.Forms.Button SwitchToSimpleInterfaceButton;
        private System.Windows.Forms.SaveFileDialog decodeFortressMapToZipSFD;
        private System.Windows.Forms.SaveFileDialog encodeCMVSFD;
        private System.Windows.Forms.OpenFileDialog selectCMVFileToEncodeOFD;
        private System.Windows.Forms.SaveFileDialog encodeFCMVSFD;
        private System.Windows.Forms.Button compressCmvButton2;
        private System.Windows.Forms.Button compressCmvButton;
        private System.Windows.Forms.Button compressTilesetButton;
        private System.Windows.Forms.CheckBox deleteImagesCheckbox;
        private System.Windows.Forms.CheckBox doNotAnalyzeCheckbox;
        private System.Windows.Forms.TextBox colorMatchSensitivityTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox eraseDarkGreyGroundCheckbox;

    }
}

