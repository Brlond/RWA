using Lib.Models;
using Lib.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogService _logger;

        public LogController(RwaContext context, ILogService logger)
        {
            _context = context;
            _logger = logger;
        }




        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<List<LogDTO>> GetAll()
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError("Error in Log/GetAll", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<List<LogDTO>> Count()
        {
            try
            {
                var count = _context.Logs.Count();
                return Ok(count);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Log/Count", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<PagedResult<LogDTO>> GetSome(int pageNumber, int pageSize)
        {
            try
            {
                var totalLogs = _context.Logs.Count();
                var totalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);

                var logs = _context.Logs
                    .OrderBy(x => x.DateOf)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var list = logs.Select(item => new LogDTO
                {
                    ErrorText = item.ErrorText,
                    Id = item.Id,
                    Message = item.Message,
                    Severity = item.Severity,
                }).ToList();

                var result = new PagedResult<LogDTO>
                {
                    Items = list,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber
                };

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Log/GetSome", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }


        [HttpGet("{n}/{OrderBy}")]
        public ActionResult<List<LogDTO>> Get(int n, int OrderBy)
        {
            try
            {
                if (n <= 0)
                {
                    _logger.LogError("Error in Log/Get/{n}/{OrderBy}", $"User tried to get negative amount of logs", 1);
                    return BadRequest("N must be greater than 0");
                }
                if (OrderBy > 2 || OrderBy < 0)
                {
                    _logger.LogError("Error in Log/Get", $"User tried to sort by invalid argument", 1);
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
            catch (Exception e)
            {
                _logger.LogError("Error in Log/Get", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }



        [HttpPost()]
        public ActionResult<LogDTO> Post(LogDTO log)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Error in Log/Post", $"Modelstate isnt valid", 1);
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

                log.Id = dbLog.Id;
                return Ok(log);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Log/Post", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpPost("many")]
        public ActionResult<LogDTO[]> Post(LogDTO[] logs)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Error in Log/Post/many", $"Modelstate isnt valid", 1);
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
                    item.Id = dbLog.Id;
                }
                return Ok(logs);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Log/Post/many", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }

        [HttpDelete("{n}")]
        public ActionResult<LogDTO> Delete(int n) 
        {
            try
            {
                if (n <= 0)
                {
                    _logger.LogError("Error in Tag/Get", $"User tried to delete first {n} logs", 1);
                    return BadRequest("N must be greater than 0");
                }

                List<Log> logs = _context.Logs.OrderBy(x => x.Id).Take(n).ToList();

                if (!logs.Any()) return NotFound();

                _context.Logs.RemoveRange(logs);
                _context.SaveChanges();


                return Ok(logs);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in Log/Delete/n", e.Message, 5);
                return StatusCode(StatusCodes.Status500InternalServerError, "There has been a problem while fetching the data you requested");
            }
        }
    }
}
