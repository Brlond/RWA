using Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.ViewModels;
using System.Diagnostics;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly RwaContext _context;

        public HomeController(RwaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var topTopics = _context.Topics
                .Include(t => t.Posts).Include(x=>x.Tags).Include(x=>x.Category)
                .OrderByDescending(t => t.Posts.Count)
                .Take(6)
                .ToList();
            var posts = topTopics.Select(x => new TopicVM
            {
                Id = x.Id,
                Title = x.Title,
                CategoryId = x.Category.Id,
                CategoryName = x.Category.Name,
                Description = x.Description,
                PostsCount = x.Posts.Count,
                Publish_Date = x.CreatedAt,
                TagIds = x.Tags.Select(x => x.Id).ToList(),
                TagNames = x.Tags.Select(x => x.Name).ToList()
            });

            ViewBag.TopTopics = topTopics;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
