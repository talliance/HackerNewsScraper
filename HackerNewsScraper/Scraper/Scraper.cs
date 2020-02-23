using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HackerNewsScraper.Interfaces;
using HackerNewsScraper.Models;

namespace HackerNewsScraper.Scraper
{
    public class Scraper : IScraper
    {
        const int _postsPerPage = 30;
        string _uri;
        List<PostResponseModel> _posts;
        string _xpathPostsNode;
        int _numberOfRequestedPosts;

        public Scraper(string uri, string xpathPostsNode)
        {
            _uri = uri;
            _xpathPostsNode = xpathPostsNode;
        }

        public Scraper(string uri, int numberOfRequestedPosts, string xpathPostsNode)
            : this(uri, xpathPostsNode)
        {
            _posts = new List<PostResponseModel>();
            _numberOfRequestedPosts = numberOfRequestedPosts;
        }


        public List<PostResponseModel> parse()
        {
            List<PostResponseModel> outPutPosts = new List<PostResponseModel>();

            int numberOfPagesToParse = 1;

            if (_numberOfRequestedPosts > _postsPerPage)
            {
                //make sure only the smallest whole number is returned
                numberOfPagesToParse = Utility.calculatePagesToScrape(_postsPerPage, _numberOfRequestedPosts);
            }

            // scraping by page
            for (int page = 1; page <= numberOfPagesToParse; page++)
                outPutPosts = outPutPosts.Concat(GetPosts(page, _numberOfRequestedPosts - outPutPosts.Count)).ToList();

            return outPutPosts;
        }

        public List<PostResponseModel> parse(int numberOfRequestedPosts)
        {
            _numberOfRequestedPosts = numberOfRequestedPosts;
            return parse();
        }

        //The scraping takes place in this method and returns a concatanated list of posts
        public List<PostResponseModel> GetPosts(int pageNumber, int numberOfPostsRequested)
        {
            List<PostResponseModel> returnedPosts = new List<PostResponseModel>();

            //validate uri
            bool isValidUri = Utility.validateURI(_uri);

            if (isValidUri)
            {
                //download page as html document 
                //easier to work with and more efficient than making continuous calls to the website
                HtmlDocument content = new HtmlWeb().Load(_uri + "?p=" + pageNumber);

                //check that the document matches the xpath posts pattern
                if(content.DocumentNode.SelectNodes(_xpathPostsNode) != null)
                {
                    int totalRows = content.DocumentNode.SelectNodes(_xpathPostsNode).Count();
                    PostResponseModel post = new PostResponseModel();
                    int rowCount = 0;
                    bool gottenFullPost = false;

                    //loop through rows
                    for (int row = 1; row <= totalRows; row++)
                    {
                        string currentRow = _xpathPostsNode + "[" + row + "]/td";

                        //needed to bypass the blank row that separates each post in the table
                        if (content.DocumentNode.SelectNodes(currentRow) != null)
                        {
                            rowCount++;

                            int totalElementsInRow = content.DocumentNode.SelectNodes(currentRow).Count();

                            //loop through elements in a row
                            for (int element = 1; element <= totalElementsInRow; element++)
                            {
                                string currentElement = currentRow + "[" + element + "]";

                                //get title and uri if element contains it - they're in the same element
                                if (content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"storylink\"]") != null)
                                {
                                    post.title = content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"storylink\"]").InnerText;

                                    //will only allow up to 256 character - anything beyond that is trimmed off
                                    if (post.title.Length > 256)
                                        post.title = post.title.Substring(0, 256);

                                    post.uri = content.DocumentNode.SelectSingleNode(currentElement + "//a[@class=\"storylink\"]").Attributes["href"].Value;
                                    if (!Utility.validateURI(_uri))
                                        post.uri = "no valid uri";

                                    if (post.uri.Contains("item?id="))
                                        post.uri = _uri + post.uri;
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
                                post = new PostResponseModel();

                                //if the number of posts being returned matche the amount of posts requested, then exit the loop
                                if (returnedPosts.Count == numberOfPostsRequested)
                                    return returnedPosts;
                            }
                        }
                        else
                            rowCount = 0;
                    }
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
