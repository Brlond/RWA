using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly RwaContext _context;
        public ValuesController(RwaContext _context)
        {
            this._context = _context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> GetTopics()
        {
            var list = _context.Users.ToList();

            return Ok(list);
            
        }



    }
}
