using GhpAPI.Data;
using GhpAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly AppDbContext _db;
        protected readonly HistoryService _historyService;

        public BaseController(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        protected int GetSchoolId()
        {
            return int.Parse(User.FindFirst("schoolId")!.Value);
        }

        protected string GetUsername()
        {
            return User.FindFirst("username")!.Value;
        }

        protected string GetName()
        {
            return User.FindFirst("userName")!.Value ?? "";
        }
    }
}