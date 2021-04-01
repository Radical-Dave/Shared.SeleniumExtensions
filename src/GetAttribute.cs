using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {       
        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <param name="e"></param>
        /// <param name="attr"></param>
        /// <param name="d"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static string GetAttribute(this IWebElement e, string attr, IWebDriver d, WebDriverWait w)
        {
            var c = WaitInSeconds(w);

            c -= 2;

            var w2 = new WebDriverWait(d, TimeSpan.FromSeconds(c));
			//WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string att = null;

            if (c <= 1)
            {
                throw new Exception("Could not get attribute.");
            }

            try
            {
                att = e.GetAttribute(attr);
            }

            catch (Exception ex)
            {
                Console.WriteLine("\nTried to get attribute, but got an exception: \n" + ex + "\n");
                //Pause(500);
                e.GetAttribute(attr, d, w2);

            }

            Console.WriteLine("Got: " + attr + " value: " + att + " and counter is at: " + c);

            return att;
        }
    }
}