using OpenQA.Selenium;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        /// <summary>
        /// PageHasContent
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public static bool PageHasContent(this IWebDriver driver, string url, string query = "*", bool force = false)
        {
            return driver.Search(url, "content", query, force);
        }
    }
}