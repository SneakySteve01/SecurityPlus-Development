using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.ULSA
{
    // A ULSA Student believes they may be being stalked, and has called security
    [CalloutInfo("StudentFeelingUnsafe1", CalloutProbability.High)]
    
    public class StudentFeelingUnsafe1 : Callout
    {
        private Ped student, stalker, cop;
        private Vector3 calloutPosition, stalkerPosition, dormPosition, copPosition;
        private Blip blip, posBlip;

        private int counter;

        private bool iPressed, oPressed;
        private bool convoStarted;

        private Vector3[] points1;
        private Vector3[] points2;
        private Vector3[] points3;

        private Vehicle copCar;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - StudentFeelingUnsafe1");
            
            calloutPosition = new Vector3(-1669.694f, 170.3684f, 61.75383f);
            stalkerPosition = new Vector3(-1671.677f, 133.9299f, 63.34041f);
            dormPosition = new Vector3(-1719.95f, 233.8827f, 62.39095f);
            copPosition = new Vector3(-1668.477f, 128.1322f, 62.64659f);

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

            points1 = new[]
            {
                new Vector3(-1671.213f, 169.8456f, 61.75514f), new Vector3(-1673.533f, 172.3852f, 61.75632f), new Vector3(-1676.396f, 171.0068f, 62.2124f), new Vector3(-1678.528f, 169.9848f, 62.78191f), new Vector3(-1682.591f, 168.0917f, 63.5528f), new Vector3(-1683.98f, 169.3979f, 63.61052f), new Vector3(-1685.727f, 172.1477f, 63.69289f), new Vector3(-1688.938f, 177.6257f, 63.85496f)
            };
            points2 = new[]
            {
                new Vector3(-1693.389f, 184.2922f, 63.94279f), new Vector3(-1698.327f, 195.4983f, 63.80468f), new Vector3(-1701.479f, 203.3434f, 63.70896f), new Vector3(-1703.848f, 208.4463f, 62.39154f), new Vector3(-1705.374f, 209.9845f, 62.39136f), new Vector3(-1710.154f, 211.3189f, 62.39136f), new Vector3(-1712.643f, 215.4823f, 62.391f), new Vector3(-1719.04f, 227.9399f, 62.39095f)
            };
            points3 = new[]
            {
                new Vector3(-1721.075f, 231.2057f, 62.39095f), new Vector3(-1719.917f, 233.8028f, 62.39095f)
            };
            
            student = new Ped(peds[new Random().Next(peds.Length)], calloutPosition, 107.8853f);

            stalker = new Ped(stalkerPosition, 339.4058f);

            stalker.Metadata.searchPed = "~r~Photos of a student~s~";
            
            blip = new Blip(calloutPosition);
            blip.IsRouteEnabled = true;
            blip.Color = Color.Red;

            counter = 0;
            iPressed = false;
            oPressed = false;
            convoStarted = false;
            
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

                if (Game.IsKeyDown(Keys.Y))
                {
                    counter++;
                }

                if (!convoStarted)
                {
                    Game.DisplaySubtitle("Student: Thanks for showing up, I've been getting this erie feeling like someone is following me, and I think I may have saw someone at one point.");
                    Game.DisplayHelp("Press 'Y' to advance the conversation");
                    if (blip.Exists())
                    {
                        blip.Delete();
                    }
                }

                if (counter == 1)
                {
                    Game.DisplaySubtitle(Main.Name + ": Alright, try to keep calm, you're safe now. Are they still nearby?");
                    convoStarted = true;
                }
                else if (counter == 2)
                {
                    Game.DisplaySubtitle("Student: I think so, they were over that way.");
                    posBlip = new Blip(stalkerPosition);
                    posBlip.Alpha = 50f;
                    posBlip.Scale = 4f;
                    
                    Game.DisplayHelp("Press 'I' to look for the stalker, or 'O' to escort the student home and call police for the stalker.");
                    counter++;
                }

                if (Game.IsKeyDown(Keys.I) && !iPressed && !oPressed)
                {
                    iPressed = true;

                    if (blip.Exists())
                    {
                        blip.Delete();
                    }
                    
                    Game.DisplaySubtitle(Main.Name + ": I'm going to go check it out and try to find them, you hurry back to your dorm, okay?");
                    Game.DisplayHelp("Search for the stalker");
                    stalker.Tasks.Wander();
                    stalker.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, false);
                    student.Tasks.FollowPointRoute(points1, 0.7f).WaitForCompletion();
                    if (student.Exists())
                        student.Delete();
                }

                if (Game.IsKeyDown(Keys.O) && !oPressed && !iPressed)
                {
                    oPressed = true;
                    if (posBlip.Exists())
                    {
                        posBlip.Delete();
                    }
                    Model[] peds = new Model[]
                    {
                        "s_f_y_cop_01", "s_m_y_cop_01"
                    };
                    
                    copCar = new Vehicle("POLICE", new Vector3(-1614.024f, 156.8796f, 59.59328f), 112.4772f);
                    cop = new Ped(peds[new Random().Next(peds.Length)], new Vector3(-1614.024f, 156.8796f, 59.59328f), 112.4772f);
                    cop.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
                    Functions.SetPedAsCop(cop);
                    Functions.SetCopAsBusy(cop, true);

                    cop.WarpIntoVehicle(copCar, -1);

                    copCar.IsSirenOn = true;
                    
                    Game.DisplaySubtitle(Main.Name + ": We'll let the police handle that person, for now I'll accompany you back to your building.");
                    
                    cop.Tasks.DriveToPosition(copCar,copPosition, 20f, VehicleDrivingFlags.Emergency, 2f).WaitForCompletion();
                    copCar.IsSirenSilent = true;
                    Functions.StartPedArrestPed(cop, stalker, true);
                    
                    
                    student.Tasks.FollowPointRoute(points1, 1f).WaitForCompletion();
                    Functions.RequestSuspectTransport(stalker); // replace with cop who arrived acting as transport
                    student.Tasks.FollowPointRoute(points2, 1f).WaitForCompletion();
                    student.Tasks.FollowPointRoute(points3, 1f).WaitForCompletion();
                    if (stalker.Exists())
                    {
                        stalker.Delete();
                    }
                }
            }

            if (student.DistanceTo(new Vector3(-1719.917f, 233.8028f, 62.39095f)) < 3f)
            {
                if (oPressed)
                {
                    Game.DisplaySubtitle("Student: Thank you!");
                }
                student.IsVisible = false;
                if (student.Exists())
                    student.Delete();
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(stalker) < 10f && convoStarted)
            {
                stalker.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Game.DisplaySubtitle("Stalker: You'll never take me alive!");
                if (posBlip.Exists())
                    posBlip.Delete();
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
            
            if (cop.Exists())
                cop.Dismiss();
            if (copCar.Exists())
                copCar.Dismiss();
            if (posBlip.Exists())
                posBlip.Delete();
            if (stalker.Exists())
                stalker.Dismiss();
            if (student.Exists())
                student.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ StudentFeelingUnsafe1 has been cleaned up");
            Game.LogTrivial("Security+ StudentFeelingUnsafe1 has been cleaned up");
        }
    }
}