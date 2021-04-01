using OpenQA.Selenium;
using System;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        /// <summary>
        /// Search
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <param name="query"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public static bool Search(this IWebDriver driver, string url, string value, string query, bool force = false)
        {
            try
            {
                if (driver.Url != url || force) driver.Navigate().GoToUrl(url);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"IWebDriverExtensions.Search({url},{value},{query},{force}) ERROR:{exception.Message}");
            }
            switch (value.ToLower())
            {
                case "title":
                    value = driver.Title.Trim();
                    break;
                default:
                    value = driver.PageSource.Trim();
                    break;
            }
            if (query == "*")
            {
                return value.Trim().Length > 0;
            }
            else if (query.StartsWith("*") && query.EndsWith("*"))
            {
                return value.Contains(query.Replace("*", ""));
            }
            else if (query.StartsWith("*"))
            {
                return value.EndsWith(query.Remove(0, 1));
            }
            else if (query.EndsWith("*"))
            {
                return value.StartsWith(query.Remove(query.Length - 1, 1));
            }
            return value == query;
        }
    }
}