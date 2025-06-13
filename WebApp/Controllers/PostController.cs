using Lib.Models;
using Lib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using WebApp.DTO;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly RwaContext _context;

        private readonly ILogService _logger;
        public PostController(RwaContext context, ILogService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<PostDTO>> GetAll()
        {
            try
            {
                var postss = _context.Posts.Include(t => t.Ratings).Include(x => x.User).Include(x=> x.Topic);
                List<PostDTO> posts = new List<PostDTO>();
                foreach (var post in postss)
                {
                    var postdto = new PostDTO
                    {
                        Id = post.Id,
                        Content = post.Content,
                        TopicId = post.Topic.Id,
                        UserId = post.User.Id,
                        Scores = post.Ratings.Select(r => (int)r.Score).ToList(),

                    };
                    posts.Add(postdto);
                }

                return Ok(posts);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/GetAll", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<PostDTO> Get(int id)
        {
            try
            {
                var dbpost = _context.Posts.FirstOrDefault(t => t.Id == id);
                if (dbpost is null)
                {
                    _logger.LogError("Not Found", "Not Found", 1);
                    return NotFound();
                }

                var post = new PostDTO
                {
                    Id = dbpost.Id,
                    Content = dbpost.Content,
                    TopicId = (int)dbpost.TopicId,
                    UserId = (int)dbpost.UserId,
                    Scores = dbpost.Ratings.Select(r => (int)r.Score).ToList(),
                };

                return Ok(post);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


        [HttpPost()]
        public ActionResult<PostDTO> Post(PostDTO post)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Modelstate isnt valid", " wa ", 1);
                    return BadRequest();
                }

                var dbpost = new Post
                {
                    UserId = post.UserId,
                    Content = post.Content,
                    TopicId = post.TopicId,
                };
                var ratings = _context.Ratings.Where(x => x.Score.HasValue && post.Scores.Contains(x.Score.Value));
                dbpost.Ratings = ratings.Select(x => new Rating { Score = x.Score }).ToList();
                dbpost.User = _context.Users.FirstOrDefault(x => x.Id == dbpost.UserId);
                dbpost.Topic = _context.Topics.FirstOrDefault(x => x.Id == dbpost.TopicId);

                _context.Posts.Add(dbpost);
                _context.SaveChanges();

                var scores = dbpost.Ratings.Select(x => x.Score.Value).ToList();
                post = new PostDTO
                {
                    UserId = (int)dbpost.UserId,
                    Content = dbpost.Content,
                    TopicId = (int)dbpost.TopicId,
                    Id = dbpost.Id,
                    Scores = (List<int>)scores,

                };

                return Ok(post);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Post", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<PostDTO> Put(int id, PostDTO post)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Modelstate isnt valid", " wa ", 1);
                    return BadRequest();
                }

                var dbPost = _context.Posts.FirstOrDefault(x => x.Id == id);

                if (dbPost is null)
                {
                    _logger.LogError("Post not found", "Null reference", 1);
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
                post = new PostDTO
                {
                    Content = dbPost.Content,
                    Id = dbPost.Id,
                    Scores = (List<int>)scores,
                    TopicId = (int)dbPost.TopicId,
                    UserId = (int)dbPost.UserId
                };
                return Ok(post);

            }
            catch (Exception e)
            {
                _logger.LogError("Error in Post/Put", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }


        }

        [HttpDelete("{id}")]
        public ActionResult<PostDTO> Delete(int id)
        {
            try
            {
                var dbPost = _context.Posts.FirstOrDefault(x => x.Id == id);
                if (dbPost is null)
                {
                    _logger.LogError("Post not found", "Null reference", 1);
                    return NotFound();
                }

                var scores = dbPost.Ratings.Select(x => x.Score.Value);
                var ratings = _context.Ratings.Select(x => x.PostId==id);
                var post = new PostDTO
                {
                    Content = dbPost.Content,
                    Id = dbPost.Id,
                    Scores = (List<int>)scores,
                    TopicId = (int)dbPost.TopicId,
                    UserId = (int)dbPost.UserId
                };
                foreach (var rating in ratings) 
                {
                    _context.Remove(rating);
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
