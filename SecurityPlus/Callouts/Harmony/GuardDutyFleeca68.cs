using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.Harmony
{
    [CalloutInfo("GuardDutyFleeca68", CalloutProbability.High)]
    
    public class GuardDutyFleeca68 : Callout
    {
        private Vector3 calloutPosition;
        private Vector3 sus1Pos, sus2Pos;
        private Vector3 checkPos1, checkPos2, checkPos3;
        private Vector3 robberSpawn;

        private Vehicle susCar;
        
        private int option;

        private bool shiftStarted, shiftDone;

        private Blip blip;

        private Ped suspect1, suspect2;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - GuardDutyFleeca68");
            
            sus1Pos = new Vector3(1161.857f, 2722.115f, 38.00418f);
            sus2Pos = new Vector3(1176.594f, 2712.157f, 38.09778f);
            checkPos1 = new Vector3(1160.106f, 2722.954f, 38.00421f);
            checkPos2 = new Vector3(1172.957f, 2705.975f, 38.09392f);
            checkPos3 = new Vector3(1177.403f, 2711.868f, 38.09777f);
            robberSpawn = new Vector3(1173.093f, 2668.989f, 37.42283f);
            
            calloutPosition = new Vector3(1175.664f, 2699.285f, 38.17244f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Security required for guard duty";
            CalloutAdvisory = "A shift of guard duty requires a security officer";
            
            Functions.PlayScannerAudio("guard_duty");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            option = new Random().Next(0, 6);

            blip = new Blip(calloutPosition);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;
            shiftStarted = false;
            shiftDone = false;
            
            switch (option)
            {
                case 0:
                {
                    suspect1 = new Ped(sus1Pos, 75.0012f);
                    suspect1.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
                    
                    break;
                }
                case 1:
                {
                    suspect2 = new Ped(sus2Pos, 249.9422f);
                    suspect2.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
                    
                    break;
                }
                case 2:
                {
                    suspect1 = new Ped(sus1Pos, 75.0012f);
                    suspect1.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
                    suspect2 = new Ped(sus2Pos, 249.9422f);
                    suspect2.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
                    
                    break;
                }
                case 3:
                {
                    break;
                }
                case 4:
                {
                    break;
                }
                case 5:
                {
                    
                    
                    
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
            
            if (!shiftStarted && Game.LocalPlayer.Character.DistanceTo(calloutPosition) < 4f)
            {
                Game.DisplayNotification(Main.UID + ": Attention dispatch, I'm on scene and beginning my rounds.");
                shiftStarted = true;
                Game.DisplayHelp("Start by checking on the back of the bank");
                if (blip.Exists())
                    blip.Delete();
                blip = new Blip(checkPos1);
                blip.Color = Color.Yellow;
            }

            if (shiftStarted)
            {
                if (suspect1.Exists())
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect1) < 5f)
                    {
                        suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }

                if (suspect2.Exists())
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect2) < 5f)
                    {
                        suspect2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
                
                if (Game.LocalPlayer.Character.DistanceTo(checkPos1) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(checkPos2);
                    Game.DisplayHelp("Now check on the lobby");
                }

                if (Game.LocalPlayer.Character.DistanceTo(checkPos2) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(checkPos3);
                    Game.DisplayHelp("Now check on the vault");
                }

                if (Game.LocalPlayer.Character.DistanceTo(checkPos3) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(calloutPosition);
                    Game.DisplayHelp("Return to the bank entrance to end your shift");
                    shiftDone = true;
                    
                    if (option == 5)
                    {
                        suspect1 = new Ped(robberSpawn, 356.8657f);
                        suspect2 = new Ped(robberSpawn, 356.8657f);
                        susCar = new Vehicle("banshee", robberSpawn, 356.8657f);
                        suspect1.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);
                        suspect2.Inventory.GiveNewWeapon(WeaponHash.BullpupShotgun, -1, true);
                        suspect1.WarpIntoVehicle(susCar, -1);
                        suspect2.WarpIntoVehicle(susCar, 0);

                        suspect1.Tasks.DriveToPosition(new Vector3(1174.465f, 2695.34f, 37.51678f), 20f, VehicleDrivingFlags.Emergency).WaitForCompletion(10000);
                        suspect1.Tasks.LeaveVehicle(susCar, LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(5000);
                        suspect2.Tasks.LeaveVehicle(susCar, LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(5000);
                        suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        suspect2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        Game.DisplaySubtitle("Robber: Everyone on the ground this is a robbery!");
                    }
                }

                if (Game.LocalPlayer.Character.DistanceTo(calloutPosition) < 2 && shiftDone)
                {
                    End();
                }
            }
            
            if (Game.IsKeyDown(Main.endKey) || Game.LocalPlayer.Character.IsDead)
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
            if (susCar.Exists())
                susCar.Dismiss();
            
            Log.log("Security+ GuardDutyFleeca68 callout has been cleaned up");
            Game.LogTrivial("Security+ GuardDutyFleeca68 callout has been cleaned up");
        }
    }
}