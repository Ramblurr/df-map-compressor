namespace DwarfFortressMapCompressor {
    partial class TileSizeChooser {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileSizeChooser));
            this.label1 = new System.Windows.Forms.Label();
            this.xSizeBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ySizeBox = new System.Windows.Forms.NumericUpDown();
            this.guessButton = new System.Windows.Forms.Button();
            this.goButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize) (this.xSizeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.ySizeBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label1.Location = new System.Drawing.Point(25, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "What size are the tiles in the image?";
            // 
            // xSizeBox
            // 
            this.xSizeBox.Location = new System.Drawing.Point(36, 19);
            this.xSizeBox.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.xSizeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.xSizeBox.Name = "xSizeBox";
            this.xSizeBox.Size = new System.Drawing.Size(43, 20);
            this.xSizeBox.TabIndex = 1;
            this.xSizeBox.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Y";
            // 
            // ySizeBox
            // 
            this.ySizeBox.Location = new System.Drawing.Point(36, 46);
            this.ySizeBox.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.ySizeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ySizeBox.Name = "ySizeBox";
            this.ySizeBox.Size = new System.Drawing.Size(43, 20);
            this.ySizeBox.TabIndex = 4;
            this.ySizeBox.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // guessButton
            // 
            this.guessButton.Location = new System.Drawing.Point(127, 109);
            this.guessButton.Name = "guessButton";
            this.guessButton.Size = new System.Drawing.Size(75, 23);
            this.guessButton.TabIndex = 5;
            this.guessButton.Text = "Guess";
            this.guessButton.UseVisualStyleBackColor = true;
            this.guessButton.Click += new System.EventHandler(this.guessButton_Click);
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(6, 72);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(75, 23);
            this.goButton.TabIndex = 6;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.goButton);
            this.groupBox1.Controls.Add(this.xSizeBox);
            this.groupBox1.Controls.Add(this.ySizeBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(97, 109);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enter size:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(124, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 52);
            this.label4.TabIndex = 8;
            this.label4.Text = "Note: \'Guess\' is\r\nslow and may\r\nvery well guess\r\nwrong.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(182, 182);
            this.label5.TabIndex = 9;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // TileSizeChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 357);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.guessButton);
            this.Controls.Add(this.label1);
            this.Name = "TileSizeChooser";
            this.Text = "DF Map Compressor";
            ((System.ComponentModel.ISupportInitialize) (this.xSizeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.ySizeBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown xSizeBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown ySizeBox;
        private System.Windows.Forms.Button guessButton;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}