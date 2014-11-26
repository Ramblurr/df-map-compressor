using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using SL.Automation;
using System.Windows.Forms;

namespace DwarfFortressMapCompressor {
    public class PartialBitmapLoader {

        public struct BitmapFileHeader {
            public ushort bfType;
            public uint bfSize;
            public ushort bfReserved;
            public ushort bfReserved2;
            public uint bfOffBits;

            public BitmapFileHeader(Stream stream) {
                byte[] shortBytes = new byte[2];
                byte[] intBytes = new byte[4];
                stream.Read(shortBytes, 0, 2);
                bfType = BitConverter.ToUInt16(shortBytes, 0);
                stream.Read(intBytes, 0, 4);
                bfSize = BitConverter.ToUInt32(intBytes, 0);
                stream.Read(shortBytes, 0, 2);
                bfReserved = BitConverter.ToUInt16(shortBytes, 0);
                stream.Read(shortBytes, 0, 2);
                bfReserved2 = BitConverter.ToUInt16(shortBytes, 0);
                stream.Read(intBytes, 0, 4);
                bfOffBits = BitConverter.ToUInt32(intBytes, 0);
                
            }
        }
        public struct BitmapInfoHeader {
            public uint biSize;
            public uint biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public uint biXPelsPerMeter;
            public uint biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;

            public BitmapInfoHeader(Stream stream) {
                byte[] shortBytes = new byte[2];
                byte[] intBytes = new byte[4];
                stream.Read(intBytes, 0, 4);
                biSize = BitConverter.ToUInt32(intBytes, 0);
                Sanity.IfTrueThrow(biSize<40, "BitmapInfoHeader size is less than 40 bytes!");
                stream.Read(intBytes, 0, 4);
                biWidth = BitConverter.ToUInt32(intBytes, 0);
                stream.Read(intBytes, 0, 4);
                biHeight = BitConverter.ToInt32(intBytes, 0);
                stream.Read(shortBytes, 0, 2);
                biPlanes = BitConverter.ToUInt16(shortBytes, 0);
                stream.Read(shortBytes, 0, 2);
                biBitCount = BitConverter.ToUInt16(shortBytes, 0);
                stream.Read(intBytes, 0, 4);
                biCompression = BitConverter.ToUInt32(intBytes, 0);
                stream.Read(intBytes, 0, 4);
                biSizeImage = BitConverter.ToUInt32(intBytes, 0);
                stream.Read(intBytes, 0, 4);
                biXPelsPerMeter = BitConverter.ToUInt32(intBytes, 0);
                stream.Read(intBytes, 0, 4);
                biYPelsPerMeter = BitConverter.ToUInt32(intBytes, 0);
                stream.Read(intBytes, 0, 4);
                biClrUsed = BitConverter.ToUInt32(intBytes, 0);
                stream.Read(intBytes, 0, 4);
                biClrImportant = BitConverter.ToUInt32(intBytes, 0);
                if (biSize>40) {
                    byte[] junk = new byte[biSize-40];
                    stream.Read(junk, 0, (int)(biSize-40));
                }
            }
        }
        public static bool GetBitmapSize(FileInfo file, out int width, out int height) {
            width = 0;
            height = 0;
            Stream stream = file.OpenRead();
            BitmapFileHeader bfh = new BitmapFileHeader(stream);
            if (bfh.bfType==19778) {
                BitmapInfoHeader bih = new BitmapInfoHeader(stream);
                if (bih.biBitCount!=24) {
                    MessageBox.Show("This bitmap isn't 24 BPP, so the partial bitmap loader can't process it.");
                    return false;
                }
                if (bih.biCompression!=0) {
                    MessageBox.Show("This bitmap has compression, so the partial bitmap loader can't process it.");
                    return false;
                }
                width = (int) bih.biWidth;
                height = (int) bih.biHeight;
                return true;
            }
            return false;
        }

		//TODO: The bitmap data is upside-down, besides being all screwed up in other ways.
        public static byte[] Load(Stream stream, int xstart, int ystart, int width, int height) {
            int bitmapByteSize = width * height * 3;
            byte[] dataBytes = new byte[bitmapByteSize];
            int bitmapRowSize = width + width + width;
            int bitmapPos = 0;

            BitmapFileHeader bfh = new BitmapFileHeader(stream);
            if (bfh.bfType==19778) {
                BitmapInfoHeader bih = new BitmapInfoHeader(stream);
                if (bih.biBitCount!=24) {
                    MessageBox.Show("This bitmap isn't 24 BPP, so the partial bitmap loader can't process it.");
                    return null;
                }
                if (bih.biCompression!=0) {
                    MessageBox.Show("This bitmap has compression, so the partial bitmap loader can't process it.");
                    return null;
                }
                //biWidth&4 : extraBytesPerRow
                //0         : 0 
                //1         : 3
                //2         : 2
                //3         : 1
                int extraBytesPerRow = (int) ((4-(bih.biWidth&3))&3);
                int rowLen = (int) (bih.biWidth+bih.biWidth+bih.biWidth);
                byte[] junkBuffer = new byte[rowLen+extraBytesPerRow];
                int bRead = 0;
                for (int y=0; y<ystart; y++) {
                    bRead = stream.Read(junkBuffer, 0, rowLen+extraBytesPerRow);
                }
                for (int y=ystart; y<ystart+height; y++) {
                    if (xstart>0) {
                        bRead = stream.Read(junkBuffer, 0, xstart+xstart+xstart);
                    }
                    bRead = stream.Read(dataBytes, bitmapPos, width+width+width);
                    bitmapPos += width+width+width;
                    bRead = stream.Read(junkBuffer, 0, (rowLen+extraBytesPerRow) - ((width+xstart)*3));
                }

                return dataBytes;
            } else {
                return null;    //not a valid bitmap
            }            
        }
    }
}
