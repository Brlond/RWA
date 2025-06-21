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

namespace MVC.Controllers
{
    [Authorize]
    public class TopicController : Controller
    {

        private readonly RwaContext _context;
        private readonly IConfiguration _configuration;

        public TopicController(RwaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            List<TopicVM> topicVMs = topics.Select(x => new TopicVM
            {
                Id = x.Id,
                TagIds = x.Tags.Select(x => x.Id).ToList(),
                CategoryId = (int)x.CategoryId,
                CategoryName = x.Category.Name,
                Description = x.Description,
                PostsCount = x.Posts.Count(),
                Publish_Date = x.CreatedAt,
                TagNames = x.Tags.Select(x => x.Name + " ").ToList(),
                Title = x.Title,
            }).ToList();
            searchVm.Topics = topicVMs;
            return View(searchVm);
        }

        // GET: TopicController/Details/5
        public ActionResult Details(int id)
        {
            var dbtopic = _context
                .Topics
                .Include(x => x.Category)
                .Include(x => x.Posts)
                .Include(x => x.Tags).ThenInclude(x => x.Topics).FirstOrDefault(x => x.Id == id);


            ViewBag.Posts = dbtopic.Posts.Select(x => new PostVM
            {
                Id = x.Id,
                Content = x.Content,
                CreatorUsername = x?.User?.Username,
                Score = x?.Ratings.Count == 0 ? (int)Math.Round((decimal)x?.Ratings.Select(r => r?.Score).DefaultIfEmpty(0).Average()) : 0,
                TopicId = (int)x.TopicId,
                TopicTitle = x?.Topic.Title
            }).ToList();

            var topic = new TopicVM
            {
                Id = dbtopic.Id,
                TagIds = dbtopic.Tags.Select(x => x.Id).ToList(),
                CategoryId = (int)dbtopic.CategoryId,
                CategoryName = dbtopic.Category.Name,
                Description = dbtopic.Description,
                PostsCount = dbtopic.Posts.Count(),
                Publish_Date = dbtopic.CreatedAt,
                TagNames = dbtopic.Tags.Select(x => x.Name + " ").ToList(),
                Title = dbtopic.Title,
            };

            return View(topic);
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
        [Authorize(Roles ="Admin")]
        public ActionResult Create(TopicVM topic)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    ViewBag.Tags = GetTagsListItems();
                    ViewBag.Categories = GetCatsListItems();
                    ModelState.AddModelError("", "Failed to create a topic");
                    return View();
                }
                var tags = topic.TagIds.ToList();
                var dbtopic = new Topic
                {
                    CategoryId = topic.CategoryId,
                    Description = topic.Description,
                    Title = topic.Title,
                    Tags = _context.Tags.Where(t => topic.TagIds.Contains(t.Id)).ToList(),
                    Category = _context.Categories.FirstOrDefault(x => x.Id == topic.CategoryId),
                };
                _context.Topics.Add(dbtopic);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
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

            var topicvm = new TopicVM
            {
                Id = dbtopic.Id,
                CategoryId = category.Id,
                CategoryName = category.Name,
                TagIds = tagids.ToList(),
                Description = dbtopic.Description,
                PostsCount = dbtopic.Posts.Count(),
                Publish_Date = dbtopic.CreatedAt,
                TagNames = dbtopic.Tags.Select(x => x.Name).ToList(),
                Title = dbtopic.Title,
            };
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
                var dbtopic = _context.Topics.FirstOrDefault(x => x.Id == id);

                dbtopic.Title = topicVM.Title;
                dbtopic.Description = topicVM.Description;
                dbtopic.CategoryId=topicVM.CategoryId;
                dbtopic.Category = _context.Categories.FirstOrDefault(x=>x.Id==topicVM.CategoryId);
                _context.RemoveRange(dbtopic.Tags);
                dbtopic.Tags.AddRange(_context.Tags.Where(t => topicVM.TagIds.Contains(t.Id)).ToList());

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TopicController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var dbtopic = _context.Topics.FirstOrDefault(x=>x.Id==id);

            var category = _context.Categories.FirstOrDefault(x => x.Id == dbtopic.CategoryId);

            var tagids = dbtopic.Tags.Select(x => x.Id);

            var topicvm = new TopicVM
            {
                Id = dbtopic.Id,
                CategoryId = category.Id,
                CategoryName = category.Name,
                TagIds = tagids.ToList(),
                Description = dbtopic.Description,
                PostsCount = dbtopic.Posts.Count(),
                Publish_Date = dbtopic.CreatedAt,
                TagNames = dbtopic.Tags.Select(x => x.Name).ToList(),
                Title = dbtopic.Title,
            };
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
                var dbtopic = _context.Topics.FirstOrDefault(x=>x.Id==id);

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
}
