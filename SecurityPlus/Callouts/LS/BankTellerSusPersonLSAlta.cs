using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.LS
{
    [CalloutInfo("BankTellerSusPersonLSAlta", CalloutProbability.High)]
    
    public class BankTellerSusPersonLSAlta : Callout
    {
        private Vector3 calloutPosition;
        
        private Ped suspect;
        
        private Blip blip;

        private int counter, option;

        private bool convoStarted;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - BankTellerSusPersonLSAlta");
            
            calloutPosition = new Vector3(316.4229f, -270.3796f, 53.9092f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Suspicious person at a bank";
            CalloutAdvisory = "Bank teller requesting a security response unit for a suspicious individual";
            
            Functions.PlayScannerAudio("sus_person_bank");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            suspect = new Ped(calloutPosition, 177.0761f);
            suspect.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, false);

            suspect.IsPersistent = true;
            suspect.Metadata.hasGunPermit = false;
            
            blip = new Blip(suspect);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;

            counter = 0;
            convoStarted = false;
            
            option = new Random().Next(0, 3);
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            switch (option)
            {
                case 0:
                {
                    if (!convoStarted && Game.LocalPlayer.Character.DistanceTo(suspect) < 8f)
                    {
                        Game.DisplayHelp("Press 'Y' to advance conversation");
                        if (blip.Exists()) 
                            blip.Delete();
                        convoStarted = true;
                        Game.DisplaySubtitle(Main.Name + ": Hello! I'm " + Main.Name + " with security, can I ask what you're doing here?");
                    }

                    if (Game.IsKeyDown(Keys.Y))
                    {
                        counter++;
                    }

                    if (counter == 1)
                    {
                        Game.DisplaySubtitle("Subject: I'm just collecting some data for my new book: 'People of San Andreas'");
                    }

                    if (counter == 2)
                    {
                        Game.DisplaySubtitle(Main.Name + ": Sounds really neat! Unfortunately you can't be doing that as we're on private property and it's making the employees and customers uncomfortable.");
                    }

                    if (counter == 3)
                    {
                        Game.DisplaySubtitle("Subject: Oh, I'm sorry I didn't realise. Is there any way I can get a special permission to be here?");
                    }

                    if (counter == 4)
                    {
                        Game.DisplaySubtitle(Main.Name + ": You could try contacting the bank via the website, but for today I'll have to ask you to leave.");
                    }

                    if (counter == 5)
                    {
                        Game.DisplaySubtitle("Subject: Okay I guess I understand, have a good day.");
                    }

                    if (counter == 6)
                    {
                        Game.DisplaySubtitle(Main.Name + ": You as well, thanks for being understanding.");
                        End();
                    }

                    if (Game.LocalPlayer.Character.IsDead || suspect.IsDead || suspect.IsCuffed || Game.IsKeyDown(Main.endKey))
                    {
                        End();
                    }
                    
                    break;
                }
                case 1:
                {
                    if (!convoStarted && Game.LocalPlayer.Character.DistanceTo(suspect) < 8f)
                    {
                        Game.DisplayHelp("Press 'Y' to advance conversation");
                        if (blip.Exists()) 
                            blip.Delete();
                        convoStarted = true;
                        Game.DisplaySubtitle(Main.Name + ": Hello! I'm " + Main.Name + " with security, can I ask what you're doing here?");
                    }

                    if (Game.IsKeyDown(Keys.Y))
                    {
                        counter++;
                    }

                    if (counter == 1)
                    {
                        Game.DisplaySubtitle("Subject: Get lost mall-cop, nobody asked you.");
                    }

                    if (counter == 2)
                    {
                        Game.DisplaySubtitle(Main.Name + ": This is private property, you need to leave right now.");
                    }

                    if (counter == 3)
                    {
                        Game.DisplaySubtitle("Subject: Not happening pig, get outta my face before you get hurt.");
                    }

                    if (counter == 4)
                    {
                        Game.DisplaySubtitle(Main.Name + ": Last warning, you leave now or I'll detain you for trespassing and call the police.");
                    }

                    if (counter == 5)
                    {
                        Game.DisplaySubtitle("Subject: Make me!");
                        suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }

                    if (Game.LocalPlayer.Character.IsDead || suspect.IsDead || suspect.IsCuffed || Game.IsKeyDown(Main.endKey))
                    {
                        End();
                    }
                    
                    break;
                }
                case 2:
                {
                    if (!convoStarted && Game.LocalPlayer.Character.DistanceTo(suspect) < 8f)
                    {
                        Game.DisplayHelp("Press 'Y' to advance conversation");
                        if (blip.Exists()) 
                            blip.Delete();
                        convoStarted = true;
                        Game.DisplaySubtitle(Main.Name + ": Hello! I'm " + Main.Name + " with security, can I ask what you're doing here?");
                    }

                    if (Game.IsKeyDown(Keys.Y))
                    {
                        counter++;
                    }

                    if (counter == 1)
                    {
                        counter++;
                        Game.DisplaySubtitle("Subject: I'm robbing the joint now get on the ground!");
                        suspect.Tasks.AimWeaponAt(Game.LocalPlayer.Character, 10000).WaitForCompletion(10000);
                        suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }

                    if (Game.LocalPlayer.Character.IsDead || suspect.IsDead || suspect.IsCuffed || Game.IsKeyDown(Main.endKey))
                    {
                        End();
                    }
                    
                    break;
                }
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (blip.Exists())
                blip.Delete();
            if (suspect.Exists())
                suspect.Dismiss();
            
            Log.log("Security+ BankTellerSusPersonLSAlta callout has been cleaned up");
            Game.LogTrivial("Security+ BankTellerSusPersonLSAlta callout has been cleaned up");
        }
    }
}