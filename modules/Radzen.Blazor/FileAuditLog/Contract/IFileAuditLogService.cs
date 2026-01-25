using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radzen.Blazor.FileAuditLog.Contract;

public interface IFileAuditLogService
{
    Task LogAsync(string userId, string action, string description);
}