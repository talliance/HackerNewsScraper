using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HackerNewsScraper
{
    class Utility
    {
        internal static bool validateURI(string inputURI)
        {
            if (new Regex(@"^(http://|https://|item\?id=)").Match(inputURI).Success)
                return true;
            else
                return false;
        }

        internal static int calculatePagesToScrape(int postsPerPage, int numberOfRequestedPosts)
        {
            int numberOfPagesToScrape = 1;

            var pages = (int)Math.Ceiling((double)numberOfRequestedPosts / postsPerPage);
            numberOfPagesToScrape = (int)pages;

            return numberOfPagesToScrape;
        }
    }
}
