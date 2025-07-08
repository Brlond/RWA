using AutoMapper;
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
        private readonly IMapper _mapper;


        public TagController(RwaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: TagController
        public ActionResult Index()
        {
            var tags = _context.Tags.Include(x => x.Topics).ThenInclude(x => x.Posts).Select(x => _mapper.Map<TagVM>(x));
            return View(tags);
        }

        // GET: TagController/Details/5
        public ActionResult Details(int id)
        {
            var dbtag = _context.Tags.Include(x=> x.Topics).ThenInclude(x=>x.Posts).FirstOrDefault(x => x.Id == id);
            var tagvm = _mapper.Map<TagVM>(dbtag);
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
        public ActionResult Create(TagVM tagvm,string action)
        {
            try
            {
                if (_context.Tags.Any(x=>x.Name==tagvm.Name))
                {
                    ViewBag.ErrorMessage= "Tag with this name already exists.";
                    return View("Create");
                }
                var genre = _mapper.Map<Tag>(tagvm);
                _context.Tags.Add(genre);
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

        // GET: TagController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var dbtag = _context.Tags.Include(x => x.Topics).ThenInclude(x => x.Posts).FirstOrDefault(x => x.Id == id);
                var tagVm = _mapper.Map<TagVM>(dbtag);
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
                var dbtag = _context.Tags.Include(x => x.Topics).ThenInclude(x => x.Posts).FirstOrDefault(x => x.Id == id);
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
            var dbtag = _context.Tags.Include(x => x.Topics).ThenInclude(x => x.Posts).FirstOrDefault(x => x.Id == id);
            var tag = _mapper.Map<TagVM>(dbtag);
            return View(tag);
        }

        // POST: TagController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TagVM tag)
        {
            try
            {
                var dbtag = _context.Tags.Include(x => x.Topics).ThenInclude(x => x.Posts).FirstOrDefault(x => x.Id == id);
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
