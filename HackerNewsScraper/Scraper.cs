using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;

namespace HackerNewsScraper
{
    public class Scraper
    {
        //The scraping takes place in this method and returns a concatanated list of posts
        public List<PostResponse> GetPosts(string uri, int pageNumber, string xpathPostsNode, int numberOfPostsRequested)
        {
            List<PostResponse> returnedPosts = new List<PostResponse>();

            //validate uri
            Regex uriPattern = new Regex(@"^(http://|https://|item\?id=)");
            var matches = uriPattern.Match(uri);

            if (matches.Success)
            {
                //download page as html document 
                //easier to work with and more efficient than making continuous calls to the website
                HtmlDocument content = new HtmlWeb().Load(uri + "?p=" + pageNumber);

                int totalRows = content.DocumentNode.SelectNodes(xpathPostsNode).Count();
                PostResponse post = new PostResponse();
                int rowCount = 0;
                bool gottenFullPost = false;

                //loop through rows
                for (int row = 1; row <= totalRows; row++)
                {
                    string currentRow = xpathPostsNode + "[" + row + "]/td";

                    //needed to bypass the blank row that separates each post in the table
                    if (content.DocumentNode.SelectNodes(currentRow) != null)
                    {
                        rowCount++;

                        int totalElementsInRow = content.DocumentNode.SelectNodes(currentRow).Count();

                        //loop through elements in a row
                        for (int element = 1; element <= totalElementsInRow; element++)
                        {
                            string currentElement = currentRow + "[" + element + "]";

                            var test = content.DocumentNode.SelectNodes(currentElement);
                            //get title and uri if element contains it - they're in the same element
                            if (content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"storylink\"]") != null)
                            {
                                post.title = content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"storylink\"]").InnerText;

                                //will only allow up to 256 character - anything beyond that is trimmed off
                                if (post.title.Length > 256)
                                    post.title = post.title.Substring(0, 256);

                                post.uri = content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"storylink\"]").Attributes["href"].Value;
                                if (!uriPattern.Match(post.uri).Success)
                                    post.uri = "no valid uri";

                                if (post.uri.Contains("item?id="))
                                    post.uri = uri + post.uri;
                            }

                            //get author if element contains it
                            if (content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"hnuser\"]") != null)
                            {
                                post.author = content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"hnuser\"]").InnerText;

                                //will only allow up to 256 character - anything beyond that is trimmed off
                                if (post.author.Length > 256)
                                    post.author = post.author.Substring(0, 256);
                            }

                            //get points if element contains it
                            if (content.DocumentNode.SelectSingleNode(currentElement + "//span[@class=\"score\"]") != null)
                            {
                                var points = content.DocumentNode.SelectSingleNode(currentElement + "//span[@class=\"score\"]").InnerText;

                                //remove the word 'points'
                                Regex replacePattern = new Regex(@"(?<=[0-9]\s)points");
                                points = replacePattern.Replace(points, "");

                                post.points = Int32.Parse(points);
                            }

                            //get comments if element contains it - the last set of <a> tags
                            if (content.DocumentNode.SelectSingleNode(currentElement + "//a[3]") != null)
                            {
                                var comments = content.DocumentNode.SelectSingleNode(currentElement + "//a[3]").InnerText;

                                //though not often, sometimes the word 'discuss' will appear instead if there aren't any comments
                                if (comments == "discuss")
                                    comments = "0";

                                //zero element is the number of comments only
                                comments = comments.Split('&')[0];
                                

                                post.comments = Int32.Parse(comments);
                            }

                            //get rank if element contains it
                            if (content.DocumentNode.SelectSingleNode(currentElement + "//span[@class=\"rank\"]") != null)
                            {
                                var rank = content.DocumentNode.SelectSingleNode(currentElement + "//span[@class=\"rank\"]").InnerText;
                                post.rank = Int32.Parse(rank.Replace(".", ""));
                            }

                            //needed so that the both necessary rows have been included
                            if (rowCount == 2)
                            {
                                gottenFullPost = true;
                            }
                        }

                        //if the nullable fields aren't null, then add the post to the list
                        if (gottenFullPost && post.title != null && post.uri != null && post.author != null)
                        {
                            returnedPosts.Add(post);

                            //needed to prevent duplicate data
                            post = new PostResponse();

                            //if the number of posts being returned matche the amount of posts requested, then exit the loop
                            if (returnedPosts.Count == numberOfPostsRequested)
                                return returnedPosts;
                        }
                    }
                    else
                        rowCount = 0;
                }
            }
            else
            {
                Console.WriteLine("ERROR: Non-valid URI given");
            }

            return returnedPosts;
        }
    }
}
