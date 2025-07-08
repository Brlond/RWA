using AutoMapper;
using Lib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.ViewModels;
using NuGet.Packaging;
using NuGet.Protocol;
using System;

namespace MVC.Controllers
{
    [Authorize]
    public class TopicController : Controller
    {

        private readonly RwaContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        public TopicController(RwaContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }



        // GET: TopicController
        public ActionResult Index(SearchVM searchVm)
        {
            IQueryable<Topic> topics = _context
                .Topics
                .Include(x => x.Category)
                .Include(x => x.Posts)
                .Include(x => x.Tags).ThenInclude(x => x.Topics);
            if (!string.IsNullOrEmpty(searchVm.Query))
            {
                topics = topics.Where(x => x.Title.Contains(searchVm.Query));
            }
            if (searchVm.OrderBy is not null)
            {
                switch (searchVm.OrderBy.ToLower())
                {
                    case "date":
                        topics = topics.OrderBy(x => x.CreatedAt);
                        break;
                    case "title":
                        topics = topics.OrderBy(x => x.Title);
                        break;
                    case "category":
                        topics = topics.OrderBy(x => x.Category.Name);
                        break;
                    case "posts":
                        topics = topics.OrderBy(x => x.Posts.Count);
                        break;
                    default:
                        topics = topics.OrderBy(x => x.Id);
                        break;
                }
            }
            var filteredCount = topics.Count();
            // BEGIN PAGER
            var expandPages = _configuration.GetValue<int>("Paging:ExpandPages");
            searchVm.LastPage = (int)Math.Ceiling(1.0 * filteredCount / searchVm.Size);
            searchVm.FromPager = searchVm.Page > expandPages ?
              searchVm.Page - expandPages :
              1;
            searchVm.ToPager = (searchVm.Page + expandPages) < searchVm.LastPage ?
              searchVm.Page + expandPages :
              searchVm.LastPage;
            // END PAGER

            topics = topics.Skip((searchVm.Page - 1) * searchVm.Size).Take(searchVm.Size);

            List<TopicVM> topicVMs = _mapper.Map<List<TopicVM>>(topics.ToList());
            searchVm.Topics = topicVMs;
            return View(searchVm);
        }

        // GET: TopicController/Details/5
        public ActionResult Details(int id, PostSearchVM postsearch)
        {
            string jwt = User.FindFirst("JWT")?.Value;
            string username = HttpContext.User.Identity.Name;
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            ViewBag.UserRatedPosts = _context.Ratings.Where(x => x.UserId == user.Id).Where(r => r.PostId.HasValue).ToDictionary(r => r.PostId.Value, r => r.Score);
            ViewBag.JWT = jwt;
            var dbtopic = _context
                .Topics
                .Include(x => x.Category)
                .Include(x => x.Posts)
                .Include(x => x.Tags).ThenInclude(x => x.Topics).FirstOrDefault(x => x.Id == id);


            IQueryable<Post> postss = _context.Posts.Include(x => x.Ratings).Include(x => x.Topic).Include(x => x.User).Where(x => x.TopicId == id);

            if (postsearch.OrderBy is not null)
            {
                switch (postsearch.OrderBy.ToLower())
                {
                    case "date":
                        postss = postss.OrderByDescending(x => x.PostedAt);
                        break;
                    case "content":
                        postss = postss.OrderBy(x => x.Content);
                        break;
                    case "content-length":
                        postss = postss.OrderBy(x => x.Content);
                        break;
                    case "score":
                        postss = postss.OrderByDescending(x => x.Ratings.Count > 0 ? (int)Math.Round((decimal)x.Ratings.Average(r => r.Score)) : 0);
                        break;
                    default:
                        postss = postss.OrderBy(x => x.Id);
                        break;
                }
            }


            var filteredCount = postss.Count();
            // BEGIN PAGER
            var expandPages = _configuration.GetValue<int>("Paging:ExpandPages");
            postsearch.LastPage = (int)Math.Ceiling(1.0 * filteredCount / postsearch.Size);
            postsearch.FromPager = postsearch.Page > expandPages ?
              postsearch.Page - expandPages :
              1;
            postsearch.ToPager = (postsearch.Page + expandPages) < postsearch.LastPage ?
              postsearch.Page + expandPages :
              postsearch.LastPage;
            // END PAGER

            postss = postss.Skip((postsearch.Page - 1) * postsearch.Size).Take(postsearch.Size);

            List<PostVM> postVMs = _mapper.Map<List<PostVM>>(postss.ToList());

            postsearch.Posts = postVMs;
            postsearch.Topic = _mapper.Map<TopicVM>(dbtopic);

            PostDTO postvm = new PostDTO
            {
                TopicId = dbtopic.Id,
                UserId = user.Id,
            };
            ViewBag.PostVM = postvm;
            Console.WriteLine(username);
            Console.WriteLine(user);
            return View(postsearch);
        }

