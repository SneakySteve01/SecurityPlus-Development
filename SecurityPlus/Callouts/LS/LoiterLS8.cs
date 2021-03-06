using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.LS
{
    [CalloutInfo("LoiterLS8", CalloutProbability.High)]
    
    public class LoiterLS8 : Callout
    {
        private Vector3 calloutPosition;
        
        private Ped suspect1, suspect2, suspect3;

        private Blip blip;

        private bool convoStarted, adone, bdone, cdone;

        private int counter;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - LoiterLS8");
            
            calloutPosition = new Vector3(1203.448f, -1384.108f, 35.22696f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Loitering";
            CalloutAdvisory = "Security requested for a group of people loitering.";
            
            Functions.PlayScannerAudio("loitering");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            suspect1 = new Ped(calloutPosition, 151.3969f);
            suspect2 = new Ped(new Vector3(1203.292f, -1387.186f, 35.22696f), 1.176602f);
            suspect3 = new Ped(new Vector3(1200.522f, -1386.485f, 35.22696f), 291.9308f);

            blip = new Blip(suspect1);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;

            convoStarted = false;
            counter = 0;
            adone = false;
            bdone = false;
            cdone = false;
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 10f)
            {
                if (!convoStarted)
                {
                    if (blip.Exists())
                        blip.Delete();
                    Game.DisplaySubtitle(Main.Name + ": Hey folks, can I ask what you're all doing out here?");
                    Game.DisplayHelp("Press 'Y' to advance conversation");
                    convoStarted = true;
                }

                if (Game.IsKeyDown(Keys.Y))
                {
                    counter++;
                }

                if (counter == 1)
                {
                    Game.DisplaySubtitle("Suspect: Nothing to worry about here, just hanging out.");
                }

                if (counter == 2)
                {
                    Game.DisplaySubtitle(Main.Name + ": I understand, unfortunately this place has a strict no loitering policy, you'll have to get moving.");
                }

                if (counter == 3 && !adone)
                {
                    Game.DisplaySubtitle("Other Suspect: Fine fine, c'mon guys let's get outta here.");
                    suspect1.Tasks.Wander();
                    suspect3.Tasks.Wander();
                    adone = true;
                }

                if (counter == 4 && !bdone)
                {
                    Game.DisplaySubtitle("Suspect: Oh you won't get rid of *me* that easily! Back off!");
                    suspect2.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, true);
                    bdone = true;
                }

                if (counter == 5 && !cdone)
                {
                    suspect2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    cdone = true;
                }
            }

            if (Game.IsKeyDown(Main.endKey))
            {
                End();
            }
            
            if (suspect1.IsDead || suspect1.IsCuffed)
            {
                if (suspect2.IsDead || suspect2.IsCuffed)
                {
                    if (suspect3.IsDead || suspect3.IsCuffed)
                    {
                        End();
                    }    
                }
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (suspect1.Exists())
                suspect1.Dismiss();
            if (suspect2.Exists())
                suspect2.Dismiss();
            if (suspect3.Exists())
                suspect3.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ LoiterLS8 callout has been cleaned up");
            Game.LogTrivial("Security+ LoiterLS8 callout has been cleaned up");
        }
    }
}