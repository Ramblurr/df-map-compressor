using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DwarfFortressMapCompressor {
    class BitmapPartBytes : IEquatable<BitmapPartBytes>, IComparable<BitmapPartBytes>, IDisposable {
        public static Comparison<BitmapPartBytes> PopularitySorter = new Comparison<BitmapPartBytes>(ComparePopularity);
        protected byte[] dataBytes;
        protected byte tileWidth;
        protected byte tileHeight;
        protected int index = -1;
        protected int popularity = 0;
        protected byte tileIndex;
        protected byte bgColor;
        protected byte fgColor;
        protected bool beingDeleted;
        protected bool beingCloned = false;

        //protected bool isMap;
        private static byte[] intBytes = new byte[4];

        public bool BeingDeleted {
            get {
                return beingDeleted;
            }
            set {
                beingDeleted = value;
            }
        }
        public bool BeingCloned {
            get {
                return beingCloned;
            }
            set {
                beingCloned = value;
            }
        }

        public int Index {
            get {
                return index;
            } set {
                index = value;
            }
        }

        public byte[] DataBytes {
            get {
                return dataBytes;
            }
        }

        public int Popularity {
            get {
                return popularity;
            }
            set {
                popularity = value;
            }
        }

        public byte TileIndex { get { return tileIndex; } }
        public byte BGColor { get { return bgColor; } }
        public byte FGColor { get { return fgColor; } }
        
        public BitmapPartBytes(byte tileWidth, byte tileHeight, byte[] bytes) {
            this.dataBytes = bytes;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }

        public BitmapPartBytes(byte tileWidth, byte tileHeight, byte[,] bytes, int tileIndex) {
            int len = bytes.GetLength(1);
            this.dataBytes = new byte[len];
            for (int i=0; i<len; i+=3) {
                dataBytes[i] = bytes[tileIndex, i];
                dataBytes[i+1] = bytes[tileIndex, i+1];
                dataBytes[i+2] = bytes[tileIndex, i+2];
            }
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.index = tileIndex;
        }

        //For TIFF output. index is not useable.
        public BitmapPartBytes(byte tileWidth, byte tileHeight, byte[,,] bytes, int tileIndexX, int tileIndexY) {
            int len = bytes.GetLength(1);
            this.dataBytes = new byte[len];
            for (int i=0; i<len; i+=3) {
                dataBytes[i+2] = bytes[tileIndexX, tileIndexY, i];
                dataBytes[i+1] = bytes[tileIndexX, tileIndexY, i+1];
                dataBytes[i] = bytes[tileIndexX, tileIndexY, i+2];
            }
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.index = 0;
        }

        public BitmapPartBytes(byte tileWidth, byte tileHeight, Stream stream) {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            int bytesLoaded = 0;
            int length = tileWidth * tileHeight * 3;
            dataBytes = new byte[length];
            byte[] readBytes = new byte[4];
            while (bytesLoaded < length) {
                int streamLength = stream.Read(readBytes, 0, 4);
                if (streamLength==4) {
                    for (int numPixels = readBytes[0]; numPixels>0; numPixels--) {
                        dataBytes[bytesLoaded]=readBytes[1];
                        dataBytes[bytesLoaded+1]=readBytes[2];
                        dataBytes[bytesLoaded+2]=readBytes[3];
                        bytesLoaded+=3;
                    }
                } else {
                    throw new ApplicationException("File is incomplete? (Truncated in a tile image)");
                }
            }
            
            //
            //if (stream.Read(dataBytes, 0, length)==length) {
            //    return;
            //} else {
            //    throw new ApplicationException("File is incomplete? (Truncated in a tile image)");
            //}
        }

        private byte ValidateByteFromFile(int value) {
            if (value>=0) {
                return (byte) value;
            } else {
                throw new ApplicationException("File is incomplete? (Truncated in a tile image header)");
            }
        }
        
        public bool Equals(BitmapPartBytes other) {
            if (this==other) {
                return true;
            }
            return (CompareTo(other)==0);
        }

        public int CompareTo(BitmapPartBytes other) {
            if (dataBytes.Length == other.dataBytes.Length) {
                //if (!isMap) {
                    //if (!other.isMap) {
                        for (int i=0; i<dataBytes.Length; i++) {
                            byte mine = dataBytes[i];
                            byte theirs = other.dataBytes[i];
                            if (mine<theirs) {
                                return -1;
                            } else if (mine>theirs) {
                                return 1;
                            }
                        }
                    /*} else {
                        //other.isMap
                        for (int i=0; i<dataBytes.Length; i+=3) {
                            if (dataBytes[i]!=255 || dataBytes[i+1]!=0 || dataBytes[i+2]!=255) {
                                for (int i2=0; i2<3; i2++) {
                                    byte mine = dataBytes[i+i2];
                                    byte theirs = other.dataBytes[i+i2];
                                    if (mine<theirs) {
                                        return -1;
                                    } else if (mine>theirs) {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    //isMap

                }*/
                return 0;
            } else if (dataBytes.Length < other.dataBytes.Length) {
                return -1;
            } else {
                return 1;
            }
        }

        public int WriteToStream(Stream stream) {
            //stream.WriteByte(tileWidth);
            //stream.WriteByte(tileHeight);
            //stream.Write(BitConverter.GetBytes((int)dataBytes.Length), 0, 4);
            /*byte[] fakeDataBytes = new byte[dataBytes.Length];
            for (int i=0; i<dataBytes.Length; i+=3) {
                fakeDataBytes[i]=255;
                fakeDataBytes[i+1]=127;
                fakeDataBytes[i+2]=63;
            }
            stream.Write(fakeDataBytes, 0, dataBytes.Length);*/

            int bytesSaved = 0;
            int length = tileWidth * tileHeight * 3;
            int bytePos = 0;
            while (bytePos < length) {
                intBytes[0] = 1;
                intBytes[1] = dataBytes[bytePos];
                intBytes[2] = dataBytes[bytePos+1];
                intBytes[3] = dataBytes[bytePos+2];
                bytePos+=3;
                bool cont = true;
                while (cont) {
                    if (bytePos < length && dataBytes[bytePos] == intBytes[1] && dataBytes[bytePos+1] == intBytes[2] && dataBytes[bytePos+2] == intBytes[3] && intBytes[0]<255) {
                        intBytes[0] += 1;
                        bytePos+=3;
                    } else {
                        stream.Write(intBytes, 0, 4);
                        bytesSaved+=4;
                        cont=false;
                    }
                }

            }
            //stream.Write(dataBytes, 0, dataBytes.Length);
            //return dataBytes.Length;
            return bytesSaved;
        }

        public static int ComparePopularity(BitmapPartBytes alpha, BitmapPartBytes beta) {
            if (alpha==null || beta==null) throw new ApplicationException("Invalid comparison.");
            if (alpha.Popularity > beta.Popularity) {
                return -1;
            } else if (alpha.Popularity < beta.Popularity) {
                return 1;
            } else if (alpha.Index > beta.Index) {
                return -1;
            } else if (alpha.Index < beta.Index) {
                return 1;
            } else {
                return 0;
            }
        }

        #region IDisposable Members

        public void Dispose() {
            dataBytes = null;
        }

        #endregion

        public int GetDetail() {
            int detail = 0;
            int r = dataBytes[2];
            int g = dataBytes[1];
            int b = dataBytes[0];
            for (int i=0; i<dataBytes.Length; i+=3) {
                if (r!=dataBytes[i+2] || g!=dataBytes[i+1] || b!=dataBytes[i]) {
                    detail+=1;
                }
            }
            return detail;
        }

        internal void WriteToByteBuffer(ref byte[] combinedImage, int index, int tileWidth, int tileHeight, int combinedImageWidthInTiles, int combinedImageHeightInTiles) {
            int tileIndexY = index / combinedImageWidthInTiles;
            int tileIndexX = index % combinedImageWidthInTiles;
            int rowWidth = combinedImageWidthInTiles * tileWidth * 3;
            int rowStartBytePos = (tileIndexY * combinedImageWidthInTiles * tileWidth * tileHeight + tileIndexX * tileWidth) * 3;
            int rowEndBytePos = rowStartBytePos + tileWidth * 3;
            int lastRowEndBytePos = rowEndBytePos + rowWidth * (tileHeight-1);
            int bytePos = rowStartBytePos;
            int ourBytePos = 0;
            bool cont = true;
            while (cont) {
                combinedImage[bytePos] = dataBytes[ourBytePos];
                combinedImage[bytePos+1] = dataBytes[ourBytePos+1];
                combinedImage[bytePos+2] = dataBytes[ourBytePos+2];
                bytePos += 3;
                ourBytePos += 3;
                if (bytePos >= rowEndBytePos) {
                    if (bytePos < lastRowEndBytePos) {
                        rowEndBytePos += rowWidth;
                        rowStartBytePos += rowWidth;
                        bytePos = rowStartBytePos;
                    } else {
                        cont=false;
                    }
                }
            }
        }

        internal bool AttemptFind(DFColor[] colorArray, BitmapPartBytes[,] fontTileBitmaps, out int tilex, out int tiley, out DFColor bgColor, out DFColor fgColor, out int bestTotalDifference, bool bypassPixelLimit) {
            tilex = -1;
            tiley = -1;
            bgColor = null;
            fgColor = null;
            bestTotalDifference=-1;
            for (int x=0; x<16; x++) {
                for (int y=0; y<16; y++) {
                    if (!(x==0 && y==2) && !(x==15 && y==15)) {
                        BitmapPartBytes fontTile = fontTileBitmaps[x, y];
                        //if (x==10 && y==14) {
                        //    Console.WriteLine("moo");
                        //}
                        for (int bg = 0; bg<colorArray.Length; bg++) {
                            DFColor ourBgColor = colorArray[bg];
                            for (int fg = 1; fg<colorArray.Length; fg++) {
                                DFColor ourFgColor = colorArray[fg];
                                //if (fg==15) {
                                //    Console.WriteLine("moo");
                                //}
                                if (ourBgColor.Difference(ourFgColor)>0) {
                                    int totalDifference;
                                    int sameness = 0;
                                    int val = CompareTo(fontTile, ourBgColor, ourFgColor, out totalDifference, out sameness, bypassPixelLimit);
                                    if (val==0) {
                                        if (totalDifference<bestTotalDifference || bestTotalDifference==-1) {
                                            
                                            //Console.WriteLine("Found one!");
                                            tilex = x;
                                            tiley = y;
                                            bgColor = ourBgColor;
                                            fgColor = ourFgColor;
                                            bestTotalDifference = totalDifference;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return (bestTotalDifference!=-1);
        }

        public void CopyFrom(DFColor[] colors, BitmapPartBytes[,] fontTileBitmaps, int tilex, int tiley, DFColor bgColor, DFColor fgColor) {
            BitmapPartBytes other = fontTileBitmaps[tilex, tiley];
            if (dataBytes.Length == other.dataBytes.Length) {
                for (int i=0; i<dataBytes.Length; i+=3) {
                    //DFColor mine = new DFColor("mine", dataBytes[i+2], dataBytes[i+1], dataBytes[i]);
                    DFColor theirs = new DFColor("theirs", other.dataBytes[i+2], other.dataBytes[i+1], other.dataBytes[i]);

                    //transform their colors
                    if (theirs.IsMagenta()) {
                        theirs.Become(bgColor);
                    } else {
                        theirs.Shade(fgColor);
                    }
                    dataBytes[i] = (byte) theirs.Blue;
                    dataBytes[i+1] = (byte) theirs.Green;
                    dataBytes[i+2] = (byte) theirs.Red;
                }
            }
        }

        public void DetermineBlankedRowsColumns(out bool[] rowsBlanked, out bool[] columnsBlanked) {
            rowsBlanked = new bool[tileHeight];
            columnsBlanked = new bool[tileWidth];
            int blankBytePos = 0;
            int baseBlankBytePos = 0;
            int rowStride = tileWidth+tileWidth+tileWidth;
            for (int x=0; x<tileWidth; x++, baseBlankBytePos+=3) {
                blankBytePos = baseBlankBytePos;
                columnsBlanked[x] = true;
                for (int y=0; y<tileHeight; y++, blankBytePos+=rowStride) {
                    if (dataBytes[blankBytePos]!=0 || dataBytes[blankBytePos+1]!=0 || dataBytes[blankBytePos+2]!=0) {
                        columnsBlanked[x] = false;
                        break;
                    }
                }
            }
            baseBlankBytePos = 0;
            for (int y=0; y<tileHeight; y++, baseBlankBytePos+=rowStride) {
                blankBytePos = baseBlankBytePos;
                rowsBlanked[y] = true;
                for (int x=0; x<tileWidth; x++, blankBytePos+=3) {
                    if (dataBytes[blankBytePos]!=0 || dataBytes[blankBytePos+1]!=0 || dataBytes[blankBytePos+2]!=0) {
                        rowsBlanked[y] = false;
                        break;
                    }
                }
            }
        }

        public int CompareTo(BitmapPartBytes other, DFColor bgColor, DFColor fgColor, out int totalDifference, out int sameness, bool bypassPixelLimit) {
            totalDifference = 0;
            sameness = 0;
            if (dataBytes.Length == other.dataBytes.Length) {
                int retval = 0;
                bool[] rowsBlanked = null;
                bool[] columnsBlanked = null;
                if (bypassPixelLimit) {
                    DetermineBlankedRowsColumns(out rowsBlanked, out columnsBlanked);
                }
                int x = 0; int y = 0;
                for (int i=0; i<dataBytes.Length; i+=3, x+=1) {
                    if (x>=tileWidth) {
                        x-=tileWidth;
                        y+=1;
                    }
                    DFColor mine = new DFColor("mine", dataBytes[i+2], dataBytes[i+1], dataBytes[i]);
                    DFColor theirs = new DFColor("theirs", other.dataBytes[i+2], other.dataBytes[i+1], other.dataBytes[i]);
                    
                    //transform their colors
                    if (theirs.IsMagenta()) {
                        theirs.Become(bgColor);
                    } else {
                        theirs.Shade(fgColor);
                    }
                    int difference = mine.Difference(theirs);
                    if (difference<=MainMenuForm.GetColorMatchSensitivity() || (bypassPixelLimit && mine.Blue==0 && mine.Green==0 && mine.Red==0 && (rowsBlanked[y] || columnsBlanked[x]))) {
                        totalDifference+=difference;
                    } else {
                        if (mine.Blue<theirs.Blue) {
                            retval = -1;
                        } else if (mine.Blue>theirs.Blue) {
                            retval = 1;
                        } else if (mine.Green<theirs.Green) {
                            retval = -1;
                        } else if (mine.Green>theirs.Green) {
                            retval = 1;
                        } else if (mine.Red<theirs.Red) {
                            retval = -1;
                        } else if (mine.Red>theirs.Red) {
                            retval = 1;
                        }
                        break;
                    }
                }
                return retval;
            } else if (dataBytes.Length < other.dataBytes.Length) {
                return -1;
            } else {
                return 1;
            }
        }

        public int CompareTo(BitmapPartBytes other, out int totalDifference, bool bypassPixelLimit) {
            totalDifference = 0;
            if (dataBytes.Length == other.dataBytes.Length) {
                int retval = 0;
                bool[] rowsBlanked = null;
                bool[] columnsBlanked = null;
                if (bypassPixelLimit) {
                    DetermineBlankedRowsColumns(out rowsBlanked, out columnsBlanked);
                }
                int x = 0; int y = 0;
                for (int i=0; i<dataBytes.Length; i+=3, x+=1) {
                    if (x>=tileWidth) {
                        x-=tileWidth;
                        y+=1;
                    }
                    DFColor mine = new DFColor("mine", dataBytes[i+2], dataBytes[i+1], dataBytes[i]);
                    DFColor theirs = new DFColor("theirs", other.dataBytes[i+2], other.dataBytes[i+1], other.dataBytes[i]);

                    int difference = mine.Difference(theirs);
                    if (difference<=12 || (bypassPixelLimit && mine.Blue==0 && mine.Green==0 && mine.Red==0 && (rowsBlanked[y] || columnsBlanked[x]))) {
                        totalDifference+=difference;
                    } else {
                        if (mine.Blue<theirs.Blue) {
                            retval = -1;
                        } else if (mine.Blue>theirs.Blue) {
                            retval = 1;
                        } else if (mine.Green<theirs.Green) {
                            retval = -1;
                        } else if (mine.Green>theirs.Green) {
                            retval = 1;
                        } else if (mine.Red<theirs.Red) {
                            retval = -1;
                        } else if (mine.Red>theirs.Red) {
                            retval = 1;
                        }
                        break;
                    }
                }
                return retval;
            } else if (dataBytes.Length < other.dataBytes.Length) {
                return -1;
            } else {
                return 1;
            }
        }

        internal void StoreIdentification(byte tileIndex, byte bgColor, byte fgColor) {
            this.tileIndex = tileIndex;
            this.bgColor = bgColor;
            this.fgColor = fgColor;
        }

        internal void CopyFrom(BitmapPartBytes other) {
            if (dataBytes.Length == other.dataBytes.Length) {
                for (int i=0; i<dataBytes.Length; i+=3) {
                    dataBytes[i] = (byte) other.dataBytes[i];
                    dataBytes[i+1] = (byte) other.dataBytes[i+1];
                    dataBytes[i+2] = (byte) other.dataBytes[i+2];
                }
            }
        }
                        
        public override String ToString() {
            int tilex = tileIndex&0xf;
            int tiley = tileIndex>>4;
            return "Tile ("+tilex+","+tiley+"), BGColor "+bgColor+", FGColor "+fgColor+", index in list of tiles "+index;
        }
    }
}
