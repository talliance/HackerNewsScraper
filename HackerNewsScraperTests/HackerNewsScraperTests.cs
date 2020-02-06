using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HackerNewsScraper;
using System.Collections.Generic;
using System.Linq;

namespace HackerNewsScraperTests
{
    [TestClass]
    public class HackerNewsScraperTests
    {
        int postsPerPage = 30;
        int numberOfPagesToScrape = 1;
        string uri = "https://news.ycombinator.com/";
        string xpathPostsNode = "//table[@class=\"itemlist\"][1]/tr";

        [TestMethod]
        public void OnePageScrapeTest()
        {
            int numberOfRequestedPosts = 1;
            Scraper scraper = new Scraper();

            List<PostResponse> postsList = new List<PostResponse>();

            for (int page = 1; page <= numberOfPagesToScrape; page++)
                postsList = postsList.Concat(scraper.GetPosts(uri, page, xpathPostsNode, numberOfRequestedPosts - postsList.Count())).ToList();

            Assert.AreEqual(1, postsList.Count);
        }

        [TestMethod]
        public void TwoPageScrapeTest()
        {
            int numberOfRequestedPosts = 40;
            Scraper scraper = new Scraper();

            numberOfPagesToScrape = calculatePages(numberOfRequestedPosts);
            List<PostResponse> postsList = new List<PostResponse>();

            for (int page = 1; page <= numberOfPagesToScrape; page++)
                postsList = postsList.Concat(scraper.GetPosts(uri, page, xpathPostsNode, numberOfRequestedPosts - postsList.Count())).ToList();

            Assert.AreEqual(40, postsList.Count);
        }

        [TestMethod]
        public void ThreePageScrapeTest()
        {
            int numberOfRequestedPosts = 70;
            Scraper scraper = new Scraper();

            numberOfPagesToScrape = calculatePages(numberOfRequestedPosts);
            List<PostResponse> postsList = new List<PostResponse>();

            for (int page = 1; page <= numberOfPagesToScrape; page++)
                postsList = postsList.Concat(scraper.GetPosts(uri, page, xpathPostsNode, numberOfRequestedPosts - postsList.Count())).ToList();

            Assert.AreEqual(70, postsList.Count);
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
    }
}
