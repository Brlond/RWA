using AutoMapper;
using Lib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.ViewModels;
using System.Diagnostics;

namespace MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly RwaContext _context;
        private readonly IMapper _mapper;


        public HomeController(RwaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var topTopics = _context.Topics
                .Include(t => t.Posts).Include(x=>x.Tags).Include(x=>x.Category)
                .OrderByDescending(t => t.Posts.Count)
                .Take(6)
                .ToList();
            var posts = _mapper.Map<List<TopicVM>>(topTopics);
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
