using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.Sandy_Shores
{
    [CalloutInfo("SheriffRequestingSecurity", CalloutProbability.VeryHigh)]
    
    /*
     * This is a weird one, essentially theres a rebellion happening in sandy shores,
     * and the sheriff is overwhelmed so they deputised the security officers nearby.
     */
    public class SheriffRequestingSecurity : Callout
    {
        private RelationshipGroup foes, friends;
        private Blip blip;
        
        private Vehicle blockadeVehicle1;
        private Vehicle blockadeVehicle2;
        private Vehicle blockadeVehicle3;
        private Vehicle blockadeVehicle4;

        private Vehicle sheriffBackupCar, sheriffCar, trooperCar;

        private Ped sheriffBackup, sheriff, trooper;

        private Ped hostage;

        private Ped blockadeDriver1,
            blockadeDriver2,
            blockadeDriver3,
            blockadeDriver4,
            blockadeGunner1,
            blockadeGunner2,
            blockadeGunner3,
            blockadeGunner4,
            suspect1,
            suspect2,
            suspect3,
            suspect4,
            suspect5,
            suspect6;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - SheriffRequestingSecurity");
            
            Vector3 spawn = new Vector3(1805.143f, 3279.407f, 42.83224f);

            CalloutPosition = spawn;
            ShowCalloutAreaBlipBeforeAccepting(spawn, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(100f, spawn);
                AddMaximumDistanceCheck(8200f, spawn);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Insurgents! County sheriff calling on all security personnel";
            CalloutAdvisory = "County sheriff advises that all responding security personnel are hearby deputised.";
            
            Functions.PlayScannerAudio("insurgency_sandy_air");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            Game.DisplayNotification(
                "BCSO County Sheriff advises all deputies to respond code 3 for an active insurgency");
            
            // Blockade setup
            blockadeVehicle1 = new Vehicle("vetir", new Vector3(1761.82f, 3293.916f, 41.4470f), 334.7083f);
            blockadeVehicle2 = new Vehicle("guardian", new Vector3(1759.155f, 3287.549f, 41.41528f), 173.8649f);
            blockadeVehicle3 = new Vehicle("insurgent", new Vector3(1757.205f, 3281.469f, 41.03519f), 158.8386f);
            blockadeVehicle4 = new Vehicle("insurgent", new Vector3(1760.19f, 3266.562f, 41.06341f), 17.28463f);

            blockadeDriver1 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            blockadeDriver2 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            blockadeDriver3 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            blockadeDriver4 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            
            blockadeGunner1 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            blockadeGunner2 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            blockadeGunner3 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            blockadeGunner4 = new Ped(new Vector3(1761.82f, 3293.916f, 41.4470f), 180f);
            
            blockadeGunner1.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
            blockadeGunner2.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
            blockadeGunner3.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
            blockadeGunner4.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
            
            blockadeDriver1.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);
            blockadeDriver2.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
            blockadeDriver3.Inventory.GiveNewWeapon(WeaponHash.AssaultSMG, -1, true);
            blockadeDriver4.Inventory.GiveNewWeapon(WeaponHash.BullpupShotgun, -1, true);

            blockadeDriver1.IsPersistent = true;
            //blockadeDriver1.BlockPermanentEvents = true;
            blockadeDriver2.IsPersistent = true;
            //blockadeDriver2.BlockPermanentEvents = true;
            blockadeDriver3.IsPersistent = true;
           //blockadeDriver3.BlockPermanentEvents = true;
            blockadeDriver4.IsPersistent = true;
            //blockadeDriver4.BlockPermanentEvents = true;
            
            blockadeDriver1.WarpIntoVehicle(blockadeVehicle1, -1);
            blockadeDriver2.WarpIntoVehicle(blockadeVehicle2, -1);
            blockadeDriver3.WarpIntoVehicle(blockadeVehicle3, -1);
            blockadeDriver4.WarpIntoVehicle(blockadeVehicle4, -1);
            
            blockadeGunner1.WarpIntoVehicle(blockadeVehicle1, 0);
            blockadeGunner2.WarpIntoVehicle(blockadeVehicle2, 0);
            blockadeGunner3.WarpIntoVehicle(blockadeVehicle3, 7);
            blockadeGunner4.WarpIntoVehicle(blockadeVehicle4, 7);

            // Center suspects setup
            
            suspect1 = new Ped("s_m_y_blackops_01", new Vector3(1741.128f, 3288.116f, 41.10332f), 250.547f); 
            suspect2 = new Ped("s_m_y_blackops_02", new Vector3(1740.404f, 3286.234f, 41.10303f), 244.4426f); 
            suspect3 = new Ped("s_m_y_blackops_03", new Vector3(1738.943f, 3285.225f, 41.11918f), 242.815f); 
            suspect4 = new Ped("ig_terry", new Vector3(1737.878f, 3283.039f, 41.12503f), 205.7952f); 
            suspect5 = new Ped("mp_m_weapexp_01", new Vector3(1736.614f, 3282.045f, 41.12038f), 224.0644f); 
            suspect6 = new Ped("csb_mweather", new Vector3(1736.541f, 3280.385f, 41.11689f), 263.5432f);

            suspect1.Inventory.GiveNewWeapon(WeaponHash.AdvancedRifle, -1, true);
            suspect2.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
            suspect3.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);
            suspect4.Inventory.GiveNewWeapon(WeaponHash.AssaultShotgun, -1, true);
            suspect5.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
            suspect6.Inventory.GiveNewWeapon(WeaponHash.HeavySniper, -1, true);

            suspect1.IsPersistent = true;
            //suspect1.BlockPermanentEvents = true;
            suspect2.IsPersistent = true;
            //suspect2.BlockPermanentEvents = true;
            suspect3.IsPersistent = true;
            //suspect3.BlockPermanentEvents = true;
            suspect4.IsPersistent = true;
            //suspect4.BlockPermanentEvents = true;
            suspect5.IsPersistent = true;
            //suspect5.BlockPermanentEvents = true;
            suspect6.IsPersistent = true;
            //suspect6.BlockPermanentEvents = true;
            
            // Suspect setup finished, begin leo setup

            sheriffBackupCar = new Vehicle("SHERIFF2", new Vector3(1792.024f, 3286.586f, 41.74596f), 209.2175f);
            sheriffCar = new Vehicle("SHERIFF", new Vector3(1794.131f, 3281.602f, 41.98149f), 188.2383f);
            trooperCar = new Vehicle("POLICE4", new Vector3(1796.409f, 3276.614f, 42.08496f),  212.032f);

            sheriffBackup = new Ped("s_f_y_sheriff_01", new Vector3(1792.264f, 3288.945f, 42.23204f), 28.84984f);
            sheriff = new Ped("s_m_y_sheriff_01", new Vector3(1795.173f, 3283.192f, 42.42078f), 7.411717f);
            trooper = new Ped("s_m_y_hwaycop_01", new Vector3(1797.512f, 3277.288f, 42.49274f), 31.39828f);

            sheriffBackup.Inventory.GiveNewWeapon(WeaponHash.PumpShotgun, -1, true);
            sheriff.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, true);
            trooper.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);

            sheriffBackup.IsPersistent = true;
            //sheriffBackup.BlockPermanentEvents = true;
            sheriff.IsPersistent = true;
            //sheriff.BlockPermanentEvents = true;
            trooper.IsPersistent = true;
            //trooper.BlockPermanentEvents = true;

            sheriffBackup.Tasks.TakeCoverFrom(suspect1, 10000000);
            sheriff.Tasks.TakeCoverFrom(suspect1, 10000000);
            trooper.Tasks.TakeCoverFrom(suspect1, 10000000);

            blockadeDriver1.RelationshipGroup = foes;
            blockadeDriver2.RelationshipGroup = foes;
            blockadeDriver3.RelationshipGroup = foes;
            blockadeDriver4.RelationshipGroup = foes;
            blockadeGunner1.RelationshipGroup = foes;
            blockadeGunner2.RelationshipGroup = foes;
            blockadeGunner3.RelationshipGroup = foes;
            blockadeGunner4.RelationshipGroup = foes;

            suspect1.RelationshipGroup = foes;
            suspect2.RelationshipGroup = foes;
            suspect3.RelationshipGroup = foes;
            suspect4.RelationshipGroup = foes;
            suspect5.RelationshipGroup = foes;
            suspect6.RelationshipGroup = foes;

            sheriffBackup.RelationshipGroup = friends;
            sheriff.RelationshipGroup = friends;
            trooper.RelationshipGroup = friends;

            hostage = new Ped(suspect1.GetOffsetPositionFront(5f));
            hostage.RelationshipGroup = friends;
            
            Game.LocalPlayer.Character.RelationshipGroup = friends;
            
            Game.SetRelationshipBetweenRelationshipGroups(friends, foes, Relationship.Hate);

            blip = new Blip(CalloutPosition);
            blip.Color = Color.Red;
            blip.IsRouteEnabled = true;
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(CalloutPosition) < 10f)
            {   
                if (blip.Exists())
                    blip.Delete();

                suspect2.Tasks.FightAgainst(hostage);
                blockadeGunner1.Tasks.FightAgainst(sheriffBackup);
                blockadeGunner2.Tasks.FightAgainst(trooper);
                suspect1.Tasks.FireWeaponAt(Game.LocalPlayer.Character, 30000, FiringPattern.FullAutomatic);
            }

            if (blockadeDriver1.IsDead && blockadeDriver2.IsDead && blockadeDriver3.IsDead && blockadeDriver4.IsDead && blockadeGunner1.IsDead &&
                blockadeGunner2.IsDead && blockadeGunner3.IsDead && blockadeGunner4.IsDead && suspect1.IsDead && suspect2.IsDead && suspect3.IsDead && suspect4.IsDead &&
                suspect5.IsDead && suspect6.IsDead)
            {
                End();
            }

            if (Game.IsKeyDown(Main.endKey))
            {
                End();
            }
            
            if (Game.LocalPlayer.Character.IsDead)
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (blockadeDriver1.Exists())
                blockadeDriver1.Dismiss();
            if (blockadeDriver2.Exists())
                blockadeDriver2.Dismiss();
            if (blockadeDriver3.Exists())
                blockadeDriver3.Dismiss();
            if (blockadeDriver4.Exists())
                blockadeDriver4.Dismiss();
            
            if (blockadeGunner1.Exists())
                blockadeGunner1.Dismiss();
            if (blockadeGunner2.Exists())
                blockadeGunner2.Dismiss();
            if (blockadeGunner3.Exists())
                blockadeGunner3.Dismiss();
            if (blockadeGunner4.Exists())
                blockadeGunner4.Dismiss();
            
            if (suspect1.Exists())
                suspect1.Dismiss();
            if (suspect2.Exists())
                suspect2.Dismiss();
            if (suspect3.Exists())
                suspect3.Dismiss();
            if (suspect4.Exists())
                suspect4.Dismiss();
            if (suspect5.Exists())
                suspect5.Dismiss();
            if (suspect6.Exists())
                suspect6.Dismiss();
            
            if (blockadeVehicle1.Exists())
                blockadeVehicle1.Dismiss();
            if (blockadeVehicle2.Exists())
                blockadeVehicle2.Dismiss();
            if (blockadeVehicle3.Exists())
                blockadeVehicle3.Dismiss();
            if (blockadeVehicle4.Exists())
                blockadeVehicle4.Dismiss();
            
            if (sheriffCar.Exists())
                sheriffCar.Dismiss();
            if (sheriffBackupCar.Exists())
                sheriffBackupCar.Dismiss();
            if (trooperCar.Exists())
                trooperCar.Dismiss();
            
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ SheriffRequestingSecurity callout has been cleaned up");
            Game.LogTrivial("Security+ SheriffRequestingSecurity callout has been cleaned up");
        }
    }
}