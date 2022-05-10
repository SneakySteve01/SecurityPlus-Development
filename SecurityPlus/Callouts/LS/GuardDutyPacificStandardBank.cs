using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.LS
{
    [CalloutInfo("GuardDutyPacificStandardBank", CalloutProbability.High)]
    
    public class GuardDutyPacificStandardBank : Callout
    {
        private Vector3 calloutPosition;
        private Vector3 sus1Pos, sus2Pos, sus3Pos;
        private Vector3 checkPos1, checkPos2, checkPos3, checkPos4, checkPos5;

        private int option;

        private bool shiftStarted, shiftDone;

        private Blip blip;

        private Ped suspect1, suspect2, suspect3;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            sus1Pos = new Vector3(236.1822f, 229.7518f, 110.2781f);
            sus2Pos = new Vector3(263.7065f, 216.2404f, 110.283f);
            sus3Pos = new Vector3(265.7032f, 221.4441f, 101.6833f);
            checkPos1 = new Vector3(235.6463f, 223.5061f, 110.2827f);
            checkPos2 = new Vector3(260.0694f, 211.4278f, 110.283f);
            checkPos3 = new Vector3(252.2879f, 226.2408f, 106.2869f);
            checkPos4 = new Vector3(260.23f, 225.1068f, 101.6833f);
            checkPos5 = new Vector3(262.7754f, 212.5688f, 106.2832f);
            
            calloutPosition = new Vector3(229.7702f, 214.4306f, 105.5555f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            CalloutMessage = "Security required for guard duty";
            CalloutAdvisory = "A shift of guard duty requires a security officer";
            
            Functions.PlayScannerAudio("guard_duty");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            option = new Random().Next(0, 7);

            blip = new Blip(calloutPosition);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;
            shiftStarted = false;
            shiftDone = false;
            
            switch (option)
            {
                case 0:
                {
                    suspect1 = new Ped(sus1Pos, 267.3041f);
                    suspect1.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
                    
                    break;
                }
                case 1:
                {
                    suspect2 = new Ped(sus2Pos, 65.41304f);
                    suspect2.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
                    
                    break;
                }
                case 2:
                {
                    suspect1 = new Ped(sus1Pos, 267.3041f);
                    suspect1.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
                    suspect2 = new Ped(sus2Pos, 65.41304f);
                    suspect2.Inventory.GiveNewWeapon(WeaponHash.AssaultRifle, -1, true);
                    suspect3 = new Ped(sus3Pos, 53.8764f);
                    suspect3.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, true);
                    
                    break;
                }
                case 3:
                {
                    suspect3 = new Ped(sus3Pos, 53.8764f);
                    suspect3.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, true);
                    
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
                case 6:
                {
                    
                    break;
                }
            }
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!shiftStarted && Game.LocalPlayer.Character.DistanceTo(calloutPosition) < 4f)
            {
                Game.DisplayNotification(Main.UID + ": Attention dispatch, I'm on scene and beginning my rounds.");
                shiftStarted = true;
                Game.DisplayHelp("Start by checking on the second level");
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

                if (suspect3.Exists())
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect3) < 5f)
                    {
                        suspect3.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
                
                if (Game.LocalPlayer.Character.DistanceTo(checkPos1) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(checkPos2);
                    Game.DisplayHelp("Now check on the offices");
                }

                if (Game.LocalPlayer.Character.DistanceTo(checkPos2) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(checkPos3);
                    Game.DisplayHelp("Now check behind the counter");
                }

                if (Game.LocalPlayer.Character.DistanceTo(checkPos3) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(checkPos4);
                    Game.DisplayHelp("Now check on the vault");
                }
                
                if (Game.LocalPlayer.Character.DistanceTo(checkPos4) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(checkPos5);
                    Game.DisplayHelp("Now check on the lobby");
                }
                
                if (Game.LocalPlayer.Character.DistanceTo(checkPos5) < 2f)
                {
                    if (blip.Exists())
                        blip.Delete();
                    blip = new Blip(calloutPosition);
                    Game.DisplayHelp("Return to the bank entrance to end your shift");
                    shiftDone = true;
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
        }

        public override void End()
        {
            base.End();
            
            if (blip.Exists())
                blip.Delete();
            if (suspect1.Exists())
                suspect1.Dismiss();
            if (suspect2.Exists())
                suspect2.Dismiss();
            if (suspect3.Exists())
                suspect3.Dismiss();
            
            Game.LogTrivial("Security+ GuardDutyPacificStandardBank callout has been cleaned up");
        }
    }
}