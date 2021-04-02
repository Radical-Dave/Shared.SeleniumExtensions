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
        //private static Dictionary<string, string> Links { get; set; }

        /// <summary>
        /// Crawl
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="url"></param>
        /// <param name="urlBase"></param>
        /// <param name="excludes"></param>
        /// <param name="emptyLinks"></param>
        /// <param name="currentDepth"></param>
        /// <param name="maxDepth"></param>
        /// <param name="errorChecks"></param> 
        /// <param name="notCrawledMessage"></param>
        /// <returns></returns>
        public static string Crawl(this IWebDriver driver, string url, string urlBase = null, string[] excludes = null, bool emptyLinks = false, int currentDepth = 1, int maxDepth = 1, bool verbose = false, string[] errorChecks = null, string notCrawledMessage = "n/a")
        {
            var Links = new Dictionary<string, string>() { { url, "started" } };

            if (string.IsNullOrEmpty(urlBase)) urlBase = url;
            if (excludes == null) excludes = Array.Empty<string>();
            var methodSignature = $"{typeof(SeleniumExtensions).FullName}.Crawl({url},{string.Join(",", excludes)},{emptyLinks},{currentDepth},{maxDepth})";
            var results = new StringBuilder();
            if (verbose) results.AppendLine($"{methodSignature}:start");

            driver.Navigate().GoToUrl(url);

            if (driver.PageSource.Length == 0) return $"ERROR - no results";
            if ((errorChecks != null || errorChecks.Any() && !string.IsNullOrEmpty(driver.PageSource)) && errorChecks.Any(errorCheck => errorCheck.IndexOf(driver.PageSource, StringComparison.CurrentCultureIgnoreCase) > -1)) return $"ERROR CHECK:{errorChecks}";

            var links = GetLinks(driver, excludes);
            var linksCount = (links == null || !links.Any()) ? 0 : links.Length;
            if (linksCount == 0)
            {
                if (verbose) results.AppendLine($"no links?");
            }
            else
            {
                IWebDriver backseatDriver = null;

                var tested = 1;//current
                var untested = 0;

                foreach (var link in links)
                {
                    var linkTested = false;
                    try
                    {
                        var href = link;
                        if (href == null || !href.StartsWith(urlBase)) continue;
                        var linkTest = $"{href.Remove(0, urlBase.Length)}";
                        if (linkTest.StartsWith("/")) linkTest = linkTest.Remove(0, 1);
                        if (string.IsNullOrEmpty(href) || excludes.Any(linkTest.StartsWith)) continue;

                        var queryStringTags = new string[] { "#", "?" };
                        if (queryStringTags.Any(href.Contains))
                        {
                            foreach (var queryStringTag in queryStringTags)
                            {
                                if (!href.Contains(queryStringTag)) continue;
                                href = href.Substring(0, href.IndexOf(queryStringTag));
                            }
                        }

                        if (href.EndsWith("/")) href = href.Remove(href.Length - 1, 1);

                        //var hrefUrl = $"{url}/{href}";
                        var hrefUrl = href;

                        if (Links.ContainsKey(hrefUrl) || string.IsNullOrEmpty(href)) continue;

                        var linkResults = string.Empty;

                        if (currentDepth < maxDepth || maxDepth == -1)
                        {
                            if (backseatDriver == null) backseatDriver = GetDriver();
                            linkResults = Crawl(backseatDriver, hrefUrl, urlBase, excludes, false, currentDepth + 1, maxDepth, verbose, errorChecks, notCrawledMessage);
                            if (verbose) results.AppendLine($"{hrefUrl}:{linkResults} *VERBOSE* Crawled...");
                            //if (driver.Title != backseatDriver.Title) results.AppendLine($"BAD DRIVERS:{driver.Title} & {backseatDriver.Title}");
                            linkTested = true;
                        }
                        else
                        {
                            linkResults = notCrawledMessage;
                        }

                        if (!Links.ContainsKey(hrefUrl))
                        {
                            Links.Add(hrefUrl, linkResults);
                        }
                        else
                        {
                            Links[hrefUrl] = linkResults;
                        }
                        if (verbose) results.AppendLine($"{hrefUrl}={linkResults}");

                        if (linkTested)
                        {
                            ++tested;
                        }
                        else
                        {
                            ++untested;
                        }
                    }
                    catch (Exception exception)
                    {
                        var msg = $"ERROR processing {link}:{exception.Message}";
                        Trace.TraceError(msg);
                        if (verbose) results.AppendLine(msg);
                    }
                }
                if (maxDepth > 1 && backseatDriver != null) backseatDriver.Quit();
                results.AppendLine($"tested:{tested},untested:{untested},anchors:{linksCount},links:{Links.Count}");
                Links[url] = results.ToString();
            }

            if (currentDepth == 1)
            {
                foreach (var link in Links.Where(l => l.Value != notCrawledMessage || verbose == false))
                {
                    results.Append($"{link.Key}={link.Value}");
                }
            }

            if (verbose) results.AppendLine($"{methodSignature}:end");

            return results.ToString();
        }
    }
}