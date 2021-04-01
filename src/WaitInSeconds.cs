using OpenQA.Selenium.Support.UI;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        /// <summary>
        /// WaitInSeconds
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static double WaitInSeconds(WebDriverWait w) => w.Timeout.TotalSeconds;      
    }
}