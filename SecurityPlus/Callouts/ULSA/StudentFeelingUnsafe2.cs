using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.ULSA
{
    // A ULSA Student believes they may be being stalked, and has called security
    [CalloutInfo("StudentFeelingUnsafe2", CalloutProbability.High)]
    
    public class StudentFeelingUnsafe2 : Callout
    {
        private Ped student, stalker;
        private Vector3 calloutPosition, stalkerPosition;
        private Blip blip;

        private bool convoStarted;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - StudentFeelingUnsafe2");
            
            calloutPosition = new Vector3(-1749.673f, 234.6256f, 64.44878f);
            stalkerPosition = new Vector3(-1749.671f, 230.3943f, 64.44385f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(100f, calloutPosition);
                AddMaximumDistanceCheck(8200f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "ULSA Campus security requested by student";
            CalloutAdvisory = "Student feeling unsafe, requesting security";
            
            Functions.PlayScannerAudio("student_feeling_unsafe");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            Model[] peds = new Model[]
            {
                "a_f_m_bevhills_01", "a_f_y_bevhills_01", "a_f_y_bevhills_02", "a_f_y_clubcust_01", "a_f_y_clubcust_02", "a_f_y_clubcust_03", "a_f_y_hipster_02", "a_f_y_hipster_03", "a_f_y_indian_01", "a_m_m_skater_01", "a_m_y_beachvesp_01", "a_m_y_bevhills_02", "a_m_y_clubcust_01", "a_m_y_hipster_01", "a_m_y_skater_02", "a_m_y_vinewood_01"
            };
            
            student = new Ped(peds[new Random().Next(peds.Length)], calloutPosition, 129.1711f);

            stalker = new Ped(stalkerPosition, 6.689859f);

            stalker.Metadata.searchPed = "~r~Photos of a student~s~";
            
            blip = new Blip(calloutPosition);
            blip.IsRouteEnabled = true;
            blip.Color = Color.Red;
            convoStarted = false;

            stalker.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(student) < 10f)
            {
                if (Game.IsKeyDown(Main.endKey))
                {
                    End();
                }

                if (!convoStarted)
                {
                    stalker.Tasks.FightAgainst(student);
                    if (blip.Exists())
                    {
                        blip.Delete();
                    }

                    convoStarted = true;
                }
            }

            if (Game.LocalPlayer.Character.DistanceTo(stalker) < 5f && convoStarted)
            {
                stalker.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Game.DisplaySubtitle("Stalker: You'll never take me alive!");
            }

            if (stalker.IsDead || stalker.IsCuffed || Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (stalker.Exists())
                stalker.Dismiss();
            if (student.Exists())
                student.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ StudentFeelingUnsafe2 has been cleaned up");
            Game.LogTrivial("Security+ StudentFeelingUnsafe2 has been cleaned up");
        }
    }
}