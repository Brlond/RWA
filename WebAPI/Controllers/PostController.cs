using AutoMapper;
using Lib.Models;
using Lib.Services;
using Lib.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using WebAPI.Controllers;
using WebApp.DTO;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly RwaContext _context;
        private readonly ILogService _logger;
        private readonly IMapper _mapper;

        public PostController(RwaContext context, ILogService logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<IEnumerable<PostView>> GetAll()
        {
            try
            {
                var dbposts = _context.Posts.Include(t => t.Ratings).Include(x => x.User).Include(x => x.Topic);
                List<PostView> posts = new List<PostView>();
                foreach (var post in dbposts)
                {
                    var postview = _mapper.Map<PostView>(post);
                    posts.Add(postview);
                }

                return Ok(posts);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/GetAll", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }




        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<IEnumerable<PostView>> Search(string searchPart, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var dbposts = _context.Posts.Include(x => x.Topic).Include(x => x.User).Include(x => x.Ratings).Skip((pageNumber - 1) * pageSize).Take(pageSize).Where(x => x.Content.Contains(searchPart));
                var posts = _mapper.Map<List<PostView>>(dbposts);

                return Ok(posts);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Search", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<PostView> Get(int id)
        {
            try
            {
                var dbpost = _context.Posts.Include(x => x.Topic).Include(x => x.User).Include(x => x.Ratings).FirstOrDefault(t => t.Id == id);
                if (dbpost is null)
                {
                    _logger.LogError("User Error in Post/Get", $"User tried to get post of id = {id}", 1);
                    return NotFound();
                }

                var post = _mapper.Map<PostView>(dbpost);

                return Ok(post);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<PostDTO> Put(int id, PostDTO post)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Post/Put", $"Modelstate isnt valid", 1);
                    return BadRequest();
                }

                var dbPost = _context.Posts.Include(x => x.Topic).Include(x => x.User).Include(x => x.Ratings).FirstOrDefault(x => x.Id == id);
                var topic = _context.Topics.FirstOrDefault(x => x.Id == post.TopicId);
                if (dbPost is null || topic is null)
                {
                    _logger.LogError("User Error in Post/Put", $"User tried to put post of id = {id}", 1);
                    return NotFound();
                }

                dbPost.Content = post.Content;
                dbPost.TopicId = post.TopicId;
                dbPost.UserId = post.UserId;

                var removable = dbPost.Ratings.Where(x => !post.Scores.Contains(x.Score.Value));
                foreach (var rating in removable)
                {
                    _context.Ratings.Remove(rating);
                }

                var existingposts = dbPost.Ratings.Select(x => x.Score.Value);
                var newPosts = post.Scores.Except(existingposts);
                foreach (var npost in newPosts)
                {
                    var dbrating = _context.Ratings.FirstOrDefault(x => npost == x.Score.Value);

                    if (dbrating is null)
                        continue;

                    dbPost.Ratings.Add(new Rating
                    {
                        Score = dbrating.Score,
                        Post = dbPost,
                    });
                }
                _context.SaveChanges();


                var scores = dbPost.Ratings.Select(x => x.Score.Value);
                var postview = _mapper.Map<PostView>(dbPost);
                return Ok(post);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Put", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }


        }

        [HttpPost("[action]")]
        [Authorize]
        public ActionResult<PostDTO> Post(PostDTO post)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("User Error in Post/Post", $"Modelstate isnt valid", 1);
                    return BadRequest();
                }
                var dbuser = _context.Users.FirstOrDefault(x => x.Id == post.UserId);
                var dbtopic = _context.Topics.FirstOrDefault(x => x.Id == post.TopicId);
                var dbpost = _mapper.Map<Post>(post);
                _context.Posts.Add(dbpost);
                _context.SaveChanges();
                post.Id = dbpost.Id;
                return Ok(post);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Post", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult<PostDTO> Delete(int id)
        {
            try
            {
                var dbPost = _context.Posts.FirstOrDefault(x => x.Id == id);
                if (dbPost is null)
                {
                    _logger.LogError("User Error in Post/Get", $"User tried to delete post of id = {id}", 1);
                    return NotFound();
                }

                var ratings = _context.Ratings.Where(x => x.PostId == id);
                var post = _mapper.Map<PostDTO>(dbPost);
                foreach (var rating in ratings)
                {
                    _context.Ratings.Remove(rating);
                }

                _context.Posts.Remove(dbPost);

                _context.SaveChanges();

                return Ok(post);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Delete", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



    }
}
