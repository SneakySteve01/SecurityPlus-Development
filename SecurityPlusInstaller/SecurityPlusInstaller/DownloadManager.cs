using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Windows.Forms;
using SecurityPlusInstaller.Properties;

namespace SecurityPlusInstaller
{
    /*
     * So this is the big one, the one that actually does the installation.
     * It's called DownloadManager because originally I was planning on
     * having the files be downloaded rather than packaged in the installer.
     * Scrapped that idea because of hosting issues.
     */
    public class DownloadManager
    {
        // Just some vars to hold data
        private string gtavPath;
        private string tempPath;
        private bool ultimateBackup, eupMode, rnuNeeded, wardrobeAccepted;
        private ProgressBar progressBar;
        
        // Constructor, this is what sets up the data and creates a temp folder
        public DownloadManager(string gtavPath, bool ultimateBackup, bool eupMode, ProgressBar progressBar, bool wardrobeAccepted)
        {
            this.gtavPath = gtavPath;
            this.ultimateBackup = ultimateBackup;
            this.eupMode = eupMode;
            this.rnuNeeded = false;
            this.wardrobeAccepted = wardrobeAccepted;
            Console.WriteLine("[INFO] Creating temporary directory");
            tempPath = this.gtavPath + "\\temp";
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            this.progressBar = progressBar;
        }
        
