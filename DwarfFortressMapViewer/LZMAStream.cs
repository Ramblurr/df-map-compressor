using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using SevenZip;
using SevenZip.Compression;
using SevenZip.Compression.LZMA;

namespace DwarfFortressMapCompressor {
    class LZMAStream : Stream, IDisposable {
        private CompressionMode compressionMode;
        private bool streamOpen;
        private MemoryStream memoryStream;
        SevenZip.Compression.LZMA.Encoder encoder;
        SevenZip.Compression.LZMA.Decoder decoder;

        private Stream baseStream;
        public Stream BaseStream {
            get {
                return baseStream;
            }
        }

        public override bool CanRead {
            get { return (compressionMode==CompressionMode.Decompress && streamOpen); }
        }

        public override bool CanSeek {
            get { return false; }
        }

        public override bool CanWrite {
            get { return (compressionMode==CompressionMode.Compress && streamOpen); }
        }

        public override long Length {
            get { throw new NotSupportedException("The method or operation is not supported."); }
        }

        public override long Position {
            get {
                throw new NotSupportedException("The method or operation is not supported.");
            }
            set {
                throw new NotSupportedException("The method or operation is not supported.");
            }
        }

        public LZMAStream(Stream stream, CompressionMode mode) : base() {
            if (stream==null) throw new ArgumentNullException("Stream cannot be null.");
            if (!stream.CanRead && mode==CompressionMode.Decompress) throw new ArgumentException("Write-only stream cannot be compressed.");
            if (!stream.CanWrite && mode==CompressionMode.Compress) throw new ArgumentException("Read-only stream cannot be compressed.");
            memoryStream = new MemoryStream();
            this.baseStream = stream;
            this.compressionMode = mode;
            this.streamOpen = true;

            if (compressionMode==CompressionMode.Compress) {
                Int32 dictionary = 1 << 22;
                Int32 posStateBits = 2;
                Int32 litContextBits = 3; // for normal files
                // UInt32 litContextBits = 0; // for 32-bit data
                Int32 litPosBits = 0;
                // UInt32 litPosBits = 2; // for 32-bit data
                Int32 numFastBytes = 32;
                //Int32 numPasses = 32;

                encoder = new SevenZip.Compression.LZMA.Encoder();
                //CoderPropID[] propIDs = { CoderPropID.DictionarySize };
                //object[] properties = { 1 << 22 };
                CoderPropID[] propIDs = { CoderPropID.DictionarySize, CoderPropID.PosStateBits, CoderPropID.LitContextBits, CoderPropID.LitPosBits, CoderPropID.NumFastBytes, CoderPropID.MatchFinder, CoderPropID.EndMarker };
                object[] properties = { (Int32) dictionary, (Int32) posStateBits, (Int32) litContextBits, (Int32) litPosBits, (Int32) numFastBytes, "bt4", false};
                encoder.SetCoderProperties(propIDs, properties);
                encoder.WriteCoderProperties(baseStream);

            } else {
                //Run file stream into decoder and out into memory stream.
                byte[] properties = new byte[5];
                if (baseStream.Read(properties, 0, 5)<5) {
                    throw new ApplicationException("Not a valid compressed file.");
                }
                decoder = new SevenZip.Compression.LZMA.Decoder();
                decoder.SetDecoderProperties(properties);
                byte[] longBytes = new byte[8];
                if (baseStream.Read(longBytes, 0, 8)<8) {
                    throw new ApplicationException("Not a valid compressed file.");
                }
                long compressedBytes = baseStream.Length - baseStream.Position;
                decoder.Code(baseStream, memoryStream, compressedBytes, BitConverter.ToInt64(longBytes, 0), null);
                memoryStream.Seek(0, SeekOrigin.Begin);
                
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (compressionMode==CompressionMode.Decompress) {
                return memoryStream.Read(buffer, offset, count);
            } else {
                throw new NotSupportedException("Write() was called while decompressing.");
            }
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override void SetLength(long value) {
            throw new NotSupportedException("The method or operation is not supported.");
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (compressionMode==CompressionMode.Compress) {
                memoryStream.Write(buffer, offset, count);
            } else {
                throw new NotSupportedException("Write() was called while decompressing.");
            }
        }

        public override void Flush() {
            if (compressionMode==CompressionMode.Compress) {
                //okay go
                long length = memoryStream.Length;
                byte[] longBytes = BitConverter.GetBytes(length);
                baseStream.Write(longBytes, 0, 8);
                memoryStream.Seek(0, SeekOrigin.Begin);
                encoder.Code(memoryStream, baseStream, -1, -1, null);
                baseStream.Flush();
            } else {
                //throw new NotSupportedException("Flush() was called while decompressing.");
            }
        }

        public override void Close() {
            if (streamOpen) {
                Flush();
                streamOpen = false;
                memoryStream.Close();
                baseStream.Close();
                base.Close();
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose() {
            Close();
            if (memoryStream!=null) {
                memoryStream.Dispose();
                memoryStream = null;
            }
            if (baseStream!=null) {
                baseStream.Dispose();
                baseStream.Dispose();
            }
        }

        #endregion
    }
}
