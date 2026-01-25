using Radzen.Blazor.FileAuditLog.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radzen.Blazor.FileAuditLog.Service;

public class FileAuditLogService : IFileAuditLogService
{
    private readonly string logFilePath;

    public FileAuditLogService()
    {
        // Define log file path
        logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs.txt");
    }

    public async Task LogAsync(string userId, string action, string description)
    {
        var logEntry = $"{DateTime.UtcNow:u} | User: {userId ?? "Anonymous"} | Action: {action} | Description: {description}";

        // Append the log entry to the text file
        await File.AppendAllTextAsync(logFilePath, logEntry + Environment.NewLine);
    }
}