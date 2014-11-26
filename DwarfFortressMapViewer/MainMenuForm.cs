//#define TileIndexRLE

//Only one of these should be defined:
//#define TileIndexRLEMethod2
	//#define TileIndexRLEMethod1
	//#define TileIndexRLEMethod3
	//#define TileIndexRLEMethod4
#define BlackSpaceConversion


//#define AutoClipping

/*
 * Theoretical maximum tiles: 65536 (256*16*16)
 * We never get anywhere near that, in practice.
 */
                    

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Permissions;
using System.IO;
using System.IO.Compression;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections;
using SL.Automation;

[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,
   Read="HKEY_CURRENT_USER")]

namespace DwarfFortressMapCompressor {
    public partial class MainMenuForm : Form {
        private Dictionary<string, byte> colorIDs = new Dictionary<string, byte>();            
        private string chosenFolder;
        private ProgressForm progressForm = null;
        internal static MainMenuForm instance = null;
        private int tileWidth = 0;
        private int tileHeight = 0;
        private bool waitingForChooser = false;
        private static int colorMatchSensitivity = 0;
        
        public MainMenuForm() {
            colorIDs.Add("BLACK", 0);
            colorIDs.Add("BLUE", 1);
            colorIDs.Add("GREEN", 2);
            colorIDs.Add("CYAN", 3);
            colorIDs.Add("RED", 4);
            colorIDs.Add("MAGENTA", 5);
            colorIDs.Add("BROWN", 6);
            colorIDs.Add("LGRAY", 7);
            colorIDs.Add("DGRAY", 8);
            colorIDs.Add("LBLUE", 9);
            colorIDs.Add("LGREEN", 10);
            colorIDs.Add("LCYAN", 11);
            colorIDs.Add("LRED", 12);
            colorIDs.Add("LMAGENTA", 13);
            colorIDs.Add("YELLOW", 14);
            colorIDs.Add("WHITE", 15);

            instance = this; 
            InitializeComponent();
#if MONO
            int colorMatchSensitivity = 12;
#else
            int colorMatchSensitivity = Properties.Settings.Default.colorMatchSensitivity;
            this.colorMatchSensitivityTextBox.Text = ""+colorMatchSensitivity;
#endif
            if (IsLinux()) {
                this.Width+=100;
                foreach (Control control in Controls) {
                    if (control!=label1 && control!=label2 && control!=label3) {
                        if (control.Location.X>150) {
                            control.Location = new Point(control.Location.X+50, control.Location.Y);
                        }
                        if (control is CheckBox || control is Button) {
                            control.Width += 50;
                        }
                    } else if (control==label2) {
                        control.Location = new Point(control.Location.X+25, control.Location.Y);
                    } else if (control==label3) {
                        control.Location = new Point(control.Location.X+25, control.Location.Y);
                    }
                    /*if (control is CheckBox) {
                        Label newLabel = new Label();

                        //TODO
                    }*/
                }
            }
        }

        public bool IsLinux() {
            //Console.WriteLine("Font name: "+this.Font.Name);
            return this.Font.Name!="Microsoft Sans Serif";
        }

        public void TileSizeWrapper(TileSizeChooser chooser, Bitmap bitmap, int tileWidth, int tileHeight, ProgressForm progressForm) {
            chooser.Hide();
            chooser.Dispose();
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            waitingForChooser = false;
        }

        private string StripFilenameFrom(string filename) {
            filename = filename.Replace('\\', '/');
            return filename.Substring(0, filename.LastIndexOf("/"));
        }

        private string FindDFFolder() {
			Console.WriteLine("The map compressor is attempting to look for registry keys to try to find the DF folder by looking for DF in the MUICache list of exe files. Any 'Missing Registry.blahblah' messages are just the map compressor reporting why it failed to find it.");
            if (Registry.CurrentUser==null) {
                Console.WriteLine("Missing Registry.CurrentUser");
                return null;
            }
            if (Registry.CurrentUser.OpenSubKey("Software")==null) {
                Console.WriteLine("Missing Registry.CurrentUser.Software"); 
                return null;
            }
            if (Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft")==null) {
                Console.WriteLine("Missing Registry.CurrentUser.Software.Microsoft");
                return null;
            }
            if (Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Windows")==null) {
                Console.WriteLine("Missing Registry.CurrentUser.Software.Microsoft.Windows");
                return null;
            }
            if (Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("ShellNoRoam")==null) {
                Console.WriteLine("Missing Registry.CurrentUser.Software.Microsoft.Windows.ShellNoRoam");
                return null;
            }
            if (Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("ShellNoRoam").OpenSubKey("MUICache")==null) {
                Console.WriteLine("Missing Registry.CurrentUser.Software.Microsoft.Windows.ShellNoRoam.MUICache");
                return null;
            }
            try {
                Console.WriteLine("Checking registry to try to find DF.");
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("ShellNoRoam").OpenSubKey("MUICache");
                string[] valueNames = key.GetValueNames();
                foreach (string valueName in valueNames) {
                    string progName = key.GetValue(valueName) as string;
                    if (progName!=null) {
                        if (String.Compare(progName, "dwarfort", true)==0) {
                            //found it
                            //strip out the filename
                            return StripFilenameFrom(valueName);
                        }
                    }
                }
            } catch (Exception e) {
                Console.WriteLine("Caught Exception "+e+" while trying to check registry. (Don't panic)");
            }
            return null;
        }

        private void EncodeFortressMapButton_Click(object sender, EventArgs e) {
            try {
                Properties.Settings.Default.colorMatchSensitivity=Int32.Parse(colorMatchSensitivityTextBox.Text);
                Properties.Settings.Default.Save();
            } catch (Exception) {
            }
            chosenFolder = FindDFFolder();
            if (chosenFolder!=null) {
                selectMapFileToEncodeOFD.InitialDirectory=chosenFolder;
            }
            selectMapFileToEncodeOFD.DefaultExt=".df-map"; 
            DialogResult result = selectMapFileToEncodeOFD.ShowDialog();
            if (result == DialogResult.OK) {
                //Stream inputStream = selectMapFileToEncodeOFD.OpenFile();
                chosenFolder = StripFilenameFrom(selectMapFileToEncodeOFD.FileName);
                string originalFilename = selectMapFileToEncodeOFD.FileName;
                string strippedFilename = StripNumberFromNewDFFilename(originalFilename);
                if (strippedFilename!=null) {
                    originalFilename = strippedFilename;
                }
                string filename = originalFilename;
                int indexOfDot = filename.LastIndexOf('.');
                string extension = "";
                if (indexOfDot>-1) {
                    filename = originalFilename.Substring(0, indexOfDot);
                    if (indexOfDot < originalFilename.Length-1) {
                        extension = originalFilename.Substring(indexOfDot+1);
                    }
                }
                decodeFortressMapSFD.FileName=filename; 
                encodeFortressMapSFD.FileName=filename;
                result = encodeFortressMapSFD.ShowDialog();
                if (result == DialogResult.OK) {
                    if (String.Compare(extension, "df-map", true)==0) {
                        MessageBox.Show(LinuxInsertLineBreaks("You asked to translate "+filename+" into a df-map file, but it already appears to be one (unless you renamed it yourself, which would be bad)..."));
                        return;
                    } else if (String.Compare(extension, "fdf-map", true)==0) {
                        this.Hide();
                        ConvertCompression(filename+".fdf-map", extension, encodeFortressMapSFD.FileName, "df-map");
                        this.Show();
                    } else {
                        this.Hide();
                        progressForm = new ProgressForm();
                        progressForm.Show();
                        Run_EncodeFortressMap(false, false, null);
                        progressForm.SetStatus("Cleaning up...");
                        GC.Collect();
                        progressForm.Hide();
                        progressForm.Dispose();
                        progressForm = null;
                        this.Show();
                    }
                    
                }
            }
            
        }
        private void Run_EncodeFortressMap(bool textFormat, bool zlibFormat) {
            Run_EncodeFortressMap(textFormat, zlibFormat, null);
        }

