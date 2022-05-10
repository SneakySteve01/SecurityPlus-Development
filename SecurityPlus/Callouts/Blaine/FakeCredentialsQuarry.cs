using System;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.Blaine
{
    [CalloutInfo("FakeCredentialsQuarry", CalloutProbability.High)]
    
    public class FakeCredentialsQuarry : Callout
    {
        private Vector3 calloutPosition;

        private Ped gatePed, suspect, sheriff, sheriff2;

        private Blip blip;

        private Vehicle susCar, sheriffCar;

        private int counter, option;

        private bool convoStarted, optionTime, iPressed, oPressed;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - FakeCredentialsQuarry");
            
            calloutPosition = new Vector3(2556.833f, 2698.774f, 41.58325f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Fake Credentials at a gate";
            CalloutAdvisory = "Security requested for someone attempting to access a facility using fake credentials.";
            
            Functions.PlayScannerAudio("fake_credentials");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            Model[] cars = new Model[]
            {
                "cavalcade", "baller", "granger", "asea", "dubsta", "fq2", "gresley", "habanero", "landstalker", "mesa", "asterope"
            };
            
            suspect = new Ped(calloutPosition, 338.1547f);
            susCar = new Vehicle(cars[new Random().Next(cars.Length)], calloutPosition, 338.1547f);

            suspect.WarpIntoVehicle(susCar, -1);
            
            gatePed = new Ped("IG_TRAFFICWARDEN", new Vector3(2563.236f, 2705.186f, 41.87234f), 116.9832f);

            blip = new Blip(suspect);
            blip.Color = Color.Yellow;
            blip.IsRouteEnabled = true;

            Random random = new Random();
            int i = random.Next(0, 4);

            if (i == 0)
                suspect.Inventory.GiveNewWeapon(WeaponHash.Pistol, -1, true);
            else if (i == 1)
                suspect.Inventory.GiveNewWeapon(WeaponHash.Smg, -1, true);
            else if (i == 2)
                suspect.Inventory.GiveNewWeapon(WeaponHash.Knife, -1, true);

            counter = 0;
            convoStarted = false;
            optionTime = false;
            iPressed = false;
            oPressed = false;
            
            option = new Random().Next(0, 2);
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            switch (option)
            {
                case 0:
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect) < 7f)
                    {

                        if (!convoStarted)
                        {
                            Game.DisplayHelp("Press 'Y' to advance conversation");
                            if (blip.Exists())
                                blip.Delete();
                            Game.DisplaySubtitle("Gate Operator: Hey boss, thanks for coming.");
                            convoStarted = true;
                        }
                        
                        if (Game.IsKeyDown(Keys.Y))
                        {
                            counter++;
                        }

                        if (counter == 1)
                        {
                            Game.DisplaySubtitle(Main.Name + ": No problem, what's going on?");
                        }

                        if (counter == 2)
                        {
                            Game.DisplaySubtitle("Gate Operator: This individual in the vehicle attempted to access the facility using a fake card.");
                        }

                        if (counter == 3)
                        {
                            Game.DisplaySubtitle(Main.Name + ": And you're sure it's a fake? The system's not just acting up again?");
                        }

                        if (counter == 4)
                        {
                            Game.DisplaySubtitle("Gate Operator: It's not an up-to-date card, we haven't used the template it has in years.");
                        }

                        if (counter == 5)
                        {
                            Game.DisplaySubtitle(Main.Name + ": Alright then, have they been cooperative?");
                        }

                        if (counter == 6)
                        {
                            Game.DisplaySubtitle("Gate Operator: So far so good.");
                        }

                        if (counter == 7)
                        {
                            Game.DisplaySubtitle(Main.Name + ": Okay, I'll have a chat with them.");
                            Game.DisplayHelp("Speak with the suspect.");
                        }

                        if (counter == 8)
                        {
                            Game.DisplaySubtitle(Main.Name + ": Good day, would you mind telling me why it is you're trying to access the property?");
                        }

                        if (counter == 9)
                        {
                            Game.DisplaySubtitle("Suspect: Uh.., I was just wanting to explore it a little.");
                        }

                        if (counter == 10)
                        {
                            Game.DisplaySubtitle(Main.Name + ": Sorry, but people can't just come in for a look around like this, you need to try and book an official tour via the website.");
                        }

                        if (counter == 11)
                        {
                            Game.DisplaySubtitle("Suspect: Oh, okay then. In that case can I get going?");
                        }

                        if (counter == 12)
                        {
                            Game.DisplaySubtitle(Main.Name + ": We've determined that the card you were using is a fake, we're going to have to detain you and wait on law enforcement's arrival. Please step out of the vehicle.");
                        }

                        if (counter == 13)
                        {
                            Game.DisplaySubtitle("Suspect: Uh... well.. okay.");
                            suspect.Tasks.LeaveVehicle(susCar, LeaveVehicleFlags.None);
                            Game.DisplayHelp("Press 'I' to call 911 or 'O' to let the suspect go.");
                            optionTime = true;
                        }
                    }

                    if (optionTime)
                    {
                        if (Game.IsKeyDown(Keys.I) && !iPressed)
                        {
                            iPressed = true;
                            Game.DisplayNotification("Security Dispatcher: " + Main.UID + " be advised, 911 is sending a sheriff unit. Please extend to them your full cooperation.");

                            sheriff = new Ped("s_f_y_sheriff_01", new Vector3(2457.868f, 2831.714f, 48.08223f), 222.1868f);
                            sheriff2 = new Ped("s_m_y_sheriff_01", new Vector3(2457.868f, 2831.714f, 48.08223f), 222.1868f);
                            sheriffCar = new Vehicle("sheriff", new Vector3(2457.868f, 2831.714f, 48.08223f), 222.1868f);

                            sheriff.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
                            sheriff2.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, true);
                            
                            sheriff.WarpIntoVehicle(sheriffCar, -1);
                            sheriff2.WarpIntoVehicle(sheriffCar, 0);

                            sheriffCar.IsSirenOn = true;
                            
                            sheriff.Tasks.DriveToPosition(sheriffCar, new Vector3(2538.519f, 2697.773f, 41.74892f), 15f, VehicleDrivingFlags.Emergency, 2f).WaitForCompletion();
                            sheriffCar.IsSirenSilent = true;
                            
                            Game.DisplaySubtitle("Sheriff: Step back guard, we'll take it from here.");
                            sheriff.Tasks.LeaveVehicle(sheriffCar, LeaveVehicleFlags.None).WaitForCompletion();
                            sheriff.Tasks.GoToWhileAiming(suspect, suspect, 2f, 2.5f, false, FiringPattern.FullAutomatic).WaitForCompletion();
                            suspect.Tasks.PutHandsUp(2000, sheriff).WaitForCompletion();
                            Functions.SetPedCuffedTask(suspect, true);

                            if (suspect.IsCuffed)
                            {
                                suspect.Tasks.ClearImmediately();
                                sheriff.Tasks.ClearImmediately();
                                sheriff.Tasks.EnterVehicle(sheriffCar, -1).WaitForCompletion(6000);
                                if (!sheriff.IsInVehicle(sheriffCar, false))
                                {
                                    sheriff.WarpIntoVehicle(sheriffCar, -1);
                                }
                                suspect.WarpIntoVehicle(sheriffCar, 1);
                                
                                sheriffCar.IsSirenSilent = false;
                                sheriff.Tasks.DriveToPosition(sheriffCar, new Vector3(2598.602f, 2501.81f, 28.27002f), 15f, VehicleDrivingFlags.Emergency, 2f);

                                if (sheriff.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || sheriff2.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || suspect.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || sheriffCar.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f)
                                {
                                    Game.DisplayNotification("Security Dispatcher: Attention " + Main.UID + ", call is code 4.");
                                    End();
                                }
                            }
                            
                            if (sheriff.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || sheriff2.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || suspect.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || sheriffCar.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f)
                            {
                                Game.DisplayNotification("Security Dispatcher: Attention " + Main.UID + ", call is code 4.");
                                End();
                            }
                        }
                        else if (Game.IsKeyDown(Keys.O) && !oPressed)
                        {
                            oPressed = true;
                            Game.DisplaySubtitle(Main.Name + ": Sorry for the inconvenience, you're free to go.");
                            suspect.Tasks.EnterVehicle(susCar, -1).WaitForCompletion();
                            End();
                        }

                        if (sheriff.Exists() && sheriff2.Exists() && sheriffCar.Exists() && suspect.Exists())
                        {
                            if (sheriff.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || sheriff2.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || suspect.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f || sheriffCar.DistanceTo(new Vector3(2598.602f, 2501.81f, 28.27002f)) < 5f)
                            {
                                Game.DisplayNotification("Security Dispatcher: Attention " + Main.UID + ", call is code 4.");
                                End();
                            }
                        }
                    }
                    
                    if (suspect.IsDead)
                    {
                        End();
                    }
                    
                    if (Game.IsKeyDown(Main.endKey) || Game.LocalPlayer.Character.IsDead)
                    {
                        End();
                    }
                    break;
                }
                case 1:
                {
                    if (Game.LocalPlayer.Character.DistanceTo(suspect) < 7f)
                    {

                        if (!convoStarted)
                        {
                            Game.DisplayHelp("Press 'Y' to advance conversation");
                            if (blip.Exists())
                                blip.Delete();
                            Game.DisplaySubtitle("Gate Operator: Hey boss, thanks for coming.");
                            convoStarted = true;
                        }
                    
                        if (Game.IsKeyDown(Keys.Y))
                        {
                            counter++;
                        }

                        if (counter == 1)
                        {
                            Game.DisplaySubtitle(Main.Name + ": No problem, what's going on?");
                        }

                        if (counter == 2)
                        {
                            Game.DisplaySubtitle("Gate Operator: This individual in the vehicle attempted to access the facility using a fake card.");
                        }

                        if (counter == 3)
                        {
                            Game.DisplaySubtitle(Main.Name + ": And you're sure it's a fake? The system's not just acting up again?");
                        }

                        if (counter == 4)
                        {
                            Game.DisplaySubtitle("Gate Operator: It's not an up-to-date card, we haven't used the template it has in years.");
                        }

                        if (counter == 5)
                        {
                            Game.DisplaySubtitle(Main.Name + ": Alright then, have they been cooperative?");
                        }

                        if (counter == 6)
                        {
                            Game.DisplaySubtitle("Gate Operator: So far so good....");
                            suspect.Tasks.FightAgainst(gatePed);
                        }
                    }
                
                    if (suspect.IsDead || suspect.IsCuffed)
                    {
                        End();
                    }
                
                    if (Game.IsKeyDown(Main.endKey) || Game.LocalPlayer.Character.IsDead)
                    {
                        End();
                    }
                    
                    break;
                }
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (suspect.Exists())
                suspect.Dismiss();
            if (sheriff.Exists())
                sheriff.Dismiss();
            if (sheriff2.Exists())
                sheriff2.Dismiss();
            if (sheriffCar.Exists())
                sheriffCar.Dismiss();
            if (susCar.Exists())
                susCar.Dismiss();
            if (gatePed.Exists())
                gatePed.Dismiss();
            if (blip.Exists())
                blip.Delete();
            
            Log.log("Security+ FakeCredentialsQuarry callout has been cleaned up");
            Game.LogTrivial("Security+ FakeCredentialsQuarry callout has been cleaned up");
        }
    }
}