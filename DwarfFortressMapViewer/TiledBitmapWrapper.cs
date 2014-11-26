using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;

namespace DwarfFortressMapCompressor {
    class TiledBitmapWrapper : IDisposable {
        protected bool valid;
        protected byte[] dataBytes;
        protected int tileWidth;
        protected int tileHeight;
        protected int bitmapWidth;
        protected int bitmapHeight;
        protected int realBitmapWidth;
        protected int realBitmapHeight;
        protected int tileByteSize;
        protected int numTilesX;
        protected int numTilesY;
        protected int numTiles;

        protected bool waitingForChooser;
		
		public int BitmapWidth {
			get {
				return bitmapWidth;
			}
		}
		
		public int BitmapHeight {
			get {
				return bitmapHeight;
			}
		}
		
        public int NumTiles {
            get {
                return numTiles;
            }
        }

        public int NumTilesX {
            get {
                return numTilesX;
            }
        }
        public int NumTilesY {
            get {
                return numTilesY;
            }
        }
        public int TileWidth {
            get {
                return tileWidth;
            }
        }
        public int TileHeight {
            get {
                return tileHeight;
            }
        }

        public TiledBitmapWrapper(byte[] bytes, int bitmapWidth, int bitmapHeight, int tileWidth, int tileHeight) {
            valid = true;
            this.dataBytes=bytes;
            this.bitmapWidth=bitmapWidth;
            this.bitmapHeight=bitmapHeight;
            this.realBitmapWidth=bitmapWidth;
            this.realBitmapHeight=bitmapHeight;
            this.tileWidth=tileWidth;
            this.tileHeight=tileHeight;
            this.tileByteSize = tileWidth*tileHeight*3;
            numTilesX = bitmapWidth/tileWidth;
            numTilesY = bitmapHeight/tileHeight;
            numTiles = numTilesX * numTilesY;
        }
        public TiledBitmapWrapper(Bitmap bitmap, int tileWidth, int tileHeight, ProgressForm progressForm) {
            valid=false;
            if (tileWidth==0 && tileHeight==0) {
                TileSizeChooser chooser = new TileSizeChooser(bitmap, progressForm, new TileSizeChosen(InitWrapper));
                waitingForChooser=true;
                chooser.Show();
                while (waitingForChooser) {
                    Application.DoEvents();
                    Thread.Sleep(10); 
                }
            } else {
                Init(bitmap, tileWidth, tileHeight, progressForm);
            }
        }
        public TiledBitmapWrapper(string filename, int tileWidth, int tileHeight, ProgressForm progressForm) {
            valid=false;
            if (progressForm!=null) {
                string[] splitFolderFilename = filename.Split('\\', '/');
                progressForm.SetStatus("Loading "+splitFolderFilename[splitFolderFilename.Length-1]);
            }
            Bitmap bitmap = (Bitmap) Bitmap.FromFile(filename);
            if (tileWidth==0 && tileHeight==0) {
                TileSizeChooser chooser = new TileSizeChooser(bitmap, progressForm, new TileSizeChosen(InitWrapper));
                waitingForChooser=true;
                chooser.Show();
                while (waitingForChooser) {
                    Application.DoEvents(); 
                    Thread.Sleep(10);                    
                }
            } else {
                Init(bitmap, tileWidth, tileHeight, progressForm);
            }
        }
        public void InitWrapper(TileSizeChooser chooser, Bitmap bitmap, int tileWidth, int tileHeight, ProgressForm progressForm) {
            chooser.Hide();
            chooser.Dispose();
            Init(bitmap, tileWidth, tileHeight, progressForm);
            waitingForChooser = false;
        }

        public void SetupData(Bitmap bitmap, int tileWidth, int tileHeight) {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.bitmapWidth=bitmap.Width;
            this.bitmapHeight=bitmap.Height;
            this.realBitmapWidth=bitmap.Width;
            this.realBitmapHeight=bitmap.Height;
            this.numTilesX = bitmapWidth/tileWidth;
            this.numTilesY = bitmapHeight/tileHeight;
            this.bitmapWidth = numTilesX*tileWidth;
            this.bitmapHeight = numTilesY*tileHeight;
            this.tileByteSize = tileWidth*tileHeight*3;

            this.numTiles = numTilesX * numTilesY;
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            IntPtr ptr = data.Scan0;
            int bitmapByteSize = bitmap.Width * bitmap.Height * 3;
            this.dataBytes = new byte[bitmapByteSize];
            System.Runtime.InteropServices.Marshal.Copy(ptr, dataBytes, 0, bitmapByteSize);
            bitmap.UnlockBits(data);
        }