        // GET: TopicController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Tags = GetTagsListItems();
            ViewBag.Categories = GetCatsListItems();
            return View();
        }

        public List<SelectListItem> GetCatsListItems()
        {
            var genreListItemsJson = HttpContext.Session.GetString("CatsListItems");

            List<SelectListItem> genreListItems;
            if (genreListItemsJson == null)
            {
                genreListItems = _context.Categories
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();

                HttpContext.Session.SetString("CatsListItems", genreListItems.ToJson());
            }
            else
            {
                genreListItems = genreListItemsJson.FromJson<List<SelectListItem>>();
            }

            return genreListItems;
        }

        private List<SelectListItem> GetTagsListItems()
        {
            var genreListItemsJson = HttpContext.Session.GetString("TagListItems");

            List<SelectListItem> genreListItems;
            if (genreListItemsJson == null)
            {
                genreListItems = _context.Tags
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();

                HttpContext.Session.SetString("TagListItems", genreListItems.ToJson());
            }
            else
            {
                genreListItems = genreListItemsJson.FromJson<List<SelectListItem>>();
            }

            return genreListItems;
        }


        // POST: TopicController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(TopicVM topic, string action)
        {
            try
            {
                if (_context.Topics.Any(x => x.Title == topic.Title))
                {
                    ViewBag.ErrorMessage = "Topic with this title already exists.";
                    ViewBag.Tags = GetTagsListItems();
                    ViewBag.Categories = GetCatsListItems();
                    return View("Create");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Tags = GetTagsListItems();
                    ViewBag.Categories = GetCatsListItems();
                    ModelState.AddModelError("", "Failed to create a topic");
                    return View();
                }
                var tags = topic.TagIds.ToList();
                var dbtopic = _mapper.Map<Topic>(topic);
                _context.Topics.Add(dbtopic);
                _context.SaveChanges();

                if (action == "Create")
                {
                    return RedirectToAction(nameof(Index));

                }
                return RedirectToAction("Create");

            }
            catch
            {
                return View();
            }
        }

        // GET: TopicController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Tags = GetTagsListItems();
            ViewBag.Categories = GetCatsListItems();

            var dbtopic = _context.Topics.FirstOrDefault(x => x.Id == id);
            var category = _context.Categories.FirstOrDefault(x => x.Id == dbtopic.CategoryId);

            var tagids = dbtopic.Tags.Select(x => x.Id);

            var topicvm = _mapper.Map<TopicVM>(dbtopic);
            return View(topicvm);
        }

        // POST: TopicController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, TopicVM topicVM)
        {
            try
            {
               
                var dbtopic = _context.Topics.Include(x=>x.Posts).Include(x=>x.Tags).Include(x=>x.Category).FirstOrDefault(x => x.Id == id);
                foreach (var tag in dbtopic.Tags.ToList())
                {
                    tag.Topics.Remove(dbtopic);
                }
                dbtopic.Tags.Clear();
                _context.SaveChanges();
                dbtopic.Title = topicVM.Title;
                dbtopic.Description = topicVM.Description;
                dbtopic.CategoryId = topicVM.CategoryId;
                dbtopic.Category = _context.Categories.FirstOrDefault(x => x.Id == topicVM.CategoryId);
                var newTags = _context.Tags.Where(t => topicVM.TagIds.Contains(t.Id)).ToList();

                foreach (var tag in newTags)
                {
                    dbtopic.Tags.Add(tag);
                    tag.Topics.Add(dbtopic);
                }


                _context.SaveChanges();
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Tags = GetTagsListItems();
                ViewBag.Categories = GetCatsListItems();
                return View();
            }
        }

        // GET: TopicController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var dbtopic = _context.Topics.FirstOrDefault(x => x.Id == id);

            var category = _context.Categories.FirstOrDefault(x => x.Id == dbtopic.CategoryId);

            var tagids = dbtopic.Tags.Select(x => x.Id);

            var topicvm = _mapper.Map<TopicVM>(dbtopic);
            return View(topicvm);
        }

        // POST: TopicController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, TopicVM topic)
        {
            try
            {
                var dbtopic = _context.Topics.FirstOrDefault(x => x.Id == id);

                _context.Topics.Remove(dbtopic);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

    internal class PostDTO
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int TopicId { get; set; }
        public string Content { get; set; }
        public List<int>? Scores { get; set; }
    }
}
