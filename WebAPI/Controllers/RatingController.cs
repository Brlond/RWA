using AutoMapper;
using Lib.Models;
using Lib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;
using WebApp.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly RwaContext _context;
        private readonly ILogService _logger;
        private readonly IMapper _mapper;


        public RatingController(RwaContext context, ILogService logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public ActionResult<RatingDTO> Post(RatingDTO rating) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Rating/Post", $"Modelstate Isnt Valid", 1);
                    return BadRequest(ModelState);
                }

                var post = _context.Posts.FirstOrDefault(x => x.Id == rating.PostId);
                var user = _context.Users.FirstOrDefault(x=>x.Username == rating.UserName);
                if (user is null || post is null)
                {
                    _logger.LogError("User Error in Rating/Post", $"User tried to rate unkown post or user doesnt exist", 1);
                    return NotFound();
                }
                var dbrating = new Rating()
                {
                    Post = post,
                    PostId = post.Id,
                    Score = rating.Score,
                    User = user,
                    UserId = user.Id
                };
                

                _context.Ratings.Add(dbrating);
                _context.SaveChanges();

                rating.Id = dbrating.Id;

                return Ok(rating);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Topic/Post", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }

        }
    }
}
