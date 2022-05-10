using System;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.Harmony
{
    [CalloutInfo("CasingHarmony", CalloutProbability.High)]
    
    public class CasingHarmony : Callout
    {
        private Vector3 calloutPosition;

        private Ped suspect1, suspect2;

        private Blip blip;

        private bool convoStarted, adone, bdone;

        private int counter, option;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - CasingHarmony");
            
            calloutPosition = new Vector3(1164.783f, 2701.459f, 38.18038f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "People possibly casing for a robbery";
            CalloutAdvisory = "Store clerk reports suspicious individuals.";
            
            Functions.PlayScannerAudio("casing_robbery");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            option = new Random().Next(0, 2);
            
            suspect1 = new Ped(calloutPosition, 312.8686f);
            suspect1.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
            
            suspect2 = new Ped(new Vector3(1168.136f, 2701.803f, 38.17796f), 45.65288f);
            suspect2.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, false);

            blip = new Blip(suspect1);
            blip.IsFriendly = false;
            blip.IsRouteEnabled = true;

            convoStarted = false;
            counter = 0;
            adone = false;
            bdone = false;
            
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
                    if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f)
                    {
                        if (!convoStarted)
                        {
                            Game.DisplaySubtitle(Main.Name + ": Hi there, I've gotten a report of some suspicious individuals on the property. Would you happen to know anything about that?");
                            Game.DisplayHelp("Press 'Y' to advance conversation");
                            blip.Delete();
                            convoStarted = true;
                        }

                        if (Game.IsKeyDown(Keys.Y))
                        {
                            counter++;
                        }

                        if (counter == 1 && !adone)
                        {
                            adone = true;
                            Game.DisplaySubtitle("Suspect: Sorry, don't think I can be any help. Have a nice day!");
                            suspect1.Tasks.Wander();
                        }

                        if (counter == 2 && !bdone)
                        {
                            bdone = true;
                            Game.DisplaySubtitle("Other Suspect: You shouldn't be poking around in things that don't concern you!");
                            suspect2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            suspect1.Tasks.ClearImmediately();
                            suspect1.Tasks.ReactAndFlee(suspect2);
                        }
                    }
                    if (suspect1.IsDead || suspect1.IsCuffed)
                    {
                        if (suspect2.IsDead || suspect2.IsCuffed)
                        {
                            End();
                        }
                    }
                
                    if (Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
                    {
                        End();
                    }
                    break;
                }
                case 1:
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f)
                    {
                        if (!convoStarted)
                        {
                            Game.DisplaySubtitle(Main.Name + ": Hi there, I've gotten a report of some suspicious individuals on the property. Would you happen to know anything about that?");
                            Game.DisplayHelp("Press 'Y' to advance conversation");
                            blip.Delete();
                            convoStarted = true;
                        }

                        if (Game.IsKeyDown(Keys.Y))
                        {
                            counter++;
                        }

                        if (counter == 1 && !adone)
                        {
                            adone = true;
                            Game.DisplaySubtitle("Suspect: Sorry, don't think I can be any help. Have a nice day!");
                            suspect1.Tasks.Wander();
                        }

                        if (counter == 2 && !bdone)
                        {
                            bdone = true;
                            Game.DisplaySubtitle("Other Suspect: Sorry, I uhh, I have a thing. Bye!");
                            suspect2.Tasks.Flee(Game.LocalPlayer.Character, 60, 10000);
                        }
                    }
                    if (suspect1.IsDead || suspect1.IsCuffed)
                    {
                        if (suspect2.IsDead || suspect2.IsCuffed)
                        {
                            End();
                        }
                    }
                
                    if (Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
                    {
                        End();
                    }
                    break;
                }
            }
            
            if (Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (blip.Exists())
                blip.Delete();
            if (suspect1.Exists())
                suspect1.Dismiss();
            if (suspect2.Exists())
                suspect2.Dismiss();
            
            Log.log("Security+ CasingHarmony callout has been cleaned up");
            Game.LogTrivial("Security+ CasingHarmony callout has been cleaned up");
        }
    }
}