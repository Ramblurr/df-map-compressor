using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.Layout;
using System.Drawing.Drawing2D;

namespace DwarfFortressMapCompressor {
    class WindowBox : PictureBox {
        public WindowBox() : base() {
            base.Image = null;
        }

        private Image image;
        private Rectangle srcRect;
        private Rectangle destRect;
        
        public void SetValues(Image bitmap, Rectangle destRect, int mapSrcX, int mapSrcY, int mapSrcWidth, int mapSrcHeight) {
            this.image = bitmap;
            this.destRect = destRect;
            this.srcRect = new Rectangle(mapSrcX, mapSrcY, mapSrcWidth, mapSrcHeight);
            base.Invalidate();
        }

        public Rectangle GetWindowRect() {
            Rectangle rect = base.ClientRectangle;
            rect.X += base.Padding.Left;
            rect.Y += base.Padding.Top;
            rect.Width -= base.Padding.Horizontal;
            rect.Height -= base.Padding.Vertical;
            return rect;
        }

        protected override void OnPaint(PaintEventArgs eventArgs) {
            if (this.image==null) {
                base.OnPaint(eventArgs);
            } else {
                Graphics g = eventArgs.Graphics;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(this.image, destRect, srcRect, GraphicsUnit.Pixel);
            }
            //base.OnPaint(eventArgs);
        }
    }
}
