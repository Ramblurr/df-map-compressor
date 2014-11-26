using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DwarfFortressMapCompressor {
    public delegate void TileSizeChosen(TileSizeChooser chooser, Bitmap bitmap, int tileSizeX, int tileSizeY, ProgressForm form);
    public partial class TileSizeChooser : Form {
        
        TileSizeChosen tileSizeChosen;
        Bitmap bitmap;
        ProgressForm progressForm;

        public TileSizeChooser(Bitmap bitmap, ProgressForm progressForm, TileSizeChosen del) {
            this.tileSizeChosen = del;
            this.bitmap = bitmap;
            this.progressForm = progressForm;
            InitializeComponent();
        }

        private void goButton_Click(object sender, EventArgs e) {
            tileSizeChosen(this, bitmap, (int) Math.Round(xSizeBox.Value), (int) Math.Round(ySizeBox.Value), progressForm);
        }

        private void guessButton_Click(object sender, EventArgs e) {
            tileSizeChosen(this, bitmap, 0, 0, progressForm);
        }
    }
}