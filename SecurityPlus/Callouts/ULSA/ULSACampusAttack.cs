using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.ULSA
{
    [CalloutInfo("ULSACampusAttack", CalloutProbability.Medium)]
    
    public class ULSACampusAttack : Callout
    {
        private Vector3 calloutPosition;
        private Ped attacker;
        private Ped victim1, victim2, victim3;

        private Blip blip;
        private bool convoStarted;

        private int option;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - ULSACampusAttack");
            
            calloutPosition = new Vector3(-1717.736f, 207.1482f, 63.70126f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(100f, calloutPosition);
                AddMaximumDistanceCheck(8200f, calloutPosition); 
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "ULSA Campus has shots fired";
            CalloutAdvisory = "Active attack on campus, security personnel respond";
            
            Functions.PlayScannerAudio("active_shooter_ulsa");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            option = new Random().Next(0, 3);

            switch (option)
            {
                case 0:
                {
                    attacker = new Ped(calloutPosition, 197.5663f);
                    attacker.RelationshipGroup = RelationshipGroup.Gang1;
                    attacker.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);

                    Model[] peds = new Model[]
                    {
                        "a_f_m_bevhills_01", "a_f_y_bevhills_01", "a_f_y_bevhills_02", "a_f_y_clubcust_01", "a_f_y_clubcust_02", "a_f_y_clubcust_03", "a_f_y_hipster_02", "a_f_y_hipster_03", "a_f_y_indian_01", "a_m_m_skater_01", "a_m_y_beachvesp_01", "a_m_y_bevhills_02", "a_m_y_clubcust_01", "a_m_y_hipster_01", "a_m_y_skater_02", "a_m_y_vinewood_01"
                    };
            
                    victim1 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1708.918f, 212.5931f, 62.39138f), 133.7018f);
                    victim2 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1705.194f, 195.6479f, 63.9404f), 50.53364f);
            
                    while (victim2.Model == victim1.Model)
                    {
                        victim2.Delete();
                        victim2 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1705.194f, 195.6479f, 63.9404f), 50.53364f);
                    }
            
                    victim3 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1732.758f, 205.4256f, 64.39848f), 304.8374f);
                    while (victim3.Model == victim1.Model || victim3.Model == victim2.Model)
                    {
                        victim3.Delete();
                        victim3 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1732.758f, 205.4256f, 64.39848f), 304.8374f);
                    }
                    break;
                }
                case 1:
                {
                    calloutPosition = new Vector3(-1584.336f, 238.2772f, 59.04975f);
                    attacker = new Ped(calloutPosition, 76.08547f);
                    attacker.RelationshipGroup = RelationshipGroup.Gang1;
                    attacker.Inventory.GiveNewWeapon(WeaponHash.AssaultShotgun, -1, true);

                    Model[] peds = new Model[]
                    {
                        "a_f_m_bevhills_01", "a_f_y_bevhills_01", "a_f_y_bevhills_02", "a_f_y_clubcust_01", "a_f_y_clubcust_02", "a_f_y_clubcust_03", "a_f_y_hipster_02", "a_f_y_hipster_03", "a_f_y_indian_01", "a_m_m_skater_01", "a_m_y_beachvesp_01", "a_m_y_bevhills_02", "a_m_y_clubcust_01", "a_m_y_hipster_01", "a_m_y_skater_02", "a_m_y_vinewood_01"
                    };
            
                    victim1 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1585.416f, 244.2009f, 59.09281f), 30.42416f);
                    victim2 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1587.105f, 246.9679f, 59.03777f), 208.944f);
            
                    while (victim2.Model == victim1.Model)
                    {
                        victim2.Delete();
                        victim2 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1587.105f, 246.9679f, 59.03777f), 208.944f);
                    }
            
                    victim3 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1593.686f, 223.5465f, 58.933f), 24.98994f);
                    while (victim3.Model == victim1.Model || victim3.Model == victim2.Model)
                    {
                        victim3.Delete();
                        victim3 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1593.686f, 223.5465f, 58.933f), 24.98994f);
                    }
                    break;
                }
                case 2:
                {
                    calloutPosition = new Vector3(-1636.487f, 179.9201f, 61.75728f);
                    attacker = new Ped(calloutPosition, 296.6454f);
                    attacker.RelationshipGroup = RelationshipGroup.Gang1;
                    attacker.Inventory.GiveNewWeapon(WeaponHash.APPistol, -1, true);

                    Model[] peds = new Model[]
                    {
                        "a_f_m_bevhills_01", "a_f_y_bevhills_01", "a_f_y_bevhills_02", "a_f_y_clubcust_01", "a_f_y_clubcust_02", "a_f_y_clubcust_03", "a_f_y_hipster_02", "a_f_y_hipster_03", "a_f_y_indian_01", "a_m_m_skater_01", "a_m_y_beachvesp_01", "a_m_y_bevhills_02", "a_m_y_clubcust_01", "a_m_y_hipster_01", "a_m_y_skater_02", "a_m_y_vinewood_01"
                    };
            
                    victim1 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1633.284f, 184.7695f, 61.30249f), 271.0685f);
                    victim2 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1629.476f, 176.7348f, 61.3021f), 341.7788f);
            
                    while (victim2.Model == victim1.Model)
                    {
                        victim2.Delete();
                        victim2 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1629.476f, 176.7348f, 61.3021f), 341.7788f);
                    }
            
                    victim3 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1625.422f, 177.487f, 60.62566f), 353.0159f);
                    while (victim3.Model == victim1.Model || victim3.Model == victim2.Model)
                    {
                        victim3.Delete();
                        victim3 = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1625.422f, 177.487f, 60.62566f), 353.0159f);
                    }
                    break;
                }
            }
            
            

            victim1.RelationshipGroup = RelationshipGroup.Cop;
            victim2.RelationshipGroup = RelationshipGroup.Cop;
            victim3.RelationshipGroup = RelationshipGroup.Cop;
            
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, RelationshipGroup.Gang1, Relationship.Hate);

            blip = new Blip(calloutPosition);
            blip.Color = Color.Red;
            blip.IsRouteEnabled = true;

            convoStarted = false;
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(attacker) < 10f)
            {
                if (blip.Exists())
                    blip.Delete();
            }
            
            if (Game.LocalPlayer.Character.DistanceTo(attacker) < 50f)
            {
                if (Game.LocalPlayer.Character.DistanceTo(attacker) < 10f)
                {
                    if (blip.Exists())
                        blip.Delete();
                }
                
                if (!convoStarted)
                {
                    convoStarted = true;
                    attacker.Tasks.FightAgainst(victim1).WaitForCompletion(5000);
                    attacker.Tasks.FightAgainst(victim2).WaitForCompletion(5000);
                    attacker.Tasks.FightAgainst(victim3).WaitForCompletion(5000);
                }

                if (Game.IsKeyDown(Main.endKey))
                {
                    End();
                }

                if (attacker.IsDead || attacker.IsCuffed || Game.LocalPlayer.Character.IsDead)
                {
                    End();
                }
            }

            if (Game.IsKeyDown(Main.endKey))
            {
                End();
            }

            if (attacker.IsDead || attacker.IsCuffed || Game.LocalPlayer.Character.IsDead)
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (attacker.Exists())
                attacker.Dismiss();
            if (victim1.Exists())
                victim1.Dismiss();
            if (victim2.Exists())
                victim2.Dismiss();
            if (victim3.Exists())
                victim3.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ ULSACampusAttack has been cleaned up");
            Game.LogTrivial("Security+ ULSACampusAttack has been cleaned up");
        }
    }
}