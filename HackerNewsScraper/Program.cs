using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using HackerNewsScraper.Interfaces;
using HackerNewsScraper.Models;

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
                    //initialize the scraper
                    ILauncher start = new Scraper.Launcher();
                    start.initialize();

                    List<PostResponseModel> postsList = new List<PostResponseModel>();

                    try
                    {
                        //get posts and populate the posts list
                        postsList = start.GetPosts(numberOfRequestedPosts);

                        // formating the output
                        string JsonFormattedPostsList = JsonConvert.SerializeObject(postsList, Formatting.Indented);

                        // show the new json list in console
                        Console.WriteLine(JsonFormattedPostsList);
                    }
                    catch(JsonException jEx)
                    {
                        Console.WriteLine("Unable to perform JSON serialization: {0}", jEx.Message);
                        throw new JsonException();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was an issue during the scraping: {0}", ex.Message);
                        Environment.Exit(2);
                    }
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
