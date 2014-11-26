using System;
using System.Collections.Generic;
using System.Text;

namespace DwarfFortressMapCompressor {
    public class DFColor {
        private int red, green, blue;
        private string name;
        //private static int[,] cache;

        /*static DFColor() {
            cache = new int[256,256];
            for (int x=0; x<256; x++) {
                for (int y=0; y<256; y++) {
                    cache[x,y] = (x*y)>>8;
                }
            }
        }*/

        public DFColor(string name) {
            this.name = name;
            this.red=0;
            this.green=0;
            this.blue=0;
        }
        public DFColor(string name, int red, int green, int blue) {
            this.red=red;
            this.green=green;
            this.blue=blue;
            this.name=name;
        }

        public int Red {
            get {
                return red;
            }
            set {
                red = value;
            }
        }
        public int Green {
            get {
                return green;
            }
            set {
                green = value;
            }
        }
        public int Blue {
            get {
                return blue;
            }
            set {
                blue = value;
            }
        }
        public string Name {
            get {
                return name;
            }
        }

        internal bool IsMagenta() {
            return (red==255 && green==0 && blue==255);
        }

        internal void Become(DFColor bgColor) {
            red = bgColor.red;
            green = bgColor.green;
            blue = bgColor.blue;
        }

        internal void Shade(DFColor fgColor) {
            /*
            red = cache[red, fgColor.red];
            green = cache[green, fgColor.green];
            blue = cache[blue, fgColor.blue];
            */
            
            red = (red * fgColor.red) >> 8;
            green = (green * fgColor.green) >> 8;
            blue = (blue * fgColor.blue) >> 8;
            
            /*
            red = (int) Math.Round(((red * fgColor.red) / 256.0f));
            green = (int) Math.Round(((green * fgColor.green) / 256.0f));
            blue = (int) Math.Round(((blue * fgColor.blue) / 256.0f));
             */
        }

        internal int Difference(DFColor otherColor) {
            int rdist = (red-otherColor.red);
            int gdist = (green-otherColor.green);
            int bdist = (blue-otherColor.blue);
            int distance = rdist*rdist + gdist*gdist + bdist*bdist;
            return (int) distance;
        }
    }
}
