
namespace DwarfFortressMapCompressor {
    partial class MapViewer {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapViewer));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.actionTimer = new System.Windows.Forms.Timer(this.components);
            this.mouseDraggingRepelsMapRadioButton = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mouseDragsMapRadioButton = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.zoomOnCenterRadioButton = new System.Windows.Forms.RadioButton();
            this.zoomOnMouseRadioButton = new System.Windows.Forms.RadioButton();
            this.zLevelChooser = new DomainUpDownIgnoringMousewheel();
            this.zLevelLabel = new System.Windows.Forms.Label();
            this.mapBox = new DwarfFortressMapCompressor.WindowBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.mapBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mapBox);
            this.panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(913, 572);
            this.panel1.TabIndex = 1;
            this.panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseWheel);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Location = new System.Drawing.Point(12, 597);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Use the mousewheel to zoom, and click and drag to scroll around the map.";
            // 
            // actionTimer
            // 
            this.actionTimer.Tick += new System.EventHandler(this.actionTimer_Tick);
            // 
            // mouseDraggingRepelsMapRadioButton
            // 
            this.mouseDraggingRepelsMapRadioButton.AutoSize = true;
            this.mouseDraggingRepelsMapRadioButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.mouseDraggingRepelsMapRadioButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mouseDraggingRepelsMapRadioButton.Location = new System.Drawing.Point(0, 23);
            this.mouseDraggingRepelsMapRadioButton.Name = "mouseDraggingRepelsMapRadioButton";
            this.mouseDraggingRepelsMapRadioButton.Size = new System.Drawing.Size(175, 17);
            this.mouseDraggingRepelsMapRadioButton.TabIndex = 4;
            this.mouseDraggingRepelsMapRadioButton.Text = "Mouse dragging repels map";
            this.mouseDraggingRepelsMapRadioButton.UseVisualStyleBackColor = true;
            this.mouseDraggingRepelsMapRadioButton.CheckedChanged += new System.EventHandler(this.mouseDraggingRepelsMapRadioButton_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.mouseDragsMapRadioButton);
            this.panel2.Controls.Add(this.mouseDraggingRepelsMapRadioButton);
            this.panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel2.Location = new System.Drawing.Point(377, 597);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(175, 40);
            this.panel2.TabIndex = 7;
            // 
            // mouseDragsMapRadioButton
            // 
            this.mouseDragsMapRadioButton.AutoSize = true;
            this.mouseDragsMapRadioButton.Checked = true;
            this.mouseDragsMapRadioButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.mouseDragsMapRadioButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.mouseDragsMapRadioButton.Location = new System.Drawing.Point(0, 0);
            this.mouseDragsMapRadioButton.Name = "mouseDragsMapRadioButton";
            this.mouseDragsMapRadioButton.Size = new System.Drawing.Size(175, 17);
            this.mouseDragsMapRadioButton.TabIndex = 3;
            this.mouseDragsMapRadioButton.TabStop = true;
            this.mouseDragsMapRadioButton.Text = "Mouse drags map";
            this.mouseDragsMapRadioButton.UseVisualStyleBackColor = true;
            this.mouseDragsMapRadioButton.CheckedChanged += new System.EventHandler(this.mouseDragsMapRadioButton_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.zoomOnCenterRadioButton);
            this.panel3.Controls.Add(this.zoomOnMouseRadioButton);
            this.panel3.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel3.Location = new System.Drawing.Point(558, 597);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(179, 40);
            this.panel3.TabIndex = 8;
            this.panel3.Visible = false;
            // 
            // zoomOnCenterRadioButton
            // 
            this.zoomOnCenterRadioButton.AutoSize = true;
            this.zoomOnCenterRadioButton.Checked = true;
            this.zoomOnCenterRadioButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.zoomOnCenterRadioButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.zoomOnCenterRadioButton.Location = new System.Drawing.Point(0, 0);
            this.zoomOnCenterRadioButton.Name = "zoomOnCenterRadioButton";
            this.zoomOnCenterRadioButton.Size = new System.Drawing.Size(179, 17);
            this.zoomOnCenterRadioButton.TabIndex = 3;
            this.zoomOnCenterRadioButton.TabStop = true;
            this.zoomOnCenterRadioButton.Text = "Zoom on center";
            this.zoomOnCenterRadioButton.UseVisualStyleBackColor = true;
            this.zoomOnCenterRadioButton.Visible = false;
            // 
            // zoomOnMouseRadioButton
            // 
            this.zoomOnMouseRadioButton.AutoSize = true;
            this.zoomOnMouseRadioButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.zoomOnMouseRadioButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.zoomOnMouseRadioButton.Location = new System.Drawing.Point(0, 23);
            this.zoomOnMouseRadioButton.Name = "zoomOnMouseRadioButton";
            this.zoomOnMouseRadioButton.Size = new System.Drawing.Size(179, 17);
            this.zoomOnMouseRadioButton.TabIndex = 4;
            this.zoomOnMouseRadioButton.Text = "Zoom on mouse";
            this.zoomOnMouseRadioButton.UseVisualStyleBackColor = true;
            this.zoomOnMouseRadioButton.Visible = false;
            // 
            // zLevelChooser
            // 
            this.zLevelChooser.InterceptArrowKeys = false;
            this.zLevelChooser.Location = new System.Drawing.Point(759, 616);
            this.zLevelChooser.Name = "zLevelChooser";
            this.zLevelChooser.Size = new System.Drawing.Size(43, 20);
            this.zLevelChooser.TabIndex = 9;
            this.zLevelChooser.TabStop = false;
            this.zLevelChooser.Text = "0";
            this.zLevelChooser.SelectedItemChanged += new System.EventHandler(this.zLevelChooser_SelectedItemChanged);
            this.zLevelChooser.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.zLevelChooser_MouseWheel);
            // 
            // zLevelLabel
            // 
            this.zLevelLabel.AutoSize = true;
            this.zLevelLabel.Location = new System.Drawing.Point(756, 597);
            this.zLevelLabel.Name = "zLevelLabel";
            this.zLevelLabel.Size = new System.Drawing.Size(169, 13);
            this.zLevelLabel.TabIndex = 10;
            this.zLevelLabel.Text = "Z Level: (negative is underground)";
            // 
            // mapBox
            // 
            this.mapBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.mapBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapBox.Location = new System.Drawing.Point(0, 0);
            this.mapBox.Name = "mapBox";
            this.mapBox.Size = new System.Drawing.Size(913, 572);
            this.mapBox.TabIndex = 0;
            this.mapBox.TabStop = false;
            this.mapBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.mapBox_MouseWheel);
            this.mapBox.Click += new System.EventHandler(this.mapBox_Click);
            this.mapBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapBox_MouseDown);
            this.mapBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapBox_MouseMove);
            this.mapBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapBox_MouseUp);
            // 
            // MapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 648);
            this.Controls.Add(this.zLevelLabel);
            this.Controls.Add(this.zLevelChooser);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "MapViewer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "DF Map Compressor\'s Map Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MapViewer_FormClosed);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.form_MouseWheel);
            this.Resize += new System.EventHandler(this.mapViewer_Resize);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.mapBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private WindowBox mapBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer actionTimer;
        private System.Windows.Forms.RadioButton mouseDraggingRepelsMapRadioButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton mouseDragsMapRadioButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton zoomOnCenterRadioButton;
        private System.Windows.Forms.RadioButton zoomOnMouseRadioButton;
        private DomainUpDownIgnoringMousewheel zLevelChooser;
        private System.Windows.Forms.Label zLevelLabel;

    }
}