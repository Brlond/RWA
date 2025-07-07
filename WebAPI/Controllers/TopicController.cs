using AutoMapper;
using Lib.Models;
using Lib.Services;
using Lib.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using WebApp.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {

        private readonly RwaContext _context;
        private readonly ILogService _logger;
        private readonly IMapper _mapper;

        public TopicController(RwaContext context, ILogService logService, IMapper mapper)
        {
            _context = context;
            _logger = logService;
            _mapper = mapper;
        }



        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<IEnumerable<TopicView>> GetAll()
        {
            try
            {
                var dbtopics = _context.Topics.Include(t => t.Tags).Include(t => t.Category).Include(t => t.Posts).ThenInclude(x => x.User);
                var topics = _mapper.Map<List<TopicView>>(dbtopics);    
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
        public ActionResult<IEnumerable<TopicView>> Search(string searchPart, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var dbtopics = _context.Topics.Include(t => t.Tags).Include(t => t.Category).Include(t => t.Posts).ThenInclude(x => x.User).Skip((pageNumber - 1) * pageSize).Take(pageSize).Where(x => x.Title.Contains(searchPart));
                var topics = _mapper.Map<List<TopicView>>(dbtopics);
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
                var view = _mapper.Map<TopicView>(dbTopic);

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
                if (_context.Categories.Any(x => x.Name == topic.Title))
                {
                    _logger.LogError("User Error in Topic/Post", $"User tried to insert duplicate Topic", 1);
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Topic/Post", $"Modelstate Isnt Valid", 1);
                    return BadRequest(ModelState);
                }
               
                var dbtopic = _mapper.Map<Topic>(topic);
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
                var dbtopic = _context.Topics.Include(x=>x.Posts).FirstOrDefault(x => x.Id == id);
                var dbratings = _context.Ratings.Where(r => _context.Posts.Any(p => p.Id == r.PostId && p.TopicId == id)).ToList();
                var dbposts = _context.Posts.Where(x=>x.Topic.Id==id);
                if (dbtopic is null)
                {
                    _logger.LogError("User Error in Topic/Delete", $"User tried to delete topic of id={id}", 1);
                    return NotFound();
                }
                _context.Ratings.RemoveRange(dbratings);
                _context.Posts.RemoveRange(dbposts);
                _context.Topics.Remove(dbtopic);
                _context.SaveChanges();

                TopicDTO topic =_mapper.Map<TopicDTO>(dbtopic);

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
