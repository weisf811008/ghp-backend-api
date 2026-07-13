using GhpAPI.Data;
using GhpAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Services
{
    public class HistoryService
    {
        private readonly AppDbContext _db;

        public HistoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task Log(string level, string message, string? username = null, string? name = null, int? schoolId = null, string? type = null, string? controller = null, string? instanceKey = null)
        {
            var history = new History
            {
                Level = level,
                Message = message,
                Type = type,
                Controller = controller,
                InstanceKey = instanceKey,
                Username = username,
                Name = name,
                SchoolId = schoolId,
            };

            _db.Histories.Add(history);
            await _db.SaveChangesAsync();
        }

        public async Task Info(string message, string? username = null, string? name = null, int? schoolId = null, string? type = null, string? controller = null, string? instanceKey = null)
        {
            await Log("info", message, username, name, schoolId, type, controller, instanceKey);
        }

        public async Task Warn(string message, string? username = null, string? name = null, int? schoolId = null, string? type = null, string? controller = null, string? instanceKey = null)
        {
            await Log("warn", message, username, name, schoolId, type, controller, instanceKey);
        }
    }
}