        // This is the method that actually does the installation
        public void beginExtraction()
        {
            // We start off by initializing the progress bar
            progressBar.Visible = true;

            // If the user doesn't already have RAGENativeUI installed, we need to install it for them
            if (!File.Exists(gtavPath + "\\RAGENativeUI.dll"))
                rnuNeeded = true;

            /*
             * This switch handles the progress bar,
             * it makes sure its going to be accurate
             * no matter how much stuff is being installed
             */
            switch (rnuNeeded)
            {
                // All bools
                case true when eupMode && ultimateBackup:
                    progressBar.Maximum = 40;
                    break;
                // Only UltimateBackup
                case false when !eupMode && ultimateBackup:
                // Only EUP
                case false when eupMode && !ultimateBackup:
                    progressBar.Maximum = 34;
                    break;
                // EUP and UltimateBackup
                case false when eupMode && ultimateBackup:
                    progressBar.Maximum = 35;
                    break;
                // RageNativeUI and UltimateBackup
                case true when !eupMode && ultimateBackup:
                    progressBar.Maximum = 39;
                    break;
                // Only RageNativeUI
                case true when !eupMode && !ultimateBackup:
                    progressBar.Maximum = 38;
                    break;
                // No bools
                case false when !eupMode && !ultimateBackup:
                    progressBar.Maximum = 33;
                    break;
                default:
                    progressBar.Maximum = 30;
                    break;
            }
            
            progressBar.Minimum = 0;
            progressBar.Step = 1;
            Console.WriteLine("[INFO] Progress Bar Initialized");

            // Installing rage native ui
            if (rnuNeeded)
            {
                /*
                 * Now would be a good time to mention, we're storing the packaged data as resources
                 * in the project, so we can just write it to the folder byte by byte
                 */
                
                // dll
                File.WriteAllBytes(gtavPath + "\\RAGENativeUI.dll", Resources.RNUI_RAGENativeUIDLL);
                progressBar.PerformStep();
                // pdb 
                File.WriteAllBytes(gtavPath + "\\RAGENativeUI.pdb", Resources.RNUI_RAGENativeUIPDB);
                progressBar.PerformStep();
                // xml
                File.WriteAllText(gtavPath + "\\RAGENativeUI.XML", Resources.RNUI_RAGENativeUIXML);
                progressBar.PerformStep();
                // notice
                File.WriteAllBytes(gtavPath + "\\NOTICE.md", Resources.RNUI_NOTICE);
                progressBar.PerformStep();
                // license
                File.WriteAllBytes(gtavPath + "\\LICENSE.md", Resources.RNUI_LICENSE);
                progressBar.PerformStep();
            }
            
            // heap adjuster, this is needed for the custom cars
            File.WriteAllBytes(gtavPath + "\\HeapAdjuster.asi", Resources.HeapAdjuster);
            File.WriteAllText(gtavPath + "\\HeapAdjuster.ini", Resources.HeapConfig);
            progressBar.PerformStep();
            Console.WriteLine("[INFO] Heap adjuster installed");
            
            // dll, this is the actual mod itself, very simple install and very important
            File.WriteAllBytes(gtavPath + "\\plugins\\LSPDFR\\SecurityPlus.dll", Resources.SecurityPlus);
            progressBar.PerformStep();
            Console.WriteLine("[INFO] dll installed");
            // ini, this is the customization file for the main mod
            File.WriteAllText(gtavPath + "\\plugins\\LSPDFR\\SecurityPlus.ini", Resources.SecurityConfig);
            progressBar.PerformStep();
            Console.WriteLine("[INFO] ini installed");

            // Install the wardrobe file if it was requested
            if (eupMode && wardrobeAccepted)
            {
                File.WriteAllText(gtavPath + "\\plugins\\EUP\\wardrobe.ini", Resources.wardrobe);
                Console.WriteLine("[INFO] wardrobe installed");
            }
            
            // This block sets up the folders for the audio files if they don't exist
            if (!Directory.Exists(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS"))
                Directory.CreateDirectory(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS");
            if (!Directory.Exists(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls"))
                Directory.CreateDirectory(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls");
            if (!Directory.Exists(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\other"))
                Directory.CreateDirectory(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\other");
            if (!Directory.Exists(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\prefix"))
                Directory.CreateDirectory(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\prefix");

            Console.WriteLine("[INFO] Audio directories created");
            Console.WriteLine("[INFO] Starting audio install");
            
            /*
             * Here we install the audio files, there's
             * a lot of them, so I'm not going to list them all.
             * I couldn't think of a better way to go about this,
             * but I'm sure there is one, because this is one hell
             * of a messy block of code.
             */
            
            var stream = new MemoryStream();
            Resources.activated.CopyTo(stream);
            var bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\other\\activated.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.any_avail_armoured_security_officer.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\prefix\\any_avail_armoured_security_officer.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.any_avail_security_officer.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\prefix\\any_avail_security_officer.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.attention_avail_armoured_security.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\prefix\\attention_avail_armoured_security.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.attention_avail_security.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\prefix\\attention_avail_security.wav", bytes);
            progressBar.PerformStep();
            //calls
            stream = new MemoryStream();
            Resources.active_shooter_ulsa.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\active_shooter_ulsa.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.armoured_car_escort.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\armoured_car_escort.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.armoured_car_money_transfer.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\armoured_car_money_transfer.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.casing_robbery.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\casing_robbery.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.code_adam.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\code_adam.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.consumer_refusing_to_pay.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\consumer_refusing_to_pay.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.fake_credentials.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\fake_credentials.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.guard_duty.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\guard_duty.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.insurgency_sandy_air.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\insurgency_sandy_air.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.loitering.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\loitering.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.open_carry.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\open_carry.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.private_escort.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\private_escort.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.residence_alarm.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\residence_alarm.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.student_feeling_unsafe.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\student_feeling_unsafe.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.student_in_distress.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\student_in_distress.wav", bytes);
            progressBar.PerformStep();
            stream = new MemoryStream();
            Resources.sus_person_bank.CopyTo(stream);
            bytes = stream.ToArray();
            File.WriteAllBytes(gtavPath + "\\lspdfr\\audio\\scanner\\SECURITYPLUS\\calls\\sus_person_bank.wav", bytes);
            progressBar.PerformStep();
            Console.WriteLine("[INFO] Audio install complete");
            // data
            Console.WriteLine("[INFO] Installing data files");
            
            // Alright, that's all done. Next up we install all the EUP stuff if requested
            
            // 29
            switch (eupMode)
            {
                case true:
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\agency_gru.xml", Resources.eup_agency_gru);
                    progressBar.PerformStep();
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\duty_selection_gru.xml", Resources.eup_duty_selection_gru);
                    progressBar.PerformStep();
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\inventory_gru.xml", Resources.eup_inventory_gru);
                    progressBar.PerformStep();
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\outfits_gru.xml", Resources.eup_outfits_gru);
                    progressBar.PerformStep();
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\stations_gru.xml", Resources.eup_stations_gru);
                    progressBar.PerformStep();
                    break;
                case false:
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\agency_gru.xml", Resources.agency_gru);
                    progressBar.PerformStep();
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\duty_selection_gru.xml", Resources.duty_selection_gru);
                    progressBar.PerformStep();
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\inventory_gru.xml", Resources.inventory_gru);
                    progressBar.PerformStep();
                    File.WriteAllText(gtavPath + "\\lspdfr\\data\\custom\\stations_gru.xml", Resources.stations_gru);
                    progressBar.PerformStep();
                    break;
            }
            Console.WriteLine("[INFO] Installing ELS files");
            // els, these are the things that make the pretty lights happen
            File.WriteAllText(gtavPath + "\\ELS\\pack_default\\armouredcar.xml", Resources.armouredcar);
            progressBar.PerformStep();
            File.WriteAllText(gtavPath + "\\ELS\\pack_default\\charger.xml", Resources.charger);
            progressBar.PerformStep();
            File.WriteAllText(gtavPath + "\\ELS\\pack_default\\cvpi.xml", Resources.cvpi);
            progressBar.PerformStep();
            File.WriteAllText(gtavPath + "\\ELS\\pack_default\\fpiu.xml", Resources.fpiu);
            progressBar.PerformStep();
            
            // special units, required for the ultimate backup switch
            if (ultimateBackup)
            {
                Console.WriteLine("[INFO] Installing SpecialUnits.xml");
                switch (eupMode)
                {
                    case true:
                        File.WriteAllText(gtavPath + "\\plugins\\LSPDFR\\UltimateBackup\\SpecialUnits.xml", Resources.eup_SpecialUnits);
                        progressBar.PerformStep();
                        break;
                    case false:
                        File.WriteAllText(gtavPath + "\\plugins\\LSPDFR\\UltimateBackup\\SpecialUnits.xml", Resources.SpecialUnits);
                        progressBar.PerformStep();
                        break;
                }
            }
            
            Console.WriteLine("[INFO] Starting oiv install");
            
            /*
             * This is where we install the oiv file (cars)
             * First we move the oiv file to the temp directory
             * Then we start the oiv file as a process, if it's not
             * set to run through OpenIV, then we got a problem,
             * and it's one I never really bothered to fix
             */
            
            File.WriteAllBytes(tempPath + "\\Security+.oiv", Resources.Security_);
            progressBar.PerformStep();

            DialogResult dialogResult = MessageBox.Show("The installer will now launch Security+.oiv, if a program choosing dialogue pops up, select \"Open with OpenIV\"", "OIV Install", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            if (dialogResult == DialogResult.OK)
            {
                Process oiv = Process.Start(tempPath + "\\Security+.oiv");
                oiv.WaitForExit();
            }
            else
            {
                Console.WriteLine("[ERROR] The OIV install was aborted, fatal crash of program");
                MessageBox.Show("A Fatal Error has occured in the installation, installer will now exit.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Application.Exit(); 
            }

            Console.WriteLine("[INFO] OIV install complete");
            progressBar.Visible = false;
            
            // And we're done, all installed now.            
            
            DialogResult result = MessageBox.Show("Security+ has finished installing, enjoy!", "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        // This just cleans up the temporary files and directory
        public void beginCleanup()
        {
            Console.WriteLine("[INFO] Removing temporary directory");
            if (File.Exists(tempPath + "\\Security+.oiv"))
                File.Delete(tempPath + "\\Security+.oiv");
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath);
            Console.WriteLine("[INFO] All Installed!");
            Application.Exit();
        }
    }
}