        private void Run_EncodeFortressMap(bool textFormat, bool zlibFormat, Bitmap image) {
            string folder = chosenFolder;
            string fontFile = null;
            bool usingGraphics = false;
            bool fullscreen = false;
            Dictionary<string, DFColor> colors = new Dictionary<string, DFColor>();
            BitmapPartBytes[,] fontTileBitmaps = null;
            progressForm.SetPrefix("");
            progressForm.SetStatus("Looking for data/init/init.txt or init.txt in folder '"+folder+"'...");
            try {
                int windowedX = 0;
                int windowedY = 0;
                int graphicsWindowedX = 0;
                int graphicsWindowedY = 0;
                int fullscreenX = 0;
                int fullscreenY = 0;
                int graphicsFullscreenX = 0;
                int graphicsFullscreenY = 0;
                StreamReader reader;
                try {
                    reader = new StreamReader(folder+"/data/init/init.txt");
                } catch (DirectoryNotFoundException) {
                    reader = new StreamReader(folder+"/init.txt");
                } catch (FileNotFoundException) {
                    reader = new StreamReader(folder+"/init.txt");
                }
                {
                    //bool graphicsBlackSpace = false;
                    string font = "";
                    string fullfont = "";
                    string graphicsFont = "";
                    string graphicsFullfont = "";
                    
                    string line;
                    while ((line = reader.ReadLine())!=null) {
                        line = line.Trim();
                        //[WINDOWEDX:640]
                        //[WINDOWEDY:300]
                        //[FONT:curses_640x300.bmp]

                        //[GRAPHICS:YES]
                        //[GRAPHICS_WINDOWEDX:1280]
                        //[GRAPHICS_WINDOWEDY:408]
                        //[GRAPHICS_FONT:superfouleggfont.bmp]
                        char[] paramChars = { ':', '[', ']' };
                        string[] splitLine = line.Split(paramChars, StringSplitOptions.RemoveEmptyEntries);
                        if (splitLine.Length>=2) {
                            if (String.Compare(splitLine[0], "WINDOWEDX")==0) {
                                windowedX = Int32.Parse(splitLine[1]);
                                progressForm.SetProgress(15);
                            } else if (String.Compare(splitLine[0], "WINDOWEDY")==0) {
                                windowedY = Int32.Parse(splitLine[1]);
                                progressForm.SetProgress(30);
                            } else  if (String.Compare(splitLine[0], "FULLSCREENX")==0) {
                                fullscreenX = Int32.Parse(splitLine[1]);
                            } else if (String.Compare(splitLine[0], "FULLSCREENY")==0) {
                                fullscreenY = Int32.Parse(splitLine[1]);
                            } else if (String.Compare(splitLine[0], "FONT")==0) {
                                font = splitLine[1];
                                progressForm.SetProgress(45);
                            } else if (String.Compare(splitLine[0], "FULLFONT")==0) {
                                fullfont = splitLine[1];
                            } else if (String.Compare(splitLine[0], "GRAPHICS")==0) {
                                usingGraphics = String.Compare(splitLine[1], "YES", true)==0;
                                progressForm.SetProgress(60);
                            } else if (String.Compare(splitLine[0], "GRAPHICS_WINDOWEDX")==0) {
                                graphicsWindowedX = Int32.Parse(splitLine[1]);
                                progressForm.SetProgress(75);
                            } else if (String.Compare(splitLine[0], "GRAPHICS_WINDOWEDY")==0) {
                                graphicsWindowedY = Int32.Parse(splitLine[1]);
                                progressForm.SetProgress(90);
                            } else if (String.Compare(splitLine[0], "GRAPHICS_FULLSCREENX")==0) {
                                graphicsFullscreenX = Int32.Parse(splitLine[1]);
                            } else if (String.Compare(splitLine[0], "GRAPHICS_FULLSCREENY")==0) {
                                graphicsFullscreenY = Int32.Parse(splitLine[1]);
                            } else if (String.Compare(splitLine[0], "GRAPHICS_FONT")==0) {
                                graphicsFont = splitLine[1];
                                progressForm.SetProgress(100);
                            } else if (String.Compare(splitLine[0], "GRAPHICS_FULLFONT")==0) {
                                graphicsFullfont = splitLine[1];
                            //} else if (String.Compare(splitLine[0], "GRAPHICS_BLACK_SPACE")==0) {
                            //    graphicsBlackSpace = (String.Compare(splitLine[1],"YES", true)==0);
                            } else if (String.Compare(splitLine[0], "WINDOWED")==0) {
                                if (String.Compare(splitLine[1], "ASK", true)==0 || String.Compare(splitLine[1], "PROMPT", true)==0) {
                                    DialogResult result = MessageBox.Show("Did you export this image from DF fullscreen?", "A question", MessageBoxButtons.YesNo);
                                    fullscreen = (result==DialogResult.Yes);
                                } else {
                                    fullscreen = (String.Compare(splitLine[1], "NO", true)==0);
                                }
                            } else if (splitLine[0].Length>=2) {
                                if (splitLine[0].LastIndexOf("_R")==splitLine[0].Length-2) { 
                                    string colorName = splitLine[0].Substring(0, splitLine[0].Length-2);
                                    if (!colors.ContainsKey(colorName)) {
                                        colors[colorName] = new DFColor(colorName);
                                    }
                                    colors[colorName].Red = Int32.Parse(splitLine[1]);
                                } else if (splitLine[0].LastIndexOf("_G")==splitLine[0].Length-2) {
                                    string colorName = splitLine[0].Substring(0, splitLine[0].Length-2);
                                    if (!colors.ContainsKey(colorName)) {
                                        colors[colorName] = new DFColor(colorName);
                                    }
                                    colors[colorName].Green = Int32.Parse(splitLine[1]);
                                } else if (splitLine[0].LastIndexOf("_B")==splitLine[0].Length-2) {
                                    string colorName = splitLine[0].Substring(0, splitLine[0].Length-2);
                                    if (!colors.ContainsKey(colorName)) {
                                        colors[colorName] = new DFColor(colorName);
                                    }
                                    colors[colorName].Blue = Int32.Parse(splitLine[1]);
                                }
                            }
                        }
                    }
                    if (usingGraphics) {
                        if (fullscreen) {
                            fontFile = graphicsFullfont;
                        } else {
                            fontFile = graphicsFont;
                        }
                    } else {
                        if (fullscreen) {
                            fontFile = fullfont;
                        } else {
                            fontFile = font;
                        }
                    }
                    //fontFile = folder+"\\data\\art\\"+fontFile;
                }
                reader.Dispose(); reader = null;
                if (usingGraphics) {
                    windowedX = graphicsWindowedX;
                    windowedY = graphicsWindowedY;
                    fullscreenX = graphicsFullscreenX;
                    fullscreenY = graphicsFullscreenY;
                }
                if (fullscreen) {
                    windowedX = fullscreenX;
                    windowedY = fullscreenY;
                }
                if (fontFile=="") {
                    tileWidth = 0;
                    tileHeight = 0;
                    MessageBox.Show(LinuxInsertLineBreaks("Couldn't find font filename in DF's init.txt. You'll have to tell us what size your tiles are, and we won't be able to analyze the tile images to match them to font characters and colors."));
                } else {
                    if (!doNotAnalyzeCheckbox.Checked) {
                        fontTileBitmaps = new BitmapPartBytes[16, 16];
                    }
                    //Determine tile size
                    Bitmap bitmap;
                    try {
                        bitmap = (Bitmap) Bitmap.FromFile(folder+"/data/art/"+fontFile);
                        fontFile = folder+"/data/art/"+fontFile;
                    } catch (FileNotFoundException) {
                        bitmap = (Bitmap) Bitmap.FromFile(fontFile);
                    }
                    {
                        tileWidth = bitmap.Width/16;
                        tileHeight = bitmap.Height/16;
                        if (!doNotAnalyzeCheckbox.Checked) {
                            using (TiledBitmapWrapper fontmap = new TiledBitmapWrapper(bitmap, tileWidth, tileHeight, null)) {
                                for (int x=0; x<16; x++) {
                                    for (int y=0; y<16; y++) {
                                        fontTileBitmaps[x, y] = fontmap.GetTile(x, y);
                                    }
                                }
                            }
                        }
                    }
                    bitmap.Dispose(); bitmap = null;
                }
                string errorMsg = "";
                if (windowedX < (tileWidth*80)) {
                    //windowed width too small
                    errorMsg = "Window width ("+windowedX+") less than "+(tileWidth*80)+". Tiles may be squished and black vertical bars may be in the map image.";
                }
                if (windowedY < (tileHeight*25)) {
                    //windowed height too small
                    if (errorMsg != "") {
                        errorMsg = errorMsg+" ";
                    }
                    errorMsg = errorMsg + "Window height ("+windowedY+") less than "+(tileHeight*25)+". Tiles may be squished and black horizontal bars may be in the map image.";
                }
                if (errorMsg!="") {
                    DialogResult dr = MessageBox.Show(LinuxInsertLineBreaks("If you are using DF 0.23.130.23a or earlier, with your current DF init settings, the bmps DF is writing out may be flawed (we assume you are using windowed mode - we don't check fullscreen). The problem is: "+errorMsg+" Do you want to continue compressing the image anyways?"), "Warning", MessageBoxButtons.YesNo);
                    if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                        return;
                    }
                }
            } catch (DirectoryNotFoundException) {
                //it would be nice if we could have one fall through to another...
                tileWidth = 0;
                tileHeight = 0;
                DialogResult dr = MessageBox.Show(LinuxInsertLineBreaks("Couldn't find your init file and/or your font bmp. You'll have to tell us what tile size you're using, and we won't be able to identify the font character and colors and such of the tiles in the image(s). If you want us to find the init file, cancel this attempt, and move the image(s) into your DF folder where they were originally written out by DF, or move the init.txt and font bmp into the same folder as these images. Continue compressing?"), "Continue?", MessageBoxButtons.YesNo);
                if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                    return;
                }
            } catch (FileNotFoundException) {
                tileWidth = 0;
                tileHeight = 0;
                DialogResult dr = MessageBox.Show(LinuxInsertLineBreaks("Couldn't find your init file and/or your font bmp. You'll have to tell us what tile size you're using, and we won't be able to identify the font character and colors and such of the tiles in the image(s). If you want us to find the init file, cancel this attempt, and move the image(s) into your DF folder where they were originally written out by DF, or move the init.txt and font bmp into the same folder as these images. Continue compressing?"), "Continue?", MessageBoxButtons.YesNo);
                if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                    return;
                }
            }
            
            /*TiledBitmapWrapper fontBitmap = new TiledBitmapWrapper(fontFile, tileWidth, tileHeight, progressForm);
            TiledBitmapWrapper[] objectGraphics = null;
            if (usingGraphics) {
                //progressForm.SetStatus("Reading graphics_example.txt.");
                using (StreamReader reader = new StreamReader(folder+"\\raw\\graphics\\graphics_example.txt")) {
                    List<TiledBitmapWrapper> tilemapWrappers = new List<TiledBitmapWrapper>();
                    string lastTilePage = null;
                    string lastTileFile = null;
                    string line;
                    while ((line = reader.ReadLine())!=null) {
                        char[] paramChars = { ':', '[', ']' };
                        string[] splitLine = line.Split(paramChars, StringSplitOptions.RemoveEmptyEntries);
                        if (splitLine.Length>=2) {
                            if (String.Compare(splitLine[0], "TILE_PAGE")==0) {
                                lastTilePage = splitLine[1];
                            } else if (String.Compare(splitLine[0], "FILE")==0) {
                                if (lastTilePage!=null) {
                                    lastTileFile = splitLine[1].Replace("/", "\\");
                                }
                            } else if (String.Compare(splitLine[0], "TILE_DIM")==0) {
                                if (lastTilePage!=null) {
                                    int graphicsTileWidth = Int32.Parse(splitLine[1]);
                                    int graphicsTileHeight = Int32.Parse(splitLine[2]);
                                    tilemapWrappers.Add(new TiledBitmapWrapper(folder+"\\raw\\graphics\\"+lastTileFile, graphicsTileWidth, graphicsTileHeight, progressForm));
                                    //progressForm.SetStatus("Reading graphics_example.txt.");
                                    lastTilePage = null;
                                }
                            }
                        }
                    }
                    objectGraphics = tilemapWrappers.ToArray();
                }
            }*/
            string filename = "";
            string outFilename = "";
            if (zlibFormat) {
                filename = selectMapFileToEncodeOFD2.FileName;
                outFilename = encodeFortressMapSFD2.FileName;
            } else {
                filename = selectMapFileToEncodeOFD.FileName;
                outFilename = encodeFortressMapSFD.FileName;
            }
            bool tileSizeHadToBeEntered = false;
            if (tileWidth==0 && tileHeight==0) {
                tileSizeHadToBeEntered = true;
                TileSizeChooser chooser = new TileSizeChooser(null, progressForm, new TileSizeChosen(TileSizeWrapper));
                waitingForChooser=true;
                chooser.Show();
                while (waitingForChooser) {
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
            }
            ZLayerFileList zLayerFileList = CheckIsNewDFFilename(filename);
            if (zLayerFileList!=null) {
                int[] zLevels = new int[zLayerFileList.Count];
                MapBitmapData[] mapBitmapData = new MapBitmapData[zLayerFileList.Count];
                BitmapPartBytes[][] outReferencesArray = new BitmapPartBytes[zLayerFileList.Count][];
                SortedDictionary<BitmapPartBytes, BitmapPartBytes> splitTiles = new SortedDictionary<BitmapPartBytes, BitmapPartBytes>();
                int outReferencesLength = 0;
                for (int entryNum=0; entryNum<zLayerFileList.Count; entryNum++) {
                    progressForm.SetPrefix("(Layer "+(entryNum+1)+" of "+zLayerFileList.Count+") ");
                    zLevels[entryNum] = zLayerFileList.GetZLayer(entryNum);
                    FileInfo fi = zLayerFileList.GetFile(entryNum);
                    int bitmapWidth = 0;
                    int bitmapHeight = 0;
					bool doPartial = false; //this was for an experimental feature
					/*if (PartialBitmapLoader.GetBitmapSize(fi, out bitmapWidth, out bitmapHeight)) {
						if ((bitmapWidth * bitmapHeight) >= 4000000) {	//number was lowered from 20000000 for testing purposes
							doPartial = true;
						}
					}*/
					if (doPartial) {
#region disabledcode
						
					    int width = tileWidth*50;
                        int height = tileHeight*50;
                        int numTilesX = bitmapWidth/tileWidth;
                        int numTilesY = bitmapHeight/tileHeight;
                        int numTiles = numTilesX * numTilesY;
                        BitmapPartBytes[] outReferences = new BitmapPartBytes[numTiles];
                        for (int xStart=0, tileXStart=0; xStart<bitmapWidth; xStart+=width, xStart+=50) {
                            for (int yStart=0, tileYStart=0; yStart<bitmapHeight; yStart+=height, tileYStart+=50) {
                                Stream zipEntryInputStream = fi.OpenRead();
                                int xwidth = width;
                                int yheight = height;
                                if (xStart+xwidth>bitmapWidth) {
                                    xwidth = bitmapWidth - xStart;
                                }
                                if (yStart+yheight>bitmapHeight) {
                                    yheight = bitmapHeight - yStart;
                                }
                                progressForm.SetStatus("Loading image");
                                byte[] bytes = PartialBitmapLoader.Load(zipEntryInputStream, xStart, yStart, xwidth, yheight);
                                progressForm.SetStatus("Processing image");
                                using (TiledBitmapWrapper map = new TiledBitmapWrapper(bytes, xwidth, yheight, tileWidth, tileHeight)) {
                                    zipEntryInputStream.Close();
                                    zipEntryInputStream.Dispose();
                                    zipEntryInputStream = null;
                                    GC.Collect();
                                    SplitTilesetsIntoPart(tileXStart, tileYStart, xwidth/tileWidth, yheight/tileHeight, numTilesX, numTilesY, ref splitTiles, progressForm, map, ref outReferences);
                                    outReferencesArray[entryNum] = outReferences;
                                    outReferencesLength += outReferences.Length;
                                    mapBitmapData[entryNum] = new MapBitmapData(map);
                                    //Console.WriteLine("GC Total Memory splitting TBW for entry "+entryNum+": "+GC.GetTotalMemory(false));
                                }
                            }
                        }
#endregion disabledcode
                    } else {
                        Stream zipEntryInputStream = fi.OpenRead();
                        //Console.WriteLine("GC Total Memory before entry "+entryNum+": "+GC.GetTotalMemory(false));
                        progressForm.SetStatus("Loading image");

                        Bitmap bitmap = (Bitmap) Bitmap.FromStream(zipEntryInputStream);
                        //Console.WriteLine("GC Total Memory after loading bitmap for entry "+entryNum+": "+GC.GetTotalMemory(false));
                        /*if (zLevels[entryNum]==26) {
                            Console.WriteLine("Moo.");
                        }*/
                        progressForm.SetStatus("Processing image");
                        using (TiledBitmapWrapper map = new TiledBitmapWrapper(bitmap, tileWidth, tileHeight, progressForm)) {
                            //Console.WriteLine("GC Total Memory after initializing TBW for entry "+entryNum+": "+GC.GetTotalMemory(false));
                            bitmap.Dispose();
                            bitmap = null;
                            zipEntryInputStream.Close();
                            zipEntryInputStream.Dispose();
                            zipEntryInputStream = null;
                            GC.Collect();
                            //Console.WriteLine("GC Total Memory after disposing of bitmap for entry "+entryNum+": "+GC.GetTotalMemory(false));
                            BitmapPartBytes[] outReferences;
                            SplitTilesetsInto(ref splitTiles, progressForm, map, out outReferences);
                            outReferencesArray[entryNum] = outReferences;
                            outReferencesLength += outReferences.Length;
                            mapBitmapData[entryNum] = new MapBitmapData(map);
                            //Console.WriteLine("GC Total Memory splitting TBW for entry "+entryNum+": "+GC.GetTotalMemory(false));
                        }
                    }
                    //Console.WriteLine("GC Total Memory after finishing with TBW for entry "+entryNum+": "+GC.GetTotalMemory(false));
                    GC.Collect();
                    //Console.WriteLine("GC Total Memory after calling GC collection for entry "+entryNum+": "+GC.GetTotalMemory(false));
                }
                progressForm.SetPrefix("");

                float uniqueTileImagesPercent = 100.0f*splitTiles.Count/outReferencesLength;
				Console.WriteLine("Unique tile images: "+splitTiles.Count+"/"+outReferencesLength+", or "+(uniqueTileImagesPercent)+"%"); //this was originally being written for debugging purposes
                if (uniqueTileImagesPercent > 2.0f) {
                    DialogResult dr;
                    bool retry = true;
                    while (retry) {
                        retry = false;
                        if (tileSizeHadToBeEntered) {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("This multi-level map has "+Math.Round(uniqueTileImagesPercent, 2)+"% unique tile images ("+splitTiles.Count+" identified tile images in "+outReferencesLength+" total tiles). This is bad. The more unique tile images, the worse the compression will be. You probably entered the wrong tile size (unless you were compressing a JPG or GIF or something). Continue compressing anyways?"), "Uh oh", MessageBoxButtons.YesNo);
                        } else {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("This multi-level map has "+Math.Round(uniqueTileImagesPercent, 2)+"% unique tile images ("+splitTiles.Count+" identified tile images in "+outReferencesLength+" total tiles). This is bad. The more unique tile images, the worse the compression will be. Did you put a map image from someone else's fort into your DF folder? Did you change your font after making the image? The compressor identified your tile size as "+tileWidth+"x"+tileHeight+" pixels from your init file and font bmp. If this is incorrect, move the image(s) out of your DF folder, tell it to compress them again, and enter the correct tile size. If that is correct, however, something else is wrong. The image(s) aren't JPGs or GIFs or something, are they? You should be using this on the original BMPs that DF wrote out, or PNGs which were saved at 24 bpp or 32 bpp. Continue compressing anyways?"), "Uh oh", MessageBoxButtons.YesNo);
                        }

                        if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                            return;
                        } else {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("Did you read the entire error message that was just shown to you?"), "That was an important message.", MessageBoxButtons.YesNo);
                            if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                                MessageBox.Show(LinuxInsertLineBreaks("You should. It was important. It is basically impossible to get that error message if the compressor has the right tile size and the image(s) are not damaged by having been changed into a lossy compression format, or reduced in color depth (bits per pixel), etc."));
                                retry = true;
                            }
                        }
                    }
                }
                
                
                Encode(progressForm, mapBitmapData, splitTiles, zLevels, outReferencesArray, outFilename, textFormat, zlibFormat, colors, fontTileBitmaps);
                //delete files
                if (deleteImagesCheckbox.Checked) {
                    for (int entryNum=0; entryNum<zLayerFileList.Count; entryNum++) {
                        FileInfo file = zLayerFileList.GetFile(entryNum);
                        file.Delete();
                        /*
                        int slashpos = file.FullName.LastIndexOf("/");
                        int backslashpos = file.FullName.LastIndexOf("\\");
                        if (backslashpos>slashpos) {
                            slashpos=backslashpos;
                        }
                        string path = file.FullName.Substring(0, slashpos+1);
                        string restOfName = file.FullName.Substring(slashpos+1);
                        File.Move(file.FullName, path+"Done-"+restOfName);
                        */
                    }
                }
            #if !MONO
            } else if (filename.EndsWith(".zip")) {
                ZipFile zipFile = new ZipFile(filename);
                int[] zLevels = new int[zipFile.Count];
                MapBitmapData[] mapBitmapData = new MapBitmapData[zipFile.Count];
                BitmapPartBytes[][] outReferencesArray = new BitmapPartBytes[zipFile.Count][];
                SortedDictionary<BitmapPartBytes, BitmapPartBytes> splitTiles = new SortedDictionary<BitmapPartBytes, BitmapPartBytes>();
                int outReferencesLength = 0;

                for (int entryNum=0; entryNum<zipFile.Count; entryNum++ ) {
                    progressForm.SetPrefix("(Layer "+(entryNum+1)+" of "+zipFile.Count+") "); 
                    zLevels[entryNum] = Int32.Parse(zipFile[entryNum].Name.Split('.')[0]);
                    Stream zipEntryInputStream = zipFile.GetInputStream(entryNum);
                    //Console.WriteLine("GC Total Memory before entry "+entryNum+": "+GC.GetTotalMemory(false));
                    progressForm.SetStatus("Decompressing image from zip file");
                    Bitmap bitmap = (Bitmap) Bitmap.FromStream(zipEntryInputStream);
                    //Console.WriteLine("GC Total Memory after loading bitmap for entry "+entryNum+": "+GC.GetTotalMemory(false));
                    progressForm.SetStatus("Loading image");
                    using (TiledBitmapWrapper map = new TiledBitmapWrapper(bitmap, tileWidth, tileHeight, progressForm)) {
                        //Console.WriteLine("GC Total Memory after initializing TBW for entry "+entryNum+": "+GC.GetTotalMemory(false));
                        bitmap.Dispose();
                        bitmap = null;
                        zipEntryInputStream.Close();
                        zipEntryInputStream.Dispose();
                        zipEntryInputStream = null;
                        GC.Collect();
                        //Console.WriteLine("GC Total Memory after disposing of bitmap for entry "+entryNum+": "+GC.GetTotalMemory(false));
                        BitmapPartBytes[] outReferences;
                        SplitTilesetsInto(ref splitTiles, progressForm, map, out outReferences);
                        outReferencesArray[entryNum] = outReferences;
                        outReferencesLength += outReferences.Length;
                        mapBitmapData[entryNum] = new MapBitmapData(map);
                        //Console.WriteLine("GC Total Memory splitting TBW for entry "+entryNum+": "+GC.GetTotalMemory(false));
                    }
                    //Console.WriteLine("GC Total Memory after finishing with TBW for entry "+entryNum+": "+GC.GetTotalMemory(false));
                    GC.Collect();
                    //Console.WriteLine("GC Total Memory after calling GC collection for entry "+entryNum+": "+GC.GetTotalMemory(false));
                }
                progressForm.SetPrefix(""); 

                float uniqueTileImagesPercent = 100.0f*splitTiles.Count/outReferencesLength;
                Console.WriteLine("Unique tile images: "+splitTiles.Count+"/"+outReferencesLength+", or "+(uniqueTileImagesPercent)+"%");
                if (uniqueTileImagesPercent > 2.0f) {
                    DialogResult dr;
                    bool retry = true;
                    while (retry) {
                        retry = false;
                        if (tileSizeHadToBeEntered) {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("This multi-level map has "+Math.Round(uniqueTileImagesPercent, 2)+"% unique tile images ("+splitTiles.Count+" identified tile images in "+outReferencesLength+" total tiles). This is bad. The more unique tile images, the worse the compression will be. You probably entered the wrong tile size (unless you were compressing a JPG or GIF or something). Continue compressing anyways?"), "Uh oh", MessageBoxButtons.YesNo);
                        } else {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("This multi-level map has "+Math.Round(uniqueTileImagesPercent, 2)+"% unique tile images ("+splitTiles.Count+" identified tile images in "+outReferencesLength+" total tiles). This is bad. The more unique tile images, the worse the compression will be. Did you put a map image from someone else's fort into your DF folder? Did you change your font after making the image? The compressor identified your tile size as "+tileWidth+"x"+tileHeight+" pixels from your init file and font bmp. If this is incorrect, move the image(s) out of your DF folder, tell it to compress them again, and enter the correct tile size. If that is correct, however, something else is wrong. The image(s) aren't JPGs or GIFs or something, are they? You should be using this on the original BMPs that DF wrote out, or PNGs which were saved at 24 bpp or 32 bpp. Continue compressing anyways?"), "Uh oh", MessageBoxButtons.YesNo);
                        }

                        if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                            return;
                        } else {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("Did you read the entire error message that was just shown to you?"), "That was an important message.", MessageBoxButtons.YesNo);
                            if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                                MessageBox.Show(LinuxInsertLineBreaks("You should. It was important. It is basically impossible to get that error message if the compressor has the right tile size and the image(s) are not damaged by having been changed into a lossy compression format, or reduced in color depth (bits per pixel), etc."));
                                retry = true;
                            }
                        }
                    }
                }
                Encode(progressForm, mapBitmapData, splitTiles, zLevels, outReferencesArray, outFilename, textFormat, zlibFormat, colors, fontTileBitmaps);
