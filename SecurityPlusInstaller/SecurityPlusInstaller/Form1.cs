using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * SO! This is the program, its quite a mess, but hey, it works.
 */
namespace SecurityPlusInstaller
{
    public partial class Form1 : Form
    {
        private bool ultimateBackup, eupMode, wardrobeAccepted;
        private string gtavPath;
        
        public Form1()
        {
            // On initialisation, warn the user that if they mess up, it's not on me
            DialogResult result = MessageBox.Show("By continuing you are accepting liability for any damages sustained to your GTAV installation OR computer. This application is in BETA and may not always work as intended", "This is in Beta!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

            // If they click cancel, close the application
            if (result == DialogResult.Cancel)
            {
                Close();
                return;
            }
            
            // Create a log file for the installer program, and send console output to it
            FileStream filestream = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\security+.log", FileMode.Create);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);
            
            Console.WriteLine("[INFO] Log File Start");
            
            InitializeComponent();
        }

        // This gets called when someone clicks the button to select the GTAV directory
        private void btnSelectDirectory_Click(object sender, EventArgs e)
        {
            // Open a browser dialog to select the directory
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.Description = "Select your root GTA5 Directory (Contains GTA5.exe)";
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            Console.WriteLine("[INFO] Selecting Directory");
            DialogResult dr = folderBrowserDialog.ShowDialog();
            string path = "";
            if (dr == DialogResult.Yes || dr == DialogResult.OK)
                path = folderBrowserDialog.SelectedPath;
            else
                Application.Exit();
            gtavPath = path;
            Console.WriteLine("[INFO] Directory Selected");

            // If the directory doesn't contain GTA5.exe, warn the user that they may have selected the wrong directory
            if (!File.Exists(gtavPath + "\\GTA5.exe"))
            {
                Console.WriteLine("[WARN] Directory may not be gtav");
                DialogResult dialogResult = MessageBox.Show("Please make sure that you selected the correct directory, could not detect GTA5.exe", "GTAV Directory Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                Console.WriteLine("[INFO] Correct Directory Selected");
                lblPath.Text = gtavPath;
            }
        }

        // Handles the click event for the ultimate backup checkbox
        private void chkUltimateBackup_CheckedChanged(object sender, EventArgs e)
        {
            ultimateBackup = chkUltimateBackup.Checked;
        }

        // This gets called when the user clicks the button to start the install.
        private void btnInstall_Click(object sender, EventArgs e)
        {
            eupMode = chkEUP.Checked;
            ultimateBackup = chkUltimateBackup.Checked;
            Console.WriteLine("[INFO] Eup mode is " + eupMode);
            Console.WriteLine("[INFO] Ultimate Backup mode is " + ultimateBackup);
            
            // This initializes the installer, read the DownloadManager class for more information
            DownloadManager dm = new DownloadManager(gtavPath, ultimateBackup, eupMode, progressBar, wardrobeAccepted);
            Console.WriteLine("[INFO] Beginning install");
            // This starts the install
            dm.beginExtraction();
            Console.WriteLine("[INFO] Beginning cleanup");
            // This cleans up the temp directory
            dm.beginCleanup();
        }

        // Handles the click event for the eup checkbox
        private void chkEUP_CheckedChanged(object sender, EventArgs e)
        {
            eupMode = chkEUP.Checked;
            chkWardrobe.Enabled = eupMode;
        }

        // Handles the click event for the eup wardrobe checkbox
        private void chkWardrobe_CheckedChanged(object sender, EventArgs e)
        {
            wardrobeAccepted = chkWardrobe.Checked;
        }
    }
}