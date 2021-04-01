using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        /// <summary>
        /// GetDriver
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static IWebDriver GetDriver(string path = null, string[] arguments = null)
        {
            var options = new ChromeOptions();
            //if (arguments == null) arguments = config["DriverArguments"]?.Split(",");
            if (arguments == null) arguments = "--user-agent='Mozilla/5.0 (Windows NT 6.1; WOW64)',no-sandbox,--headless".Split(',');
            if (arguments != null && arguments.Any()) options.AddArguments(arguments);
            if (string.IsNullOrEmpty(path)) path = "\\packages\\Selenium.WebDriver.ChromeDriver.89.0.4389.2300\\driver\\win32";
            return new ChromeDriver(path, options);
        }
    }
}