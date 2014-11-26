using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DwarfFortressMapCompressor {
    public partial class MapViewer : Form {
        private MainMenuForm mainMenuForm;
        private string mapFilename;
        private string mapExtension;
        private ZLayerList zLayerList = null;
        private int zLayer = 0;
        private float mapZoom = 20.0f;
        private Bitmap mapBitmap = null;
        //private Bitmap backBuffer = null;
        private float mapCenterX = 0;
        private float mapCenterY = 0;
        private float mapSrcX = 0;
        private float mapSrcY = 0;
        private float mapSrcWidth = 0;
        private float mapSrcHeight = 0;
        private int accumulatedMousewheel = 0;
        private int startMousePosX;
        private int startMousePosY;
        private int lastMousePosX;
        private int lastMousePosY;
        private bool mouseDown;
        private float mapReverseZoomX;
        private float mapReverseZoomY;

        private Rectangle mapDestRect;
        private Rectangle backbufferDest;
        private float aspectRatio;

        //private byte[] dataBytes;
        //private byte[] blitDataBytes;
        //private int[, ,] blitBuffer;

        public MapViewer() {
            InitializeComponent();
            mouseDown=false;
#if MONO
            bool mvmd = true;
#else
            bool mvmd = Properties.Settings.Default.mapViewerMouseDrags;
            
#endif
            mouseDragsMapRadioButton.Checked=mvmd;
            mouseDraggingRepelsMapRadioButton.Checked=!mvmd;
        }

        public void ShowMap(MainMenuForm mainMenuForm, string filename, string extension) {
            this.mainMenuForm = mainMenuForm;
            this.mapFilename = filename;
            this.mapExtension = extension;
            zLayerList = mainMenuForm.GetZLayerList(filename, extension);
            zLayer = zLayerList.GetInitialZLayer();
            zLevelChooser.Items.Clear();
            zLevelChooser.Items.InsertRange(0, zLayerList.GetZLayers());
            //zLevelChooser.Text = ""+zLayer;
            zLevelChooser.SelectedItem = zLayer;
            zLevelChooser.Visible = true;
            zLevelLabel.Visible = true;
            ShowMap(mainMenuForm.Run_DecodeFortressMap(filename, extension, zLayer, false), false);
        }

        public void ShowMap(Bitmap bitmap, bool noZLayers) {
            if (noZLayers) {
                this.mainMenuForm = null;
                this.mapFilename = "";
                this.mapExtension = ""; 
                zLevelChooser.Visible = false;
                zLevelLabel.Visible = false;
                zLayerList = null;
                zLayer = 0;
                zLevelChooser.Items.Clear();
                ArrayList arrayList = new ArrayList();
                arrayList.Add(0);
                zLevelChooser.Items.InsertRange(0, arrayList);
                zLevelChooser.Text = "0";
            }
            Rectangle mapBoxRect = mapBox.GetWindowRect();
            if (mapBoxRect.Height<10 || mapBoxRect.Width<10) {
                return; //Let's try not to crash.
            }
            this.aspectRatio = ((float) bitmap.Width / (float) bitmap.Height) / ((float) mapBox.Width / (float) mapBox.Height);
            //Console.WriteLine("Aspect ratio is "+aspectRatio);
            this.mapBitmap = bitmap;
            this.mapZoom = 1.0f;
            this.mapCenterX = 0.5f * bitmap.Width;
            this.mapCenterY = 0.5f * bitmap.Height;
            mapDestRect = new Rectangle(0, 0, mapBoxRect.Width, mapBoxRect.Height);
            backbufferDest = new Rectangle(0, 0, mapBoxRect.Width, mapBoxRect.Height);
            //backBuffer = new Bitmap((int) mapBoxRect.Width, (int) mapBoxRect.Height);
            //blitBuffer = new int[mapBoxRect.Width, mapBoxRect.Height, 4];
            
            /*BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = data.Scan0;
            dataBytes = new byte[data.Width*data.Height*3];
            System.Runtime.InteropServices.Marshal.Copy(ptr, dataBytes, 0, dataBytes.Length);
            bitmap.UnlockBits(data);

            data = backBuffer.LockBits(new Rectangle(0, 0, backBuffer.Width, backBuffer.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            ptr = data.Scan0;
            blitDataBytes = new byte[data.Width*data.Height*3];
            System.Runtime.InteropServices.Marshal.Copy(blitDataBytes, 0, ptr, blitDataBytes.Length);
            backBuffer.UnlockBits(data);*/
            //mapBox.Image = backBuffer;
            
            RecalculateZoom();
            GC.Collect();
        }

        private bool RecalculateZoom() {
            Rectangle mapBoxRect = mapBox.GetWindowRect();
            this.mapSrcWidth = mapBitmap.Width / this.mapZoom;
            this.mapSrcHeight = (mapBitmap.Height * aspectRatio) / this.mapZoom;
            this.mapSrcX = this.mapCenterX - 0.5f * this.mapSrcWidth;
            this.mapSrcY = this.mapCenterY - 0.5f * this.mapSrcHeight;
            if (mapSrcX<0) {
                //Console.WriteLine("mapSrcX<0");
                this.mapCenterX -= this.mapSrcX;
                this.mapSrcX = 0;
            } else if (mapSrcX + mapSrcWidth>=mapBitmap.Width) {
                //Console.WriteLine("mapSrcX>wv");
                float oldMapSrcX = mapSrcX;
                mapSrcX = mapBitmap.Width-mapSrcWidth;
                mapCenterX += (mapSrcX - oldMapSrcX);
            }
            if (mapSrcY<0) {
                //Console.WriteLine("mapSrcY<0");
                this.mapCenterY -= this.mapSrcY;
                this.mapSrcY = 0;
            } else if (mapSrcY + mapSrcHeight>=mapBitmap.Height) {
                //Console.WriteLine("mapSrcY>wy");
                float oldMapSrcY = mapSrcY;
                mapSrcY = mapBitmap.Height-mapSrcHeight;
                mapCenterY += (mapSrcY - oldMapSrcY);
            }
            if (mapSrcWidth < mapBoxRect.Width) {
                mapSrcWidth = mapBoxRect.Width;
                return false;
            }
            if (mapSrcHeight < mapBoxRect.Height) {
                mapSrcHeight = mapBoxRect.Height;
                return false;
            }
            //At maximum zoom:
            //mapSrcWidth = mapBox.Width, and mapSrcHeight = mapBox.Height
            //using (Graphics g = Graphics.FromImage(backBuffer)) {
                //g.DrawImage(mapBitmap, backbufferDest, mapSrcX, mapSrcY, mapSrcWidth, mapSrcHeight, GraphicsUnit.Pixel);
            mapReverseZoomX = mapSrcWidth / mapBoxRect.Width;
            mapReverseZoomY = mapSrcHeight / mapBoxRect.Height;

            
            //Blit to: Bitmap backBuffer [0, 0] to [mapBox.Width, mapBox.Height]
            //From: mapBitmap [mapSrcX, mapSrcY] to [mapSrcX+mapSrcWidth, mapSrcY+mapSrcHeight]
            /*
            blitBuffer[0, 0, 0]=0;
            blitBuffer[0, 0, 1]=0;
            blitBuffer[0, 0, 2]=0;
            blitBuffer[0, 0, 3]=0;
            float numXPer = (float) mapSrcWidth/(float) mapBox.Width;
            float numYPer = (float) mapSrcHeight/(float) mapBox.Height;
            float ystep = 0.0f;
            int destY = 0;
            int initializedY = 0;
            int index = 0;
            for (int y=(int) mapSrcY, relY=0; relY<mapSrcHeight; y++, relY++, ystep+=1.0f) {
                if (ystep>numYPer) {
                    ystep-=numYPer;
                    destY+=1;
                }
                int destX = 0;
                float xstep = 0.0f;
                int initializedX = 0;
                for (int x=(int) mapSrcX, relX=0; relX<mapSrcWidth; x++, relX++, xstep+=1.0f, index+=3) {
                    if (xstep>numXPer) {
                        xstep-=numXPer;
                        destX+=1;
                    }
                    if (initializedY<destY || initializedX<destX) {
                        blitBuffer[destX, destY, 0]=dataBytes[index];
                        blitBuffer[destX, destY, 1]=dataBytes[index+1];
                        blitBuffer[destX, destY, 2]=dataBytes[index+2];
                        blitBuffer[destX, destY, 3]=1;
                        initializedX = destX;
                        initializedY = destY;
                    } else {
                        blitBuffer[destX, destY, 0]+=dataBytes[index];
                        blitBuffer[destX, destY, 1]+=dataBytes[index+1];
                        blitBuffer[destX, destY, 2]+=dataBytes[index+2];
                        blitBuffer[destX, destY, 3]+=1;
                    }
                }
            }
            index=0;
            for (int y=0; y<backBuffer.Height; y++) {
                for (int x=0; x<backBuffer.Width; x++, index+=3) {
                    if (blitBuffer[x, y, 0]!=0) {
                        int rc = blitBuffer[x, y, 2];
                        int gc = blitBuffer[x, y, 1];
                        int bc = blitBuffer[x, y, 0];
                    }
                    blitDataBytes[index]=(byte)(blitBuffer[x, y, 0]/blitBuffer[x, y, 3]);
                    blitDataBytes[index+1]=(byte)(blitBuffer[x, y, 1]/blitBuffer[x, y, 3]);
                    blitDataBytes[index+2]=(byte)(blitBuffer[x, y, 2]/blitBuffer[x, y, 3]);
                }
            }
            BitmapData data = backBuffer.LockBits(new Rectangle(0, 0, backBuffer.Width, backBuffer.Height), ImageLockMode.ReadWrite, backBuffer.PixelFormat);
            IntPtr ptr = data.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(ptr, blitDataBytes, 0, blitDataBytes.Length);
            backBuffer.UnlockBits(data);
            */
            Blit();
            //}
            
            return true;
        }

        private void Blit() {
            //using (Graphics g = Graphics.FromImage(backBuffer)) {
                //g.DrawImage(mapBitmap, backbufferDest, mapSrcX, mapSrcY, mapSrcWidth, mapSrcHeight, GraphicsUnit.Pixel);
            //}
            //mapBox.Image=backBuffer;
            mapBox.SetValues(mapBitmap, backbufferDest, (int) mapSrcX, (int) mapSrcY, (int) mapSrcWidth, (int) mapSrcHeight);
        }

        private void mapBox_Click(object sender, EventArgs e) {
            //Console.WriteLine("Click.");
        }

        private void mapBox_MouseDown(object sender, MouseEventArgs e) {
            //Console.WriteLine("MouseDown "+e.Button+" "+e.Location);
            if (e.Button==MouseButtons.Left) {
                startMousePosX = e.X;
                startMousePosY = e.Y;
                lastMousePosX = startMousePosX;
                lastMousePosY = startMousePosY;
                mouseDown = true;
                actionTimer.Enabled=true;
            }
        }

        private void mapBox_MouseUp(object sender, MouseEventArgs e) {
            HandleMouseMove(e.X, e.Y, true);
            mouseDown = false;
            actionTimer.Enabled=false;
        }

        void mapBox_MouseMove(object sender, MouseEventArgs e) {
            HandleMouseMove(e.X, e.Y, false);
        }

        private void mapBox_MouseWheel(object sender, MouseEventArgs e) {
            //Console.WriteLine("MouseWheel "+e.Delta+" "+e.Button+" "+e.Location);
            GhrkOnMouseWheel(e);
        }

        public void zLevelChooser_MouseWheel(object sender, MouseEventArgs e) {
            //GhrkOnMouseWheel(e);
        }

        public void panel1_MouseWheel(object sender, MouseEventArgs e) {
            //Console.WriteLine("MouseWheel2 "+e.Delta+" "+e.Button+" "+e.Location);
            GhrkOnMouseWheel(e);
        }
        public void form_MouseWheel(object sender, MouseEventArgs e) {
            //Console.WriteLine("MouseWheel3 "+e.Delta+" "+e.Button+" "+e.Location);
            GhrkOnMouseWheel(e);
        }
        public void GhrkOnMouseWheel(MouseEventArgs e) {
            int wheelMovedAmount = e.Delta;
            accumulatedMousewheel += wheelMovedAmount;
            actionTimer.Enabled=false;
            actionTimer.Interval=200; 
            actionTimer.Enabled=true;
        }

        /*private void mapBox_Paint(object sender, PaintEventArgs e) {
                            
        }*/

        internal void ReInit() {
            this.mapBitmap = null;
        }

        private void actionTimer_Tick(object sender, EventArgs e) {
            bool wheelUsed=false;
            if (accumulatedMousewheel!=0) {
                int absAmount = Math.Abs(accumulatedMousewheel);

                float zoomMultiplier = absAmount/2400.0f;
                if (accumulatedMousewheel>0) {
                    zoomMultiplier = 1.0f+zoomMultiplier;
                } else {
                    zoomMultiplier = 1.0f-zoomMultiplier;
                }
                mapZoom *= zoomMultiplier;
                if (mapZoom<1.0f) {
                    mapZoom=1.0f;
                }
                float xChange = 0.0f;
                float yChange = 0.0f;
                if (zoomOnMouseRadioButton.Checked) {
                    Point p = mapBox.PointToScreen(new Point((int) (mapBox.Left + mapBox.Width * 0.5f), (int) (mapBox.Top + mapBox.Height * 0.5f)));
                    float mapBoxCenterX = p.X;
                    float mapBoxCenterY = p.Y;
                    xChange = (lastMousePosX-mapBoxCenterX) * mapReverseZoomX * zoomMultiplier;
                    yChange = (lastMousePosY-mapBoxCenterY) * mapReverseZoomY * zoomMultiplier;
                    this.mapCenterX += xChange;
                    this.mapCenterY += yChange;
                }
                if (!RecalculateZoom()) {
                    //Console.WriteLine("Map zoom now "+mapZoom);
                    this.mapCenterX -= xChange;
                    this.mapCenterY -= yChange;
                    mapZoom /= zoomMultiplier;
                    RecalculateZoom();
                }
                accumulatedMousewheel=0;
                
            }
            if (mouseDown) {
                HandleMouseMove(lastMousePosX, lastMousePosY, true);
            }
            if (!mouseDown && !wheelUsed) {
                actionTimer.Enabled=false;
            }
            
        }

        private void HandleMouseMove(int x, int y, bool final) {
            lastMousePosX = x;
            lastMousePosY = y;
            if (!final) {
                actionTimer.Enabled=true;
            } else {
                DraggedFromTo();
            }
        }

        private void DraggedFromTo() {
            float xChange = (lastMousePosX-startMousePosX) * mapReverseZoomX;
            float yChange = (lastMousePosY-startMousePosY) * mapReverseZoomY;
            if (mouseDragsMapRadioButton.Checked) {
                this.mapCenterX -= xChange;
                this.mapCenterY -= yChange;
            } else {
                this.mapCenterX += xChange;
                this.mapCenterY += yChange;
            }
            startMousePosX=lastMousePosX;
            startMousePosY=lastMousePosY;
            RecalculateZoom();
        }

        private void mapViewer_Resize(object sender, EventArgs e) {
            //resize components:
            panel2.Top = this.Height - 85;
            panel3.Top = this.Height - 85;
            label1.Top = this.Height - 85;
            panel1.Top = 12;
            panel1.Left = 12;
            panel1.Height = this.Height - 110;
            panel1.Width = this.Width - 32;

            ShowMap(mapBitmap, false);
        }

        private void mouseDragsMapRadioButton_CheckedChanged(object sender, EventArgs e) {
            #if !MONO
            Properties.Settings.Default.mapViewerMouseDrags = true;
            Properties.Settings.Default.Save();
            #endif
        }

        private void mouseDraggingRepelsMapRadioButton_CheckedChanged(object sender, EventArgs e) {
            #if !MONO
            Properties.Settings.Default.mapViewerMouseDrags = false;
            Properties.Settings.Default.Save();
            #endif
        }

        private void MapViewer_FormClosed(object sender, FormClosedEventArgs e) {
            mapBitmap.Dispose();
            mapBitmap = null;
            GC.Collect();
        }

        private void zLevelChooser_ValueChanged(object sender, EventArgs e) {
            
        }

        private void zLevelChooser_SelectedItemChanged(object sender, EventArgs e) {
            if (zLayerList==null) {
                if (((int)zLevelChooser.SelectedItem)!=0) {
                    zLevelChooser.SelectedItem = 0;
                }
                return;
            }
            int value = 0;
            try {
                value = Int32.Parse(zLevelChooser.Text);
            } catch(Exception) {
            }
            int newZLayer = zLayerList.ValidateZLayer(value);
            if (newZLayer!=zLayer) {
                zLayer = newZLayer;
                //zLevelChooser.Text = ""+zLayer;
                zLevelChooser.SelectedItem = zLayer;
                ShowMap(mainMenuForm.Run_DecodeFortressMap(mapFilename, mapExtension, zLayer, false), false);
            } else {
                //zLevelChooser.Text = ""+zLayer;
            }
        }
    }
}