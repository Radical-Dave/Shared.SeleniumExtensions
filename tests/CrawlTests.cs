using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Shared.SeleniumExtensions.Tests
{
    [TestClass]
    public class CrawlTests
    { 
        public string FullName => typeof(CrawlTests).FullName;

        private TestContext testContextInstance;
        private IWebDriver driver;
        private IConfigurationRoot config;
        private bool verbose = false; 

        public CrawlTests() {
            config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            verbose = string.IsNullOrEmpty(config["verbose"]) || config["verbose"].ToLower() != "false" ? true : false;
        }

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        //[TestMethod]
        //public void CrawlTest()
        //{
        //    var results = driver.Crawl(config["SiteUrl"], excludes: config["CrawlExcludes"]?.Split(','), maxDepth: int.Parse(config["MaxDepth"] ?? "1"), verbose: verbose);
        //    Assert.IsTrue(driver.Title.Length > 0);
        //    TestContext.WriteLine(results);
        //}

        //[TestMethod]
        //public void CrawlSingleTest()
        //{
        //    var results = driver.Crawl(config["SiteUrl"], excludes: config["CrawlExcludes"]?.Split(','), verbose: verbose); //int.Parse(config["MaxDepth"] ?? "1")
        //    Assert.IsTrue(driver.Title.Length > 0);
        //    TestContext.WriteLine(results);
        //}

        [TestMethod]
        public void CrawlTwoDeepTest()
        {
            var results = driver.Crawl(config["SiteUrl"], excludes: config["CrawlExcludes"]?.Split(','), maxDepth: 2, verbose: verbose);
            var error = !results.ToLower().Contains("error");
            if (error || verbose) TestContext.WriteLine(results);
            Assert.IsTrue(error, $"ERROR:{results}");
        }

        [TestInitialize]
        public void SetupTest()
        {
            driver = SeleniumExtensions.GetDriver();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.TestCleanup();
        }
    }
}