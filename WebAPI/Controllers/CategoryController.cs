using Lib.Models;
using Lib.Services;
using Lib.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly RwaContext _context;

        private readonly ILogService _logger;

        public CategoryController(RwaContext context, ILogService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<CategoryView>> GetAll()
        {
            try
            {
                var list = _context.Categories.Include(t => t.Topics).ToList();
                List<CategoryView> lista = new List<CategoryView>();
                foreach (var item in list) 
                {
                    var categoryDTO = new CategoryView()
                    {
                        Name = item.Name,
                        TopicTitles = item.Topics.Select(t => t.Title).ToList(),
                        Id=item.Id,
                    };
                    lista.Add(categoryDTO);

                }
                return Ok(lista);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Category/GetAll", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CategoryView> Get(int id)
        {
            try
            {
                var dbCategory = _context.Categories.FirstOrDefault(t => t.Id == id);
                if (dbCategory is null)
                {
                    _logger.LogError("User Error in Category/Get", $"User tried to get category of id = {id}", 1);
                    return NotFound();
                }
                var category = new CategoryView()
                {
                    Id = dbCategory.Id,
                    Name  = dbCategory.Name,
                    TopicTitles = dbCategory.Topics.Select(x => x.Title).ToList(),
                };

                return Ok(category);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Category/Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


        [HttpPut("{id}")]
        public ActionResult<CategoryDTO> Put(int id, CategoryDTO post)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Category/Put", $"Modelstate isnt valid", 1);
                    return BadRequest();
                }

                var dbcategory = _context.Categories.FirstOrDefault(x => x.Id == id);
                if (dbcategory is null)
                {
                    _logger.LogError("User Error in Category/Put", $"User tried to put category of id = {id}", 1);
                    return NotFound();
                }

                dbcategory.Name = post.Name;

                _context.SaveChanges();

                post.Id = dbcategory.Id;
                post.Name = dbcategory.Name;

                return Ok(post);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Category/Put", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }


        }


        [HttpPost()]
        public ActionResult<CategoryDTO> Post(CategoryDTO category)
        {
            try
            {
                if (_context.Categories.Any(x=>x.Name==category.Name))
                {
                    _logger.LogError("User Error in Category/Post", $"User tried to insert duplicate Category", 1);
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Category/Post", $"Modelstate isnt valid", 1);
                    return BadRequest();
                }
                var dbCategory = new Category
                {
                    Name = category.Name,
                };


                var topics = _context.Topics.Where(x => x.Category.Name == category.Name);
                dbCategory.Topics = topics.ToList();
                _context.Categories.Add(dbCategory);
                _context.SaveChanges();

                var catdto = new CategoryDTO()
                {
                    Name = category.Name,
                    Id = category.Id,
                };


                return Ok(catdto);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Category/Post", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpDelete("{id}")]
        public ActionResult<CategoryDTO> Delete(int id)
        {
            try
            {
                var dbcategory = _context.Categories.Include(c => c.Topics).FirstOrDefault(x => x.Id == id);

                if (dbcategory is null)
                {
                    _logger.LogError("User Error in Category/Delete", $"User tried to delete category of id = {id}", 1);
                    return NotFound();
                }
                _context.Categories.Remove(dbcategory);
                _context.SaveChanges();

                return new CategoryDTO
                {
                    Id = dbcategory.Id,
                    Name = dbcategory.Name,
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Category/Delete", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }
    }
}
