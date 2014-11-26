using System;
using System.Collections.Generic;
using System.Text;

namespace DwarfFortressMapCompressor {
    class MapBitmapData {
        protected int tileWidth;
        protected int tileHeight;
        protected int bitmapWidth;
        protected int bitmapHeight;
        protected int numTilesX;
        protected int numTilesY;
        protected int numTiles;

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

        public MapBitmapData(TiledBitmapWrapper map) {
            this.bitmapWidth=map.BitmapWidth;
            this.bitmapHeight=map.BitmapHeight;
            this.tileWidth=map.TileWidth;
            this.tileHeight=map.TileHeight;
            this.numTilesX = map.NumTilesX;
            this.numTilesY = map.NumTilesY;
            this.numTiles = map.NumTiles;
        }
    }
}
