using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.LS
{
    [CalloutInfo("ArmouredCarEscortCasino", CalloutProbability.High)]
    
    public class ArmouredCarEscortCasino : Callout
    {
        private Vector3 calloutPosition, endPos;

        private Vector3 vanRear;
        private Vector3 dropOff;
        private Vector3 vanReturn;
        private Vector3 ambushBikeLocation;
        private Vector3 ambushLocation;
        
        private Blip blip;

        private Vehicle van;
        private Vehicle ambushBike;

        private Ped driver;
        private Ped biker;
        private Ped ambusher;

        private bool driveStarted;
        private bool packageDroppedOff;
        private bool endPosReached;

        private int option;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - ArmouredCarEscortCasino");
            
            calloutPosition = new Vector3(934.3613f, -2.345299f, 78.5946f);

            endPos = new Vector3(271.6597f, 181.6729f, 104.3857f);
            dropOff = new Vector3(253.8832f, 220.8503f, 106.2865f);

            ambushLocation = new Vector3(288.6862f, 189.8189f, 104.3766f);
            ambushBikeLocation = new Vector3(827.392f, -48.30149f, 79.9232f);
            
            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(1000f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Armoured Car requiring escort officer";
            CalloutAdvisory = "An armoured car transport requires a security officer to escort the driver";
            
            Functions.PlayScannerAudio("armoured_car_escort");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            van = new Vehicle(Main.armouredCar, calloutPosition, 146.818f);
            switch (Main.eupMode)
            {
                case true:
                {
                    driver = new Ped("mp_m_freemode_01", calloutPosition, 146.818f);
                    driver.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
                    driver.WarpIntoVehicle(van, -1);
                    break;
                }
                case false:
                {
                    driver = new Ped("s_m_m_armoured_01", calloutPosition, 146.818f);
                    driver.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
                    driver.WarpIntoVehicle(van, -1);
                    break;
                }
            }

            driver.RelationshipGroup = RelationshipGroup.Player;
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Player, RelationshipGroup.Player, Relationship.Companion);

            driver.BlockPermanentEvents = true;
            
            blip = new Blip(driver);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;

            driveStarted = false;
            packageDroppedOff = false;
            endPosReached = false;
            
            option = new Random().Next(0, 4);

            switch (option) 
            {
                case 0:
                {
                    break;
                }
                case 1:
                {
                    ambusher = new Ped(ambushLocation, 109.4391f);
                    ambusher.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);
                    
                    break;
                }
                case 2:
                {
                    ambushBike = new Vehicle("Sanchez", ambushBikeLocation, 64.25911f);
                    biker = new Ped(ambushBikeLocation, 64.25911f);
                    biker.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, -1, true);
                    biker.WarpIntoVehicle(ambushBike, -1);
                    
                    break;
                }
                case 3:
                {
                    ambusher = new Ped(ambushLocation, 109.4391f);
                    ambusher.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);
                    
                    ambushBike = new Vehicle("Sanchez", ambushBikeLocation, 64.25911f);
                    biker = new Ped(ambushBikeLocation, 64.25911f);
                    biker.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, -1, true);
                    biker.WarpIntoVehicle(ambushBike, -1);
                    
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
            
            if (ambusher.Exists())
            {
                if (Game.LocalPlayer.Character.DistanceTo(ambusher) < 30f)
                {
                    ambusher.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
            }
            
            if (biker.Exists())
            {
                if (Game.LocalPlayer.Character.DistanceTo(biker) < 30f)
                {
                    biker.Tasks.ChaseWithGroundVehicle(Game.LocalPlayer.Character);
                    biker.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
            }
            
            if (!driveStarted && Game.LocalPlayer.Character.DistanceTo(van) < 5f)
            {
                Game.DisplayHelp("Get in the passenger seat to start the escort");
            }

            if (!driveStarted)
            {
                if (Game.LocalPlayer.Character.CurrentVehicle == van)
                {
                    driveStarted = true;
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(endPos);
                    driver.Tasks.DriveToPosition(endPos, 10f, VehicleDrivingFlags.Normal, 2f);
                    Game.DisplayHelp("The money is now on the move, keep it safe!");
                    driver.CanBePulledOutOfVehicles = false;
                }
            }
                    
            if (driveStarted && van.DistanceTo(endPos) < 5f && !endPosReached)
            {
                Game.DisplayHelp("Retrieve the package from the rear of the van");

                vanRear = driver.GetOffsetPosition(new Vector3(0.3f, -2f, 0f));
                vanReturn = driver.GetOffsetPosition(new Vector3(3f, 3f, 0f));
                        
                if (blip.Exists())
                    blip.Delete();
                blip = new Blip(vanRear);
                endPosReached = true;
            }

            if (Game.LocalPlayer.Character.DistanceTo(vanRear) <= 1.5f)
            {
                if (blip.Exists())
                    blip.Delete();
                blip = new Blip(dropOff);
                Game.DisplayHelp("Drop off the package inside the bank");
                driver.CanBePulledOutOfVehicles = true;
            }

            if (Game.LocalPlayer.Character.DistanceTo(dropOff) <= 2f)
            {
                if (blip.Exists())
                    blip.Delete();
                blip = new Blip(vanReturn);
                Game.DisplayHelp("Return to the driver to report being finished");
                packageDroppedOff = true;
            }

            if (Game.LocalPlayer.Character.DistanceTo(vanReturn) < 2f && packageDroppedOff)
            {
                End();
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
            
            if (driver.Exists())
                driver.Dismiss();
            if (blip.Exists())
                blip.Delete();
            if (van.Exists())
                van.Dismiss();
            if (ambusher.Exists())
                ambusher.Dismiss();
            if (biker.Exists())
                biker.Dismiss();
            if (ambushBike.Exists())
                ambushBike.Dismiss();
            
            Log.log("Security+ ArmouredCarEscortCasino callout has been cleaned up");
            Game.LogTrivial("Security+ ArmouredCarEscortCasino callout has been cleaned up");
        }
    }
}