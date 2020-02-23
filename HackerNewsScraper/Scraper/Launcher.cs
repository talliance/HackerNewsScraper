using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerNewsScraper.Interfaces;
using HackerNewsScraper.Models;

namespace HackerNewsScraper.Scraper
{
    class Launcher : ILauncher
    {
        //the path to the html table
        string _xpathPostsNode = "//table[@class=\"itemlist\"][1]/tr";

        int _numberOfRequestedPosts;
        IScraper _scraper;

        public void initialize()
        {
            try
            {
                string uri = ConfigurationManager.AppSettings["uri"];

                if (Utility.validateURI(uri))
                {
                    _scraper = new Scraper(uri, _xpathPostsNode);
                }
                else
                    throw new ApplicationException("The URI (" + uri + ") found in the configuration file is not valid.");

            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("An error occurred while initializing scraping - " + ex.Message);
                System.Environment.Exit(1);
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while initializing scraping.");
                System.Environment.Exit(2);
            }
        }

        public List<PostResponseModel> GetPosts(int numberOfRequestedPosts)
        {
            _numberOfRequestedPosts = numberOfRequestedPosts;
            return _scraper.parse(_numberOfRequestedPosts);
        }
    }
}
