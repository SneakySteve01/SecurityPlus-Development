using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.ULSA
{
    // A ULSA Student has passed out from alcohol, is underage and did not call 911 for fear of prosecution.
    [CalloutInfo("StudentRequestingHelp2", CalloutProbability.High)]
    
    public class StudentRequestingHelp2 : Callout
    {
        private Ped student, friend;
        private Vector3 calloutPosition;
        private Blip blip;

        private int counter;

        private bool iPressed;
        private bool convoStarted;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - StudentRequestingHelp2");
            
            calloutPosition = new Vector3(-1639.233f, 195.7003f, 61.11744f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(100f, calloutPosition);
                AddMaximumDistanceCheck(8200f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "ULSA Campus security requested by student";
            CalloutAdvisory = "Student in distress, requesting security";
            
            Functions.PlayScannerAudio("student_in_distress");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            Model[] peds = new Model[]
            {
                "a_f_m_bevhills_01", "a_f_y_bevhills_01", "a_f_y_bevhills_02", "a_f_y_clubcust_01", "a_f_y_clubcust_02", "a_f_y_clubcust_03", "a_f_y_hipster_02", "a_f_y_hipster_03", "a_f_y_indian_01", "a_m_m_skater_01", "a_m_y_beachvesp_01", "a_m_y_bevhills_02", "a_m_y_clubcust_01", "a_m_y_hipster_01", "a_m_y_skater_02", "a_m_y_vinewood_01"
            };

            student = new Ped(peds[new Random().Next(peds.Length)], calloutPosition, 294.1492f);

            friend = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1638.215f, 198.5029f, 60.85832f), 159.901f);
            while (friend.Model == student.Model)
            {
                friend.Delete();
                friend = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1638.215f, 198.5029f, 60.85832f), 159.901f);
            }
            
            blip = new Blip(calloutPosition);
            blip.IsRouteEnabled = true;
            blip.Color = Color.Red;

            counter = 0;
            iPressed = false;
            convoStarted = false;

            student.Kill();
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(friend) < 10f)
            {
                if (Game.IsKeyDown(Main.endKey))
                {
                    End();
                }

                if (Game.IsKeyDown(Keys.Y))
                {
                    counter++;
                }

                if (!convoStarted)
                {
                    Game.DisplaySubtitle("Friend: Thanks for getting here quick, my friend has passed out after drinking too much and I was worried that calling the police would get them in trouble. Will you help us?");
                    Game.DisplayHelp("Press 'Y' to advance the conversation");
                }

                if (counter == 1)
                {
                    Game.DisplaySubtitle(Main.Name + ": Of course I will, that's what I'm here for, and I promise you won't get into trouble. If something like this ever happens again please call 911, they only want to help you.");
                    convoStarted = true;
                }
                else if (counter == 2)
                {
                    Game.DisplaySubtitle("Friend: Okay, thank you.");
                    Game.DisplayHelp("Press 'I' to check on the passed out student.");
                    counter++;
                }

                if (Game.IsKeyDown(Keys.I) && !iPressed)
                {
                    iPressed = true;

                    Random random = new Random();
                    
                    int n = random.Next(0, 2);

                    if (n == 0)
                    {
                        Game.DisplaySubtitle(Main.Name + ": I think they will be alright, but I'm going to call them an ambulance to be sure.");  
                    } else if (n == 1)
                    {
                        Game.DisplaySubtitle(Main.Name + ": I'm so sorry, they aren't responsive, I'm calling an ambulance now.");
                    }
                    Game.DisplayHelp("Call an ambulance");
                }
            }

            if (friend.IsDead || friend.IsCuffed || Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (friend.Exists())
                friend.Dismiss();
            if (student.Exists())
                student.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ StudentRequestingHelp2 has been cleaned up");
            Game.LogTrivial("Security+ StudentRequestingHelp2 has been cleaned up");
        }
    }
}