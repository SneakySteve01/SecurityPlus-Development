using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.Blaine
{
    [CalloutInfo("ArmouredCarDriverNeededHtoP", CalloutProbability.High)]
    
    public class ArmouredCarDriverNeededHtoP : Callout
    {
        private Vector3 calloutPosition, endPos;
        private Vector3 ambusher1Location, ambusher2Location, ambusher3Location;

        private Vehicle armouredCar;
        private Vehicle ambusherCar1, ambusherCar2, ambusherCar3;

        private Ped ambusher1, ambusher2, ambusher3, ambusher4;

        private int option;

        private bool driveStarted;
        private bool a1Spawned, a2Spawned, a3Spawned, a4Spawned;

        private Blip blip;
        
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - ArmouredCarDriverNeededHtoP");
            
            ambusher1Location = new Vector3(2579.448f, 3024.958f, 43.34769f);
            ambusher2Location = new Vector3(2593.545f, 5328.261f, 43.95004f);
            ambusher3Location = new Vector3(-56.20996f, 6453.607f, 30.78031f);
            
            calloutPosition = new Vector3(1175.519f, 2696.043f, 37.78687f);
            endPos = new Vector3(-121.4141f, 6479.96f, 30.7183f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Armoured Car driver needed for money transfer";
            CalloutAdvisory = "Any armoured security unit requested as driver";
            
            Functions.PlayScannerAudio("armoured_car_money_transfer");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            armouredCar = new Vehicle(Main.armouredCar, calloutPosition, 181.652f);

            blip = new Blip(armouredCar);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;
            
            option = new Random().Next(0, 4);
            driveStarted = false;

            switch (option)
            {
                case 0:
                {
                    ambusherCar1 = new Vehicle("Sanchez", ambusher1Location, 352.1735f);
                    ambusher1 = new Ped(ambusher1Location, 352.1735f);
                    ambusher1.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, -1, true);
                    ambusher1.WarpIntoVehicle(ambusherCar1, -1);
                    a1Spawned = true;
                    
                    break;
                }
                case 1:
                {
                    ambusherCar1 = new Vehicle("sanchez", ambusher1Location, 352.1735f);
                    ambusher1 = new Ped(ambusher1Location, 352.1735f);
                    ambusher1.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, -1, true);
                    ambusher1.WarpIntoVehicle(ambusherCar1, -1);
                    a1Spawned = true;
                    
                    break;
                }
                case 2:
                {
                    ambusherCar1 = new Vehicle("Sanchez", ambusher1Location, 352.1735f);
                    ambusher1 = new Ped(ambusher1Location, 352.1735f);
                    ambusher1.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, -1, true);
                    ambusher1.WarpIntoVehicle(ambusherCar1, -1);
                    a1Spawned = true;
                    
                    break;
                }
                case 3:
                {
                    ambusherCar1 = new Vehicle("Sanchez", ambusher1Location, 352.1735f);
                    ambusher1 = new Ped(ambusher1Location, 352.1735f);
                    ambusher1.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, -1, true);
                    ambusher1.WarpIntoVehicle(ambusherCar1, -1);
                    a1Spawned = true;
                    
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
            
            if (!driveStarted)
            {
                if (Game.LocalPlayer.Character.DistanceTo(armouredCar) < 10f)
                {
                    if (blip.Exists())
                        blip.Delete();
                }

                if (armouredCar.Driver == Game.LocalPlayer.Character)
                {
                    Game.DisplayNotification("Dispatch: Attention " + Main.UID + ", the money in this van is to be taken to Blaine County Savings in Paleto bay. Be advised that the details of your route may have been compromised, remain vigilant.");
                    driveStarted = true;
                    blip = new Blip(endPos);
                    blip.Color = Color.Yellow;
                    blip.IsRouteEnabled = true;
                }
            }
            
            if (driveStarted)
            {
                switch (option)
                {
                    case 0:
                    {
                        if (ambusher1.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher1) < 20f)
                            {
                                ambusher1.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                                ambusher1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                        }
                        
                        break;
                    }
                    case 1:
                    {
                        if (Game.LocalPlayer.Character.DistanceTo(ambusher2Location) < 150f && !a2Spawned)
                        {
                            ambusherCar2 = new Vehicle("sentinel", ambusher2Location, 37.08084f);
                            ambusher2 = new Ped(ambusher2Location, 37.08084f);
                            ambusher2.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);
                            ambusher2.WarpIntoVehicle(ambusherCar2, -1);
                            a2Spawned = true;
                        }
                        
                        if (ambusher1.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher1) < 20f)
                            {
                                ambusher1.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                                ambusher1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                        }

                        if (ambusher2.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher2) < 20f)
                            {
                                ambusher2.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                                ambusher2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                        }
                        
                        break;
                    }
                    case 2:
                    {
                        if (Game.LocalPlayer.Character.DistanceTo(ambusher2Location) < 150f && !a2Spawned)
                        {
                            ambusherCar2 = new Vehicle("sentinel", ambusher2Location, 37.08084f);
                            ambusher2 = new Ped(ambusher2Location, 37.08084f);
                            ambusher2.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);
                            ambusher2.WarpIntoVehicle(ambusherCar2, -1);
                            a2Spawned = true;
                        }

                        if (Game.LocalPlayer.Character.DistanceTo(ambusher3Location) < 150f && !a3Spawned && !a4Spawned)
                        {
                            ambusherCar3 = new Vehicle("dominator", ambusher3Location, 45.56232f);
                            ambusher3 = new Ped(ambusher3Location, 45.56232f);
                            ambusher3.Inventory.GiveNewWeapon(WeaponHash.PumpShotgun, -1, true);
                            ambusher3.WarpIntoVehicle(ambusherCar3, -1);
                            a3Spawned = true;

                            ambusher4 = new Ped(ambusher3Location, 45.56232f);
                            ambusher4.Inventory.GiveNewWeapon(WeaponHash.AssaultSMG, -1, true);
                            ambusher4.WarpIntoVehicle(ambusherCar3, 0);
                            a4Spawned = true;
                        }

                        if (ambusher1.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher1) < 20f)
                            {
                                ambusher1.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                                ambusher1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                        }

                        if (ambusher2.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher2) < 20f)
                            {
                                ambusher2.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                                ambusher2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                        }

                        if (ambusher3.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher3) < 20f)
                            {
                                ambusher3.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                                ambusher3.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                        }

                        if (ambusher4.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher4) < 20f)
                            {
                                ambusher4.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            }
                        }
                        
                        break;
                    }
                    case 3:
                    {
                        if (ambusher1.Exists())
                        {
                            if (Game.LocalPlayer.Character.DistanceTo(ambusher1) < 20f)
                            {
                                ambusher1.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                                ambusher1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            } 
                        }
                        
                        break;
                    }
                }
            }

            if (Game.IsKeyDown(Main.endKey) || Game.LocalPlayer.Character.IsDead || Game.LocalPlayer.Character.DistanceTo(endPos) < 10f)
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
            if (ambusher1.Exists())
                ambusher1.Dismiss();
            if (ambusher2.Exists())
                ambusher2.Dismiss();
            if (ambusher3.Exists())
                ambusher3.Dismiss();
            if (ambusher4.Exists())
                ambusher4.Dismiss();
            if (ambusherCar1.Exists())
                ambusherCar1.Dismiss();
            if (ambusherCar2.Exists())
                ambusherCar2.Dismiss();
            if (ambusherCar3.Exists())
                ambusherCar3.Dismiss();
            if (armouredCar.Exists())
                armouredCar.Dismiss();
            
            Log.log("Security+ ArmouredCarDriverNeededHtoP callout has been cleaned up");
            Game.LogTrivial("Security+ ArmouredCarDriverNeededHtoP callout has been cleaned up");
        }
    }
}