using Azure;
using Lib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.ViewModels;
using NuGet.Protocol;

namespace MVC.Controllers
{

    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        private readonly RwaContext _context;

        public TagController(RwaContext context)
        {
            _context = context;
        }

        // GET: TagController
        public ActionResult Index()
        {
            var tags = _context.Tags.Select(x => new TagVM { Name = x.Name, Id = x.Id, TopicCount = x.Topics.Count, });
            return View(tags);
        }

        // GET: TagController/Details/5
        public ActionResult Details(int id)
        {
            var dbtag = _context.Tags.Include(x=> x.Topics).FirstOrDefault(x => x.Id == id);
            var tagvm = new TagVM { Id = dbtag.Id, Name = dbtag.Name , TopicCount = dbtag.Topics.Count, };
            return View(tagvm);
        }

        // GET: TagController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TagController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TagVM tagvm)
        {
            try
            {
                if (_context.Tags.Any(x=>x.Name==tagvm.Name))
                {
                    return BadRequest("Tag Already exists");
                }
                var genre = new Tag
                {
                    Name = tagvm.Name,
                };
                _context.Tags.Add(genre);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TagController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var dbtag = _context.Tags.FirstOrDefault(x => x.Id == id);
                var tagVm = new TagVM
                {
                    Id = dbtag.Id,
                    Name = dbtag.Name,
                };
                return View(tagVm);
            }
            catch (Exception e)
            {

                return View();
            }

        }

        // POST: TagController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TagVM tag)
        {
            try
            {
                var dbtag = _context.Tags.FirstOrDefault(x => x.Id == id);
                dbtag.Name = tag.Name;
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TagController/Delete/5
        public ActionResult Delete(int id)
        {
            var dbtag = _context.Tags.FirstOrDefault(x => x.Id == id);
            var tag = new TagVM
            {
                Name = dbtag.Name,
                Id = dbtag.Id,
                TopicCount = dbtag.Topics.Count,
            };
            return View(tag);
        }

        // POST: TagController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TagVM tag)
        {
            try
            {
                var dbtag = _context.Tags.FirstOrDefault(x => x.Id == id);
                _context.Tags.Remove(dbtag);
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
