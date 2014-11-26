using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DwarfFortressMapCompressor {
    class TiffWriter {
		public TiffWriter() {
		}
		
		public static void WriteImageFileDirectoryEntry(FileStream outputStream, ushort tag, ushort type, uint values, uint valueOrOffset) {
			outputStream.Write(BitConverter.GetBytes((UInt16) tag), 0, 2);
			outputStream.Write(BitConverter.GetBytes((UInt16) type), 0, 2);
			outputStream.Write(BitConverter.GetBytes((UInt32) values), 0, 4);
			outputStream.Write(BitConverter.GetBytes((UInt32) valueOrOffset), 0, 4);
		}
		
		public static void WriteTiff(TiledBitmapWrapper mapBitmap, BitmapPartBytes[] outReferences, SortedDictionary<BitmapPartBytes, BitmapPartBytes>.ValueCollection valueCollection, string encodeFilename) {
			FileStream outputStream = File.Create(encodeFilename);
			//Image file header
			ushort usdata = 0x4949;
			outputStream.Write(BitConverter.GetBytes((UInt16) usdata), 0, 2);
			usdata = 42;
			outputStream.Write(BitConverter.GetBytes((UInt16) usdata), 0, 2);
			uint uidata = 0x8;
			outputStream.Write(BitConverter.GetBytes((UInt32) uidata), 0, 4);
			//First IFD
			//entries in an IFD must be sorted in ascending order by tag
			//If the value fits into 4 bytes, it is written in the valueOrOffset field. Otherwise, an offset is written instead.
			//IFD entry field types: 1(1 byte), 2(0-127 ascii), 3(2 bytes), 4(4 bytes), 5(8 bytes), and more (see page 16 of the TIFF 6.0 specification PDF)
			
			/*IFD:
			Number of Directory Entries 000b
			ImageWidth 0100 0003 0001 shortWidth
			ImageLength 0101 0003 0001 shortHeight
			BitsPerSample 0102 0003 0003 offsetOfBitsPerSample
			Compression 0103 0003 0001 0001 (no compression, for now)
			PhotometricInterpretation 0106 0003 0001 0002
			SamplesPerPixel 0115 0003 0001 0003
			XResolution 011A 0005 0001 offsetOfRational
			YResolution 011B 0005 0001 offsetOfRational
			ResolutionUnit 0128 0003 0001 0001
			TileWidth 0142 0003 0001 shortTileWidth <-- width in pixels of a tile
			TileLength 0143 0003 0001 shortTileHeight <-- height in pixels of a tile
			TileOffsets 0144 0004 N=TilesPerImage, offsets are ordered left-to-right and top-to-bottom.
			TileByteCounts 0145 0004 N=TilesPerImage
			Next IFD offset 00000000
			[XResolution] 000000001 000000001
			[YResolution] 000000001 000000001
			[Compressed data]
			*/

            ulong offsetOfRationalXR = 174;
            ulong offsetOfRationalYR = offsetOfRationalXR+16;
            ulong offsetOfBitsPerSample = offsetOfRationalYR+16;
            ulong offsetOfTileOffsets = offsetOfBitsPerSample+6;
            ulong offsetOfTileByteCounts = offsetOfTileOffsets + (ulong) (mapBitmap.NumTiles*4);
            ulong offsetOfFirstTileOffset = offsetOfTileByteCounts + (ulong) (mapBitmap.NumTiles*4);
            ulong sizeOfTileData = (ulong) (3*mapBitmap.TileWidth*mapBitmap.TileHeight);
			usdata = 13;
			outputStream.Write(BitConverter.GetBytes((UInt16) usdata), 0, 2);
            WriteImageFileDirectoryEntry(outputStream, 0x0100, 0x0003, 0x0001, (uint) mapBitmap.BitmapWidth);	//1
            WriteImageFileDirectoryEntry(outputStream, 0x0101, 0x0003, 0x0001, (uint) mapBitmap.BitmapHeight);	//2
            WriteImageFileDirectoryEntry(outputStream, 0x0102, 0x0003, 0x0003, (uint) offsetOfBitsPerSample);			//3
            WriteImageFileDirectoryEntry(outputStream, 0x0103, 0x0003, 0x0001, (uint) 0x0001);					//4
            WriteImageFileDirectoryEntry(outputStream, 0x0106, 0x0003, 0x0001, (uint) 0x0002);					//5
            WriteImageFileDirectoryEntry(outputStream, 0x0115, 0x0003, 0x0001, (uint) 0x0003);					//6
            WriteImageFileDirectoryEntry(outputStream, 0x011a, 0x0005, 0x0001, (uint) offsetOfRationalXR);				//7
            WriteImageFileDirectoryEntry(outputStream, 0x011b, 0x0005, 0x0001, (uint) offsetOfRationalYR);				//8
            WriteImageFileDirectoryEntry(outputStream, 0x0128, 0x0003, 0x0001, (uint) 0x0001);					//9
            WriteImageFileDirectoryEntry(outputStream, 0x0142, 0x0003, 0x0001, (uint) mapBitmap.TileWidth);		//10
            WriteImageFileDirectoryEntry(outputStream, 0x0143, 0x0003, 0x0001, (uint) mapBitmap.TileHeight);		//11
            WriteImageFileDirectoryEntry(outputStream, 0x0144, 0x0004, (uint) mapBitmap.NumTiles, (uint) offsetOfTileOffsets);	//12
            WriteImageFileDirectoryEntry(outputStream, 0x0145, 0x0004, (uint) mapBitmap.NumTiles, (uint) offsetOfTileByteCounts);//13
			ulong uldata = 0;
			outputStream.Write(BitConverter.GetBytes((UInt64) uldata), 0, 8);
			//All data written previously: 8+2+(13*12)+8 bytes = 174 bytes
			//Write rationalXR:
			uldata = 1;
			outputStream.Write(BitConverter.GetBytes((UInt64) uldata), 0, 8);
			outputStream.Write(BitConverter.GetBytes((UInt64) uldata), 0, 8);
			//Write rationalYR:
			uldata = 1;
			outputStream.Write(BitConverter.GetBytes((UInt64) uldata), 0, 8);
			outputStream.Write(BitConverter.GetBytes((UInt64) uldata), 0, 8);
			//Write offsetOfBitsPerSample:
			usdata = 8;
			outputStream.Write(BitConverter.GetBytes((UInt16) usdata), 0, 2);
			outputStream.Write(BitConverter.GetBytes((UInt16) usdata), 0, 2);
			outputStream.Write(BitConverter.GetBytes((UInt16) usdata), 0, 2);
			//Write tileOffsets:
			uldata = offsetOfFirstTileOffset;
            int index=0;
            for (int y=0; y<mapBitmap.NumTilesY; y++) {
                for (int x=0; x<mapBitmap.NumTilesX; x++) {
                    index = x*mapBitmap.NumTilesY + y;
                    //outputStream.Write(BitConverter.GetBytes((UInt32) (uldata+sizeOfTileData*(ulong)outReferences[index].Index)), 0, 4);
                    outputStream.Write(BitConverter.GetBytes((UInt32) uldata), 0, 4);
                    uldata += sizeOfTileData;
                }
            }
            for (int i=0; i<mapBitmap.NumTiles; i++) {
				outputStream.Write(BitConverter.GetBytes((UInt32) uldata), 0, 4);
				uldata+=sizeOfTileData;
			}
			//Write byteCounts:
			for (int i=0; i<mapBitmap.NumTiles; i++) {
				outputStream.Write(BitConverter.GetBytes((UInt32) sizeOfTileData), 0, 4);
			}
			//Write tile data:
			byte[] dataBytes = new byte[outReferences[0].DataBytes.Length];
            /*int tileNum = 0;
            foreach (BitmapPartBytes tile in valueCollection) {
                for (int i=0; i<dataBytes.Length; i+=3) {
                    dataBytes[i] = tile.DataBytes[i+2];
                    dataBytes[i+1] = tile.DataBytes[i+1];
                    dataBytes[i+2] = tile.DataBytes[i];
                }
                outputStream.Write(dataBytes, 0, dataBytes.Length);
                tileNum++;
            }*/
            
            index=0;
            for (int y=0; y<mapBitmap.NumTilesY; y++) {
                for (int x=0; x<mapBitmap.NumTilesX; x++) {
                    BitmapPartBytes tile = outReferences[index];
					outputStream.Write(tile.DataBytes, 0, tile.DataBytes.Length);
					index+=1;
				}
			}
            
			/* Could add later:
			06A6 Software “PageMaker 4.0”
			06B6 DateTime “1988:02:18 13:59:59”
			*/
        }
	}
}