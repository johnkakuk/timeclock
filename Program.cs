using System;
using System.IO;

namespace timeclock
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check for command line arguments
            if (args.Length > 0)
            {
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




        public static void ClockIn()
        {
            if (GetClockStatus())
            {
                Console.WriteLine("\r\nYou are already clocked in.");
                return;
            }

            // Save clock-in time
            WriteTime("/clockin.txt", DateTime.Now.ToString(), false);
            SetClockStatus("in");
        }




        public static void ClockOut()
        {
            if (!GetClockStatus())
            {
                Console.WriteLine("\r\nYou are already clocked out.");
                return;
            }

            // Get clock-out time
            WriteTime("/clockout.txt", DateTime.Now.ToString(), false);
            SetClockStatus("out");

            // Print clockout report
            DateTime clockin = ReadTime("/clockin.txt");
            DateTime clockout = ReadTime("/clockout.txt");
            TimeSpan duration = clockout - clockin;
            Console.WriteLine("Time logged during this session: {0} hours.", duration.TotalHours.ToString("N2"));

            // Save to master record:
            WriteTime("/record.txt", duration.ToString(), true);
        }




        public static void SetClockStatus(string setOption)
        {
            string clockStatus = null;

            switch (setOption)
            {
                case ("in"):
                    clockStatus = "true";
                    break;
                case ("out"):
                    clockStatus = "false";
                    break;
            }

            try
            {
                string path = GetPath("/clockstatus.txt");

                File.WriteAllText(path, clockStatus);
                Console.WriteLine("\r\nYou have been clocked {0}.", setOption);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Environment.Exit(0);
            }
        }




        public static bool GetClockStatus()
        {
            string line = null;
            string path = GetPath("/clockstatus.txt");

            try
            {
                StreamReader sr = new StreamReader(path);
                line = sr.ReadLine();
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Environment.Exit(0);
            }

            if (line == "true")
            {
                return true;
            }
            return false;
        }




        public static void Total()
        {
            string line;
            string path = GetPath("/record.txt");
            int sessionCounter = -1;
            TimeSpan totalTime = new TimeSpan(0);
            TimeSpan currentTime;
            
            try
            {
                StreamReader sr = new StreamReader(path);

                line = sr.ReadLine();

                while (line != null)
                {
                    sessionCounter++;
                    currentTime = TimeSpan.Parse(line);
                    totalTime = totalTime + currentTime;
                    line = sr.ReadLine();                    
                }

                Console.WriteLine("\r\nTotal hours worked: {0} hours over {1} sessions.", totalTime.TotalHours.ToString("N2"), sessionCounter);

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Environment.Exit(0);
            }
        }




        public static void ResetTotal()
        {
            string overwrite = "0";
            WriteTime("/record.txt", overwrite, false);
            Console.WriteLine("\r\nTime log reset.");
        }




        public static void Report()
        {
            DateTime clockin = ReadTime("/clockin.txt");
            DateTime clockout = ReadTime("/clockout.txt");
            TimeSpan totalTime = DateTime.Now - clockin;
            bool clockedIn = GetClockStatus();

            switch (clockedIn)
            {
                case (true):
                    Console.WriteLine("\r\nYou are clocked in.");
                    Console.WriteLine("You last clocked in on {0} at {1}.", clockin.ToString("MM/dd"), clockin.ToString("HH:mm"));
                    Console.WriteLine("You have been working for {0} hours.", totalTime.TotalHours.ToString("N2"));
                    break;
                case (false):
                    Console.WriteLine("\r\nYou are clocked out.");
                    Console.WriteLine("You last clocked in on {0} at {1}.", clockin.ToString("MM/dd"), clockin.ToString("HH:mm"));
                    Console.WriteLine("You last clocked out on {0} at {1}.", clockout.ToString("MM/dd"), clockout.ToString("HH:mm"));
                    Console.WriteLine("Time logged during this session: {0} hours.", (clockout - clockin).TotalHours.ToString("N2"));
                    break;
            }
        }




        public static DateTime ReadTime(string path)
        {
            string line;
            DateTime time = new DateTime(0);

            try
            {
                string functionPath = GetPath(path);

                StreamReader sr = new StreamReader(functionPath);
                line = sr.ReadLine();
                time = Convert.ToDateTime(line);
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Environment.Exit(0);
            }

            return time;
        }




        public static void WriteTime(string path, string input, bool append)
        {
            try
            {
                string functionPath = GetPath(path);

                StreamWriter sw = new StreamWriter(functionPath, append);
                sw.WriteLine(input);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Environment.Exit(0);
            }
        }




        public static string GetPath(string path)
        {
            path = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, path);
            return path;
        }
    }
}