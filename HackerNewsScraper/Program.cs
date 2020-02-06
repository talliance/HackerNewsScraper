using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;

namespace HackerNewsScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfRequestedPosts = 0;

            //make sure that there is more than one argument and that one of them is '--posts'
            if (args.Count() > 1 && args[0].Equals("--posts"))
            {
                int.TryParse(args[1], out numberOfRequestedPosts);

                if (numberOfRequestedPosts == 0)
                    Console.WriteLine("WARNING: The number of requested posts is 0");

                if (numberOfRequestedPosts < 0)
                    Console.WriteLine("ERROR: The number of requested posts cannot be less than 0");

                //check that the number of requested posts meets the requirements of being <= 100
                if(numberOfRequestedPosts <= 100)
                {
                    //Calculate the number of pages to scrape
                    int postsPerPage = 30;
                    int numberOfPagesToScrape = 1;

                    if (numberOfRequestedPosts > postsPerPage)
                    {
                        //make sure only the smallest whole number is returned
                        var pages = (int)Math.Ceiling((double)numberOfRequestedPosts / postsPerPage);
                        numberOfPagesToScrape = (int)pages;
                    }

                    List<PostResponse> postsList = new List<PostResponse>();

                    try
                    {
                        //although the uri is stored in the app.config, this scraper is geared towards the HackerNews website only
                        //it's stored in the config as a standard practice
                        string uri = ConfigurationManager.AppSettings["uri"];

                        //the path to the html table
                        string xpathPostsNode = "//table[@class=\"itemlist\"][1]/tr";

                        //intialize a new instance of the scraper class
                        Scraper scraper = new Scraper();

                        //loop through the number of requested pages to build the posts list
                        for (int page = 1; page <= numberOfPagesToScrape; page++)
                            postsList = postsList.Concat(scraper.GetPosts(uri, page, xpathPostsNode, numberOfRequestedPosts - postsList.Count())).ToList();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was an issue during the scraping: {0}", ex.Message);
                        Environment.Exit(2);
                    }

                    // formating the output
                    string JsonFormattedPostsList = JsonConvert.SerializeObject(postsList, Formatting.Indented);

                    // show the new json list in console
                    Console.WriteLine(JsonFormattedPostsList);
                }
                else
                {
                    Console.WriteLine("ERROR: The number of posts requested need to be greater than 0 and equal to or less than 100");
                }
                
            }
            else
            {
                Console.WriteLine("ERROR: Can't find the correct number of arguments");
                Console.WriteLine("Please make sure the arguments are inputted as: --posts n");
                Console.WriteLine("n = the number of posts you require");
            }
        }
    }

}
