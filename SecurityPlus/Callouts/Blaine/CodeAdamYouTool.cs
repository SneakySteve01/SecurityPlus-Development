using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.Blaine
{
    [CalloutInfo("CodeAdamYouTool", CalloutProbability.High)]
    
    public class CodeAdamYouTool : Callout
    {
        private Vector3 calloutPosition;

        private Ped victim, suspect;

        private Vehicle susCar;

        private Blip blip;

        private int counter, option;

        private bool convoStarted;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - CodeAdamYouTool");
            
            calloutPosition = new Vector3(2780.656f, 3486.15f, 54.95049f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Code Adam : Possible child in danger";
            CalloutAdvisory = "Store clerk reports a possible child in danger";
            
            Functions.PlayScannerAudio("code_adam");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            option = new Random().Next(0, 2);

            switch (option)
            {
                case 0:
                {
                    blip = new Blip(calloutPosition);
                    blip.Color = Color.Red;
                    blip.IsRouteEnabled = true;

                    susCar = new Vehicle("SPEEDO", calloutPosition, 250.0008f);
                    suspect = new Ped(calloutPosition, 97.07672f);
                    suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);

                    suspect.WarpIntoVehicle(susCar, -1);
            
                    victim = new Ped("cs_tracydisanto", calloutPosition, 101.1328f);
            
                    victim.WarpIntoVehicle(susCar, 1);
                    break;
                }
                case 1:
                {
                    blip = new Blip(calloutPosition);
                    blip.Color = Color.Red;
                    blip.IsRouteEnabled = true;

                    susCar = new Vehicle("BALLER", calloutPosition, 250.0008f);
                    suspect = new Ped(calloutPosition, 97.07672f);

                    suspect.WarpIntoVehicle(susCar, -1);
            
                    victim = new Ped("cs_tracydisanto", calloutPosition, 101.1328f);
            
                    victim.WarpIntoVehicle(susCar, 1);
                    break;
                }
            }
            
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
                    if (Game.LocalPlayer.Character.DistanceTo(suspect) < 30f && !convoStarted)
                    {
                        convoStarted = true;
                        if (blip.Exists())
                            blip.Delete();

                        suspect.Tasks.CruiseWithVehicle(susCar, 15f, VehicleDrivingFlags.Emergency);
                    }
                    
                    if (suspect.IsDead || suspect.IsCuffed || Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
                    {
                        End();
                    }
                    break;
                }
                case 1:
                {
                    if (!convoStarted && Game.LocalPlayer.Character.DistanceTo(suspect) < 20f)
                    {
                        Game.DisplaySubtitle(Main.Name + ": You there! Stop a moment!");
                        convoStarted = true;
                        if (blip.Exists())
                            blip.Delete();
                        Game.DisplayHelp("Press 'Y' to advance conversation");
                    }

                    if (convoStarted && Game.IsKeyDown(Keys.Y))
                    {
                        counter++;
                    }

                    if (counter == 1)
                    {
                        Game.DisplaySubtitle("Suspect: Is there a problem?");
                        suspect.Tasks.LeaveVehicle(susCar, LeaveVehicleFlags.None);
                    }

                    if (counter == 2)
                    {
                        Game.DisplaySubtitle(Main.Name + ": Who's that you have in your vehicle?");
                    }

                    if (counter == 3)
                    {
                        if (victim.IsMale)
                            Game.DisplaySubtitle("Suspect: Well that's my son of course. Why? Is something the matter?");
                        else if (victim.IsFemale)
                            Game.DisplaySubtitle("Suspect: Well that's my daughter of course. Why? Is something the matter?");
                        Game.DisplayHelp("Handle the situation with your discretion");
                    }
                    break;
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
            if (susCar.Exists())
                susCar.Dismiss();
            if (victim.Exists())
                victim.Dismiss();
            if (suspect.Exists())
                suspect.Dismiss();
            
            Log.log("Security+ CodeAdamYouTool callout has been cleaned up");
            Game.LogTrivial("Security+ CodeAdamYouTool callout has been cleaned up");
        }
    }
}