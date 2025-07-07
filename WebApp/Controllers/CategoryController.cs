using AutoMapper;
using Lib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.ViewModels;
using System.Data.Common;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly RwaContext _context;
        private readonly IMapper _mapper;


        public CategoryController(RwaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: CategoryController
        public ActionResult Index()
        {
            var categories = _context.Categories.Select(x=> _mapper.Map<CategoryVM>(x)).ToList();
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            var dbcategory = _context.Categories.Include(x=>x.Topics).FirstOrDefault(x => x.Id == id);
            var categoryvm = _mapper.Map<CategoryVM>(dbcategory);

            return View(categoryvm);
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryVM categoryVm,string action)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryVm);
                if (_context.Categories.Any(x => x.Name == categoryVm.Name))
                {
                    ViewBag.ErrorMessage = "Category with this name already exists.";
                    return View("Create"); 
                }
                _context.Categories.Add(category);
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

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var dbcategory = _context.Categories.FirstOrDefault(x => x.Id == id);
                var category = _mapper.Map<CategoryVM>(dbcategory);
                return View(category);
            }
            catch (Exception)
            {

                return View();
            }
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CategoryVM categoryVM)
        {
            try
            {
                var dbcategory = _context.Categories.FirstOrDefault(x => x.Id == id);
                dbcategory.Name= categoryVM.Name;
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var dbcategory = _context.Categories.FirstOrDefault(x => x.Id == id);
                var category = _mapper.Map<CategoryVM>(dbcategory); 
                return View(category);
            }
            catch 
            {
                return View();
            }
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, CategoryVM category)
        {
            try
            {
                var dbcategory = _context.Categories.FirstOrDefault(x =>x.Id == id);
                _context.Categories.Remove(dbcategory);
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