        public void GuessTileSize(Bitmap bitmap, out int bestWidth, out int bestHeight) {
            int width = 8;
            int height = 8;
            bestWidth = 8;
            bestHeight = 8;
            int bestCount = 0;
            ProgressForm progressForm = new ProgressForm();
            progressForm.Show();
            progressForm.SetStatus("Attempting to determine tile size...");
            int cycles = 0;
            int MaxSize = 24;
            int MinSize = 8;
            int NumScanXTiles = 3;
            int maxCycles = (MaxSize-MinSize+1)*(MaxSize-MinSize+1);
            progressForm.SetMaximum(maxCycles);
            BitmapPartBytes[,] tiles = new BitmapPartBytes[bitmapHeight/MinSize, NumScanXTiles];
            //int[,] matches = new int[tiles.GetLength(0), tiles.GetLength(1)];
            while (true) {
                SetupData(bitmap, width, height);
                
                //find most detailed tile
                int highDetail = 0;
                int highDetailX = 0;
                int highDetailY = 0;
                BitmapPartBytes highDetailTile = null;
                for (int y=0; y<numTilesY; y++) {
                    for (int x=0; x<NumScanXTiles; x++) {
                        BitmapPartBytes tile = GetTile(numTilesX-x-1, y);
                        tiles[y, x] = tile;
                        int tileDetail = tile.GetDetail();
                        if (tileDetail > highDetail) {
                            highDetail = tileDetail;
                            highDetailTile = tile;
                            highDetailX = x;
                            highDetailY = y;
                        }
                    }
                }
                int matchCount = 0;
                if (highDetailTile!=null) {
                    //int bestX = 0;
                    //int bestY = 0;
                    //int bestMatches = 0;
                    for (int y=0; y<numTilesY; y++) {
                        for (int x=0; x<NumScanXTiles; x++) {
                            BitmapPartBytes tile = tiles[y, x];
                            if (tile.Equals(highDetailTile)) {
                                matchCount+=1;
                            }
                            /*for (int y2=0; y2<numTilesY; y2++) {
                                for (int x2=0; x2<NumScanXTiles; x2++) {
                                    BitmapPartBytes tile2 = tiles[y2, x2];
                                    if ((x2==x && y2==y) || tile.Equals(tile2)) {
                                        matches[y, x] += 1;
                                    }
                                }
                            }
                            if (matches[y, x] > bestMatches) {
                                bestMatches = matches[y, x];
                                bestX = x;
                                bestY = y;
                            }*/
                        }
                    }
                    //int bestDetail = tiles[bestY, bestX].GetDetail();
                    int size = (width*height);
                    float detailWeight = 1.0f-Math.Abs(((float) highDetail / (float) size) - 0.5f);
                    float sizeWeight = (Math.Abs(width-height)*0.2f+1.0f);
                    int weight = (int) ((matchCount * detailWeight) / sizeWeight);
                    Console.WriteLine("Highest detail tile for W"+width+",H"+height+" is "+highDetailX+","+highDetailY+" (at pixel position "+highDetailX*tileWidth+","+highDetailY*tileHeight+") with detail "+highDetail+" and matches "+matchCount+". Detail weight "+detailWeight+", sizeWeight "+1.0f/sizeWeight+", weight "+weight);
                    //detailWeight = 1.0f-Math.Abs(((float) bestDetail / (float) size) - 0.5f);
                    //sizeWeight = (Math.Abs(width-height)*0.2f+1.0f);
                    //weight = (int) ((bestMatches * detailWeight) / sizeWeight);
                    //Console.WriteLine("Best tile for W"+width+",H"+height+" is "+bestX+","+bestY+" (at pixel position "+bestX*tileWidth+","+bestY*tileHeight+") with detail "+bestDetail+" and matches "+bestMatches+". Detail weight "+detailWeight+", sizeWeight "+1.0f/sizeWeight+", weight "+weight);
                    
                    if (weight>bestCount) {
                        bestCount = weight;
                        bestWidth = width;
                        bestHeight = height;
                    }
                } else {
                    Console.WriteLine("No non-blank tiles found for W"+width+",H"+height+"!?");
                }
                cycles+=1;
                progressForm.SetProgress(cycles);
                width+=1;
                if (width>MaxSize) {
                    width = MinSize;
                    height+=1;
                    if (height>MaxSize) {
                        break;
                    }
                }
                //Console.WriteLine("Trying W"+width+",H"+height);
            }
            Console.WriteLine("Best match: W"+bestWidth+",H"+bestHeight+" with "+bestCount+" weight for the most detailed rightmost tile.");
            MessageBox.Show(MainMenuForm.instance.LinuxInsertLineBreaks("We think the tiles are "+bestWidth+" pixels wide and "+bestHeight+" pixels high. (If that's wrong, and you want to see how the correct size was ranked, you can run this program from a command prompt, and it will write out the weights it got for each potential tile size)"));
            progressForm.Hide();
            progressForm.Dispose();
            progressForm = null;
        }

