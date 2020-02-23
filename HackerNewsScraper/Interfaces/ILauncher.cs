using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerNewsScraper.Models;

namespace HackerNewsScraper.Interfaces
{
    public interface ILauncher
    {
        void initialize();

        List<PostResponseModel> GetPosts(int numberOfRequestedPosts);
    }
}
