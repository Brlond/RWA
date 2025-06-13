using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Services
{
    public interface ILogService
    {
        void LogError(string message, string? errorText = null, int? severity = null);
    }
}
