using OpenQA.Selenium;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        /// <summary>
        /// TestCleanup
        /// </summary>
        /// <param name="driver"></param>
        public static void TestCleanup(this IWebDriver driver)
        {
            if (driver != null) driver.Quit();
            StopSeleniumDrivers();
        }
    }
}