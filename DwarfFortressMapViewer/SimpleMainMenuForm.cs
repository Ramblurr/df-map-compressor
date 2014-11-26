using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DwarfFortressMapCompressor {
    public partial class SimpleMainMenuForm : Form {
        internal static SimpleMainMenuForm instance = null;
        
        public SimpleMainMenuForm() {
            instance = this;
            InitializeComponent();
            this.AutoCompressButton.Height = 200;
#if MONO
            int colorMatchSensitivity = 12;
#else
            int colorMatchSensitivity = Properties.Settings.Default.colorMatchSensitivity;
            this.colorMatchSensitivityTextBox.Text = ""+colorMatchSensitivity;
#endif
            if (IsLinux()) {
                this.Width+=100;
                foreach (Control control in Controls) {
                    if (control!=label1 && control!=label2 && control!=label3) {
                        control.Location = new Point(control.Location.X+50, control.Location.Y);
                        if (control is CheckBox || control is Button) {
                            control.Width += 50;
                        }
                    } else if (control==label2) {
                        control.Location = new Point(control.Location.X+25, control.Location.Y);
                    } else if (control==label3) {
                        control.Location = new Point(control.Location.X+25, control.Location.Y);
                    }
                    /*if (control is CheckBox) {
                        Label newLabel = new Label();

                        //TODO
                    }*/
                }
            }
        }

        private void OutputFlashFilesButton_Click(object sender, EventArgs e) {
            MainMenuForm form = MainMenuForm.instance;
            if (form==null) {
                form = new MainMenuForm();
            }
            form.DoOutputFlashFilesButton(this, deleteImagesCheckbox.Checked, doNotAnalyzeCheckbox.Checked, eraseDarkGreyGroundCheckbox.Checked, colorMatchSensitivityTextBox.Text);
        }

        private void SwitchToAdvancedInterfaceButton_Click(object sender, EventArgs e) {
            MainMenuForm form = MainMenuForm.instance;
            if (form==null) {
                form = new MainMenuForm();
            }
            form.CloneCheckboxes(deleteImagesCheckbox.Checked, doNotAnalyzeCheckbox.Checked, eraseDarkGreyGroundCheckbox.Checked, colorMatchSensitivityTextBox.Text);
            form.Show();
            form.Location=this.Location;
            this.Hide();
        }

        private void VisitArchiveButton_Click(object sender, EventArgs e) {
            MainMenuForm form = MainMenuForm.instance;
            if (form==null) {
                form = new MainMenuForm();
            }
            form.DoVisitArchiveButton(this);
        }

        private void compressCmvButton_Click(object sender, EventArgs e) {
            MainMenuForm form = MainMenuForm.instance;
            if (form==null) {
                form = new MainMenuForm();
            }
            form.DoCompressCMVButton(this);
        }

        private void compressCmvButton2_Click(object sender, EventArgs e) {
            MainMenuForm form = MainMenuForm.instance;
            if (form==null) {
                form = new MainMenuForm();
            }
            form.DoCompressCMV2Button(this);
        }

        private void ViewFortressMapButton_Click(object sender, EventArgs e) {
            MainMenuForm form = MainMenuForm.instance;
            if (form==null) {
                form = new MainMenuForm();
            }
            form.ViewFortressMapButton_Click(sender, e);
        }

        public void CloneCheckboxes(bool delImgValue, bool doNotAnalyzeValue, bool eraseDarkGreyGround, string colorMatchSensitivityString) {
            deleteImagesCheckbox.Checked = delImgValue;
            doNotAnalyzeCheckbox.Checked = doNotAnalyzeValue;
            eraseDarkGreyGroundCheckbox.Checked = eraseDarkGreyGround;
            colorMatchSensitivityTextBox.Text = colorMatchSensitivityString;
        }

        public bool IsLinux() {
            //Console.WriteLine("Font name: "+this.Font.Name);
            return this.Font.Name!="Microsoft Sans Serif";
        }
    }
}