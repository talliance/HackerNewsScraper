using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using HackerNewsScraper.Interfaces;
using HackerNewsScraper.Models;
using HackerNewsScraper.Scraper;

namespace HackerNewsScraperTests
{
    [TestClass]
    public class HackerNewsScraperTests
    {
        int postsPerPage = 30;
        int numberOfPagesToScrape = 1;

        [TestMethod]
        public void OnePageScrapeTest()
        {
            int numberOfRequestedPosts = 1;

            ILauncher start = new FakeLauncher();
            start.initialize();

            Assert.AreEqual(start.GetPosts(numberOfRequestedPosts).Count, numberOfRequestedPosts);
        }

        [TestMethod]
        public void TwoPageScrapeTest()
        {
            int numberOfRequestedPosts = 40;

            numberOfPagesToScrape = calculatePages(numberOfRequestedPosts);
            List<PostResponseModel> postsList = new List<PostResponseModel>();

            ILauncher start = new FakeLauncher();
            start.initialize();

            Assert.AreEqual(start.GetPosts(numberOfRequestedPosts).Count, numberOfRequestedPosts);
        }

        [TestMethod]
        public void ThreePageScrapeTest()
        {
            int numberOfRequestedPosts = 70;

            numberOfPagesToScrape = calculatePages(numberOfRequestedPosts);
            List<PostResponseModel> postsList = new List<PostResponseModel>();

            ILauncher start = new FakeLauncher();
            start.initialize();

            Assert.AreEqual(start.GetPosts(numberOfRequestedPosts).Count, numberOfRequestedPosts);
        }

        public int calculatePages(int numberOfRequestedPosts)
        {
            numberOfPagesToScrape = 0;

            if (numberOfRequestedPosts > postsPerPage)
            {
                //make sure only the smallest whole number is returned
                var pages = (int)Math.Ceiling((double)numberOfRequestedPosts / postsPerPage);
                numberOfPagesToScrape = (int)pages;
            }

            return numberOfPagesToScrape;
        }

        public class FakeLauncher : ILauncher
        {
            IScraper _scraper;
            string _xpathPostsNode = "//table[@class=\"itemlist\"][1]/tr";

            public void initialize()
            {
                _scraper = new Scraper("https://news.ycombinator.com/news", _xpathPostsNode);
            }

            public List<PostResponseModel> GetPosts(int numberOfRequestedPosts)
            {
                return _scraper.parse(numberOfRequestedPosts);
            }
        }
    }
}
