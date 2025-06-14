using Lib.Models;
using Lib.Services;
using Lib.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.DTO;

namespace WebApp.Controllers
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

        [HttpGet("{id}")]
        public ActionResult<TagView> Get(int id)
        {
            try
            {
                var dbTag = _context.Tags.FirstOrDefault(x => x.Id == id);
                if (dbTag is null)
                {
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
                _logger.LogError("Error in Tag/Get", e.Message, 3);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }

        }

        [HttpPut("{id}")]
        public ActionResult<TagDTO> Put(int id, TagDTO tag)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var dbTag = _context.Tags.FirstOrDefault(x => x.Id == id);

                if (dbTag is null)
                {
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
        public ActionResult<TagDTO> Post(TagDTO tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
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
        public ActionResult<TagDTO> Delete(int id)
        {

            try
            {
                var dbTag = _context.Tags.FirstOrDefault(x => x.Id == id);
                if (dbTag is null)
                {
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
