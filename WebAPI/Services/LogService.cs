using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public class LogService : ILogService
    {
        private readonly RwaContext _context;

        public LogService(RwaContext context)
        {
            _context = context;
        }

        public void LogError(string message, string? errorText = null, int? severity = 1)
        {
            var log = new Log
            {
                DateOf = DateTime.Now,
                Message = message,
                Severity = severity,
                ErrorText = errorText,
            };
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
