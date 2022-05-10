using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.Paleto
{
    [CalloutInfo("OpenCarryPaleto3", CalloutProbability.High)]
    
    public class OpenCarryPaleto3 : Callout
    {
        private Vector3 calloutPosition;

        private Ped suspect;

        private Blip blip;
        
        private int counter;
        
        private bool convoStarted, d;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - OpenCarryPaleto3");
            
            calloutPosition = new Vector3(-97.92923f, 6406.397f, 31.64036f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Illegal open-carry";
            CalloutAdvisory = "Armed security requested for open-carry on a premises which does not allow it.";
            
            Functions.PlayScannerAudio("open_carry");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            blip = new Blip(calloutPosition);
            blip.Color = Color.Red;
            blip.IsRouteEnabled = true;

            suspect = new Ped(calloutPosition, 38.69176f);
            suspect.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);

            convoStarted = false;
            counter = 0;
            d = false;
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(suspect) < 10f)
            {
                if (blip.Exists())
                    blip.Delete();

                if (Game.IsKeyDown(Keys.Y))
                {
                    counter++;
                }

                if (!convoStarted)
                {
                    Game.DisplayHelp("Press 'Y' to advance conversation");
                    Game.DisplaySubtitle(Main.Name + ": Excuse me, I'm sorry to inform you but this facility does not allow open carry, you're going to have to leave.");
                    convoStarted = true;
                }

                if (counter == 1)
                {
                    Game.DisplaySubtitle("Suspect: What are talking about? This is a rights violation!");
                }

                if (counter == 2)
                {
                    Game.DisplaySubtitle(Main.Name + ": Please try to calm down, I'm only doing my job. You're more than welcome to open-carry as soon as you leave the property.");
                }

                if (counter == 3 && !d)
                {
                    Random random = new Random();
                    int n = random.Next(0, 2);

                    if (n == 0)
                    {
                        d = true;
                        Game.DisplaySubtitle("Suspect: Fine! I'll take my business elsewhere, but I'm reporting this place!");
                        suspect.Tasks.Wander();
                    } else if (n == 1)
                    {
                        d = true;
                        Game.DisplaySubtitle("Suspect: Try and take my rights now!");
                        suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
            }

            if (suspect.IsDead || suspect.IsCuffed || Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
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
            if (suspect.Exists())
                suspect.Dismiss();
            
            Log.log("Security+ OpenCarryPaleto3 callout has been cleaned up");
            Game.LogTrivial("Security+ OpenCarryPaleto3 callout has been cleaned up");
        }
    }
}