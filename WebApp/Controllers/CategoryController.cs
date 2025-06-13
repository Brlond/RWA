using Lib.Models;
using Lib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public ActionResult<IEnumerable<CategoryDTO>> GetAll()
        {
            try
            {
                var list = _context.Categories.Include(t => t.Topics).ToList();
                List<CategoryDTO> lista = new List<CategoryDTO>();
                foreach (var item in list) 
                {
                    var categoryDTO = new CategoryDTO() 
                    {
                        Name = item.Name,
                        Id=item.Id,
                    };
                    lista.Add(categoryDTO);

                }
                return Ok(lista);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> Get(int id)
        {
            try
            {
                var item = _context.Categories.FirstOrDefault(t => t.Id == id);
                if (item is null)
                {
                    _logger.LogError("wrong", "wrong", 1);
                    return NotFound();
                }
                var category = new CategoryDTO()
                {
                    Id = item.Id,
                    Name  = item.Name,
                };

                return Ok(category);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


        [HttpPost()]
        public ActionResult<CategoryDTO> Post(CategoryDTO category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Modelstate isnt valid", " wa ", 1);
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
                _logger.LogError("Error in Get", e.Message, 5);
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
                    _logger.LogError("Modelstate isnt valid", " wa ", 1);
                    return BadRequest();
                }

                var dbcategory = _context.Categories.FirstOrDefault(x => x.Id == id);
                if (dbcategory is null)
                {
                    return NotFound();
                }

                dbcategory.Name= post.Name;

                _context.SaveChanges();

                post.Id= dbcategory.Id;
                post.Name = dbcategory.Name;

                return Ok(post);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Get", e.Message, 5);
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
                    _logger.LogError("Modelstate isnt valid", " wa ", 1);
                    return NotFound();
                }
                if (dbcategory.Topics.Any())
                {
                    _logger.LogError("Modelstate isnt valid", " wa ", 1);
                    return BadRequest();
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
                _logger.LogError("Error in Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }
    }
}
