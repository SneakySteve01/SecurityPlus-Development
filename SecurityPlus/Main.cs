using LSPD_First_Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SecurityPlus;
using SecurityPlus.Callouts.Sandy_Shores;
using SecurityPlus.Callouts.ULSA;
using Graphics = Rage.Graphics;
using Timer = System.Timers.Timer;

/*
 * Hi there, I'm not going to document all the code in all
 * the callouts because for some reason I made a million,
 * but feel free to peruse to your own liking, all the
 * other classes should have decent(ish) documentation.
 */
namespace SecurityPlus
{
    public class Main : Plugin
    {
        // INI File Variables
        private IniFile ini;
        public static string LogName;
        public static string Name;
        public static string UID;
        public static string armouredCar;
        public static string crownVictoria;
        public static string charger;
        public static string explorer;
        public static string fbiCar;
        public static bool distanceCheck;
        public static bool autoActivate;
        public static bool timedActivation;
        public static bool eupMode;
        public static int timerLength;
        public static Keys sendLogsKey;
        public static Keys activateKey;
        public static Keys endKey;
        
        // Used for timer bar on screen
        public static Screen screen = Screen.PrimaryScreen;
        private static TimerBarPool timerBarPool;
        private static Timer timer;
        private static int counter;
        private static TextTimerBar timerBar = new TextTimerBar("Press " + activateKey + " to activate Security+", "30");
        
        // Log file variables
        private static FileStream ostrm;
        private static StreamWriter writer = null;
        private static TextWriter oldOut = Console.Out;
        private static string dataFolderPath;
        
        // Initialization of the plugin
        public override void Initialize()
        {
            // Create localappdata folder
            dataFolderPath = (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) + "\\Security+";
            Directory.CreateDirectory(dataFolderPath);

            // Use CMD commands to hide the created folder
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = "/C attrib +s +h \"" + dataFolderPath + "\""
            };
            process.StartInfo = startInfo;
            process.Start();
            
