using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using SL.Automation;

namespace DwarfFortressMapCompressor {
    public partial class FastMapViewer : Form {
        private bool isInvalid;
        public bool IsInvalid {
            get {
                return isInvalid;
            }
        }
        public FastMapViewer() {
            isInvalid = false;
            try {
                InitializeComponent();
            } catch (FileNotFoundException) {
                //flashControl = null;
                MessageBox.Show(MainMenuForm.instance.LinuxInsertLineBreaks("The integrated flash viewer failed to load because the Flash control didn't work - You seem to be missing, or we could not find, the flash9.ocx file. Try looking in 'c:\\windows\\system32\\Macromed\\Flash' for 'NPSWF32_FlashUtil.exe, and run it if it is there. (Substitute your actual windows path for c:\\windows if that isn't it)\n\nIf flash9.ocx is there and this still isn't working, try clicking [Start]->[Run] and typing in 'regsvr32 c:\\windows\\system32\\Macromed\\Flash\\flash9.ocx' (without the 's, and fix the path if c:\\windows isn't your windows folder) and hit enter."));
                isInvalid = true;
                /*int flash9ocx = fnfe.FileName.IndexOf("flash9.ocx");
                if (flash9ocx>-1) {
                    Process process = Process.Start(fnfe.FileName.Substring(0, flash9ocx)+"NPSWF32_FlashUtil.exe");
                    while (!process.HasExited) {
                        Thread.Sleep(5);
                    }
                    InitializeComponent();
                } else {
                    throw fnfe;
                }*/
            }            
        }

        public void ShowMap(string filename) {
            flashControl.StopPlay();
            filename = filename.Replace('\\','/');
            filename = filename.Substring(0, filename.ToLower().IndexOf(".fdf-map"));
            int lastSlashPos = filename.LastIndexOf('/');
            
            string filenamePath = filename.Substring(0, lastSlashPos+1);
            string filenameName = filename.Substring(lastSlashPos+1);
            string sendToFlash = "MAPNAME="+filenameName+"&DIRECTORY="+filenamePath;
            flashControl.FlashVars = sendToFlash;
            flashControl.LoadMovie(0, Application.StartupPath+"/dfmapviewer_h1a.swf");
            
        }

        public void ShowMapNew(string filename) {
            flashControl.StopPlay();
            filename = filename.Replace('\\', '/');
            filename = filename.Substring(0, filename.ToLower().IndexOf(".fdf-map"));
            int lastSlashPos = filename.LastIndexOf('/');

            string filenamePath = filename.Substring(0, lastSlashPos+1);
            string filenameName = filename.Substring(lastSlashPos+1);
            string sendToFlash = "MAPNAME="+filenameName+"&DIRECTORY="+filenamePath;
            flashControl.FlashVars = sendToFlash;
            flashControl.LoadMovie(0, Application.StartupPath+"/dfmapviewer_h1a.swf");
        }

        private void FastMapViewer_Resize(object sender, EventArgs e) {
            //resize components:
            label1.Top = this.Height - 85;
            flashControl.Height = this.Height - 99;
            flashControl.Width = this.Width - 31;
        }
    }
}