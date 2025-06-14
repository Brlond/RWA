using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.DTO;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly RwaContext _context;

        public LogController(RwaContext context)
        {
            _context = context;
        }




        [HttpGet("[action]")]
        public ActionResult<List<LogDTO>> GetAll()
        {
            var logs = _context.Logs;
            List<LogDTO> list = new List<LogDTO>();
            foreach (Log item in logs)
            {
                var log = new LogDTO()
                {
                    ErrorText = item.ErrorText,
                    Id = item.Id,
                    Message = item.Message,
                    Severity = item.Severity,
                };
                list.Add(log);
            }
            return Ok(list);
        }



        [HttpGet("{n}/{OrderBy}")]
        public ActionResult<List<LogDTO>> Get(int n, int OrderBy)
        {
            if (n <= 0)
            {
                return BadRequest("N must be greater than 0");
            }
            if (OrderBy > 2 || OrderBy < 0)
            {
                return BadRequest();
            }

            IEnumerable<Log> logs = new List<Log>();

            switch (OrderBy)
            {
                case 0:
                    logs = _context.Logs.ToList().OrderBy(x => x.Id).TakeLast(n);
                    break;
                case 1:
                    logs = _context.Logs.ToList().OrderBy(x => x.DateOf ?? DateTime.MinValue).TakeLast(n);
                    break;
                case 2:
                    logs = _context.Logs.ToList().OrderBy(x => x.Message).TakeLast(n);
                    break;
            }
            List<LogDTO> list = new List<LogDTO>();
            foreach (Log item in logs)
            {
                var log = new LogDTO()
                {
                    ErrorText = item.ErrorText,
                    Id = item.Id,
                    Message = item.Message,
                    Severity = item.Severity,
                };
                list.Add(log);
            }
            return Ok(list);
        }



        [HttpPost()]
        public ActionResult<LogDTO> Post(LogDTO log)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dbLog = new Log() 
            {
                DateOf = DateTime.Now,
                ErrorText = log.ErrorText,
                Severity = log.Severity,
                Message = log.Message,
                Id = log.Id ?? 0
            };
            _context.Logs.Add(dbLog);
            _context.SaveChanges();

            log.Id =  dbLog.Id;
            return Ok(log);
        }

        [HttpPost("many")]
        public ActionResult<LogDTO[]> Post(LogDTO[] logs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (LogDTO item in logs)
            {
                var dbLog = new Log()
                {
                    DateOf = DateTime.Now,
                    ErrorText = item.ErrorText,
                    Severity = item.Severity,
                    Message = item.Message,
                    Id = item.Id ?? 0
                };
                _context.Add(item);
                _context.SaveChanges();
                item.Id=dbLog.Id;
            }
            return Ok(logs);
        }

        [HttpDelete("{n}")]
        public ActionResult<LogDTO> Delete(int n) 
        {
            if (n <=0 )
            {
                return BadRequest("N must be greater than 0");
            }

            List<Log> logs = _context.Logs.OrderBy(x => x.Id).Take(n).ToList();

            if (!logs.Any()) return NotFound();

            _context.Logs.RemoveRange(logs);
            _context.SaveChanges();


            return Ok(logs);
        }
    }
}
