using Lib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MVC.ViewModels;
using NuGet.Protocol;

namespace MVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {

        private readonly RwaContext _context;

        public PostController(RwaContext context)
        {
            _context = context;
        }

        // GET: PostController
        [Authorize(Roles = "Admin")]

        public ActionResult Index()
        {
            List<PostVM> posts = _context.Posts.Include(x => x.User).Include(x => x.Ratings).Include(x => x.Topic).ToList().Select(x => new PostVM
            {
                Id = x.Id,
                Content = x.Content,
                CreatorUsername = x.User.Username,
                Score = x.Ratings.Count > 0 ? (int)Math.Round((decimal)x.Ratings.Average(r => r.Score)) : 0,
                TopicId = (int)x.TopicId,
                TopicTitle = x.Topic.Title
            }).ToList();
            return View(posts);
        }

        // GET: PostController/Details/5
        [Authorize]

        public ActionResult Details(int id)
        {
            var post = _context.Posts.Include(x => x.Ratings).Include(x => x.Topic).Include(x => x.User).FirstOrDefault(x => x.Id == id);

            if (!HttpContext.User.IsInRole("Admin") && post.User.Username!= User.Identity.Name)
            {
                return Unauthorized();
            }
            var postvm = new PostVM
            {
                Id = post.Id,
                Content = post.Content,
                CreatorUsername = post.User.Username,
                Score = post.Ratings.Count > 0 ? (int)Math.Round((decimal)post.Ratings.Average(r => r.Score)) : 0,
                TopicId = (int)post.TopicId,
                TopicTitle = post.Topic.Title,
                Published_Date = post.PostedAt,
            };
            return View(postvm);
        }

        // GET: PostController/Create
        [Authorize(Roles = "Admin")]

        public ActionResult Create()
        {

            ViewBag.Topics = GetTopicItems();
            ViewBag.Users = GetUsersItems();
            return View();
        }

        private dynamic GetUsersItems()
        {
            var ItemsJson = HttpContext.Session.GetString("UsersListItems");

            List<SelectListItem> itemsList;
            if (ItemsJson == null)
            {
                itemsList = _context.Users
                    .Select(x => new SelectListItem
                    {
                        Text = x.Username,
                        Value = x.Id.ToString()
                    }).ToList();

                HttpContext.Session.SetString("UsersListItems", itemsList.ToJson());
            }
            else
            {
                itemsList = ItemsJson.FromJson<List<SelectListItem>>();
            }

            return itemsList;
        }

        public List<SelectListItem> GetTopicItems()
        {
            var ItemsJson = HttpContext.Session.GetString("TopicListItems");

            List<SelectListItem> itemsList;
            if (ItemsJson == null)
            {
                itemsList = _context.Topics
                    .Select(x => new SelectListItem
                    {
                        Text = x.Title,
                        Value = x.Id.ToString()
                    }).ToList();

                HttpContext.Session.SetString("TopicListItems", itemsList.ToJson());
            }
            else
            {
                itemsList = ItemsJson.FromJson<List<SelectListItem>>();
            }

            return itemsList;
        }

        // POST: PostController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public ActionResult Create(PostVM postvm)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Username == postvm.CreatorUsername);
                var post = new Post
                {
                    Id = postvm.Id,
                    Approved = CheckProfanity(postvm.Content),
                    Content = postvm.Content,
                    Topic = _context.Topics.FirstOrDefault(x => x.Id == postvm.TopicId),
                    TopicId = postvm.TopicId,
                    User = user,
                    UserId = user.Id,
                };
                _context.Posts.Add(post);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool? CheckProfanity(string content)
        {
            if (content.Contains("Fuck") || content.Contains("Shit"))
            {
                return false;
            }
            return true;
        }

        // GET: PostController/Edit/5
        [Authorize]

