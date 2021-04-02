using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Linq;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        private static IConfigurationRoot config;
        private static bool verbose = false;
        /// <summary>
        /// GetDriver
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static IWebDriver GetDriver(string path = null, string[] arguments = null)
        {
            if (config == null)
            {
                config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            }
            verbose = string.IsNullOrEmpty(config["verbose"]) || config["verbose"].ToLower() != "false";

            if (string.IsNullOrEmpty(path))
            {
                if (File.Exists("chromedriver.exe"))
                {
                    path = ".";
                }
                else
                {
                    path = config["ChromeDriverPath"];
                }
            }
            var options = new ChromeOptions();
            if (arguments == null) arguments = config["DriverArguments"]?.Split(',');
            if (arguments != null && arguments.Any()) options.AddArguments(arguments);
            return new ChromeDriver(path, options);
        }
    }
}