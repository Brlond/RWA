using Lib.Models;
using Lib.Services;
using Lib.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApp.DTO;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {

        private readonly RwaContext _context;

        private readonly ILogService _logger;
        public TopicController(RwaContext context, ILogService logService)
        {
            _context = context;
            _logger = logService;
        }



        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<IEnumerable<TopicView>> GetAll()
        {
            try
            {
                var dbtopics = _context.Topics.Include(t => t.Tags).Include(t => t.Category).Include(t => t.Posts).ThenInclude(x => x.User);
                List<TopicView> topics = new List<TopicView>();
                foreach (var item in dbtopics)
                {
                    var view = new TopicView()
                    {
                        CategoryName = item.Category.Name,
                        CreatedAt = item.CreatedAt.Value,
                        Description = item.Description,
                        Id = item.Id,
                        Posts = item.Posts.Select(x => new PostPreview() { Id = x.Id, Content = x.Content, PostedAt = x.PostedAt.Value, Username = x.User.Username }).ToList(),
                        TagNames = item.Tags.Select(x => x.Name).ToList(),
                        Title = item.Title
                    };
                    topics.Add(view);
                }
                return Ok(topics);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Topic/GetAll", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<IEnumerable<TopicView>> Search(string searchPart)
        {
            try
            {
                var dbtopics = _context.Topics.Include(t => t.Tags).Include(t => t.Category).Include(t => t.Posts).ThenInclude(x => x.User);
                dbtopics.Where(x => x.Title.Contains(searchPart));
                List<TopicView> topics = new List<TopicView>();
                foreach (var item in dbtopics)
                {
                    var view = new TopicView()
                    {
                        CategoryName = item.Category.Name,
                        CreatedAt = item.CreatedAt.Value,
                        Description = item.Description,
                        Id = item.Id,
                        Posts = item.Posts.Select(x => new PostPreview() { Id = x.Id, Content = x.Content, PostedAt = x.PostedAt.Value, Username = x.User.Username }).ToList(),
                        TagNames = item.Tags.Select(x => x.Name).ToList(),
                        Title = item.Title
                    };
                    topics.Add(view);
                }

                return Ok(topics);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Topic/Search", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<TopicView> Get(int id)
        {
            try
            {
                var dbTopic = _context.Topics.Include(t => t.Tags).Include(t => t.Category).Include(t => t.Posts).ThenInclude(x => x.User).FirstOrDefault(x => x.Id == id); ;
                if (dbTopic is null) 
                {
                    _logger.LogError("User Error in Topic/Get", $"User tried to get topic of id={id}", 1);
                    return NotFound(); 
                }
                var view = new TopicView()
                {
                    CategoryName = dbTopic.Category.Name,
                    CreatedAt = dbTopic.CreatedAt.Value,
                    Description = dbTopic.Description,
                    Id = dbTopic.Id,
                    Posts = dbTopic.Posts.Select(x => new PostPreview() { Id = x.Id, Content = x.Content, PostedAt = x.PostedAt.Value, Username = x.User.Username }).ToList(),
                    TagNames = dbTopic.Tags.Select(x => x.Name).ToList(),
                    Title = dbTopic.Title
                };

                return Ok(view);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Topic/Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<TopicDTO> Put(int id, TopicDTO topic)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Topic/Put", $"Modelstate Isnt Valid", 1);

                    return BadRequest(ModelState);
                }

                var dbTopic = _context.Topics.Include(x => x.Tags).Include(x => x.Posts).FirstOrDefault(x => x.Id == id);

                if (dbTopic is null)
                {
                    _logger.LogError("User Error in Topic/Put", $"User tried to put topic of id={id}", 1);
                    return NotFound();
                }
                var dbtags = _context.Tags;
                List<Tag> tags = new List<Tag>();
                foreach (var item in topic.TagIds)
                {

                    Tag bah = dbtags.FirstOrDefault(x => x.Id == item);
                    if (bah is not null)
                    {
                        tags.Add(bah);
                    }

                }
                dbTopic.Title = topic.Title;
                dbTopic.Description = topic.Description;
                dbTopic.CategoryId = topic.CategoryId;
                dbTopic.Category = _context.Categories.FirstOrDefault(x => x.Id == dbTopic.CategoryId);
                dbTopic.Tags = tags;

                _context.SaveChanges();

                return Ok(topic);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Topic/Put", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpPost()]
        [Authorize]
        public ActionResult<TopicDTO> Post(TopicDTO topic)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Topic/Post", $"Modelstate Isnt Valid", 1);
                    return BadRequest(ModelState);
                }

                var dbtags = _context.Tags;
                List<Tag> tags = new List<Tag>();
                foreach (var item in topic.TagIds)
                {

                    Tag bah = dbtags.FirstOrDefault(x => x.Id == item);
                    if (bah is not null)
                    {
                        tags.Add(bah);
                    }

                }

                var dbtopic = new Topic
                {
                    Title = topic.Title,
                    Description = topic.Description,
                    CategoryId = topic.CategoryId,
                    Category = _context.Categories.FirstOrDefault(x => x.Id == topic.CategoryId),
                    Tags = tags,
                };
                _context.Topics.Add(dbtopic);
                _context.SaveChanges();

                return Ok(topic);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Topic/Post", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<TopicDTO> Delete(int id)
        {
            try
            {
                var dbtopic = _context.Topics.FirstOrDefault(x => x.Id == id);
                if (dbtopic is null)
                {
                    _logger.LogError("User Error in Topic/Delete", $"User tried to delete topic of id={id}", 1);
                    return NotFound();
                }

                _context.Topics.Remove(dbtopic);
                _context.SaveChanges();

                TopicDTO topic = new TopicDTO
                {
                    Title = dbtopic.Title,
                    Description = dbtopic.Description,
                    CategoryId = dbtopic.CategoryId,
                    TagIds = dbtopic.Tags.Select(x => x.Id).ToList()
                };

                return Ok(topic);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Topic/Delete", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }

        }

    }
}
