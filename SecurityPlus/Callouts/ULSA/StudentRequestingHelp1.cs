using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.ULSA
{
    // A ULSA Student was recently mugged and called security when the mugger left
    [CalloutInfo("StudentRequestingHelp1", CalloutProbability.High)]
    
    public class StudentRequestingHelp1 : Callout
    {
        private Ped student, mugger;
        private Vector3 calloutPosition;
        private Blip blip;

        private int counter;

        private Vector3[] points1;
        private Vector3[] points2;
        private Vector3[] points3;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - StudentRequestingHelp1");
            
            calloutPosition = new Vector3(-1720.049f, 222.7599f, 61.73586f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(100f, calloutPosition);
                AddMaximumDistanceCheck(8200f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "ULSA Campus security requested by student";
            CalloutAdvisory = "Student in distress, requesting security";
            
            Functions.PlayScannerAudio("student_in_distress");
            
            points1 = new[]
            {
                new Vector3(-1717.524f, 225.9037f, 62.39096f), new Vector3(-1712.815f, 215.7224f, 62.39099f), new Vector3(-1701.651f, 213.5216f, 62.39133f), new Vector3(-1700.331f, 211.7621f, 62.39096f), new Vector3(-1689.547f, 216.5182f, 62.39095f), new Vector3(-1677.276f, 221.308f, 62.39098f), new Vector3(-1665.985f, 226.7164f, 62.39098f), new Vector3(-1655.642f, 231.78f, 62.39098f)
            };

            points2 = new[]
            {
                new Vector3(-1644.606f, 237.202f, 62.39096f), new Vector3(-1641.134f, 231.0802f, 60.64111f), new Vector3(-1635.11f, 233.8633f, 60.40915f), new Vector3(-1627.718f, 237.6943f, 59.87552f), new Vector3(-1623.107f, 235.2076f, 59.97356f), new Vector3(-1617.905f, 232.2426f, 59.99762f), new Vector3(-1612.552f, 229.1401f, 59.77057f), new Vector3(-1606.412f, 225.8817f, 59.37472f)
            };

            points3 = new[]
            {
                new Vector3(-1602.41f, 224.1381f, 59.21915f), new Vector3(-1595.708f, 226.1927f, 59.00928f), new Vector3(-1588.528f, 228.7584f, 58.78208f), new Vector3(-1579.477f, 232.8766f, 58.67508f), new Vector3(-1576.046f, 234.7078f, 58.76485f), new Vector3(-1574.06f, 232.299f, 58.85237f)
            };
            
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

            student = new Ped(peds[new Random().Next(peds.Length)], calloutPosition, 309.635f);

            blip = new Blip(calloutPosition);
            blip.IsRouteEnabled = true;
            blip.Color = Color.Red;

            mugger = new Ped(new Vector3(-1684.594f, 269.4893f, 62.39095f), 180f);
            mugger.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);
            mugger.Metadata.searchPed = "~r~Stolen laptop~s~";

            mugger.Tasks.GoStraightToPosition(new Vector3(-1618.73f, 389.3347f, 86.41787f), 0.6f, 49f, 10f, 90000);

            counter = 0;
            
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
                
                if (blip.Exists())
                    blip.Delete();
                
                Game.DisplaySubtitle("Student: Thank you for coming, I wasn't sure what to do.");
                Game.DisplayHelp("Press 'Y' to advance the conversation");

                if (Game.IsKeyDown(Keys.Y))
                {
                    counter++;
                }
                
                if (counter == 1)
                    Game.DisplaySubtitle(Main.Name + ": Take a deep breath, and tell me what happened.");
                else if (counter == 2)
                    Game.DisplaySubtitle("Student: Okay, thank you. I was walking back to my dorm from class, when out of nowhere someone jumps out with a knife. They told me to give them my bag, it had my laptop in it. I gave it to them and they took off running towards the hills.");
                else if (counter == 3)
                    Game.DisplaySubtitle(Main.Name + ": Alright, are you hurt at all?");
                else if (counter == 4)
                    Game.DisplaySubtitle("Student: I think I'm okay, could you please walk with me back to my building?");
                else if (counter == 5)
                {
                    Game.DisplaySubtitle("Guard: Of course, I'm here to help.");
                    student.Tasks.FollowPointRoute(points1, 0.6f).WaitForCompletion();
                    student.Tasks.FollowPointRoute(points2, 0.6f).WaitForCompletion();
                    student.Tasks.FollowPointRoute(points3, 0.6f).WaitForCompletion();
                }
                
            }

            if (student.DistanceTo(new Vector3(-1574.06f, 232.299f, 58.85237f)) < 3f)
            {
                Game.DisplaySubtitle("Student: Thank you!");
                student.IsVisible = false;
                if (student.Exists())
                    student.Delete();
                End();
            }

            if (student.IsDead || student.IsCuffed || Game.LocalPlayer.Character.IsDead || Game.IsKeyDown(Main.endKey))
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (mugger.Exists())
                mugger.Dismiss();
            if (student.Exists())
                student.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ StudentRequestingHelp1 has been cleaned up");
            Game.LogTrivial("Security+ StudentRequestingHelp1 has been cleaned up");
        }
    }
}