            // Create log file
            try
            {
                ostrm = new FileStream(dataFolderPath + "\\SecurityPlus.spl", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
                writer.AutoFlush = true;
            }
            catch (Exception e)
            {
                Game.LogTrivial("Cannot open SecurityPlus.spl for writing");
                Game.LogTrivial(e.Message);
                Console.WriteLine("Cannot open SecurityPlus.spl for writing");
                Console.WriteLine(e.Message);
            }
            
            // This sends any Console.WriteLine() output to the log file, read the Log class for more info
            Console.SetOut(writer);
            Console.SetError(writer);
            Log.log("SecurityPlus.spl initialized at " + DateTime.Now);
            
            /*
             * This code is deprecated, or maybe it's not, I don't even remember anymore TBH
             * eupMode was going to allow for non-eup players to also play, but I think I gave up on that at some point
             */
            
            //eupMode = File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\plugins\\EUP");
            eupMode = false;

            // This code will log all the INI values to variables for easy access
            Log.log("Fetching ini values");
            ini = new IniFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\plugins\\LSPDFR\\SecurityPlus.ini");
            UID = ini.IniReadValue("Identifier", "ID");
            LogName = ini.IniReadValue("Identifier", "LogSendingName");
            Name = ini.IniReadValue("Identifier", "Name");
            armouredCar = ini.IniReadValue("Vehicles", "ArmouredCar");
            crownVictoria = ini.IniReadValue("Vehicles", "CrownVictoria");
            charger = ini.IniReadValue("Vehicles", "DodgeCharger");
            explorer = ini.IniReadValue("Vehicles", "Explorer");
            fbiCar = ini.IniReadValue("Vehicles", "FBICar");
            string dC = ini.IniReadValue("Options", "DistanceCheck");
            string aA = ini.IniReadValue("Options", "AutoActivate");
            string tA = ini.IniReadValue("Options", "TimedActivation");
            switch (dC)
            {
                case "false":
                    distanceCheck = false;
                    break;
                case "true":
                    distanceCheck = true;
                    break;
                default:
                    Log.error("INI file not proper");
                    Game.LogTrivial("Security+ FATAL ERROR. INI FILE NOT PROPER"); return;
            }
            switch (aA)
            {
                case "false":
                    autoActivate = false;
                    break;
                case "true":
                    autoActivate = true;
                    break;
                default:
                    Log.error("INI file not proper");
                    Game.LogTrivial("Security+ FATAL ERROR. INI FILE NOT PROPER"); return;
            }
            switch (tA)
            {
                case "false":
                    timedActivation = false;
                    break;
                case "true":
                    timedActivation = true;
                    break;
                default:
                    Log.error("INI file not proper");
                    Game.LogTrivial("Security+ FATAL ERROR. INI FILE NOT PROPER"); return;
            }

            sendLogsKey = (Keys)Enum.Parse(typeof(Keys), ini.IniReadValue("Keys", "SendLogs"));
            activateKey = (Keys) Enum.Parse(typeof(Keys), ini.IniReadValue("Keys", "Activate"));
            endKey = (Keys) Enum.Parse(typeof(Keys), ini.IniReadValue("Keys", "EndCall"));
            string tL = ini.IniReadValue("Options", "TimerLength");
            timerLength = int.Parse(tL);
            counter = 0;
            Log.log("All ini values fetched and activated");
            
            // This is LSPDFR boiler plate code
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            
            Log.log("Security+ " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialized");
            Game.LogTrivial("Security+ " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialized");
            
            Game.LogTrivial("Go on duty to fully load Security+");
            
            GameFiber.StartNew(logger);
            Log.log("Ready to send logs");
        }

        // Simple log stuff
        public override void Finally()
        {
            Log.log("Security+ has been cleaned up");
            Game.LogTrivial("Security+ has been cleaned up");
        }

        // Called above when onduty is true
        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (!OnDuty) return;
            switch (autoActivate)
            {
                // If auto activate is true, activate the callouts
                case true:
                    RegisterCallouts(false);
                    break;
                case false: // If auto activate is false, start the timer for manual activation
                    Log.log("AutoActivate is disabled, creating timer bar");
                    timerBarPool = new TimerBarPool();
                    timerBar.Label = "Press " + activateKey + " to activate Security+";
                    timerBar.Text = timerLength.ToString();
                    timerBar.LabelStyle = new TextStyle(TextFont.ChaletLondon, HudColor.BlueLight.GetColor(), 0.3f);
                    timerBar.Accent = HudColor.BlueLight.GetColor();
                
                    timerBarPool.Add(timerBar);
                    
                    GameFiber.StartNew(ProcessTimerBars);
                    break;
            }
                
            Log.log("Security+ Beta version 1.2 has loaded successfully!");
            Game.DisplayNotification("Security+ Beta version 1.2 has loaded successfully!");
        }

        // Simple function to process the timer and stop it when needed
        private static void tickDown(object sender, ElapsedEventArgs e)
        {
            Log.log("Timer ticking down");
            counter++;
            float f = float.Parse(timerBar.Text);
            f -= 1.00f;
            timerBar.Text = f.ToString();
            if (counter < timerLength) return;
            timer.Stop();
            timerBarPool.Remove(timerBar);
        }

        // This function just waits for the user to press the activate key, then activates the callouts
        private static void register()
        {
            while (true)
            {
                GameFiber.Yield();

                if (!Game.IsKeyDown(activateKey)) continue;
                Log.log("Activation key pressed");
                RegisterCallouts(true);
                break;
            }
        }

        // This function handles the WIP in-game log system
        private static void logger()
        {
            while (true)
            {
                GameFiber.Yield();

                // If at any point, while the game is running, the user presses the log key, then begin logging
                if (!Game.IsKeyDown(sendLogsKey)) continue;
                
                // If the user hasn't specified a name for the log in the INI file, then ignore the logging request
                if (LogName == "CHANGE_ME_NOW") {
                    Game.DisplayNotification("Failed to send Security+ Log. Please change your LogSendingName in the ini file.");
                    continue;
                }
                
                // Collect the log file
                FileStream fs = new FileStream(dataFolderPath + "\\SecurityPlus.spl", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                List<string> list = new List<string>();
                while (!sr.EndOfStream)
                {
                    list.Add(sr.ReadLine());   
                }
                FileStream streamFile = new FileStream(dataFolderPath + "\\" + LogName + ".spl", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(streamFile);
                foreach (string line in list)
                {
                    streamWriter.WriteLine(line);
                }
                streamWriter.Close();
                streamFile.Close();
                sr.Close();
                fs.Close();
                    
                /*
                 * This uploads the log file to a remote server, if you plan on using it,
                 * you'll need to fill in the fields below as I've hidden my original
                 * credentials for obvious reasons.
                 */
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://SERVER_IP_HERE/" + LogName + ".spl");
                request.Credentials = new NetworkCredential("YOUR_USERNAME_HERE", "YOUR_PASSWORD_HERE");
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This code actually performs the upload
                using (Stream fileStream = File.OpenRead(dataFolderPath + "\\" + LogName + ".spl"))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }

                /*
                 * This part sends an entry through google forms to let me know of a new file.
                 * I've again hidden the original data, but it's just got two fields, name and log text.
                 * I have a google script that listens for a new entry and just shoots me an email
                 * to notify me of it.
                 */
                WebClient client = new WebClient();
                NameValueCollection nameValue = new NameValueCollection
                {
                    { "entry.137227417", LogName }, // This is field 1, it is called "Name"
                    { "entry.1811416649", "A new log file has been uploaded" } // This is field 2, it is called "Log"
                };

                Uri uri = new Uri("YOUR_FORM_URL/formResponse");

                // This actually sends the data
                byte[] response = client.UploadValues(uri, "POST", nameValue);
                string result = Encoding.UTF8.GetString(response);
                    
                // And we let the user know that it worked
                Game.DisplayNotification("Security+ Log has been sent! I'll get back to you as soon as possible.");
                    
                break;
            }
        }
        
        // Timer bar processing
        private static void ProcessTimerBars()
        {
            Log.log("Beginning timer");
            timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(tickDown);
            timer.Start();
            while (true)
            {
                GameFiber.Yield();
                
                timerBarPool.Draw();

                if (counter >= timerLength)
                    break;

                if (!Game.IsKeyDown(activateKey)) continue;
                RegisterCallouts(true);
                break;
            }
            timer.Stop();
            if (!timedActivation)
                GameFiber.StartNew(register);
        }

        // Registers the callouts of the pack, pretty simple stuff if you know lspdfr code
        private static void RegisterCallouts(bool activated)
        {
            Log.log("Registering callouts");
            if (activated)
            {
                Functions.PlayScannerAudio("activated", true);
            }
            // ULSA
            Functions.RegisterCallout(typeof(Callouts.ULSA.StudentRequestingHelp1));
            Functions.RegisterCallout(typeof(Callouts.ULSA.StudentRequestingHelp2));
            Functions.RegisterCallout(typeof(Callouts.ULSA.StudentFeelingUnsafe1));
            Functions.RegisterCallout(typeof(Callouts.ULSA.StudentFeelingUnsafe2));
            Functions.RegisterCallout(typeof(Callouts.ULSA.ULSACampusAttack));
            Log.log("ULSA Callouts Registered");
            
            // Sandy Shores
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.SheriffRequestingSecurity));
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.CodeAdamSandy));
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.OpenCarrySandy));
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.OpenCarrySandy2));
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.RefusingToPayYellowjack));
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.CasingSandy));
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.LoiterSandy));
            Functions.RegisterCallout(typeof(Callouts.Sandy_Shores.LoiterSandy2));
            Log.log("Sandy Callouts Registered");
            
            // Paleto Bay
            Functions.RegisterCallout(typeof(Callouts.Paleto.CodeAdamPaleto));
            Functions.RegisterCallout(typeof(Callouts.Paleto.CodeAdamPaleto2));
            Functions.RegisterCallout(typeof(Callouts.Paleto.OpenCarryPaleto));
            Functions.RegisterCallout(typeof(Callouts.Paleto.OpenCarryPaleto2));
            Functions.RegisterCallout(typeof(Callouts.Paleto.OpenCarryPaleto3));
            Functions.RegisterCallout(typeof(Callouts.Paleto.OpenCarryPaleto4));
            Functions.RegisterCallout(typeof(Callouts.Paleto.RefusingToPayPaleto));
            Functions.RegisterCallout(typeof(Callouts.Paleto.CasingPaleto));
            Functions.RegisterCallout(typeof(Callouts.Paleto.LoiterPaleto));
            Functions.RegisterCallout(typeof(Callouts.Paleto.LoiterPaleto2));
            Log.log("Paleto Callouts Registered");
            
            // Grapeseed
            Functions.RegisterCallout(typeof(Callouts.Grapeseed.CodeAdamGrapeseed));
            Functions.RegisterCallout(typeof(Callouts.Grapeseed.OpenCarryGrapeseed));
            Functions.RegisterCallout(typeof(Callouts.Grapeseed.OpenCarryGrapeseed2));
            Functions.RegisterCallout(typeof(Callouts.Grapeseed.CasingGrapeseed));
            Functions.RegisterCallout(typeof(Callouts.Grapeseed.CasingGrapeseed2));
            Functions.RegisterCallout(typeof(Callouts.Grapeseed.LoiterGrapeseed));
            Log.log("Grapeseed Callouts Registered");
            
            // Harmony
            Functions.RegisterCallout(typeof(Callouts.Harmony.CodeAdamHarmony));
            Functions.RegisterCallout(typeof(Callouts.Harmony.CodeAdamHarmony2));
            Functions.RegisterCallout(typeof(Callouts.Harmony.OpenCarryHarmony));
            Functions.RegisterCallout(typeof(Callouts.Harmony.OpenCarryHarmony2));
            Functions.RegisterCallout(typeof(Callouts.Harmony.OpenCarryHarmony3));
            Functions.RegisterCallout(typeof(Callouts.Harmony.CasingHarmony));
            Functions.RegisterCallout(typeof(Callouts.Harmony.CasingHarmony2));
            Functions.RegisterCallout(typeof(Callouts.Harmony.CasingHarmony3));
            Functions.RegisterCallout(typeof(Callouts.Harmony.LoiterHarmony));
            Functions.RegisterCallout(typeof(Callouts.Harmony.BankTellerSusPersonHarmony));
            Functions.RegisterCallout(typeof(Callouts.Harmony.GuardDutyFleeca68));
            Log.log("Harmony Callouts Registered");
            
            // Blaine county
            Functions.RegisterCallout(typeof(Callouts.Blaine.CodeAdamYouTool));
            Functions.RegisterCallout(typeof(Callouts.Blaine.OpenCarryGlobeOil));
            Functions.RegisterCallout(typeof(Callouts.Blaine.OpenCarryLoggerBar));
            Functions.RegisterCallout(typeof(Callouts.Blaine.OpenCarryYouTool));
            Functions.RegisterCallout(typeof(Callouts.Blaine.CasingYouTool));
            Functions.RegisterCallout(typeof(Callouts.Blaine.LoiterLoggerBar));
            Functions.RegisterCallout(typeof(Callouts.Blaine.LoiterHookies));
            Functions.RegisterCallout(typeof(Callouts.Blaine.RefusingToPayHookies));
            Functions.RegisterCallout(typeof(Callouts.Blaine.FakeCredentialsQuarry));
            Functions.RegisterCallout(typeof(Callouts.Blaine.FakeCredentialsPower));
            Functions.RegisterCallout(typeof(Callouts.Blaine.BankTellerSusPersonBlaine));
            Functions.RegisterCallout(typeof(Callouts.Blaine.ArmouredCarDriverNeededHtoP));
            Functions.RegisterCallout(typeof(Callouts.Blaine.ArmouredCarDriverNeededPtoH));
            Functions.RegisterCallout(typeof(Callouts.Blaine.GuardDutyPaletoBank));
            Log.log("Blaine Callouts Registered");
            
            // Los Santos County
            Functions.RegisterCallout(typeof(Callouts.LS_County.CodeAdamLSC));
            Functions.RegisterCallout(typeof(Callouts.LS_County.RefusingToPayLSC));
            Functions.RegisterCallout(typeof(Callouts.LS_County.CasingLSC));
            Functions.RegisterCallout(typeof(Callouts.LS_County.LoiterLSC));
            Functions.RegisterCallout(typeof(Callouts.LS_County.FakeCredentialsNOOSE));
            Functions.RegisterCallout(typeof(Callouts.LS_County.BankTellerSusPersonLSC));
            Log.log("Los Santos County Callouts Registered");
            
            // Los Santos
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS2));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS3));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS4));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS5));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS6));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS7));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS8));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS9));
            Functions.RegisterCallout(typeof(Callouts.LS.CodeAdamLS10));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS2));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS3));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS4)); 
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS5));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS6));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS7));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS8));
            Functions.RegisterCallout(typeof(Callouts.LS.OpenCarryLS9));
            Functions.RegisterCallout(typeof(Callouts.LS.RefusingToPayAldentes));
            Functions.RegisterCallout(typeof(Callouts.LS.RefusingToPayCaseys));
            Functions.RegisterCallout(typeof(Callouts.LS.RefusingToPayUPAB));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS2));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS3));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS4));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS5));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS6));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS7));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS8));
            Functions.RegisterCallout(typeof(Callouts.LS.CasingLS9));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS2));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS3));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS4));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS5));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS6));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS7));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS8));
            Functions.RegisterCallout(typeof(Callouts.LS.LoiterLS9));
            Functions.RegisterCallout(typeof(Callouts.LS.FakeCredentialsLSIA));
            Functions.RegisterCallout(typeof(Callouts.LS.FakeCredentialsBoatThing));
            Functions.RegisterCallout(typeof(Callouts.LS.FakeCredentialsHelipad));
            Functions.RegisterCallout(typeof(Callouts.LS.BankTellerSusPersonLSPillbox));
            Functions.RegisterCallout(typeof(Callouts.LS.BankTellerSusPersonLSDelPerro));
            Functions.RegisterCallout(typeof(Callouts.LS.BankTellerSusPersonLSAlta));
            Functions.RegisterCallout(typeof(Callouts.LS.BankTellerSusPersonLSBurton));
            Functions.RegisterCallout(typeof(Callouts.LS.BankTellerSusPersonLSMetro));
            Functions.RegisterCallout(typeof(Callouts.LS.GuardDutyPacificStandardBank));
            Functions.RegisterCallout(typeof(Callouts.LS.GuardDutyLSAltaBank));
            Functions.RegisterCallout(typeof(Callouts.LS.GuardDutyLSDelPerroBank));
            Functions.RegisterCallout(typeof(Callouts.LS.GuardDutyLSPillboxBank));
            Functions.RegisterCallout(typeof(Callouts.LS.ArmouredCarEscortCasino));
            Functions.RegisterCallout(typeof(Callouts.LS.ResidenceAlarmMike));
            Log.log("Los Santos Callouts Registered");
            
            // Davis Mega Mall
            Functions.RegisterCallout(typeof(Callouts.DavisMegaMall.CodeAdamDavisMegaMall));
            Functions.RegisterCallout(typeof(Callouts.DavisMegaMall.CasingDavisMegaMall));
            Log.log("Mega-Mall Callouts Registered");
        }
    }
}