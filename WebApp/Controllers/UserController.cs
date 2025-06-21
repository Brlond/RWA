using Lib.Models;
using Lib.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MVC.ViewModels;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly RwaContext _context;

        public UserController(RwaContext context)
        {
            _context = context;
        }

        public IActionResult Login(string returnUrl)
        {
            var Loginvm = new LoginVM
            {
                ReturnUrl = returnUrl,
            };
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVm)
        {
            var existingUser = _context.Users.FirstOrDefault(x => x.Username == loginVm.Username);
            if (existingUser is null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var b64hash = PasswordHashProvider.GetHash(loginVm.Password, existingUser.PasswordSalt);
            if (b64hash != existingUser.PasswordHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }
            string role = existingUser.IsAdmin == true ? "Admin" : "User";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginVm.Username),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            Task.Run(async () => await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties)).GetAwaiter().GetResult();

            if (loginVm.ReturnUrl != null)
                return LocalRedirect(loginVm.ReturnUrl);
            else if (role == "Admin")
                return RedirectToAction("Index", "Topic");
            else if (role == "User")
                return RedirectToAction("Index", "Home");
            else
                return View();

        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserVM userVM)
        {
            try
            {
                var trimmedUsername = userVM.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                {
                    return BadRequest($"Username {trimmedUsername} is taken");
                }

                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userVM.Password, b64salt);

                var user = new User
                {
                    Email = userVM.Email,
                    FirstName = userVM.FirstName,
                    LastName = userVM.LastName,
                    PasswordSalt = b64salt,
                    PasswordHash = b64hash,
                    Username = userVM.Username,
                    IsAdmin = false
                };
                _context.Add(user);
                _context.SaveChanges();
                userVM.Id = user.Id;
                return RedirectToAction("Login", "User");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        public ActionResult Logout()
        {
            Task.Run(async () => await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme)
            ).GetAwaiter().GetResult();

            return View();
        }
        public ActionResult Details()
        {
            string username = HttpContext.User.Identity.Name;
            var dbuser = _context.Users.FirstOrDefault(x => x.Username == username);
            var user = new UserVM
            {
                Id = dbuser.Id,
                Email = dbuser.Email,
                FirstName = dbuser.FirstName,
                LastName = dbuser.LastName,
                Username = dbuser.Username,
            };
            return View(user);
        }
    }
}
