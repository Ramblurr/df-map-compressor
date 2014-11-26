using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace DwarfFortressMapCompressor {
    public class ZLayerFileList {
        private List<FileInfo> files;
        private List<int> zLayers;

        public ZLayerFileList() {
            this.files = new List<FileInfo>();
            this.zLayers = new List<int>();
        }

        public void Add(FileInfo file, int layer) {
            files.Add(file);
            zLayers.Add(layer);
        }

        public int Count {
            get {
                return files.Count;
            }
        }

        public FileInfo GetFile(int i) {
            return files[i];
        }

        public int GetZLayer(int i) {
            return zLayers[i];
        }
    }
    
    public class ZLayerList {
        private List<int> zLayers;
        private List<int> indexes;

        public ZLayerList() {
            this.zLayers = new List<int>();
            this.indexes = new List<int>();
        }

        public void Add(int index, int zLayer) {
            for (int i=0; i<zLayers.Count; i++) {
                if (zLayers[i] < zLayer) {
                    indexes.Insert(i, index);
                    zLayers.Insert(i, zLayer);
                    return;
                }
            }
            indexes.Add(index);
            zLayers.Add(zLayer);
        }

        public ICollection GetZLayers() {
            return zLayers;
        }

        public int GetIndexOfZLayer(int zLayer) {
            for (int i=0; i<zLayers.Count; i++) {
                if (zLayers[i]==zLayer) {
                    return indexes[i];
                }
            }
            return -1;
        }

        public int GetInitialZLayer() {
            int idx = GetIndexOfZLayer(0);
            if (idx!=-1) {
                return 0;
            } else {
                return zLayers[0];
            }
        }

        public int ValidateZLayer(int zLayer) {
            int idx = GetIndexOfZLayer(zLayer);
            if (idx!=-1) {
                return zLayer;
            } else {
                return zLayers[0];
            }
        }

        public int GetHigherZLayer(int zLayer) {
            int idx = GetIndexOfZLayer(zLayer);
            if (idx!=-1) {
                if ((idx+1)<Count) {
                    return zLayers[idx+1];
                } else {
                    return zLayer;
                }
            } else {
                return zLayers[0];
            }
        }
        public int GetLowerZLayer(int zLayer) {
            int idx = GetIndexOfZLayer(zLayer);
            if (idx!=-1) {
                if (idx>0) {
                    return zLayers[idx-1];
                } else {
                    return zLayer;
                }
            } else {
                return zLayers[0];
            }
        }
        public int Count {
            get {
                return zLayers.Count;
            }
        }
    }
}
