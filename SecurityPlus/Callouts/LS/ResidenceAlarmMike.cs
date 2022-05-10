using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.LS
{
    [CalloutInfo("ResidenceAlarmMike", CalloutProbability.High)]
    
    public class ResidenceAlarmMike : Callout
    {
        private Vector3 calloutPosition;

        private Blip blip;

        private Ped suspect;

        private int option;

        private bool started;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - ResidenceAlarmMike");
            
            option = new Random().Next(0, 3);

            switch (option)
            {
                case 0:
                {
                    calloutPosition = new Vector3(-810.7609f, 166.7368f, 72.22813f);
                    break;
                }
                case 1:
                {
                    calloutPosition = new Vector3(-808.8f, 187.0157f, 72.4773f);
                    break;
                }
                case 2:
                {
                    calloutPosition = new Vector3(-844.0556f, 189.9526f, 72.82434f);
                    break;
                }
            }

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(1000f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Silent Alarm at a Residence";
            CalloutAdvisory = "Security is needed for a silent alarm triggered in a residence.";
            
            Functions.PlayScannerAudio("residence_alarm");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            switch (option)
            {
                case 0:
                {
                    suspect = new Ped(calloutPosition, 338.04f);
                    suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
                    break;
                }
                case 1:
                {
                    suspect = new Ped(calloutPosition, 248.409f);
                    suspect.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, true);
                    break;
                }
                case 2:
                {
                    suspect = new Ped(calloutPosition, 226.0251f);
                    suspect.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
                    break;
                }
            }

            blip = new Blip(suspect);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;

            started = false;
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            
            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(suspect) < 10f && !started)
            {
                started = true;
                if (blip.Exists())
                    blip.Delete();
                Game.DisplaySubtitle("Suspect: Stay away from me!");
                suspect.Tasks.Flee(Game.LocalPlayer.Character, 50f, 15000);
            }

            if (Game.IsKeyDown(Main.endKey) || Game.LocalPlayer.Character.IsDead || suspect.IsDead || suspect.IsCuffed)
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (suspect.Exists()) 
                suspect.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ ResidenceAlarmMike callout has been cleaned up");
            Game.LogTrivial("Security+ ResidenceAlarmMike callout has been cleaned up");
        }
    }
}