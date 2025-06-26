using Lib.Models;
using Lib.Services;
using Lib.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly RwaContext _context;

        private readonly ILogService _logger;

        public TagController(RwaContext context, ILogService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<IEnumerable<TagView>> GetAll()
        {
            try
            {
                var dbtags = _context.Tags.ToList();
                List<TagView> result = new List<TagView>();
                foreach (var tag in dbtags)
                {
                    var tagview = new TagView
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        TopicTitles = tag.Topics.Select(x => x.Title).ToList(),
                    };
                    result.Add(tagview);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Tag/GetAll", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }

        }

        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<IEnumerable<TagView>> Search(string searchPart)
        {
            try
            {
                var dbtags = _context.Tags.Where(x => x.Name.Contains(searchPart));

                List<TagView> tags = new List<TagView>();
                foreach (var tag in dbtags)
                {
                    var tagview = new TagView
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        TopicTitles = tag.Topics.Select(x => x.Title).ToList(),
                    };
                    tags.Add(tagview);
                }

                return Ok(tags);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Tag/Search", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<TagView> Get(int id)
        {
            try
            {
                var dbTag = _context.Tags.FirstOrDefault(x => x.Id == id);
                if (dbTag is null)
                {
                    _logger.LogError("User Error in Tag/Get", $"User tried to get tag of id={id}", 1);
                    return NotFound();
                }

                var tag = new TagView
                {
                    Id = dbTag.Id,
                    Name = dbTag.Name,
                    TopicTitles = dbTag.Topics.Select(x => x.Title).ToList(),
                };

                return Ok(tag);
            }
            catch (Exception e)
            {
                _logger.LogError("User Error in Tag/Get", e.Message, 3);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }

        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<TagDTO> Put(int id, TagDTO tag)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Tag/Put", $"Modelstate isnt valid", 1);
                    return BadRequest(ModelState);
                }


                var dbTag = _context.Tags.FirstOrDefault(x => x.Id == id);

                if (dbTag is null)
                {
                    _logger.LogError("User Error in Tag/Put", $"User tried to put tag of id={id}", 1);
                    return NotFound();
                }

                dbTag.Name = tag.Name;

                _context.SaveChanges();

                tag.Id = dbTag.Id;
                tag.Name = tag.Name;
                return Ok(tag);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Tag/Put", e.Message, 3);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpPost()]
        [Authorize]
        public ActionResult<TagDTO> Post(TagDTO tag)
        {
            try
            {
                if (_context.Categories.Any(x => x.Name == tag.Name))
                {
                    _logger.LogError("User Error in Tag/Post", $"User tried to insert duplicate Tag", 1);
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Tag/Post", $"Modelstate isnt valid", 1);
                    return BadRequest(ModelState);
                }

                var dbTag = new Tag
                {
                    Name = tag.Name,
                };
                _context.Tags.Add(dbTag);

                _context.SaveChanges();

                tag.Id = dbTag.Id;

                tag.Name = dbTag.Name;
                return Ok(tag);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Tag/Post", e.Message, 3);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<TagDTO> Delete(int id)
        {

            try
            {
                var dbTag = _context.Tags.FirstOrDefault(x => x.Id == id);
                if (dbTag is null)
                {
                    _logger.LogError("User Error in Tag/Delete", $"User tried to delete tag of id={id}", 1);
                    return NotFound();
                }


                _context.Tags.Remove(dbTag);
                _context.SaveChanges();
                var tag = new TagDTO
                {
                    Id = dbTag.Id,
                    Name = dbTag.Name,
                };

                return Ok(tag);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Tag/Delete", e.Message, 3);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }
    }
}
