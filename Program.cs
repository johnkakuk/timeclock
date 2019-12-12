using System;
using System.IO;

namespace timeclock
{
    class Program
    {
        static void Main(string[] args)
        {

            /* Components:
             * Dashboard: Shows current time, clock status, last clockin time, hours worked so far
             * Clock in, save clock-in time
             * Get last clock-in time
             * Get current time
             * Calculate time worked since clock-in
             * Clock out, save clock-out time
             * Generate report
            */
            if (args.Length > 0)
            {
                /*
                Console.WriteLine("Arguments:");

                foreach(Object obj in args)
                {
                    Console.WriteLine(obj);
                }
                */

                foreach (string arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "in":
                            ClockIn();
                            break;
                        case "out":
                            ClockOut();
                            break;
                        case "total":
                            Total();
                            break;
                        case "reset":
                            ResetTotal();
                            break;
                        default:
                            Console.WriteLine("Unknown command: {0}", args);
                            break;
                    }
                }
            } else
            {
                Report();
            }
        }
        

        

        public static bool GetClockStatus()
        {
            bool clockedIn = false;
            string line = null;
            try
            {
                string path = GetPath("/clockstatus.txt");

                // Open
                StreamReader sr = new StreamReader(path);

                // Read
                line = sr.ReadLine();

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            switch (line)
            {
                case ("false"):
                    clockedIn = false;
                    break;
                case ("true"):
                    clockedIn = true;
                    break;
            }
            return clockedIn;
        }




        public static void ResetTotal()
        {
            // Overwrite existing data
            try
            {
                string overwrite = "";

                string path = GetPath("/record.txt");
                //Write a line of text. Overwrite existing data
                File.WriteAllText(path, overwrite);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("\r\nTime log reset.");
                Total();
            }
        }




        public static void Total()
        {
            string line = null;
            TimeSpan totalTime = new TimeSpan(0);
            TimeSpan currentTime = new TimeSpan(0);
            int sessionCounter = 0;

            try
            {
                string path = GetPath("/record.txt");

                // Open
                StreamReader sr = new StreamReader(path);

                //Read the first line of text
                line = sr.ReadLine();

                // Read
                while (line != null)
                {
                    sessionCounter++;
                    currentTime = TimeSpan.Parse(line);
                    totalTime = totalTime + currentTime;
                    line = sr.ReadLine();                    
                }

                Console.WriteLine("\r\nTotal hours worked: {0} hours over {1} sessions.", totalTime.TotalHours.ToString("N2"), sessionCounter);

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }




        public static void Report()
        {
            bool clockedIn = GetClockStatus();
            if (!clockedIn)
            {
                Console.WriteLine("");
            }
            switch (clockedIn)
            {
                case (true):
                    goto ClockedIn;
                case (false):
                    goto ClockedOut;
            }

        ClockedIn:
            DateTime currentTime = DateTime.Now;
            DateTime clockinTime = GetLastClockin();
            TimeSpan totalTime = currentTime - clockinTime;
            Console.WriteLine("\r\nYou are clocked in.");
            PrintLastClockin();
            Console.WriteLine("You have been working for {0} hours.", totalTime.TotalHours.ToString("N2"));
            goto End;

        ClockedOut:
            // Get clockin
            string clockinString = null;
            Console.WriteLine("\r\nYou are clocked out.");
            try
            {
                string path = GetPath("/clockin.txt");
                // Open
                StreamReader sr = new StreamReader(path);

                //Read & display
                clockinString = sr.ReadLine();
                Console.WriteLine("You last clocked in on {0}.", clockinString);

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // Parse clockin
            DateTime clockin = Convert.ToDateTime(clockinString);

            // Get clockout
            string clockoutString = null;
            try
            {
                string path = GetPath("/clockout.txt");
                // Open
                StreamReader sr = new StreamReader(path);

                //Read & display
                clockoutString = sr.ReadLine();
                Console.WriteLine("You last clocked out on {0}.", clockoutString);

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // Parse clockout
            DateTime clockout = Convert.ToDateTime(clockoutString);

            // Calculate difference
            TimeSpan duration = clockout - clockin;
            Console.WriteLine("Time logged during this session: {0}", duration);

        End:;
        }




        public static void TimeTally()
        {
            // Get clockin
            string clockinString = null;
            try
            {
                string path = GetPath("/clockin.txt");

                // Open
                StreamReader sr = new StreamReader(path);

                //Read & display
                clockinString = sr.ReadLine();

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // Parse clockin
            DateTime clockin = Convert.ToDateTime(clockinString);

            // Get clockout
            string clockoutString = null;
            try
            {
                string path = GetPath("/clockout.txt");

                // Open
                StreamReader sr = new StreamReader(path);

                //Read & display
                clockoutString = sr.ReadLine();

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // Parse clockout
            DateTime clockout = Convert.ToDateTime(clockoutString);

            // Calculate difference
            TimeSpan duration = clockout - clockin;
            Console.WriteLine("Time logged during this session: {0}", duration);

            // Save to master record:

            try
            {
                string path = GetPath("/record.txt");

                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(path, true);

                //Write a line of text
                sw.WriteLine(duration);

                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Time successfully logged.");
            }
        }




        public static void ClockOut()
        {
            bool clockedIn = GetClockStatus();
            if (clockedIn == false)
            {
                Console.WriteLine("\r\nYou are already clocked out.");
                PrintLastClockout();
                goto End;
            }

            // Get clock-out time
            try
            {
                string path = GetPath("/clockout.txt");

                string clockoutString = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                //Write a line of text. Overwrite existing data
                File.WriteAllText(path, clockoutString);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // Update clock status
            try
            {
                string path = GetPath("/clockstatus.txt");

                string clockStatus = "false";

                File.WriteAllText(path, clockStatus);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("\r\nYou have been clocked out.");
            }
            TimeTally();
        End:;
        }




        public static void ClockIn()
        {
            bool clockedIn = GetClockStatus();
            if (clockedIn == true)
            {
                Console.WriteLine("\r\nYou are already clocked in.");
                PrintLastClockin();
                goto End;
            }

            // Save clock-in time
            try
            {
                string path = GetPath("/clockin.txt");

                string clockin = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                //Write a line of text. Overwrite existing data
                File.WriteAllText(path, clockin);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // Update clock status
            try
            {
                string path = GetPath("/clockstatus.txt");

                string clockStatus = "true";

                File.WriteAllText(path, clockStatus);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("\r\nYou are now clocked in.");
            }
        End:;
        }




        public static void PrintLastClockout()
        {
            string line;
            DateTime lastClockOut;
            try
            {
                string path = GetPath("/clockout.txt");

                StreamReader sr = new StreamReader(path);

                line = sr.ReadLine();
                lastClockOut = Convert.ToDateTime(line);
                Console.WriteLine("You last clocked out on {0} at {1}.", lastClockOut.ToString("MM/dd"), lastClockOut.ToString("HH:mm"));

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }




        public static DateTime GetLastClockin()
        {
            string line;
            DateTime lastClockIn = new DateTime(0);
            try
            {
                string path = GetPath("/clockin.txt");

                // Open
                StreamReader sr = new StreamReader(path);

                //Read & display
                line = sr.ReadLine();
                lastClockIn = Convert.ToDateTime(line);

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return lastClockIn;
        }





        public static string GetPath(string path)
        {
            string path2 = AppDomain.CurrentDomain.BaseDirectory;
            string fullpath = string.Format("{0}{1}", path2, path);
            return fullpath;
        }




        public static void PrintLastClockin()
        {
            string line;
            DateTime lastClockIn;
            try
            {
                string path = GetPath("/clockin.txt");

                // Open
                StreamReader sr = new StreamReader(path);

                //Read & display
                line = sr.ReadLine();
                lastClockIn = Convert.ToDateTime(line);
                Console.WriteLine("You last clocked in on {0} at {1}.", lastClockIn.ToString("MM/dd"), lastClockIn.ToString("HH:mm"));

                // Close
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}