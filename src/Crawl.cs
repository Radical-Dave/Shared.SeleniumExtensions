using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shared.SeleniumExtensions
{
    public static partial class SeleniumExtensions
    {
        private static Dictionary<string, string> links { get; set; }

        /// <summary>
        /// Crawl
        /// //https://blog.testproject.io/2020/12/28/10-common-selenium-exceptions-in-c-and-how-to-fix-them/
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="url"></param>
        /// <param name="urlBase"></param>
        /// <param name="excludes"></param>
        /// <param name="emptyLinks"></param>
        /// <param name="currentDepth"></param>
        /// <param name="maxDepth"></param>
        /// <param name="notCrawledMessage"></param>
        /// <returns></returns>
        public static string Crawl(this IWebDriver driver, string url, string urlBase = null, string[] excludes = null, bool emptyLinks = false, int currentDepth = 1, int maxDepth = 1, bool verbose = false, string notCrawledMessage = "n/a")
        {
            if (string.IsNullOrEmpty(urlBase)) urlBase = url;
            if (excludes == null) excludes = Array.Empty<string>();
            var methodSignature = $"{typeof(SeleniumExtensions).FullName}.Crawl({url},{string.Join(",", excludes)},{emptyLinks},{currentDepth},{maxDepth})";
            var results = new StringBuilder();

            if (verbose) results.AppendLine($"{methodSignature}:start");

            driver.Navigate().GoToUrl(url);

            if (driver.PageSource.Length == 0) return $"{url}:ERROR - no results";
            if (driver.PageSource.ToLower() == "<html><head></head><body></body></html>") return $"{url}:ERROR - empty results";
            if (driver.PageSource.ToLower().Contains("error")) return $"{url}:Contains ERROR";//:{driver.PageSource}";

            var anchors = driver.FindElements(By.TagName("a")).Where(a => !string.IsNullOrEmpty(a.Text) && !excludes.Contains(a.Text.Trim().ToLower())).ToList();
            var anchorsCount = anchors.Count();
            if (anchors == null || !anchors.Any())
            {
                results.AppendLine("no anchors?");
            }
            else
            {
                if (links == null) links = new Dictionary<string, string>() { { url, "started" } };

                using (var backseatDriver = GetDriver())
                {
                    foreach (var anchor in driver.FindElements(By.TagName("a")).Where(a => !string.IsNullOrEmpty(a.Text) && !excludes.Contains(a.Text.Trim().ToLower())))
                    {
                        try
                        {
                            var href = anchor.GetAttribute("href");
                            if (href == null || !href.StartsWith(urlBase)) continue;
                            href = href.Remove(0, urlBase.Length);
                            if (href.Length > 0 && href.StartsWith("/")) href = href.Remove(0, 1);

                            foreach (var queryStringTag in new string[] { "#", "?" })
                            {
                                if (!href.Contains(queryStringTag)) continue;
                                href = href.Substring(0, href.IndexOf(queryStringTag));
                            }

                            var excluded = false;
                            foreach (var exclude in excludes)
                            {
                                if (href.ToLower().StartsWith(exclude)) { excluded = true; break; }
                            }
                            if (excluded) continue;

                            if (href.EndsWith("/")) href = href.Remove(href.Length - 1, 1);

                            if (links.ContainsKey(href) || string.IsNullOrEmpty(href)) continue;

                            var linkResults = string.Empty;

                            if (currentDepth < maxDepth || maxDepth == -1)
                            {
                                linkResults = Crawl(backseatDriver, $"{url}/{href}", urlBase, excludes, false, currentDepth + 1, maxDepth);
                                if (verbose) results.AppendLine($"*VERBOSE* Crawled:{href}:{linkResults}");
                                //if (driver.Title != backseatDriver.Title) results.AppendLine($"BAD DRIVERS:{driver.Title} & {backseatDriver.Title}");
                            }
                            else
                            {
                                linkResults = notCrawledMessage;
                            }

                            if (!links.ContainsKey(href))
                            {
                                links.Add(href, linkResults);
                            }
                            else
                            {
                                links[href] = linkResults;
                            }
                            //results.AppendLine(href + "=" + linkResults);
                        }
                        catch (Exception exception)
                        {
                            var msg = $"ERROR processing {anchor.Text}:{exception.Message}";
                            Trace.TraceError(msg);
                            results.AppendLine(msg);
                        }
                    }

                    backseatDriver.Quit();
                }

                var tested = 1;//current
                var untested = 0;
                foreach (var link in links.Where(l => l.Key != url))
                {
                    if (verbose) results.AppendLine(link.Key + "=" + link.Value);
                    if (link.Value.StartsWith(notCrawledMessage))
                    {
                        ++untested;
                    }
                    else
                    {
                        ++tested;
                    }
                }
                results.AppendLine($"{url}:tested:{tested},untested:{untested},anchors:{anchorsCount},links:{links.Count}");
                links[url] = results.ToString();
            }

            if (verbose) results.AppendLine($"{methodSignature}:end");

            return results.ToString();
        }
    }
}