        private void Init(Bitmap bitmap, int tileWidth, int tileHeight, ProgressForm progressForm) {
            if (progressForm!=null) {
                progressForm.SetProgress(25);
            }
            //Console.WriteLine("bitmap pixel format is "+bitmap.PixelFormat);
            if (bitmap.PixelFormat!=PixelFormat.Format24bppRgb) {
                Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(newBitmap)) {
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    g.DrawImage(bitmap, rect, rect, GraphicsUnit.Pixel);
                }
                bitmap = newBitmap;
                //throw new ApplicationException("Unexpected pixel format in image: "+bitmap.PixelFormat+". We expected it to be Format24bppRgb.");
            } else {
                //MessageBox.Show("Loading bitmap...");
            }
            this.bitmapWidth=bitmap.Width;
            this.bitmapHeight=bitmap.Height;
            this.realBitmapWidth=bitmap.Width;
            this.realBitmapHeight=bitmap.Height;
            if (tileWidth==0 && tileHeight==0) {
                GuessTileSize(bitmap, out tileWidth, out tileHeight);                
            }
            this.tileWidth=tileWidth;
            this.tileHeight=tileHeight;
            this.tileByteSize = tileWidth*tileHeight*3;
            int bitmapByteSize;
            //Correct for a DF bug which makes it sometimes write out a bmp with the wrong size
            numTilesX = bitmapWidth/tileWidth;
            numTilesY = bitmapHeight/tileHeight;
            this.bitmapWidth = numTilesX*tileWidth;
            this.bitmapHeight = numTilesY*tileHeight;
            
