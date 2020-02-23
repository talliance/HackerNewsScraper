using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerNewsScraper.Models;

namespace HackerNewsScraper.Interfaces
{
    public interface IScraper
    {
        List<PostResponseModel> parse();

        List<PostResponseModel> parse(int numberOfRequestedPosts);

        List<PostResponseModel> GetPosts(int pageNumber, int numberOfPostsRequested);
    }
}
