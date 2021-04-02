using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        [FindsBy(How = How.TagName, Using = "a")]
        public static IList<IWebElement> LinkElements { get; set; }

        /// <summary>
        /// GetLinks - string[] array of all links (href attributes on anchor element/tags) - <a href="link"
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="excludes"></param>
        /// <returns></returns>
        public static string[] GetLinks(this IWebDriver driver, string[] excludes = null)
        {
            if (driver.PageSource.Length == 0) return null;
            
            List<IWebElement> links = driver.FindElements(By.TagName("a")).Where(a => !string.IsNullOrEmpty(a.Text) && !excludes.Contains(a.Text.Trim().ToLower())).ToList();
            int linkCount = links.Count;

            List<string> results = new List<string>();
            for (int i = 0; i < linkCount; i++)
            {
                results.Add(links[i].GetAttribute("href"));
            }
            return results.ToArray();
        }
    }
}