using Lib.Models;
using Lib.Security;
using Lib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApp.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RwaContext _context;

        private readonly ILogService _logger;

        private readonly IConfiguration _configuration;

        public UserController(RwaContext context, ILogService logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost("[action]")]
        public ActionResult<UserRegisterDTO> Register(UserRegisterDTO register)
        {
            try
            {
                var username = register.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(username)))
                {
                    _logger.LogError("Error in User/Register", "User tried to register with already existing username", 1);
                    return BadRequest($"Username : {username} already exists");
                }

                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(register.Password, b64salt);

                var user = new User
                {
                    Id = register.Id,
                    Username = register.Username,
                    Email = register.Email,
                    FirstName = register.FirstName,
                    IsAdmin = false,
                    LastName = register.LastName,
                    PasswordHash = b64hash,
                    PasswordSalt = b64salt,
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                register.Id = user.Id;

                return Ok(register);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in User/Register", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


        [HttpPost("[action]")]
        public ActionResult<UserLoginDTO> Login(UserLoginDTO login)
        {

            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Username == login.Username);
                if (user is null)
                {
                    _logger.LogError("User Error in User/Login", $"User tried to login using nonexistent username {login.Username}", 1);
                    return Unauthorized("Incorrect username or password");
                }


                var hash = PasswordHashProvider.GetHash(login.Password, user.PasswordSalt);
                if (hash != user.PasswordHash)
                {
                    _logger.LogError("User Error in User/Login", $"User tried to login into {login.Username} with wrong password", 1);
                    return Unauthorized("Incorrect username or password");
                }

                var SKey = _configuration["JWT:SecureKEy"];
                var token = JwtTokenProvider.CreateToken(SKey, 60, login.Username, "User");
                return Ok(token);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in User/Login", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpPut("[action]/{id}")]
        public ActionResult<UserLoginDTO> Edit(int id, [FromBody] UserEditDTO login)
        {

            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == id);
                if (user is null)
                {
                    _logger.LogError("User Error in User/ChangePassword", $"User not found with id = {id}", 1);
                    return NotFound();
                }
                user.FirstName = login.FirstName;
                user.LastName = login.LastName;
                user.Email = login.Email;
                _context.SaveChanges();
                return Ok("User Edited Successfully");

            }
            catch (Exception e)
            {
                _logger.LogError("Error in User/Login", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpPost("[action]/{id}")]
        public ActionResult<UserLoginDTO> ChangePassword(int id, UserChangePWDDTO login)
        {

            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == id);
                if (user is null)
                {
                    _logger.LogError("User Error in User/ChangePassword", $"User not found with id = {id}", 1);
                    return NotFound();
                }
                var hash = PasswordHashProvider.GetHash(login.Password, user.PasswordSalt);
                if (hash != user.PasswordHash)
                {
                    _logger.LogError("User Error in User/ChangePassword", $"User put wrong password", 1);
                    return Unauthorized("Incorrect username or password");
                }
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(login.NewPassword, b64salt);

                user.PasswordSalt = b64salt;
                user.PasswordHash = b64hash;
                _context.SaveChanges();
                return Ok("Password Changed Successfully");

            }
            catch (Exception e)
            {
                _logger.LogError("Error in User/Login", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

    }
}
