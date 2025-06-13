using Lib.Models;
using Lib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
        public ActionResult GetAll()
        {
            try
            {
                var list = _context.Topics.Include(t => t.Posts).Include(t => t.Tags);

                return Ok(list);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Get", e.Message, 3);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpGet("{id}")]
        public ActionResult<Topic> Get(int id)
        {
            try
            {
                var dbTopic = _context.Topics.FirstOrDefault(x => x.Id == id);
                if (dbTopic is null) return NotFound();

                return Ok(dbTopic);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Get", e.Message, 3);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


    }
}