        public ActionResult Edit(int id)
        {
            var dbpost = _context.Posts.Include(x=>x.User).Include(x=>x.Topic).Include(x=>x.Ratings).FirstOrDefault(x => x.Id == id);
            if (!HttpContext.User.IsInRole("Admin") && dbpost.User.Username != User.Identity.Name)
            {
                return Unauthorized();
            }
            var postvm = new PostVM
            {
                Id = dbpost.Id,
                Content = dbpost.Content,
                CreatorUsername = dbpost.User.Username,
                Score = dbpost.Ratings.Count == 0 ? (int)Math.Round((decimal)dbpost.Ratings.Select(r => r.Score).DefaultIfEmpty(0).Average()) : 0,
                TopicId = (int)dbpost.TopicId,
                TopicTitle = dbpost.Topic.Title
            };
            return View(postvm);
        }

        // POST: PostController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, PostVM postVM)
        {
            try
            {

                var dbpost = _context.Posts.Include(x => x.User).Include(x => x.Topic).Include(x => x.Ratings).FirstOrDefault(x => x.Id == id);
                if (!HttpContext.User.IsInRole("Admin") && dbpost.User.Username != User.Identity.Name)
                {
                    return Unauthorized();
                }
                dbpost.Content = postVM.Content;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PostController/Delete/5
        [Authorize]

        public ActionResult Delete(int id)
        {
            var dbpost = _context.Posts.Include(x => x.User).Include(x => x.Topic).Include(x => x.Ratings).FirstOrDefault(x => x.Id == id);
            if (!HttpContext.User.IsInRole("Admin") && dbpost.User.Username != User.Identity.Name)
            {
                return Unauthorized();
            }
            var postvm = new PostVM
            {
                Id = dbpost.Id,
                Content = dbpost.Content,
                CreatorUsername = dbpost.User.Username,
                Score = dbpost.Ratings.Count == 0 ? (int)Math.Round((decimal)dbpost.Ratings.Select(r => r.Score).DefaultIfEmpty(0).Average()) : 0,
                TopicId = (int)dbpost.TopicId,
                TopicTitle = dbpost.Topic.Title
            };
            return View(postvm);
        }

        // POST: PostController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Delete(int id, PostVM postvm)
        {
            try
            {
                var dbpost = _context.Posts.Include(x => x.User).Include(x => x.Topic).Include(x => x.Ratings).FirstOrDefault(x => x.Id == id);
                if (!HttpContext.User.IsInRole("Admin") && dbpost.User.Username != User.Identity.Name)
                {
                    return Unauthorized();
                }
                _context.Posts.Remove(dbpost);
                _context.SaveChanges();

                if (HttpContext.User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return GetByUsersString(User.Identity.Name);
                }
            }
            catch
            {
                return View();
            }
        }
        public ActionResult GetByUsers(int id)
        {
            var dbposts = _context.Posts.Where(x => x.UserId == id);
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            var posts = dbposts.Include(x => x.Topic).Include(x => x.Ratings).Select(x => new PostVM
            {
                Content = x.Content,
                Published_Date = x.PostedAt,
                CreatorUsername = user.Username,
                Id = x.Id,
                Score = x.Ratings.Count > 0 ? (int)Math.Round((decimal)x.Ratings.Average(r => r.Score)) : 0,
                TopicId = (int)x.TopicId,
                TopicTitle = x.Topic.Title
            });
            return View(posts);
        }


        public ActionResult GetByUsersString(string username)
        {
            var dbposts = _context.Posts.Where(x => x.User.Username == username);
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            ViewBag.User = user;
            var posts = dbposts.Include(x => x.Topic).Include(x => x.Ratings).Select(x => new PostVM
            {
                Content = x.Content,
                Published_Date = x.PostedAt,
                CreatorUsername = user.Username,
                Id = x.Id,
                Score = x.Ratings.Count > 0 ? (int)Math.Round((decimal)x.Ratings.Average(r => r.Score)) : 0,
                TopicId = (int)x.TopicId,
                TopicTitle = x.Topic.Title
            });
            return View("GetByUsers", posts);
        }
    }
}