#endif
            } else {
                MapBitmapData mapBitmapData;
                BitmapPartBytes[] outReferences; 
                SortedDictionary<BitmapPartBytes, BitmapPartBytes> splitTiles;
                {
                    TiledBitmapWrapper map;
                    if (image==null) {
                        map = new TiledBitmapWrapper(filename, tileWidth, tileHeight, progressForm);
                    } else {
                        map = new TiledBitmapWrapper(image, tileWidth, tileHeight, progressForm);
                    }
                    
                    splitTiles = SplitTilesets(progressForm, map, out outReferences);
                    mapBitmapData = new MapBitmapData(map);
                }

                float uniqueTileImagesPercent = 100.0f*splitTiles.Count/outReferences.Length;
                Console.WriteLine("Unique tile images: "+splitTiles.Count+"/"+outReferences.Length+", or "+(uniqueTileImagesPercent)+"%");
                if (uniqueTileImagesPercent > 2.0f) {
                    DialogResult dr;
                    bool retry = true;
                    while (retry) {
                        retry = false;
                        if (tileSizeHadToBeEntered) {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("This image has "+Math.Round(uniqueTileImagesPercent, 2)+"% unique tile images ("+splitTiles.Count+" identified tile images in "+outReferences.Length+" total tiles). This is bad. The more unique tile images, the worse the compression will be. You probably entered the wrong tile size (unless you were compressing a JPG or GIF or something). Continue compressing anyways?"), "Uh oh", MessageBoxButtons.YesNo);
                        } else {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("This multi-level map has "+Math.Round(uniqueTileImagesPercent, 2)+"% unique tile images ("+splitTiles.Count+" identified tile images in "+outReferences.Length+" total tiles). This is bad. The more unique tile images, the worse the compression will be. Did you put a map image from someone else's fort into your DF folder? Did you change your font after making the image? The compressor identified your tile size as "+tileWidth+"x"+tileHeight+" pixels from your init file and font bmp. If this is incorrect, move the image(s) out of your DF folder, tell it to compress them again, and enter the correct tile size. If that is correct, however, something else is wrong. The image(s) aren't JPGs or GIFs or something, are they? You should be using this on the original BMPs that DF wrote out, or PNGs which were saved at 24 bpp or 32 bpp. Continue compressing anyways?"), "Uh oh", MessageBoxButtons.YesNo);
                        }

                        if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                            return;
                        } else {
                            dr = MessageBox.Show(LinuxInsertLineBreaks("Did you read the entire error message that was just shown to you?"), "That was an important message.", MessageBoxButtons.YesNo);
                            if (dr==DialogResult.Cancel || dr==DialogResult.No) {
                                MessageBox.Show(LinuxInsertLineBreaks("You should. It was important. It is basically impossible to get that error message if the compressor has the right tile size and the image(s) are not damaged by having been changed into a lossy compression format, or reduced in color depth (bits per pixel), etc."));
                                retry = true;
                            }
                        }
                    } 
                }
                Encode(progressForm, mapBitmapData, splitTiles, outReferences, outFilename, textFormat, zlibFormat, colors, fontTileBitmaps);
                //delete files
                if (deleteImagesCheckbox.Checked) {
                    if (filename!=null) {
                        File.Delete(filename);
                    }
                }
            }
        }

        private string StripNumberFromNewDFFilename(string filename) {
            int dfmap = filename.LastIndexOf("df-map");
            if (dfmap!=filename.Length-6) {
                int localpos = filename.IndexOf("local_map");
                if (localpos>-1) {
                    return null;
                }
                string[] pathsplit = filename.Split('\\', '/');
                string pathlessFilename = pathsplit[pathsplit.Length-1];
                string[] sections = pathlessFilename.Split('-', '.');
                int results = 0;
                if (sections.Length<6) {
                    return null;
                }
                int resultsBit = 1;
                for (int i=sections.Length-2; i>=sections.Length-3; i--) {
                    string ztext = sections[i];
                    try {
                        /*int zlevel = */
                        Int32.Parse(ztext);
                        results = results|resultsBit;
                    } catch (FormatException) {
                    } catch (OverflowException) {
                    }
                    resultsBit += resultsBit;
                }
                for (int i=1; i>=0; i--) {
                    string ztext = sections[i];
                    try {
                        /*int zlevel = */
                        Int32.Parse(ztext);
                        results = results|resultsBit;
                    } catch (FormatException) {
                    } catch (OverflowException) {
                    }
                    resultsBit += resultsBit;
                }
                if (results==7) {
                    int aDash = pathlessFilename.LastIndexOf('-');
                    aDash = pathlessFilename.LastIndexOf('-', aDash-1); //end of region string
                    int zlevelDash = pathlessFilename.IndexOf('-');
                    int regionDash = pathlessFilename.IndexOf('-', zlevelDash+1);
                    int pathLen = filename.Length - pathlessFilename.Length;

                    aDash+=pathLen;
                    zlevelDash+=pathLen;
                    regionDash+=pathLen;

                    filename = filename.Substring(0, zlevelDash) + filename.Substring(regionDash);
                    return filename;
                }
            }
            return null;
        }

        private ZLayerFileList CheckIsNewDFFilename(string filename) {
            //"The Blades of Ancien-14-region1-1051-17705.bmp"
            ZLayerFileList zLayerFileList = new ZLayerFileList();
            int localpos = filename.IndexOf("local_map");
            if (localpos>-1) {
                return null;
            }
            string[] pathsplit = filename.Split('\\', '/');
            string pathlessFilename = pathsplit[pathsplit.Length-1];
            string[] sections = pathlessFilename.Split('-', '.');
            int results = 0;
            if (sections.Length<6) {
                return null;
            }
            int resultsBit = 1;
            for (int i=sections.Length-2; i>=sections.Length-3; i--) {
                string ztext = sections[i];
                try {
                    /*int zlevel = */
                    Int32.Parse(ztext);
                    results = results|resultsBit;
                } catch (FormatException) {
                } catch (OverflowException) {
                }
                resultsBit += resultsBit;
            }
            for (int i=1; i>=0; i--) {
                string ztext = sections[i];
                try {
                    /*int zlevel = */
                    Int32.Parse(ztext);
                    results = results|resultsBit;
                } catch (FormatException) {
                } catch (OverflowException) {
                }
                resultsBit += resultsBit;
            }
            if (results==7) {
                //Pineoaks the Woods o-10-pineoaks-spr-1064-1064-0.bmp
                int aDash = pathlessFilename.LastIndexOf('-');
                aDash = pathlessFilename.LastIndexOf('-', aDash-1); //end of region string
                int zlevelDash = pathlessFilename.IndexOf('-');
                int regionDash = pathlessFilename.IndexOf('-', zlevelDash+1);
                int pathLen = filename.Length - pathlessFilename.Length;

                aDash+=pathLen;
                zlevelDash+=pathLen;
                regionDash+=pathLen;

                int slashpos = pathLen-1;
                string path = filename.Substring(0, slashpos+1);
                string leftFilename = filename.Substring(slashpos+1, zlevelDash-(slashpos));
                string rightFilename = filename.Substring(regionDash);
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles(leftFilename+"*"+rightFilename);
                foreach (FileInfo file in files) {
                    string name = file.Name;
                    int aDash2 = name.LastIndexOf('-');
                    aDash2 = name.LastIndexOf('-', aDash2-1);
                    int zlevelDash2 = name.IndexOf('-');
                    if (zlevelDash2>-1) {
                        int regionDash2 = name.IndexOf('-', zlevelDash2+1);
                        if (regionDash2>-1) {
                            string ztext = name.Substring(zlevelDash2+1, regionDash2-(zlevelDash2+1));
                            try {
                                int zlevel = Int32.Parse(ztext);
                                zLayerFileList.Add(file, zlevel);
                            } catch (FormatException) {
                            } catch (OverflowException) {
                            }

                        }
                    }
                }
                return zLayerFileList;
            }
            return null;
        }

        private void SplitTilesetsIntoPart(int tileXStart, int tileYStart, int sectionNumTilesWide, int sectionNumTilesHigh, int numTilesWide, int numTilesHigh, ref SortedDictionary<BitmapPartBytes, BitmapPartBytes> tileImages, ProgressForm progressForm, TiledBitmapWrapper mapBitmap, ref BitmapPartBytes[] outReferences) {
            progressForm.SetStatus("Splitting image into tiles and identifying unique tile images");
            progressForm.SetMaximum(numTilesHigh*numTilesWide);
            int tileNum=0;
            int nextColTileNum=tileXStart*numTilesHigh + tileYStart;
            for (int x=tileXStart; x<tileXStart+sectionNumTilesWide; x++) {
                tileNum=nextColTileNum;
                nextColTileNum+=numTilesHigh;
                for (int y=tileYStart; y<tileYStart+sectionNumTilesHigh; y++, tileNum++) {
                    BitmapPartBytes mapTile = mapBitmap.GetTile(x-tileXStart, y-tileYStart);
                    BitmapPartBytes foundTile;
                    bool success = tileImages.TryGetValue(mapTile, out foundTile);
                    if (!success) {
                        tileImages.Add(mapTile, mapTile);
                        outReferences[tileNum] = mapTile;
                        mapTile.Popularity = 1;
                    } else {
                        outReferences[tileNum] = foundTile;
                        foundTile.Popularity += 1;
                    }
                }
                progressForm.SetProgress(tileNum);
            }
            progressForm.SetProgress(mapBitmap.NumTiles);
        }
        
        private void SplitTilesetsInto(ref SortedDictionary<BitmapPartBytes, BitmapPartBytes> tileImages, ProgressForm progressForm, TiledBitmapWrapper mapBitmap, out BitmapPartBytes[] outReferences) {
            progressForm.SetStatus("Splitting image into tiles and identifying unique tile images");
            progressForm.SetMaximum(mapBitmap.NumTiles);
            int tileNum=0;
            outReferences = new BitmapPartBytes[mapBitmap.NumTiles];
            for (int x=0; x<mapBitmap.NumTilesX; x++) {
                for (int y=0; y<mapBitmap.NumTilesY; y++, tileNum++) {
                    /*if (x==61 && y==135) {
                        Console.WriteLine("moo");
                    }*/
                    BitmapPartBytes mapTile = mapBitmap.GetTile(x, y);
                    BitmapPartBytes foundTile;
                    bool success = tileImages.TryGetValue(mapTile, out foundTile);
                    if (!success) {
                        bool ckey = tileImages.ContainsKey(mapTile);
                        bool cvalue = tileImages.ContainsValue(mapTile);
                        if (ckey || cvalue) {
                            Console.WriteLine("Already exists?");
                        }
                        tileImages.Add(mapTile, mapTile);
                        ckey = tileImages.ContainsKey(mapTile);
                        cvalue = tileImages.ContainsValue(mapTile);
                        if (!ckey || !cvalue) {
                            Console.WriteLine("Failed to add properly?");
                        }
                        
                        outReferences[tileNum] = mapTile;
                        mapTile.Popularity = 1;
                    } else {
                        outReferences[tileNum] = foundTile;
                        foundTile.Popularity += 1;
                    }
                }
                progressForm.SetProgress(tileNum);
            }
            progressForm.SetProgress(mapBitmap.NumTiles);
        }

        private SortedDictionary<BitmapPartBytes, BitmapPartBytes> SplitTilesets(ProgressForm progressForm, TiledBitmapWrapper mapBitmap, out BitmapPartBytes[] outReferences) {
            SortedDictionary<BitmapPartBytes, BitmapPartBytes> tileImages = new SortedDictionary<BitmapPartBytes, BitmapPartBytes>();
            progressForm.SetStatus("Encoding map tiles...");
            progressForm.SetMaximum(mapBitmap.NumTiles);
            int tileNum=0;
            outReferences = new BitmapPartBytes[mapBitmap.NumTiles];
            for (int x=0; x<mapBitmap.NumTilesX; x++) {
                for (int y=0; y<mapBitmap.NumTilesY; y++, tileNum++) {
                    BitmapPartBytes mapTile = mapBitmap.GetTile(x, y);
                    BitmapPartBytes foundTile;
                    bool success = tileImages.TryGetValue(mapTile, out foundTile);
                    if (!success) {
                        bool ckey = tileImages.ContainsKey(mapTile);
                        bool cvalue = tileImages.ContainsValue(mapTile);
                        if (ckey || cvalue) {
                            Console.WriteLine("Already exists?");
                        }
                        tileImages.Add(mapTile, mapTile);
                        ckey = tileImages.ContainsKey(mapTile);
                        cvalue = tileImages.ContainsValue(mapTile);
                        if (!ckey || !cvalue) {
                            Console.WriteLine("Failed to add properly?");
                        }
                        outReferences[tileNum] = mapTile;
                        mapTile.Popularity = 1;
                    } else {
                        outReferences[tileNum] = foundTile;
                        foundTile.Popularity += 1;
                    }
                }
                progressForm.SetProgress(tileNum);
            }
            progressForm.SetProgress(mapBitmap.NumTiles);
            return tileImages;
        }

		public byte[] ReverseBytes(byte[] dataBuffer) {
			int dataLen = dataBuffer.Length;
			byte[] newBuffer = new byte[dataLen];
			for (int pos=0; pos<dataLen; pos++) {
				newBuffer[pos] = dataBuffer[dataLen-pos-1];
			}
			return newBuffer;
		}

        #region Encode functions
        //lzmaStream is null for writing to the flash format
        private void EncodeOutput(bool isMapData, StreamWriter outputStream, Stream zipStream, string fieldName, byte data, bool endOfLine) {
            if (zipStream!=null) {
                zipStream.WriteByte(data);
            } else if (isMapData) {
                if (endOfLine) {
                    outputStream.WriteLine(data);
                } else {
                    outputStream.Write(data+" ");
                }
            } else {
                outputStream.WriteLine(fieldName+"="+data);
            }
        }
        private void EncodeOutput(bool isMapData, StreamWriter outputStream, Stream zipStream, string fieldName, sbyte data, bool endOfLine) {
            if (zipStream!=null) {
                zipStream.WriteByte((byte) data);
            } else if (isMapData) {
                if (endOfLine) {
                    outputStream.WriteLine(data);
                } else {
                    outputStream.Write(data+" ");
                }
            } else {
                outputStream.WriteLine(fieldName+"="+data);
            }
        }
        private void EncodeOutput(bool isMapData, StreamWriter outputStream, Stream zipStream, string fieldName, UInt16 data, bool endOfLine) {
            if (zipStream!=null) {
				byte[] outBytes = BitConverter.GetBytes((UInt16) data);
				if (!BitConverter.IsLittleEndian) {
					outBytes = ReverseBytes(outBytes);
				}
                zipStream.Write(outBytes, 0, 2);
            } else if (isMapData) {
                if (endOfLine) {
                    outputStream.WriteLine(data);
                } else {
                    outputStream.Write(data+" ");
                }
            } else {
                outputStream.WriteLine(fieldName+"="+data);
            }
        }
        private void EncodeOutput(bool isMapData, StreamWriter outputStream, Stream zipStream, string fieldName, Int16 data, bool endOfLine) {
            if (zipStream!=null) {
				byte[] outBytes = BitConverter.GetBytes((Int16) data);
				if (!BitConverter.IsLittleEndian) {
					outBytes = ReverseBytes(outBytes);
				}
				zipStream.Write(outBytes, 0, 2);
            } else if (isMapData) {
                if (endOfLine) {
                    outputStream.WriteLine(data);
                } else {
                    outputStream.Write(data+" ");
                }
            } else {
                outputStream.WriteLine(fieldName+"="+data);
            }
        }
        private void EncodeOutput(bool isMapData, StreamWriter outputStream, Stream zipStream, string fieldName, UInt32 data, bool endOfLine) {
            if (zipStream!=null) {
				byte[] outBytes = BitConverter.GetBytes((UInt32) data);
				if (!BitConverter.IsLittleEndian) {
					outBytes = ReverseBytes(outBytes);
				}
				zipStream.Write(outBytes, 0, 4); 
            } else if (isMapData) {
                if (endOfLine) {
                    outputStream.WriteLine(data);
                } else {
                    outputStream.Write(data+" ");
                }
            } else {
                outputStream.WriteLine(fieldName+"="+data);
            }
        }
        private void EncodeOutput(bool isMapData, StreamWriter outputStream, Stream zipStream, string fieldName, Int32 data, bool endOfLine) {
            if (zipStream!=null) {
				byte[] outBytes = BitConverter.GetBytes((Int32) data);
				if (!BitConverter.IsLittleEndian) {
					outBytes = ReverseBytes(outBytes);
				}
				zipStream.Write(outBytes, 0, 4); 
            } else if (isMapData) {
                if (endOfLine) {
                    outputStream.WriteLine(data);
                } else {
                    outputStream.Write(data+" ");
                }
            } else {
                outputStream.WriteLine(fieldName+"="+data);
            }
        }
        #endregion

        private void TileIndexRLE<TSize>(ref int tileIndex, ref int lastTileIndex, ref int numLastTiles, ref StreamWriter streamWriter, ref Stream zipStream, ref int bytesWritten, bool forceWrite) {
#if TileIndexRLEMethod1
            const int maxNumLastTiles = 255;
#elif TileIndexRLEMethod2
            const int maxNumLastTiles = 0xff;
#elif TileIndexRLEMethod3
            const int maxNumLastTiles = 0xffff;
#elif TileIndexRLEMethod4
            const int maxNumLastTiles = 0x7fffffff;
#else
            const int maxNumLastTiles = 1;
#endif
            if (lastTileIndex==-1) {
                lastTileIndex = tileIndex;
                numLastTiles = 0;
            }
            
            if (lastTileIndex==tileIndex && numLastTiles<maxNumLastTiles && !forceWrite) {
                numLastTiles+=1;
            } else {
                if (forceWrite) {
                    //this is the last tile in the layer
                    if (tileIndex==lastTileIndex && numLastTiles<maxNumLastTiles) {
                        numLastTiles++;
                    }
                }

                for (int i=0; i<2; i++) {
                    long flagEnable = (numLastTiles>1)?0xffffffff:0x00000000;

                
#if TileIndexRLEMethod1
            if (numLastTiles>1) {
                if (typeof(TSize)==typeof(sbyte)) {
                    EncodeOutput(true, streamWriter, zipStream, "", (sbyte) (-numLastTiles), false);
                    bytesWritten+=1;
                } else if (typeof(TSize)==typeof(Int16)) {
                    EncodeOutput(true, streamWriter, zipStream, "", (Int16) (-numLastTiles), false);
                    bytesWritten+=2;
                } else if (typeof(TSize)==typeof(Int32)) {
                    EncodeOutput(true, streamWriter, zipStream, "", (Int32) (-numLastTiles), false);
                    bytesWritten+=4;
                }
            }
#endif
#if TileIndexRLEMethod1
            long value = lastTileIndex;
#endif
                    if (typeof(TSize)==typeof(sbyte)) {
#if TileIndexRLEMethod1
#else
                        Sanity.IfTrueThrow(lastTileIndex>=0x80 || lastTileIndex<0, "lastTileIndex ("+lastTileIndex+") out of bounds!");
                        long value = (uint) lastTileIndex|(0x80&(uint) flagEnable);
#endif
                        EncodeOutput(true, streamWriter, zipStream, "", (Byte) value, false);
                        bytesWritten+=1;
                    } else if (typeof(TSize)==typeof(Int16)) {
#if TileIndexRLEMethod1
#else
                        Sanity.IfTrueThrow(lastTileIndex>=0x8000 || lastTileIndex<0, "lastTileIndex ("+lastTileIndex+") out of bounds!");
                        long value = (uint) lastTileIndex|(0x8000&(uint) flagEnable);
#endif
                        EncodeOutput(true, streamWriter, zipStream, "", (UInt16) value, false);
                        bytesWritten+=2;
                    } else if (typeof(TSize)==typeof(Int32)) {
#if TileIndexRLEMethod1
#else
                        Sanity.IfTrueThrow(lastTileIndex<0, "lastTileIndex ("+lastTileIndex+") out of bounds!");
                        UInt32 value = ((uint) lastTileIndex)|(uint) (0x80000000&flagEnable);
#endif
                        EncodeOutput(true, streamWriter, zipStream, "", (UInt32) value, false);
                        bytesWritten+=4;
                    }

                    if (numLastTiles>1) {
#if TileIndexRLEMethod1
#elif TileIndexRLEMethod2

                        EncodeOutput(true, streamWriter, zipStream, "", (byte) (numLastTiles), false);
                        bytesWritten+=1;
#elif TileIndexRLEMethod3
                    EncodeOutput(true, streamWriter, zipStream, "", (UInt16) (numLastTiles), false);
                    bytesWritten+=2;
#elif TileIndexRLEMethod4
                    EncodeOutput(true, streamWriter, zipStream, "", (UInt32) (numLastTiles), false);
                    bytesWritten+=4;
#endif
                    }
                    bool writeAnother = false;
                    if (forceWrite && tileIndex==lastTileIndex && numLastTiles<maxNumLastTiles) {
                        lastTileIndex = -1;
                        numLastTiles = 0;
                    } else {
                        lastTileIndex = tileIndex;
                        numLastTiles = 1;
                        if (forceWrite) {
                            writeAnother = true;
                        }
                    }
                    if (!forceWrite || (!writeAnother || lastTileIndex==-1)) {
                        break;
                    }
                }
                
            }
        }

        private void Encode(ProgressForm progressForm, MapBitmapData mapBitmap, SortedDictionary<BitmapPartBytes, BitmapPartBytes> splitTiles, BitmapPartBytes[] outReferences, string encodeFilename, bool textFormat, bool zlibFormat, Dictionary<string, DFColor> colors, BitmapPartBytes[,] fontTileBitmaps) {
            int[] zLevels = new int[1] {0};
            BitmapPartBytes[][] outReferencesArray = new BitmapPartBytes[1][] {outReferences};
            MapBitmapData[] mapBitmapDataArray = new MapBitmapData[1] { mapBitmap };
            Encode(progressForm, mapBitmapDataArray, splitTiles, zLevels, outReferencesArray, encodeFilename, textFormat, zlibFormat, colors, fontTileBitmaps);
        }

        private void Encode(ProgressForm progressForm, MapBitmapData[] mapBitmapData, SortedDictionary<BitmapPartBytes, BitmapPartBytes> splitTiles, int[] zLevels, BitmapPartBytes[][] outReferencesArray, string encodeFilename, bool textFormat, bool zlibFormat, Dictionary<string, DFColor> colors, BitmapPartBytes[,] fontTileBitmaps) {
            FileStream outputStream;
            colorMatchSensitivity = 12;
            try {
                colorMatchSensitivity = Int32.Parse(colorMatchSensitivityTextBox.Text);
            } catch (Exception) {
            }
            int outReferencesLength = 0;
            for (int layerIndex = 0; layerIndex<zLevels.Length; layerIndex++) {
                BitmapPartBytes[] outReferences = outReferencesArray[layerIndex];
                outReferencesLength += outReferences.Length;
            }
            float uniqueTileImagesPercent = 100.0f*splitTiles.Count/outReferencesLength;

            bool colorMatchSensitivityRaised = false;
            if (uniqueTileImagesPercent > 0.1f) {
                if (fontTileBitmaps!=null) {
                    DialogResult dr;
                    if (colorMatchSensitivity<200) {
                        dr = MessageBox.Show(LinuxInsertLineBreaks("This multi-level map may have color damage - Dwarf Fortress may have written out the images oddly, for instance. We may, however, be able to correct for it, by increasing the color match sensitivity. It'll make tile identification take longer, but the end result will be a smaller filesize, if it works. Would you like to increase the color match sensitivity to 200 (currently it is "+colorMatchSensitivity+")? Unique tile images percentage: "+Math.Round(uniqueTileImagesPercent, 2)+"% unique tile images ("+splitTiles.Count+" identified tile images in "+outReferencesLength+" total tiles). Any percentage above .1% triggers this message."), "Increase color match sensitivity?", MessageBoxButtons.YesNo);
                        if (dr==DialogResult.Yes) {
                            colorMatchSensitivityRaised = true;
                            colorMatchSensitivity=200;
                        }
                    }
                }
            }
            string shortEncodeFilename = encodeFilename;
            int tileWidth = mapBitmapData[0].TileWidth;
            int tileHeight = mapBitmapData[0].TileHeight;
            int tempIndex = encodeFilename.LastIndexOf('.');
            if (tempIndex>-1) {
                shortEncodeFilename = encodeFilename.Substring(0, tempIndex);
            }
            if (textFormat) {
                outputStream = File.Create(shortEncodeFilename+".txt");
            } else if (zlibFormat) {
                outputStream = File.Create(shortEncodeFilename+".fdf-map");
            } else {
                outputStream = File.Create(encodeFilename);
            }
            Stream zipStream = null;
            StreamWriter streamWriter = null;
            if (textFormat) {
                streamWriter = new StreamWriter(outputStream);
            } else if (zlibFormat) {
                zipStream = new DeflaterOutputStream(outputStream);
            } else {
                zipStream = new LZMAStream(outputStream, CompressionMode.Compress);            
            }
			int bytesWritten = 0;
            //File format:
            //Int32 negativeVersion (this would be -1 for the new multi-layer-supporting format, or >=0 for the previous format, which doesn't support multi-layer images. For the doc on the previous format, see DFMapViewer_FileFormat.txt (this is DFMapViewer_FileFormat_2.txt. Note that this variable is numberOfTiles in the old format, and is guaranteed to be >=0 in it.))
            int features = 0;
            if (fontTileBitmaps!=null) {
                features = features|0x01;
            }
#if TileIndexRLE
            features = features|0x02;
#endif
#if AutoClipping
            if (fontTileBitmaps!=null) {
                features = features|0x04;
            }
#endif
            EncodeOutput(false, streamWriter, zipStream, "negativeVersion", (int) (-1 - features), false);
            bytesWritten+=4;
            

            if (fontTileBitmaps!=null) {
                progressForm.SetStatus("Analyzing unique image tiles to identify font character and colors...");
            } else {
                progressForm.SetStatus("Writing unique image tiles and assigning indices...");
            }
            progressForm.SetMaximum(splitTiles.Count);
            SortedDictionary<BitmapPartBytes, BitmapPartBytes>.ValueCollection valueCollection = splitTiles.Values;
            List<BitmapPartBytes> valueArray = new List<BitmapPartBytes>(valueCollection);
            splitTiles.Clear();
            valueCollection = null;

            int tileNum = 0;
            
            int tilesFound = 0;
            int tilesNotFound = 0;
            if (fontTileBitmaps!=null) {
                int tempTileNum = tileNum;
                Dictionary<string, DFColor>.ValueCollection colorValues = colors.Values;

                DFColor[] colorArray = new DFColor[colorValues.Count];
                int colorIndex = 0;
                foreach (DFColor color in colorValues) {
                    colorArray[colorIndex] = color;
                    colorIndex+=1;
                }
                foreach (BitmapPartBytes value in valueArray) {
                    //Tile
                    int tilex = -1;
                    int tiley = -1;
                    DFColor bgColor;
                    DFColor fgColor;
                    //if (value==outReferencesArray[19][14775]) {
                    //    Console.WriteLine("moo");
                    //}
                    if (Analyze(colorArray, fontTileBitmaps, value, out tilex, out tiley, out bgColor, out fgColor)) {
                        int tileIndex = tilex + (tiley<<4);
                        value.StoreIdentification((byte) tileIndex, colorIDs[bgColor.Name], colorIDs[fgColor.Name]);
                        tilesFound++;
                    } else {
                        value.StoreIdentification(0xff, 0xff, 0xff);
                        tilesNotFound++;
                    }
                    tempTileNum++;
                    progressForm.SetProgress(tempTileNum);
                }
                List<BitmapPartBytes[]> valuesToDelete = new List<BitmapPartBytes[]>();
                //Checking for duplicates
                int duplicatesFound = 0;
                foreach (BitmapPartBytes baseValue in valueArray) {
                    if (!baseValue.BeingDeleted && (baseValue.TileIndex!=0xff || baseValue.FGColor!=0xff || baseValue.BGColor!=0xff)) {
                        foreach (BitmapPartBytes deleteValue in valueArray) {
                            if (deleteValue!=baseValue && !deleteValue.BeingDeleted) {
                                if ((deleteValue.TileIndex==baseValue.TileIndex && deleteValue.BGColor==baseValue.BGColor) && ((deleteValue.FGColor==baseValue.FGColor) || (baseValue.TileIndex==0) || (baseValue.TileIndex==0x20) || (baseValue.TileIndex==0xff))) {
                                    duplicatesFound += 1;
                                    valuesToDelete.Add(new BitmapPartBytes[] { deleteValue, baseValue });
                                    deleteValue.BeingDeleted = true;
                                    baseValue.BeingCloned = true;
                                }
                            }
                        }
                    }
                }
                
                Console.WriteLine("Identified "+tilesFound+", failed to identify "+tilesNotFound+", found "+duplicatesFound+" duplicates.");
                progressForm.SetStatus("Re-checking unidentified tiles...");
                progressForm.SetMaximum(valueArray.Count);
                tempTileNum=0;
                int tilesFixed = 0;
                foreach (BitmapPartBytes value in valueArray) {
                    //Tile
                    if (!value.BeingDeleted) {
                        if (value.TileIndex==0xff && value.FGColor==0xff && value.BGColor==0xff) {
                            int bestDifference = -1;
                            BitmapPartBytes bestClone = null;
                            foreach (BitmapPartBytes value2 in valueArray) {
                                if (value2!=value && !value2.BeingDeleted) {
                                    if (value2.TileIndex!=0xff || value2.FGColor!=0xff || value2.BGColor!=0xff) {
                                        int totalDifference=0;
                                        if (value.CompareTo(value2, out totalDifference, true)==0) {
                                            if (totalDifference<bestDifference || bestDifference==-1) {
                                                bestClone = value2;
                                            }
                                        }
                                    }
                                }
                            }
                            if (bestClone!=null) {
                                //int tileIndex = bestClone.TileIndex;
                                //value.CopyFrom(bestClone);
                                //value.StoreIdentification((byte) tileIndex, bestClone.BGColor, bestClone.FGColor);
                                valuesToDelete.Add(new BitmapPartBytes[] { value, bestClone });
                                value.BeingDeleted = true;
                                tilesFixed += 1;
                            }

                        }
                    }
                    tempTileNum++;
                    progressForm.SetProgress(tempTileNum);
                }
                Console.WriteLine("Identified "+tilesFixed+" damaged tiles.");
                //List<BitmapPartBytes> removedReferences = new List<BitmapPartBytes>();
                progressForm.SetStatus("Deleting duplicate tiles...");
                progressForm.SetMaximum(valuesToDelete.Count);
                int otherTempTileNum=0;
                
                foreach (BitmapPartBytes[] pair in valuesToDelete) {
                    BitmapPartBytes toDelete = pair[0];
                    BitmapPartBytes changeTo = pair[1];
                    bool cont = true;
                    //removedReferences.Clear();
                    Sanity.IfTrueThrow(!toDelete.BeingDeleted, "toDelete is not being deleted!");
                    int curpos = 0;
                    while (cont) {
                        int index = valueArray.IndexOf(toDelete, curpos);
                        if (index>-1) {
                            BitmapPartBytes item = valueArray[index];
                            if (item==toDelete) {
                                valueArray.RemoveAt(index);
                                break;
                            } else {
                                curpos = index+1;
                            }
                        } else {
                            //item not found
                            Sanity.Throw("Error in DF Map Compressor: BPB to be deleted was not found in the list of values to be deleted!");
                    
                        }
                    }
                    //if (timesRemoved!=1) {
                        //Console.WriteLine("Times removed TileIndex "+toDelete.TileIndex+" : "+timesRemoved);
                    //}
                    for (int layerIndex = 0; layerIndex<zLevels.Length; layerIndex++) {
                        MapBitmapData mapBitmap = mapBitmapData[layerIndex];
                        BitmapPartBytes[] outReferences = outReferencesArray[layerIndex];
                        tempTileNum = 0;
                        for (int x=0; x<mapBitmap.NumTilesX; x++) {
                            for (int y=0; y<mapBitmap.NumTilesY; y++, tempTileNum++) {
                                if (outReferences[tempTileNum]==toDelete) {
                                    outReferences[tempTileNum] = changeTo;
                                }
                            }
                        }
                    }
                    //toDelete.CopyFrom(changeTo);
                    //toDelete.StoreIdentification((byte) changeTo.TileIndex, changeTo.BGColor, changeTo.FGColor);
                    toDelete.Dispose();
                    progressForm.SetProgress(otherTempTileNum);
                    otherTempTileNum++;
                }
                outReferencesLength = 0;
                for (int layerIndex = 0; layerIndex<zLevels.Length; layerIndex++) {
                    BitmapPartBytes[] outReferences = outReferencesArray[layerIndex];
                    outReferencesLength += outReferences.Length;
                }
                float newUniqueTileImagesPercent = 100.0f*valueArray.Count/outReferencesLength;
                Console.WriteLine("Unique tile images after duplicates removal: "+valueArray.Count+"/"+outReferencesLength+", or "+(uniqueTileImagesPercent)+"%");
                if (colorMatchSensitivityRaised) {
					if (duplicatesFound==0) {
						MessageBox.Show(LinuxInsertLineBreaks("We didn't find any color damage after all."));
					} else {
						int tilesIdentified = tilesFound-duplicatesFound;
						MessageBox.Show(LinuxInsertLineBreaks("Due to tile identification, the increased color match sensitivity, and duplicates identification, we were able to identify "+duplicatesFound+" tiles which were the same images with small differences, and merged them. In this manner we have reduced the unique tile images percentage from "+Math.Round(uniqueTileImagesPercent, 2)+" to "+Math.Round(newUniqueTileImagesPercent, 2)+". In the end, we had "+tilesIdentified+" identified tiles, and "+tilesNotFound+" tiles which matched no combination of font character, background color, and foreground color (we think they are probably graphical tiles)."));
					}
                }
            }
            BitmapPartBytes blackTile = null;
            foreach (BitmapPartBytes value in valueArray) {
                Sanity.IfTrueThrow(value.DataBytes==null, "Error in DF Map Compressor: A deleted tile persisted in the array of tiles.");
                if (value.TileIndex==0 && value.BGColor==0) {
                    blackTile = value;
                }
            }
            
            //Do BSC
#if BlackSpaceConversion
            if (fontTileBitmaps!=null) {
                int tempTileNum = 0;
				for (int layerIndex = 0; layerIndex<zLevels.Length; layerIndex++) {
					MapBitmapData mapBitmap = mapBitmapData[layerIndex];
					BitmapPartBytes[] outReferences = outReferencesArray[layerIndex];
					{
						BitmapPartBytes[] outReferencesCopy = new BitmapPartBytes[outReferences.Length];
						Array.Copy(outReferences, outReferencesCopy, outReferences.Length);
						tempTileNum = 0;
						int tilesInRow = mapBitmap.NumTilesY;
						for (int x=0; x<mapBitmap.NumTilesX; x++) {
							for (int y=0; y<mapBitmap.NumTilesY; y++, tempTileNum++) {
								if (x>=10 && x<=12 && y>=31 && y<=33) {
									Console.WriteLine("moo "+layerIndex);
								}
								if (this.IsPossibleRandomTileOnBlackSpace(outReferences[tempTileNum])) {
									//find at least one black space tile around it
									bool bsc = false;
									if (y>0 && this.IsBlackSpace(outReferencesCopy[tempTileNum-1])) {
										bsc = true;
									} else if (y<mapBitmap.NumTilesY-1 && this.IsBlackSpace(outReferencesCopy[tempTileNum+1])) {
										bsc = true;
									} else if (x>0 && this.IsBlackSpace(outReferencesCopy[tempTileNum-tilesInRow])) {
										bsc = true;
									} else if (x<mapBitmap.NumTilesX-1 && this.IsBlackSpace(outReferencesCopy[tempTileNum+tilesInRow])) {
										bsc = true;
									}
									if (bsc) {
										outReferences[tempTileNum] = blackTile;
									}
								}
							}
						}
					}
					GC.Collect();
				}				
            }

#endif

            //Int32 numberOfTiles
            EncodeOutput(false, streamWriter, zipStream, "numberOfTiles", (int) valueArray.Count, false); bytesWritten+=4;
            //Int32 tileWidth
            EncodeOutput(false, streamWriter, zipStream, "tileWidth", (int) tileWidth, false); bytesWritten+=4; //TileWidth and TileHeight should be the same for all the map images.
            //Int32 tileHeight
            EncodeOutput(false, streamWriter, zipStream, "tileHeight", (int) tileHeight, false); bytesWritten+=4;
            //Int32 mapLayers
            EncodeOutput(false, streamWriter, zipStream, "mapLayers", (int) zLevels.Length, false); bytesWritten+=4;
            
            //For each map layer:
            int[] westClippedBounds = new int[zLevels.Length];
            int[] eastClippedBounds = new int[zLevels.Length];
            int[] northClippedBounds = new int[zLevels.Length];
            int[] southClippedBounds = new int[zLevels.Length]; 
            for (int layerIndex = 0; layerIndex<zLevels.Length; layerIndex++) {
                westClippedBounds[layerIndex]=0;
                eastClippedBounds[layerIndex]=0;
                northClippedBounds[layerIndex]=0;
                southClippedBounds[layerIndex]=0;
                if (zLevels.Length>1) {
                    progressForm.SetPrefix("(Layer "+(layerIndex+1)+" of "+zLevels.Length+") ");
                } else {
                    progressForm.SetPrefix("");
                }
                MapBitmapData mapBitmap = mapBitmapData[layerIndex];
                BitmapPartBytes[] outReferences = outReferencesArray[layerIndex];
                //Int32 mapLayerDepth (This may change, but I'm thinking 0 would be the ground layer, and higher (towards the sky) layers (if any) would have positive numbers, and lower (deeper) layers would have negative numbers - higher numbers are higher altitude, lower numbers are lower altitude.)
                EncodeOutput(false, streamWriter, zipStream, "mapLayerDepth", (int) zLevels[layerIndex], false); bytesWritten+=4;
                //Int32 mapLayerWidthInTiles (number of columns of tiles in this map layer)
                EncodeOutput(false, streamWriter, zipStream, "mapLayerWidthInTiles", (int) mapBitmap.NumTilesX, false); bytesWritten+=4;
                //Int32 mapLayerHeightInTiles (number of rows of tiles in this map layer)
                EncodeOutput(false, streamWriter, zipStream, "mapLayerHeightInTiles", (int) mapBitmap.NumTilesY, false); bytesWritten+=4;
#if AutoClipping
                if (fontTileBitmaps!=null) {
                    //scan west columns
                    int westClipped;
                    tileNum = 0;
                    bool stillValid = true;
                    progressForm.SetStatus("Auto-Clipping west border");
                    progressForm.SetMaximum(mapBitmap.NumTilesX);
                    for (westClipped=0; westClipped<mapBitmap.NumTilesX && stillValid; westClipped++) {
                        for (int y=0; y<mapBitmap.NumTilesY && stillValid; y++, tileNum++) {
                            BitmapPartBytes tile = outReferences[tileNum];
                            if (tile.TileIndex!=0 || tile.BGColor!=0) {
                                stillValid = false;
                            }
                        }
                        progressForm.SetProgress(westClipped);
                        if (!stillValid) {
                            break;
                        }
                    }
                    progressForm.SetProgress(mapBitmap.NumTilesX);
                    westClippedBounds[layerIndex]=westClipped;
                    EncodeOutput(false, streamWriter, zipStream, "mapLayerWestClipped", (int) westClipped, false); bytesWritten+=4;
                    //scan east columns
                    int eastClipped;
                    int tileNumPerColumn = mapBitmap.NumTilesY;
                    int doubleTileNumPerColumn = mapBitmap.NumTilesY+mapBitmap.NumTilesY;
                    tileNum = tileNumPerColumn*mapBitmap.NumTilesX - mapBitmap.NumTilesY;
                    stillValid = true;
                    progressForm.SetStatus("Auto-Clipping east border");
                    progressForm.SetMaximum(mapBitmap.NumTilesX);
                    for (eastClipped=0; eastClipped<mapBitmap.NumTilesX && stillValid; eastClipped++, tileNum-=doubleTileNumPerColumn) {
                        for (int y=0; y<mapBitmap.NumTilesY && stillValid; y++, tileNum++) {
                            BitmapPartBytes tile = outReferences[tileNum];
                            if (tile.TileIndex!=0 || tile.BGColor!=0) {
                                stillValid = false;
                            }
                        }
                        progressForm.SetProgress(eastClipped);
                        if (!stillValid) {
                            break;
                        }
                    }
                    progressForm.SetProgress(mapBitmap.NumTilesX);
                    eastClippedBounds[layerIndex]=eastClipped;
                    EncodeOutput(false, streamWriter, zipStream, "mapLayerEastClipped", (int) eastClipped, false); bytesWritten+=4;

                    //scan north columns
                    int northClipped;
                    tileNum = 0;
                    int baseTileNum = 0;
                    stillValid = true;
                    progressForm.SetStatus("Auto-Clipping north border");
                    progressForm.SetMaximum(mapBitmap.NumTilesY);
                    for (northClipped=0; northClipped<mapBitmap.NumTilesY && stillValid; northClipped++, baseTileNum++) {
                        int x;
                        for (x=0, tileNum=baseTileNum; x<mapBitmap.NumTilesX && stillValid; x++, tileNum+=mapBitmap.NumTilesY) {
                            BitmapPartBytes tile = outReferences[tileNum];
                            if (tile.TileIndex!=0 || tile.BGColor!=0) {
                                stillValid = false;
                            }
                        }
                        progressForm.SetProgress(northClipped);
                        if (!stillValid) {
                            break;
                        }
                    }
                    progressForm.SetProgress(mapBitmap.NumTilesY);
                    northClippedBounds[layerIndex]=northClipped;
                    EncodeOutput(false, streamWriter, zipStream, "mapLayerNorthClipped", (int) northClipped, false); bytesWritten+=4;
                    
                    //scan south columns
                    int southClipped;
                    tileNum = 0;
                    baseTileNum = mapBitmap.NumTilesY - 1;
                    stillValid = true;
                    progressForm.SetStatus("Auto-Clipping south border");
                    progressForm.SetMaximum(mapBitmap.NumTilesY);
                    for (southClipped=0; southClipped<mapBitmap.NumTilesY && stillValid; southClipped++, baseTileNum--) {
                        int x;
                        for (x=0, tileNum=baseTileNum; x<mapBitmap.NumTilesX && stillValid; x++, tileNum+=mapBitmap.NumTilesY) {
                            BitmapPartBytes tile = outReferences[tileNum];
                            if (tile.TileIndex!=0 || tile.BGColor!=0) {
                                stillValid = false;
                            }
                        }
                        progressForm.SetProgress(southClipped);
                        if (!stillValid) {
                            break;
                        }
                    }
                    progressForm.SetProgress(southClipped);
                    southClippedBounds[layerIndex]=southClipped;
                    EncodeOutput(false, streamWriter, zipStream, "mapLayerSouthClipped", (int) southClipped, false); bytesWritten+=4;
                    Console.WriteLine("Layer "+layerIndex+": Clipped "+westClipped+" west columns, "+eastClipped+" east columns, "+northClipped+" north rows, "+southClipped+" south rows.");
                    progressForm.SetProgress(mapBitmap.NumTilesY);
                }
#endif
            }

            progressForm.SetStatus("Writing unique image tiles and assigning indices...");
            //Identified 283, failed to identify 81
            //Identified 21 damaged tiles.
            progressForm.SetMaximum(valueArray.Count);

            tileNum = 0;

            foreach (BitmapPartBytes value in valueArray) {
                if (fontTileBitmaps!=null) {
                    EncodeOutput(true, streamWriter, zipStream, "", value.TileIndex, false);
                    EncodeOutput(true, streamWriter, zipStream, "", value.BGColor, false);
                    EncodeOutput(true, streamWriter, zipStream, "", value.FGColor, false);
                    bytesWritten+=3;
                }
                bytesWritten += value.WriteToStream(zipStream);
                value.Index = tileNum;
                tileNum++;
                progressForm.SetProgress(tileNum);
            }
            //Console.WriteLine("Found "+tilesFound+" tiles, didn't find "+tilesNotFound+".");
            //TiffWriter.WriteTiff(mapBitmap, outReferences, valueCollection, encodeFilename+".tif");

            int bytesWrittenPreviously = bytesWritten;
            Console.WriteLine("This map is "+mapBitmapData[0].NumTilesX+" tiles wide by "+mapBitmapData[0].NumTilesY+" tiles high, if all map layers are the same width and height, and there are "+zLevels.Length+" map layers. Each tile is "+tileWidth+" pixels wide and "+tileHeight+" pixels high.");
            for (int layerIndex = 0; layerIndex<zLevels.Length; layerIndex++) {
                if (zLevels.Length>1) {
                    progressForm.SetPrefix("(Layer "+(layerIndex+1)+" of "+zLevels.Length+") ");
                } else {
                    progressForm.SetPrefix("");
                }
                progressForm.SetStatus("Writing map data...");
                MapBitmapData mapBitmap = mapBitmapData[layerIndex];
                BitmapPartBytes[] outReferences = outReferencesArray[layerIndex];
                progressForm.SetMaximum(mapBitmap.NumTiles);
                tileNum=0;
                int lastX = mapBitmap.NumTilesX-eastClippedBounds[layerIndex]-1;
                int lastY = mapBitmap.NumTilesY-southClippedBounds[layerIndex]-1;
                
                //if (splitTiles.Count<=0xff) {
#if TileIndexRLE
                int lastTileIndex = -1;
                int numLastTiles = 0;
#endif
                int tileIndex = -1;
#if TileIndexRLE
                if (valueArray.Count<=0x7f) {
#else
                if (valueArray.Count<=0xff) {
#endif
                    for (int x=0; x<mapBitmap.NumTilesX; x++) {
                        for (int y=0; y<mapBitmap.NumTilesY; y++, tileNum++) {
                            if (x>=westClippedBounds[layerIndex] && x<mapBitmap.NumTilesX-eastClippedBounds[layerIndex] && y>=northClippedBounds[layerIndex] && y<mapBitmap.NumTilesY-southClippedBounds[layerIndex]) {
                                tileIndex = outReferences[tileNum].Index;
                                Sanity.IfTrueThrow(tileIndex>valueArray.Count || tileIndex<0, "Error in DF Map Compressor: Tile at <"+x+","+y+","+layerIndex+"> had a tile index ("+tileIndex+") which was outside the bounds of the tiles list!");
#if TileIndexRLE
                                TileIndexRLE<sbyte>(ref tileIndex, ref lastTileIndex, ref numLastTiles, ref streamWriter, ref zipStream, ref bytesWritten, (x==lastX && y==lastY));
#else
                                EncodeOutput(true, streamWriter, zipStream, "", (byte) tileIndex, (x==mapBitmap.NumTilesX-1));
                                bytesWritten+=1;
#endif
                            }
                        }
                        progressForm.SetProgress(tileNum);
                    }
#if TileIndexRLE
                    //TileIndexRLE<sbyte>(ref tileIndex, ref lastTileIndex, ref numLastTiles, ref streamWriter, ref zipStream, ref bytesWritten, true);
#endif
#if TileIndexRLE
                } else if (valueArray.Count<=0x7fff) {
#else
                } else if (valueArray.Count<=0xffff) {
#endif
                    for (int x=0; x<mapBitmap.NumTilesX; x++) {
                        for (int y=0; y<mapBitmap.NumTilesY; y++, tileNum++) {
                            if (x>=westClippedBounds[layerIndex] && x<mapBitmap.NumTilesX-eastClippedBounds[layerIndex] && y>=northClippedBounds[layerIndex] && y<mapBitmap.NumTilesY-southClippedBounds[layerIndex]) {
                                tileIndex = outReferences[tileNum].Index;
                                Sanity.IfTrueThrow(tileIndex>valueArray.Count || tileIndex<0, "Error in DF Map Compressor: Tile at <"+x+","+y+","+layerIndex+"> had a tile index ("+tileIndex+") which was outside the bounds of the tiles list!");
#if TileIndexRLE
                                TileIndexRLE<Int16>(ref tileIndex, ref lastTileIndex, ref numLastTiles, ref streamWriter, ref zipStream, ref bytesWritten, (x==lastX && y==lastY));
#else
                                EncodeOutput(true, streamWriter, zipStream, "", (ushort) tileIndex, (x==mapBitmap.NumTilesX-1));
                                bytesWritten+=2;
#endif
                            }
                        }
                        progressForm.SetProgress(tileNum);
                    }
#if TileIndexRLE
                    //TileIndexRLE<Int16>(ref tileIndex, ref lastTileIndex, ref numLastTiles, ref streamWriter, ref zipStream, ref bytesWritten, true);
#endif
                } else {
                    for (int x=0; x<mapBitmap.NumTilesX; x++) {
                        for (int y=0; y<mapBitmap.NumTilesY; y++, tileNum++) {
                            if (x>=westClippedBounds[layerIndex] && x<mapBitmap.NumTilesX-eastClippedBounds[layerIndex] && y>=northClippedBounds[layerIndex] && y<mapBitmap.NumTilesY-southClippedBounds[layerIndex]) {
                                tileIndex = outReferences[tileNum].Index;
                                Sanity.IfTrueThrow(tileIndex>valueArray.Count || tileIndex<0, "Error in DF Map Compressor: Tile at <"+x+","+y+","+layerIndex+"> had a tile index ("+tileIndex+") which was outside the bounds of the tiles list!");

#if TileIndexRLE
                                TileIndexRLE<Int32>(ref tileIndex, ref lastTileIndex, ref numLastTiles, ref streamWriter, ref zipStream, ref bytesWritten, (x==lastX && y==lastY));
#else
                                EncodeOutput(true, streamWriter, zipStream, "", tileIndex, (x==mapBitmap.NumTilesX-1));
                                bytesWritten+=4;
#endif
                            }

                        }
                        progressForm.SetProgress(tileNum);
                    }
#if TileIndexRLE
                    //TileIndexRLE<Int32>(ref tileIndex, ref lastTileIndex, ref numLastTiles, ref streamWriter, ref zipStream, ref bytesWritten, true);
#endif
                }

                progressForm.SetProgress(mapBitmap.NumTiles);
            }
            Console.WriteLine("Bytes written in map data (prior to compression): "+(bytesWritten-bytesWrittenPreviously)+", previously "+bytesWrittenPreviously);
            if (zipStream!=null) {
                zipStream.Close();
                outputStream.Close();
                //zipStream.Dispose();
                outputStream.Dispose();
            } else {
                streamWriter.Close();
                streamWriter.Dispose();

                //LZW.Program.Compress(shortEncodeFilename+".txt", shortEncodeFilename+".lzw");
                
            }
        }

        private void DecodeFortressMapButton_Click(object sender, EventArgs e) {
            chosenFolder = FindDFFolder();
            if (chosenFolder!=null) {
                selectEncodedFortressMapToViewOFD.InitialDirectory=chosenFolder;
            }
            DialogResult result = selectEncodedFortressMapToViewOFD.ShowDialog();
            if (result == DialogResult.OK) {
                string originalFilename = selectEncodedFortressMapToViewOFD.FileName;
                string filename = originalFilename;
                int indexOfDot = filename.LastIndexOf('.');
                string extension = "";
                if (indexOfDot>-1) {
                    filename = originalFilename.Substring(0, indexOfDot);
                    if (indexOfDot < originalFilename.Length-1) {
                        extension = originalFilename.Substring(indexOfDot+1);
                    }
                }
                if (String.Compare(extension, "df-map", true)==0 || (String.Compare(extension,"fdf-map", true)==0)) {
                    //valid
                } else {
                    MessageBox.Show(LinuxInsertLineBreaks("This is not a supported file type (\"."+extension+"\") for decoding. Supported filetypes are .fdf-map and .df-map."));
                }
                chosenFolder = StripFilenameFrom(selectEncodedFortressMapToViewOFD.FileName);
                ZLayerList zLayerList = GetZLayerList(selectEncodedFortressMapToViewOFD.FileName, extension);
                if (zLayerList.Count==1) {
                    decodeFortressMapSFD.FileName=filename;
                    result = decodeFortressMapSFD.ShowDialog();
                } else {
                    decodeFortressMapToZipSFD.FileName=filename;
                    result = decodeFortressMapToZipSFD.ShowDialog();
                }
                if (result == DialogResult.OK) {
                    //Stream inputStream = selectMapFileToEncodeOFD.OpenFile();
                    this.Hide();
                    progressForm = new ProgressForm();
                    progressForm.Show();
                    if (zLayerList.Count!=1) {
                        Run_DecodeFortressMap(selectEncodedFortressMapToViewOFD.FileName, extension, 0, true);
                    } else {
                        Bitmap realBitmap = Run_DecodeFortressMap(selectEncodedFortressMapToViewOFD.FileName, extension, 0, false);
                        if (realBitmap!=null) {
                            progressForm.SetStatus("Saving PNG...");
                            realBitmap.Save(decodeFortressMapSFD.FileName, ImageFormat.Png);
                        }
                    }
                    progressForm.SetProgress(100);
                    progressForm.SetStatus("Cleaning up...");
                    GC.Collect();
                    progressForm.Hide();
                    progressForm.Dispose();
                    progressForm = null;
                    this.Show();
                }
            } 
        }

        internal bool IsBlackSpace(BitmapPartBytes tile) {
            byte tileId = tile.TileIndex;
            byte bgColor = tile.BGColor;
            return tileId==0 && bgColor==0;
        }

        internal bool IsPossibleRandomTileOnBlackSpace(BitmapPartBytes tile) {
#if BlackSpaceConversion
            byte tileId = tile.TileIndex;
            byte bgColor = tile.BGColor;
            byte fgColor = tile.FGColor;
            int tilex = tileId&0xf;
            int tiley = tileId>>4;
            bool retval = false;
			if (bgColor==0) {
				//if (tilex==2 && tiley==2) {             //"
				//    tilex = 0; tiley = 3;
				if (fgColor==8) {   //dark grey
					if (tilex==5 && tiley==2) {      //%, 5x2
						retval = true;
					} else if (tilex==7 && tiley==2) {      //'
						retval = true;
					} else if (tilex==12 && tiley==2) {     //,
						retval = true;
					} else if (tilex==14 && tiley==2) {     //.
						retval = true;
					} else if (tilex==0 && tiley==6) {      //`
						retval = true;
						/*} else if (tilex==10 && tiley==15) {    //dot in center of tile
						} else if (tilex==9 && tiley==15) {     //larger dot in center of tile
						} else if (tilex==0 && tiley==0) {      //empty space
						} else if (tilex==0 && tiley==2) {      //also empty space
						} else if (tilex==15 && tiley==15) {    //another empty space
						*/
					}
				} else if (fgColor==4) {    //red
					if (tilex==12 && tiley==2) {     //,
						retval = true;
					}
				}
			}
            return retval;
#else
			return false;
#endif
			
		}

        public int GetFileVersion(string selectedFilename, string extension) {
            FileStream inputStream = File.OpenRead(selectedFilename);
            Stream zipStream = null;
            if (String.Compare(extension, "fdf-map", true)==0) {
                zipStream = new InflaterInputStream(inputStream);
            } else if (String.Compare(extension, "df-map", true)==0) {
                zipStream = new LZMAStream(inputStream, CompressionMode.Decompress);
            } else {
                //MessageBox.Show("This is not a supported file type (\"."+extension+"\") for decoding. Supported filetypes are .fdf-map and .df-map.");
                return -1;
            }
            
            //Int32 negativeVersion (this would be -1 for the new multi-layer-supporting format, or >=0 for the previous format, which doesn't support multi-layer images. For the doc on the previous format, see DFMapViewer_FileFormat.txt (this is DFMapViewer_FileFormat_2.txt. Note that this variable is numberOfTiles in the old format, and is guaranteed to be >=0 in it.))
            byte[] intBytes = new byte[4];
            zipStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			int negativeVersion = BitConverter.ToInt32(intBytes, 0);
            if (negativeVersion>0) {
                negativeVersion=0;
            }
            return -negativeVersion;
        }

        public ZLayerList GetZLayerList(string selectedFilename, string extension) {
            FileStream inputStream = File.OpenRead(selectedFilename);
            Stream zipStream = null;
            if (String.Compare(extension, "fdf-map", true)==0) {
                zipStream = new InflaterInputStream(inputStream);
            } else if (String.Compare(extension, "df-map", true)==0) {
                zipStream = new LZMAStream(inputStream, CompressionMode.Decompress);
            } else {
                MessageBox.Show(LinuxInsertLineBreaks("This is not a supported file type (\"."+extension+"\") for decoding. Supported filetypes are .fdf-map and .df-map."));
                return null;
            }
            
            //Int32 negativeVersion (this would be -1 for the new multi-layer-supporting format, or >=0 for the previous format, which doesn't support multi-layer images. For the doc on the previous format, see DFMapViewer_FileFormat.txt (this is DFMapViewer_FileFormat_2.txt. Note that this variable is numberOfTiles in the old format, and is guaranteed to be >=0 in it.))
            byte[] intBytes = new byte[4];
            zipStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			int negativeVersion = BitConverter.ToInt32(intBytes, 0);
            ZLayerList zLayerList = new ZLayerList();
            if (negativeVersion>=0) {
                zLayerList.Add(0, 0);
            } else {
                //Int32 numberOfTiles (Number of unique tile images - not the total number of tiles in the map)
                zipStream.Read(intBytes, 0, 4);
                //Int32 tileWidth (width of each tile in pixels)
                zipStream.Read(intBytes, 0, 4);
                //Int32 tileHeight (height of each tile in pixels)
                zipStream.Read(intBytes, 0, 4);
                //Int32 numMapLayers (how many map layers are in the image - for an old DF image, this would be one)
                zipStream.Read(intBytes, 0, 4);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
				int numMapLayers = BitConverter.ToInt32(intBytes, 0);
                //For each map layer:
                for (int index=0; index<numMapLayers; index++) {
                    //Int32 mapLayerDepth (This may change, but I'm thinking 0 would be the ground layer, and higher (towards the sky) layers (if any) would have positive numbers, and lower (deeper) layers would have negative numbers - higher numbers are higher altitude, lower numbers are lower altitude.)
	                zipStream.Read(intBytes, 0, 4);
					if (!BitConverter.IsLittleEndian) {
						intBytes = ReverseBytes(intBytes);
					}
					int mapLayerDepth = BitConverter.ToInt32(intBytes, 0);
                    //Int32 mapLayerWidthInTiles (number of columns of tiles in this map layer)
	                zipStream.Read(intBytes, 0, 4);
                    //int mapLayerWidthInTiles = BitConverter.ToInt32(intBytes, 0); //not used
                    //Int32 mapLayerHeightInTiles (number of rows of tiles in this map layer)
                    zipStream.Read(intBytes, 0, 4);
                    //int mapLayerHeightInTiles = BitConverter.ToInt32(intBytes, 0);    //not used
                    zLayerList.Add(index, mapLayerDepth);
                }
	            
            }
            zipStream.Close();
            inputStream.Close();
            inputStream.Dispose();
            return zLayerList;
            
        }

        public Bitmap Run_DecodeFortressMap(string selectedFilename, string extension, int zLayer, bool toZip) {
            bool tempProgressForm = false;
            if (progressForm==null) {
                tempProgressForm = true;
                progressForm = new ProgressForm();
                progressForm.Show();
            }
            progressForm.SetStatus("Reading map...");
            FileStream inputStream = File.OpenRead(selectedFilename);
            Stream zipStream = null;
            if (String.Compare(extension, "fdf-map", true)==0) {
                zipStream = new InflaterInputStream(inputStream);
            } else if (String.Compare(extension, "df-map", true)==0) {
                zipStream = new LZMAStream(inputStream, CompressionMode.Decompress);
            } else {
                MessageBox.Show(LinuxInsertLineBreaks("This is not a supported file type (\"."+extension+"\") for decoding. Supported filetypes are .fdf-map and .df-map."));
                return null;
            }
            ZipOutputStream zipOutFile = null;
            if (toZip) {
                zipOutFile = new ZipOutputStream(File.Create(decodeFortressMapToZipSFD.FileName));
                /*while (zipOutFile.Count>0) {
                    zipOutFile.Delete(zipOutFile[0]);
                }*/
                zipOutFile.SetLevel(1);
            }
            
            byte[] intBytes = new byte[4];
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			
            //File format:
            //Int32 numberOfTiles
            zipStream.Read(intBytes, 0, 4);
            int negativeVersion = BitConverter.ToInt32(intBytes, 0);
            int numberOfTiles = 0;
            
            bool multiLayer = false;
            bool tileID = false;
            bool rle = false;
            bool clipping = false;

            if (negativeVersion>=0) {
                numberOfTiles = negativeVersion;
            } else {
                multiLayer = true;
                int features = -1 - negativeVersion;
                if (features>3) {
                    MessageBox.Show(LinuxInsertLineBreaks("Unknown map format version - If this is really a df-map or fdf-map file, you may need a newer version of the compressor to read this file."));
                }
                tileID = (features&0x1)>0;
                rle = (features&0x2)>0;
                clipping = (features&0x4)>0;

                zipStream.Read(intBytes, 0, 4);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
				numberOfTiles = BitConverter.ToInt32(intBytes, 0);
            }
            string featuresString = "File features:";
            if (multiLayer) {
                featuresString += " Multi-Layer";
            }
            if (tileID) {
                featuresString += " TileID";
            }
            if (rle) {
                featuresString += " RLE";
            }
            if (clipping) {
                featuresString += " Clipping";
            }
            Console.WriteLine(featuresString);
            zipStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			int tileWidth = BitConverter.ToInt32(intBytes, 0);
            zipStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			int tileHeight = BitConverter.ToInt32(intBytes, 0);
            int tileNum;
            
            //Format v-1 only: Int32 numMapLayers
            int numMapLayers = 1;
            if (multiLayer) {
                zipStream.Read(intBytes, 0, 4);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
				numMapLayers = BitConverter.ToInt32(intBytes, 0);
            }

            int chosenLayerIndex = 0;

            int[] zLevels = new int[numMapLayers];
            int[] mapWidthsInTiles = new int[numMapLayers];
            int[] mapHeightsInTiles = new int[numMapLayers];
            int[] westClippedBounds = new int[zLevels.Length];
            int[] eastClippedBounds = new int[zLevels.Length];
            int[] northClippedBounds = new int[zLevels.Length];
            int[] southClippedBounds = new int[zLevels.Length];
            for (int layerIndex=0; layerIndex<numMapLayers; layerIndex++) {
                //Format v-1 only: Int32 mapLayerDepth (This may change, but I'm thinking 0 would be the ground layer, and higher (towards the sky) layers (if any) would have positive numbers, and lower (deeper) layers would have negative numbers - higher numbers are higher altitude, lower numbers are lower altitude.)
                if (multiLayer) {
                    zipStream.Read(intBytes, 0, 4);
					if (!BitConverter.IsLittleEndian) {
						intBytes = ReverseBytes(intBytes);
					}
					int mapLayerDepth = BitConverter.ToInt32(intBytes, 0);
                    zLevels[layerIndex] = mapLayerDepth;
                    if (mapLayerDepth==zLayer) {
                        chosenLayerIndex = layerIndex;
                    }
                }

                //Int32 mapWidthInTiles
                zipStream.Read(intBytes, 0, 4);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
				mapWidthsInTiles[layerIndex] = BitConverter.ToInt32(intBytes, 0);
                //Int32 mapHeightInTiles
                zipStream.Read(intBytes, 0, 4);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
				mapHeightsInTiles[layerIndex] = BitConverter.ToInt32(intBytes, 0);

            }
            progressForm.SetStatus("Reading unique tile images...");
            progressForm.SetMaximum(numberOfTiles);
            
            int tileNumBytes = tileWidth * tileHeight * 3;
            //int[,] tileIndices = new int[mapWidthInTiles, mapHeightInTiles];
            BitmapPartBytes[] splitTiles = new BitmapPartBytes[numberOfTiles];
            tileNum=0;
            for (tileNum=0; tileNum<numberOfTiles; tileNum++) {
                int tileIndex = 0;
                int fgColor = 0;
                int bgColor = 0;
                if (tileID) {
                    //font and color information, which we are discarding.
                    tileIndex = zipStream.ReadByte();
                    bgColor = zipStream.ReadByte();
                    fgColor = zipStream.ReadByte();
                    if (tileIndex<0 || bgColor<0 || fgColor<0) {
                        throw new ApplicationException("File incomplete or corrupt?");
                    }
                }
                progressForm.SetProgress(tileNum); 
                splitTiles[tileNum] = new BitmapPartBytes((byte) tileWidth, (byte) tileHeight, zipStream);
                splitTiles[tileNum].StoreIdentification((byte) tileIndex, (byte) bgColor, (byte) fgColor);
            }
            
            /*
            int bitsNeeded = 0;
            int tempSPTC = splitTiles.Length;
            while (tempSPTC > 0) {
                tempSPTC = tempSPTC >> 1;
                bitsNeeded +=1;
            }
            long bitsNeededMask = (1<<bitsNeeded)-1;
            long bitBuffer = 0;
            int bitBufferPos = 0;
            for (int x=0; x<mapBitmap.NumTilesX; x++) {
                for (int y=0; y<mapBitmap.NumTilesY; y++, tileNum++) {
                    while (bitBufferPos < bitsNeeded) {
                        int abyte = zipStream.ReadByte();
                        if (abyte>-1) {
                            bitBuffer = bitBuffer + (abyte<<bitBufferPos);
                            bitBufferPos += 8;
                        }
                    }
                    int tileIndex = (int) (bitBuffer&bitsNeededMask);
                    mapBitmap.SetTile(x, y, splitTiles[tileIndex]);
                    bitBuffer = bitBuffer >> bitsNeeded;
                    bitBufferPos -= bitsNeeded;
                }
                progressForm.SetProgress(tileNum);
            }
            */
            //For each map layer (again):
            
            byte[] mapDataBytes = null;
            TiledBitmapWrapper mapBitmap = null;
            for (int layerIndex=0; layerIndex<numMapLayers; layerIndex++) {
                GC.Collect();
                if (zLevels.Length>1) {
                    progressForm.SetPrefix("(Layer "+(layerIndex+1)+" of "+numMapLayers+") ");
                } else {
                    progressForm.SetPrefix("");
                }
                tileNum = 0;
                int mapWidthInTiles = mapWidthsInTiles[layerIndex];
                int mapHeightInTiles = mapHeightsInTiles[layerIndex];
                int totalNumBytes = tileNumBytes * mapWidthInTiles * mapHeightInTiles;
                int numTiles = mapWidthInTiles*mapHeightInTiles;
                if (chosenLayerIndex==layerIndex || toZip) {
                    mapDataBytes = new byte[totalNumBytes];
                    mapBitmap = new TiledBitmapWrapper(mapDataBytes, tileWidth * mapWidthInTiles, tileHeight * mapHeightInTiles, tileWidth, tileHeight);
                }
                if (chosenLayerIndex==layerIndex || toZip) {
                    progressForm.SetStatus("Reading tile IDs and converting them back to a bitmap...");
                } else {
                    progressForm.SetStatus("Skipping tile IDs since this isn't the layer we want...");
                }
                progressForm.SetMaximum(numTiles);
                if (rle) {
                    long flag;
                    long unflag;
                    if (numberOfTiles<0x7f) {
                        flag=0x80;
                        unflag=0x7f;
                    } else if (numberOfTiles<0x7fff) {
                        flag = 0x8000;
                        unflag = 0x7fff;
                    } else {
                        flag = 0x80000000;
                        unflag = 0x7fffffff;
                    }
                    int maxTiles = mapWidthInTiles*mapHeightInTiles;
                    int x=0;
                    int y=0;
                    tileNum = 0;
                    long data = 0;
                    byte[] shortBytes = new byte[2];
                    intBytes = new byte[4];
                    while (tileNum<maxTiles) {
                        if (numberOfTiles<0x7f) {
                            data = zipStream.ReadByte();
                            if (data<=-1) {
                                throw new ApplicationException("File incomplete or corrupt?");
                            }
                        } else if (numberOfTiles<0x7fff) {
                            if (zipStream.Read(shortBytes, 0, 2)==2) {
								if (!BitConverter.IsLittleEndian) {
									shortBytes = ReverseBytes(shortBytes);
								}
								data = (int) BitConverter.ToUInt16(shortBytes, 0);
                            } else {
                                throw new ApplicationException("File incomplete or corrupt?");
                            }
                        } else {
                            if (zipStream.Read(intBytes, 0, 4)==4) {
								if (!BitConverter.IsLittleEndian) {
									intBytes = ReverseBytes(intBytes);
								}
								data = (int) BitConverter.ToUInt32(intBytes, 0);
                            } else {
                                throw new ApplicationException("File incomplete or corrupt?");
                            }
                        }
                        int amount = 1;
                        long tileIndex = data&unflag;
                        if ((data&flag)>0) {
                            amount = zipStream.ReadByte();
                            Sanity.IfTrueSay(amount<=1, "Amount is "+amount+"!");
                        }
                        while (amount>0) {
                            Sanity.IfTrueThrow(tileIndex<0 || tileIndex>splitTiles.Length, "tileIndex ("+tileIndex+") out of bounds (0-"+splitTiles.Length+")");
                            if (chosenLayerIndex==layerIndex || toZip) {
                                mapBitmap.SetTile(x, y, splitTiles[(int) tileIndex]);
                            }
                            amount-=1;
                            tileNum+=1;
                            y+=1;
                            if (y>=mapHeightInTiles) {
                                y=0;
                                x+=1;
                                if (x>=mapWidthInTiles) {
                                    Sanity.IfTrueThrow(tileNum<maxTiles, "Hit end of layer but tileNum is not maxTiles!");
                                    x=0;
                                    Sanity.IfTrueThrow(amount>0, "Map file claims there are "+amount+" more tiles left to do in this particular RLE entry, but we have reached the end of the layer. The format specifies that RLE entries cannot span multiple layers.");
                                }
                            }
                        }
                    }
                } else {
                    if (numberOfTiles<=0xff) {
                        for (int x=0; x<mapWidthInTiles; x++) {
                            for (int y=0; y<mapHeightInTiles; y++, tileNum++) {
                                int abyte = zipStream.ReadByte();
                                if (abyte>-1) {
                                    if (chosenLayerIndex==layerIndex || toZip) {
                                        int tileIndex = abyte;
                                        mapBitmap.SetTile(x, y, splitTiles[tileIndex]);
                                        //tileIndices[x, y]=tileIndex;
                                    }
                                } else {
                                    throw new ApplicationException("File incomplete or corrupt?");
                                }
                            }
                            progressForm.SetProgress(tileNum);
                        }
                    } else if (numberOfTiles<=0xffff) {
                        byte[] shortBytes = new byte[2];
                        for (int x=0; x<mapWidthInTiles; x++) {
                            for (int y=0; y<mapHeightInTiles; y++, tileNum++) {
                                zipStream.Read(shortBytes, 0, 2);
								if (!BitConverter.IsLittleEndian) {
									shortBytes = ReverseBytes(shortBytes);
								}
								if (chosenLayerIndex==layerIndex || toZip) {
                                    int tileIndex = (int) BitConverter.ToUInt16(shortBytes, 0);
                                    mapBitmap.SetTile(x, y, splitTiles[tileIndex]);
                                    //tileIndices[x, y]=tileIndex;
                                }
                            }
                            progressForm.SetProgress(tileNum);
                        }
                    } else {
                        for (int x=0; x<mapWidthInTiles; x++) {
                            for (int y=0; y<mapHeightInTiles; y++, tileNum++) {
                                zipStream.Read(intBytes, 0, 4);
								if (!BitConverter.IsLittleEndian) {
									intBytes = ReverseBytes(intBytes);
								}
								if (chosenLayerIndex==layerIndex || toZip) {
                                    int tileIndex = BitConverter.ToInt32(intBytes, 0);
                                    mapBitmap.SetTile(x, y, splitTiles[tileIndex]);
                                    //tileIndices[x, y]=tileIndex;
                                }
                            }
                            progressForm.SetProgress(tileNum);
                        }
                    }
                }
                if (toZip) {
                    //Add to zip file
                    string filename = ""+zLevels[layerIndex]+".png";
                    int slash = selectedFilename.LastIndexOf('/');
                    int backslash = selectedFilename.LastIndexOf('\\');
                    if (backslash>slash) {
                        slash = backslash;
                    }
                    int dot = selectedFilename.LastIndexOf('.');
                    if (dot>-1) {
                        int thirdDash = selectedFilename.LastIndexOf('-', dot-1);
                        if (thirdDash>-1) {
                            int secondDash = selectedFilename.LastIndexOf('-', thirdDash-1);
                            if (secondDash>-1) {
                                int firstDash = selectedFilename.LastIndexOf('-', secondDash-1);
                                if (firstDash>-1) {
                                    filename = selectedFilename.Substring(slash+1, firstDash-slash-1)+"-"+zLevels[layerIndex]+selectedFilename.Substring(firstDash, dot-firstDash)+".png";
                                }
                            }
                        }
                    }
                    ZipEntry entry = new ZipEntry(filename);
                    zipOutFile.PutNextEntry(entry);
                    Bitmap bitmap = mapBitmap.ToBitmap(progressForm);
                    progressForm.SetStatus("Compressing PNG into zip file");
                    progressForm.SetMaximum(8);
                    progressForm.SetProgress(0);
                    mapBitmap.Dispose();
                    progressForm.SetProgress(1);
                    mapDataBytes = null;
                    mapBitmap = null;
                    GC.Collect();
                    progressForm.SetProgress(2);
                    MemoryStream memStream = new MemoryStream();
                    bitmap.Save(memStream, ImageFormat.Png);
                    progressForm.SetProgress(3);
                    bitmap.Dispose();
                    progressForm.SetProgress(4);
                    bitmap = null;
                    GC.Collect();
                    progressForm.SetProgress(5);
                    memStream.WriteTo(zipOutFile);
                    progressForm.SetProgress(6);
                    memStream.Dispose();
                    progressForm.SetProgress(7);
                    memStream = null;
                    GC.Collect();
                    progressForm.SetProgress(8);                    
                }
            }
            if (toZip) {
                zipOutFile.Close();
                zipOutFile.Dispose();
                zipOutFile = null;
            }
            zipStream.Close();
            inputStream.Close();
            //zipStream.Dispose();
            inputStream.Dispose();

            /*progressForm.SetStatus("Recombining map.");
            progressForm.SetMaximum(mapWidthInTiles*mapHeightInTiles);
            tileNum=0;
            for (int x=0; x<mapWidthInTiles; x++) {
                for (int y=0; y<mapHeightInTiles; y++, tileNum++) {
                    progressForm.SetProgress(tileNum);
                    mapBitmap.SetTile(x, y, splitTiles[tileIndices[x,y]]);
                }
            }*/
            progressForm.SetPrefix("");
            if (!toZip) {
                progressForm.SetStatus("Exporting bitmap...");
            }
            Bitmap realBitmap = null;
            if (mapBitmap!=null) {
                realBitmap = mapBitmap.ToBitmap(progressForm);
                progressForm.SetStatus("Cleaning up...");
                mapBitmap.Dispose();
                mapBitmap = null;
                mapDataBytes = null;
            }
            progressForm.SetStatus("Cleaning up...");
            GC.Collect();
            progressForm.SetProgress(100);
            if (tempProgressForm) {
                progressForm.Hide();
                progressForm.Dispose();
                progressForm=null;
            }
            return realBitmap;
        }

        //http://archive.dwarffortresswiki.net/index.php/User:Jifodus/CMV_File_Format
        public void ReEncodeCMV(string inFilename, string inExtension, string outFilename, string outExtension) {
            bool tempProgressForm = false;
            if (progressForm==null) {
                tempProgressForm = true;
                progressForm = new ProgressForm();
                progressForm.Show();
            }
            progressForm.SetPrefix("");
            progressForm.SetStatus("Re-Encoding CMV...");

            FileStream inputStream = File.OpenRead(inFilename);
            Stream zipInStream = null;
            if (String.Compare(inExtension, "cmv", true)==0) {
                zipInStream = inputStream;
            } else {
                MessageBox.Show(LinuxInsertLineBreaks("This is not a supported file type (\"."+inExtension+"\") for converting. Supported filetypes are .cmv."));
                return;
            }
            FileStream outputStream = File.Create(outFilename);
            Stream zipOutStream = null;
            if (String.Compare(outExtension, "fcmv", true)==0) {
                zipOutStream = new DeflaterOutputStream(outputStream);
            } else {
                MessageBox.Show(LinuxInsertLineBreaks("This is not a supported file type (\"."+outExtension+"\") for converting. Supported filetypes are .cmv."));
                return;
            }
            byte[] intBytes = new byte[4];
            int bytesWritten=0;
            zipInStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			uint version = BitConverter.ToUInt32(intBytes, 0);
            EncodeOutput(false, null, zipOutStream, "version", (uint) 9000, false); bytesWritten+=4;
            zipInStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			uint cols = BitConverter.ToUInt32(intBytes, 0);
            EncodeOutput(false, null, zipOutStream, "cols", (uint) cols, false); bytesWritten+=4;
            zipInStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			uint rows = BitConverter.ToUInt32(intBytes, 0);
            EncodeOutput(false, null, zipOutStream, "rows", (uint) rows, false); bytesWritten+=4;
            zipInStream.Read(intBytes, 0, 4);
			if (!BitConverter.IsLittleEndian) {
				intBytes = ReverseBytes(intBytes);
			}
			uint unknown2 = BitConverter.ToUInt32(intBytes, 0);
            EncodeOutput(false, null, zipOutStream, "unknown2", (uint) unknown2, false); bytesWritten+=4;
            //Don't bother keeping the sounds
            if (version==10001) {
                zipInStream.Read(intBytes, 0, 4);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
				int numSounds = BitConverter.ToInt32(intBytes, 0);
                //EncodeOutput(false, null, zipOutStream, "numSounds", (uint) numSounds, false); bytesWritten+=4;
                if (numSounds!=0) {
                    Console.WriteLine("This movie has sounds!");
                }
                byte[] soundNames = new byte[50*numSounds];
                zipInStream.Read(soundNames, 0, 50*numSounds);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
				byte[] unknownSoundData = new byte[0x3200];
                zipInStream.Read(unknownSoundData, 0, 0x3200);
				if (!BitConverter.IsLittleEndian) {
					intBytes = ReverseBytes(intBytes);
				}
            }
            int frameSize = (int)(cols*rows);
            frameSize += frameSize;
            byte[] previousFrame = new byte[frameSize];
            for (int frameByte=0; frameByte<frameSize; frameByte++) {
                previousFrame[frameByte] = 0;
            }
            byte[] curFrame = new byte[frameSize];
            int framesDone = 0;
            int frameSizeLeft = frameSize;
            int frameSizeDone = 0;
            while (zipInStream.Position<zipInStream.Length) {
                if (zipInStream.Read(intBytes, 0, 4)==4) {
					if (!BitConverter.IsLittleEndian) {
						intBytes = ReverseBytes(intBytes);
					}
					int compressedChunkSize = BitConverter.ToInt32(intBytes, 0);
                    byte[] buffer = new byte[compressedChunkSize];
                    zipInStream.Read(buffer, 0, compressedChunkSize);
                    MemoryStream memoryStream = new MemoryStream(buffer);
                    InflaterInputStream zipZipInStream = new InflaterInputStream(memoryStream);
                    int bytesRead = 0;
                    do {
                        bytesRead = zipZipInStream.Read(curFrame, frameSizeDone, frameSizeLeft);
                        if (bytesRead==frameSizeLeft) {
                            int differences = FrameDifferences(previousFrame, curFrame, frameSize, null);
                            if ((framesDone==0) || (differences>=frameSize/3)) {
                                EncodeOutput(false, null, zipOutStream, "frameHeader", (byte) 0, false);
                                zipOutStream.Write(curFrame, 0, frameSize);
                            } else {
                                EncodeOutput(false, null, zipOutStream, "frameHeader", (byte) 1, false);
                                EncodeOutput(false, null, zipOutStream, "frameNumDifferences", (int) differences, false);
                                if (differences>0) {
                                    FrameDifferences(previousFrame, curFrame, frameSize, zipOutStream);
                                }
                            }
                            framesDone++;
                            Array.Copy(curFrame, previousFrame, frameSize);
                            frameSizeLeft = frameSize;
                            frameSizeDone = 0;
                        } else {
                            frameSizeDone += bytesRead;
                            frameSizeLeft -= bytesRead;
                        }
                    } while (bytesRead==frameSize);
                } else {
                    break;
                }
            }

            if (zipInStream!=inputStream) {
                zipInStream.Close();
            }
            if (zipOutStream!=outputStream) {
                zipOutStream.Close();
            }
            inputStream.Close();
            inputStream.Dispose();
            outputStream.Close();
            outputStream.Dispose();
            if (tempProgressForm) {
                progressForm.Hide();
                progressForm.Dispose();
                progressForm=null;
            }
        }

        public int FrameDifferences(byte[] previousFrame, byte[] curFrame, int frameSize, Stream zipOutStream) {
            int differences=0;
            for (int p=0; p<frameSize; p++) {
                if (previousFrame[p]!=curFrame[p]) {
                    differences+=1;
                    if (zipOutStream!=null) {
                        EncodeOutput(false, null, zipOutStream, "p", (ushort) p, false);
                        EncodeOutput(false, null, zipOutStream, "data", (byte) curFrame[p], false);
                    }
                }
            }
            return (differences);
        }

        public void ConvertCompression(string inFilename, string inExtension, string outFilename, string outExtension) {
            bool tempProgressForm = false;
            if (progressForm==null) {
                tempProgressForm = true;
                progressForm = new ProgressForm();
                progressForm.Show();
            }
            progressForm.SetPrefix("");
            progressForm.SetStatus("Converting from "+inExtension+" to "+outExtension+"...");
            
            FileStream inputStream = File.OpenRead(inFilename);
            Stream zipInStream = null;
            if (String.Compare(inExtension, "fdf-map", true)==0) {
                zipInStream = new InflaterInputStream(inputStream);
            } else if (String.Compare(inExtension, "df-map", true)==0) {
                zipInStream = new LZMAStream(inputStream, CompressionMode.Decompress);
            } else if (String.Compare(inExtension, "cmv", true)==0) {
                zipInStream = inputStream;
            } else {
                MessageBox.Show(LinuxInsertLineBreaks("This is not a supported file type (\"."+inExtension+"\") for converting. Supported filetypes are .fdf-map and .df-map."));
                return;
            }
            FileStream outputStream = File.Create(outFilename);
            Stream zipOutStream = null;
            if (String.Compare(outExtension, "fdf-map", true)==0) {
                zipOutStream = new DeflaterOutputStream(outputStream);
            } else if (String.Compare(outExtension, "df-map", true)==0) {
                zipOutStream = new LZMAStream(outputStream, CompressionMode.Compress);
            } else if (String.Compare(outExtension, "ccmv", true)==0) {
                zipOutStream = new DeflaterOutputStream(outputStream);
            } else {
                MessageBox.Show(LinuxInsertLineBreaks("This is not a supported file type (\"."+outExtension+"\") for converting. Supported filetypes are .fdf-map and .df-map."));
                return;
            }
            //int abyte = 0;
            long bufferLen = 0;
            //try {
                //bufferLen = zipInStream.Length;
                //fileLen = bufferLen;
                //progressForm.SetMaximum((int)bufferLen);

            //} catch (NotSupportedException) {
                bufferLen = 32768;
                progressForm.SetMaximum(65536);
                byte[] buffer = new byte[bufferLen];
                while (true) {
                    int readLen = (int) (bufferLen);
                    int bytesRead = zipInStream.Read(buffer, 0, readLen);
                    if (bytesRead>0) {
                        zipOutStream.Write(buffer, 0, bytesRead);
                    } else {
                        break;
                    }
                }
            //}
            /*if (fileLen!=-1) {
                //int pos = 0;
                if (bufferLen>32768) {
                    bufferLen = 32768;
                }
                buffer = new byte[bufferLen];
                long remainingLen = fileLen;
                do {
                    int readLen = (int) (bufferLen>remainingLen?remainingLen:bufferLen);
                    int bytesRead = zipInStream.Read(buffer, 0, readLen);
                    remainingLen -= bytesRead;
                    if (remainingLen < 0) {
                        remainingLen += fileLen;
                    }
                    zipOutStream.Write(buffer, 0, bytesRead);
                    progressForm.SetProgress((int) (fileLen - remainingLen));
                } while (remainingLen>0);
            }*/
            if (zipInStream!=inputStream) {
                zipInStream.Close();
            }
            if (zipOutStream!=outputStream) {
                zipOutStream.Close();
            }
            inputStream.Close();
            inputStream.Dispose();
            outputStream.Close();
            outputStream.Dispose();
            if (tempProgressForm) {
                progressForm.Hide();
                progressForm.Dispose();
                progressForm=null;
            }
        }

        public void ViewFortressMapButton_Click(object sender, EventArgs e) {
            chosenFolder = FindDFFolder();
            if (chosenFolder!=null) {
                viewFortressMapOFD.InitialDirectory=chosenFolder;
            }
            DialogResult result = viewFortressMapOFD.ShowDialog();
            if (result == DialogResult.OK) {
                string filename = viewFortressMapOFD.FileName;
                int dotPos = filename.LastIndexOf('.');
                if (dotPos>-1) {
                    Bitmap bitmap;
                    string extension = "";
                    if (dotPos < filename.Length-1) {
                        extension = filename.Substring(dotPos+1);
                    }
                    if ((String.Compare(extension, "fdf-map", true)==0)) {
                        FastMapViewer mapViewer = new FastMapViewer();
                        if (!mapViewer.IsInvalid) {
                            mapViewer.Show();
                            if (GetFileVersion(filename, "fdf-map")==0) {
                                mapViewer.ShowMap(filename);
                            } else {
                                mapViewer.ShowMapNew(filename);
                            }
                        } else {
                            mapViewer.Dispose();
                        }
                        bitmap=null;
                    } else if ((String.Compare(extension, "df-map", true)==0) || (String.Compare(extension, "fdf-map", true)==0)) {
                        this.Hide();
                        progressForm = new ProgressForm();
                        progressForm.Show();
                        //bitmap = Run_DecodeFortressMap(filename, extension);
                        MapViewer mapViewer = new MapViewer();
                        mapViewer.ShowMap(this, filename, extension);
                        mapViewer.Show();
                        progressForm.SetProgress(100);
                        progressForm.Hide();
                        progressForm.Dispose();
                        progressForm = null;
                        this.Show();
                    } else {
                        bitmap = (Bitmap) Bitmap.FromFile(filename);
                        if (bitmap!=null) {
                            MapViewer mapViewer = new MapViewer();
                            mapViewer.Show();
                            mapViewer.ShowMap(bitmap, true);
                        }
                    }
                    
                }
            }            
        }

        internal void DoCompressCMVButton(Form boss) {
            chosenFolder = FindDFFolder();
            if (chosenFolder!=null) {
                chosenFolder+="/data/movies";
                selectCMVFileToEncodeOFD.InitialDirectory=chosenFolder;
            }
            DialogResult result = selectCMVFileToEncodeOFD.ShowDialog();
            if (result == DialogResult.OK) {
                //Stream inputStream = selectMapFileToEncodeOFD.OpenFile();
                chosenFolder = StripFilenameFrom(selectCMVFileToEncodeOFD.FileName);
                string originalFilename = selectCMVFileToEncodeOFD.FileName;
                string filename = originalFilename;
                int indexOfDot = filename.LastIndexOf('.');
                string extension = "";
                if (indexOfDot>-1) {
                    filename = originalFilename.Substring(0, indexOfDot);
                    if (indexOfDot < originalFilename.Length-1) {
                        extension = originalFilename.Substring(indexOfDot+1);
                    }
                }
                decodeFortressMapSFD.FileName=filename;
                encodeCMVSFD.FileName=filename;
                result = encodeCMVSFD.ShowDialog();
                if (result == DialogResult.OK) {
                    if (String.Compare(extension, "cmv", true)==0) {
                        boss.Hide();
                        ConvertCompression(filename+".cmv", extension, encodeCMVSFD.FileName, "ccmv");
                        //ReEncodeCMV(filename+".cmv", extension, encodeCMVSFD.FileName, "ccmv");
                        boss.Show();
                    } else {
                        MessageBox.Show("That is not a .cmv file.");
                        return;
                    }
                }
            }
        }

        internal void DoCompressCMV2Button(Form boss) {
            chosenFolder = FindDFFolder();
            if (chosenFolder!=null) {
                chosenFolder+="/data/movies";
                selectCMVFileToEncodeOFD.InitialDirectory=chosenFolder;
            }
            DialogResult result = selectCMVFileToEncodeOFD.ShowDialog();
            if (result == DialogResult.OK) {
                //Stream inputStream = selectMapFileToEncodeOFD.OpenFile();
                chosenFolder = StripFilenameFrom(selectCMVFileToEncodeOFD.FileName);
                string originalFilename = selectCMVFileToEncodeOFD.FileName;
                string extension = "";
                string strippedFilename = StripNumberFromNewDFFilename(originalFilename);
                if (strippedFilename!=null) {
                    originalFilename = strippedFilename;
                }
                string filename = originalFilename;
                int indexOfDot = filename.LastIndexOf('.');
                if (indexOfDot>-1) {
                    filename = originalFilename.Substring(0, indexOfDot);
                    if (indexOfDot < originalFilename.Length-1) {
                        extension = originalFilename.Substring(indexOfDot+1);
                    }
                }
                decodeFortressMapSFD.FileName=filename;
                encodeFCMVSFD.FileName=filename;
                result = encodeFCMVSFD.ShowDialog();
                if (result == DialogResult.OK) {
                    if (String.Compare(extension, "cmv", true)==0) {
                        boss.Hide();
                        //ConvertCompression(filename+".cmv", extension, encodeCMVSFD.FileName, "ccmv");
                        ReEncodeCMV(filename+".cmv", extension, encodeFCMVSFD.FileName, "fcmv");
                        boss.Show();
                    } else {
                        MessageBox.Show("That is not a .cmv file.");
                        return;
                    }
                }
            }
        }
        internal void DoOutputFlashFilesButton(Form boss, bool deleteImagesAfterCompressing, bool doNotAnalyze, bool eraseDarkGreyGround, string colorMatchSensitivityString) {
            deleteImagesCheckbox.Checked = deleteImagesAfterCompressing;
            doNotAnalyzeCheckbox.Checked = doNotAnalyze;
            colorMatchSensitivityTextBox.Text = colorMatchSensitivityString;
            try {
                Properties.Settings.Default.colorMatchSensitivity=Int32.Parse(colorMatchSensitivityString);
                Properties.Settings.Default.Save();
            } catch (Exception) {
            }
            chosenFolder = FindDFFolder();
            if (chosenFolder!=null) {
                selectMapFileToEncodeOFD2.InitialDirectory=chosenFolder;
            }
            DialogResult result = selectMapFileToEncodeOFD2.ShowDialog();
            if (result == DialogResult.OK) {
                //Stream inputStream = selectMapFileToEncodeOFD.OpenFile();
                chosenFolder = StripFilenameFrom(selectMapFileToEncodeOFD2.FileName);
                string originalFilename = selectMapFileToEncodeOFD2.FileName;
                string extension = "";
                string strippedFilename = StripNumberFromNewDFFilename(originalFilename);
                if (strippedFilename!=null) {
                    originalFilename = strippedFilename;
                }
                string filename = originalFilename;
                int indexOfDot = filename.LastIndexOf('.');
                if (indexOfDot>-1) {
                    filename = originalFilename.Substring(0, indexOfDot);
                    if (indexOfDot < originalFilename.Length-1) {
                        extension = originalFilename.Substring(indexOfDot+1);
                    }
                }
                decodeFortressMapSFD.FileName=filename;
                if (String.Compare(extension, "cmv", true)==0) {
                    encodeCMVSFD.FileName=filename;
                    result = encodeCMVSFD.ShowDialog();
                } else {
                    encodeFortressMapSFD2.FileName=filename;
                    result = encodeFortressMapSFD2.ShowDialog();
                }
                if (result == DialogResult.OK) {
                    if (String.Compare(extension, "fdf-map", true)==0) {
                        MessageBox.Show(LinuxInsertLineBreaks("You asked to translate "+filename+" into a fdf-map file, but it already appears to be one (unless you renamed it yourself, which would be bad)..."));
                        return;
                    } else if (String.Compare(extension, "df-map", true)==0) {
                        boss.Hide();
                        ConvertCompression(filename+".df-map", extension, encodeFortressMapSFD2.FileName, "fdf-map");
                        boss.Show();
                    } else if (String.Compare(extension, "cmv", true)==0) {
                        boss.Hide();
                        ConvertCompression(filename+".cmv", extension, encodeCMVSFD.FileName, "ccmv");
                        boss.Show();
                    } else {
                        boss.Hide();
                        progressForm = new ProgressForm();
                        progressForm.Show();
                        Run_EncodeFortressMap(false, true, null);
                        progressForm.SetStatus("Cleaning up...");
                        GC.Collect();
                        progressForm.Hide();
                        progressForm.Dispose();
                        progressForm = null;
                        boss.Show();
                        
                    }
                }
            }
        }

        internal void DoVisitArchiveButton(Form boss) {
            System.Diagnostics.Process.Start("http://mkv25.net/dfma/");
        }        

        private void OutputFlashFilesButton_Click(object sender, EventArgs e) {
            DoOutputFlashFilesButton(this, deleteImagesCheckbox.Checked, doNotAnalyzeCheckbox.Checked, eraseDarkGreyGroundCheckbox.Checked, colorMatchSensitivityTextBox.Text);
        }

        private void VisitArchiveButton_Click(object sender, EventArgs e) {
            DoVisitArchiveButton(this);
        }

        public void CloneCheckboxes(bool delImgValue, bool doNotAnalyzeValue, bool eraseDarkGreyGround, string colorMatchSensitivityString) {
            deleteImagesCheckbox.Checked = delImgValue;
            doNotAnalyzeCheckbox.Checked = doNotAnalyzeValue;
            eraseDarkGreyGroundCheckbox.Checked = eraseDarkGreyGround;
            colorMatchSensitivityTextBox.Text = colorMatchSensitivityString;
        }

        private void SwitchToSimpleInterfaceButton_Click(object sender, EventArgs e) {
            SimpleMainMenuForm form = SimpleMainMenuForm.instance;
            if (form==null) {
                form = new SimpleMainMenuForm();
            }
            form.CloneCheckboxes(deleteImagesCheckbox.Checked, doNotAnalyzeCheckbox.Checked, eraseDarkGreyGroundCheckbox.Checked, colorMatchSensitivityTextBox.Text);
            form.Location=this.Location;
            form.Show();
            this.Hide();
        }

        private void MainMenuForm_FormClosing(object sender, FormClosedEventArgs e) {
            SimpleMainMenuForm form = SimpleMainMenuForm.instance;
            form.Close();
        }

        private void compressCmvButton_Click(object sender, EventArgs e) {
            DoCompressCMVButton(this);
        }

        private void compressCmvButton2_Click(object sender, EventArgs e) {
            DoCompressCMV2Button(this);
        }

        internal bool Analyze(DFColor[] colors, BitmapPartBytes[,] fontTileBitmaps, BitmapPartBytes value, out int tilex, out int tiley, out DFColor bgColor, out DFColor fgColor) {
            return Analyze(colors, fontTileBitmaps, value, out tilex, out tiley, out bgColor, out fgColor, false);
        }

        internal bool Analyze(DFColor[] colors, BitmapPartBytes[,] fontTileBitmaps, BitmapPartBytes value, out int tilex, out int tiley, out DFColor bgColor, out DFColor fgColor, bool bypassPixelLimit) {
            int bestTotalDifference;
            if (value.AttemptFind(colors, fontTileBitmaps, out tilex, out tiley, out bgColor, out fgColor, out bestTotalDifference, bypassPixelLimit)) {
                //Console.WriteLine("Phase: "+(bypassPixelLimit?2:1)+": Tile matched font tile "+tilex+","+tiley+" with bgColor "+bgColor.Name+" and fgColor "+fgColor.Name+" with best total difference "+bestTotalDifference+".");
                //if (bestTotalDifference<=100) {
                value.CopyFrom(colors, fontTileBitmaps, tilex, tiley, bgColor, fgColor);
                //}
                return true;
            //} else {
            //    value.CopyFrom(colors, fontTileBitmaps, 3, 1, new DFColor("temp", 255, 0, 0), new DFColor("temp", 255, 255, 0));
            }
            return false;
        }

        private void compressTilesetButton_Click(object sender, EventArgs e) {
            
        }

        public static int GetColorMatchSensitivity() {
            return colorMatchSensitivity;
        }

        public string LinuxInsertLineBreaks(string originalString) {
            string outputString = "";
            while (originalString.Length>80) {
                int foundAdjust = 1;
                int endPos = originalString.LastIndexOf(' ', 79, 80);
                if (endPos==-1) {
                    endPos = 80;
                    foundAdjust = 0;
                }
                outputString += originalString.Substring(0, endPos)+"\n";
                originalString = originalString.Substring(endPos+foundAdjust);
            }
            if (originalString.Length>0) {
                outputString += originalString;
            }
            return outputString;
        }
    }
}