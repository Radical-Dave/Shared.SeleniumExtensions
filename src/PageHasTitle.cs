using OpenQA.Selenium;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        /// <summary>
        /// PageHasTitle
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public static bool PageHasTitle(this IWebDriver driver, string url, string query = "*", bool force = false)
        {
            return driver.Search(url, "title", query, force);
        }
    }
}