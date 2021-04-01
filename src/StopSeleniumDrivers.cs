using System.Diagnostics;
using System.Text;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        /// <summary>
        /// StopSeleniumDrivers
        /// </summary>
        /// <param name="drivers"></param>
        /// <returns></returns>
        public static string StopSeleniumDrivers(string drivers = null)
        {
            if (string.IsNullOrEmpty(drivers)) drivers = "chromedriver.exe,geckodriver.exe";
            var results = new StringBuilder();

            results.Append($"StopSeleniumDrivers({drivers}):start");
            var fragCount = 0;
            foreach (var driver in drivers.Split(','))
            {
                var processes = Process.GetProcessesByName(driver);
                results.Append($"{driver}:{processes.Length}");
                while (processes != null && processes.Length > 0)
                {
                    foreach (var process in processes)
                    {
                        results.Append("Killing:{process}");
                        process.Kill();
                        fragCount++;
                        results.Append("Killed:{process}");
                    }

                    processes = Process.GetProcessesByName(driver);
                }
            }
            results.Append($"StopSeleniumDrivers({drivers}):end-frags:{fragCount}");
            return results.ToString();
        }
    }
}