            numTiles = numTilesX * numTilesY;
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, realBitmapWidth, realBitmapHeight), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            //BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            if (progressForm!=null) {
                progressForm.SetProgress(50);
            }
            bitmapByteSize = bitmapWidth * bitmapHeight * 3;
            dataBytes = new byte[bitmapByteSize];
            IntPtr ptr = data.Scan0;
            int realRowSize = data.Stride;
            int rowSize = bitmapWidth + bitmapWidth + bitmapWidth;
            int pos = 0;
            if (progressForm!=null) {
                progressForm.SetProgress(75);
            }
            for (int y=0; y<bitmapHeight; y++) {
                System.Runtime.InteropServices.Marshal.Copy(ptr, dataBytes, pos, rowSize);
                try {
                    ptr=new IntPtr(ptr.ToInt32()+realRowSize);
                } catch (OverflowException) {
                    ptr=new IntPtr(ptr.ToInt64()+realRowSize);
                }
                pos+=rowSize;
            }
            bitmap.UnlockBits(data);
            realBitmapHeight = bitmapHeight;
            realBitmapWidth = bitmapWidth;
            if (progressForm!=null) {
                progressForm.SetProgress(100);
            }
            valid=true;
        }

        public void ChangeColor(byte rfrom, byte gfrom, byte bfrom, byte rto, byte gto, byte bto) {
            for (int i=0; i<dataBytes.Length; i+=3) {
                if (dataBytes[i]==rfrom && dataBytes[i+1]==gfrom && dataBytes[i+2]==bfrom) {
                    dataBytes[i]=rto;
                    dataBytes[i+1]=gto;
                    dataBytes[i+2]=bto;
                }
            }
        }

        public BitmapPartBytes GetTile(int tileX, int tileY) {
            int startXPixel = tileX*tileWidth;
            int startYPixel = tileY*tileHeight;
            int rowStart = (startYPixel*realBitmapWidth + startXPixel) * 3;
            int rowSize = realBitmapWidth+realBitmapWidth+realBitmapWidth;
            byte[] retval = new byte[tileByteSize];
            int bytePos = rowStart;
            int outBytePos = 0;

            /*if (tileWidth==14 && tileHeight==16) {
                rowStart = 0;
                for (int y=0; y<realBitmapHeight; y++, rowStart+=rowSize, bytePos = rowStart) {
                    for (int x=0; x<realBitmapWidth; x++, bytePos+=3) {
                        int b = dataBytes[bytePos];
                        int g = dataBytes[bytePos+1];
                        int r = dataBytes[bytePos+2];
                        if (b==41 && g==81 && r==104) {
                            Console.WriteLine("Found at "+x+","+y+".");
                        }
                    }
                }
                rowStart = (startYPixel*realBitmapWidth + startXPixel) * 3;
                bytePos = rowStart;
            }*/

            for (int y = startYPixel; y<startYPixel+tileHeight; y++) {
                for (int x=startXPixel; x<startXPixel+tileWidth; x++, bytePos+=3, outBytePos+=3) {
                    retval[outBytePos] = dataBytes[bytePos];
                    retval[outBytePos+1] = dataBytes[bytePos+1];
                    retval[outBytePos+2] = dataBytes[bytePos+2];
                }
                rowStart+=rowSize;
                bytePos = rowStart;
            }
            return new BitmapPartBytes((byte) tileWidth, (byte) tileHeight, retval);            
        }

        public void SetTile(int tileX, int tileY, BitmapPartBytes bitmapPartBytes) {
            int startXPixel = tileX*tileWidth;
            int startYPixel = tileY*tileHeight;
            int rowStart = (startYPixel*realBitmapWidth + startXPixel) * 3;
            int rowSize = realBitmapWidth+realBitmapWidth+realBitmapWidth;
            byte[] retval = bitmapPartBytes.DataBytes;
            int bytePos = rowStart;
            int outBytePos = 0;
            for (int y = startYPixel; y<startYPixel+tileHeight; y++) {
                for (int x=startXPixel; x<startXPixel+tileWidth; x++, bytePos+=3, outBytePos+=3) {
                    dataBytes[bytePos] = retval[outBytePos];
                    dataBytes[bytePos+1] = retval[outBytePos+1];
                    dataBytes[bytePos+2] = retval[outBytePos+2];
                }
                rowStart+=rowSize;
                bytePos = rowStart;
            }
        }

        public BitmapPartBytes[] SplitTileset(ProgressForm progressForm) {
            if (progressForm!=null) {
                progressForm.SetStatus("Splitting map into tiles.");
            } 
            BitmapPartBytes[] retval = new BitmapPartBytes[numTiles];

            int bytesPerBPB = tileWidth * tileHeight * 3;
            byte[,] tempBPBs = new byte[numTiles, bytesPerBPB];
            int inBytePos = 0;
            int tileBytePos = 0;
            int rowBytePos = 0;
            int rowByteLength = tileWidth + tileWidth + tileWidth;
            if (progressForm!=null) {
                progressForm.SetMaximum(bitmapHeight);
            }
            int numPixelsY = numTilesY*tileHeight;
            int numPixelsX = numTilesX*tileWidth;
            int tileNum = 0;
            for (int yPixel=0, yTilePixel=0; yPixel<bitmapHeight; yPixel++, yTilePixel++) {
                for (int xPixel=0; xPixel<realBitmapWidth; xPixel++, inBytePos+=3) {

                    if (xPixel<bitmapWidth) {
                        tempBPBs[tileNum, tileBytePos] = dataBytes[inBytePos];
                        tempBPBs[tileNum, tileBytePos+1] = dataBytes[inBytePos+1];
                        tempBPBs[tileNum, tileBytePos+2] = dataBytes[inBytePos+2];

                        tileBytePos+=3;
                        if (tileBytePos-rowBytePos >= rowByteLength) {
                            //next tile
                            tileBytePos = rowBytePos;
                            tileNum++;
                        }
                    }
                }
                if (yTilePixel >= TileHeight) {
                    rowBytePos = 0;
                    tileBytePos = 0;
                    yTilePixel = 0;
                } else {
                    rowBytePos = tileBytePos;
                    tileNum-=NumTilesX;
                }
                if (progressForm!=null) {
                    progressForm.SetProgress(yPixel+1);
                }
            }
            if (progressForm!=null) {
                progressForm.SetStatus("Finalizing split.");
                progressForm.SetMaximum(numTiles);
            }
            for (int numTile=0; numTile<numTiles; numTile++) {
                retval[numTile] = new BitmapPartBytes((byte) tileWidth, (byte) tileHeight, tempBPBs, numTile);
                progressForm.SetProgress(numTile+1);
            }
            return retval;
        }

        public BitmapPartBytes[,] SplitTilesetForTiff(ProgressForm progressForm) {
            if (progressForm!=null) {
                progressForm.SetStatus("Splitting map into tiles for TIFF output.");
            }
            BitmapPartBytes[,] retval = new BitmapPartBytes[numTilesX, numTilesY];

            int bytesPerBPB = tileWidth * tileHeight * 3;
            byte[, ,] tempBPBs = new byte[numTilesX, numTilesY, bytesPerBPB];
            int inBytePos = 0;
            int tileBytePos = 0;
            int rowBytePos = 0;
            int rowByteLength = tileWidth + tileWidth + tileWidth;
            if (progressForm!=null) {
                progressForm.SetMaximum(bitmapHeight);
            }
            int numPixelsY = numTilesY*tileHeight;
            int numPixelsX = numTilesX*tileWidth;
            int tileNum = 0;
            for (int yPixel=0, yTilePixel=0, yTile=0; yPixel<bitmapHeight; yPixel++, yTilePixel++) {
                for (int xPixel=0, xTile=0; xPixel<realBitmapWidth; xPixel++, inBytePos+=3) {
                    tempBPBs[xTile, yTile, tileBytePos] = dataBytes[inBytePos];
                    tempBPBs[xTile, yTile, tileBytePos+1] = dataBytes[inBytePos+1];
                    tempBPBs[xTile, yTile, tileBytePos+2] = dataBytes[inBytePos+2];

                    if (xPixel<bitmapWidth) {
                        tileBytePos+=3;
                        if (tileBytePos-rowBytePos >= rowByteLength) {
                            //next tile
                            tileBytePos = rowBytePos;
                            tileNum++;
                            xTile+=1;
                        }
                    }
                }
                if (yTilePixel >= TileHeight) {
                    rowBytePos = 0;
                    tileBytePos = 0;
                    yTilePixel = 0;
                    yTile+=1;
                } else {
                    rowBytePos = tileBytePos;
                    tileNum-=NumTilesX;
                }
                if (progressForm!=null) {
                    progressForm.SetProgress(yPixel+1);
                }
            }
            if (progressForm!=null) {
                progressForm.SetStatus("Finalizing split.");
                progressForm.SetMaximum(numTiles);
            }
            int numTile=0;
            for (int yTile=0; yTile<numTilesY; yTile++) {
                for (int xTile=0; xTile<numTilesX; xTile++) {
                    retval[xTile, yTile] = new BitmapPartBytes((byte) tileWidth, (byte) tileHeight, tempBPBs, xTile, yTile);
                    progressForm.SetProgress(numTile+1);
                }
            }
            return retval;
        }

        public Bitmap ToBitmap(ProgressForm progressForm) {
            if (progressForm!=null) {
                progressForm.SetStatus("Creating bitmap from map data.");
            }

            Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = data.Scan0;
            int realRowSize = data.Stride;
            int rowSize = bitmapWidth + bitmapWidth + bitmapWidth;
            int pos = 0;
            for (int y=0; y<bitmapHeight; y++) {
                System.Runtime.InteropServices.Marshal.Copy(dataBytes, pos, ptr, rowSize);
                try {
                    ptr=new IntPtr(ptr.ToInt32()+realRowSize);
                } catch (OverflowException) {
                    ptr=new IntPtr(ptr.ToInt64()+realRowSize);
                }
                pos+=rowSize;
            }
            if (progressForm!=null) {
                progressForm.SetProgress(100);
            }
            bitmap.UnlockBits(data);
            return bitmap;
        }

        #region IDisposable Members

        public void Dispose() {
            dataBytes = null;
            valid=false;
        }

        #endregion
    }
}

