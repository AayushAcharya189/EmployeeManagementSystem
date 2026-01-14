using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Models;
using System.Linq;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly EmployeeDbContext _context;

        public DashboardController(EmployeeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalEmployees = _context.Employees.Count(e => !e.IsDeleted);
            ViewBag.ITCount = _context.Employees.Count(e => e.Department == "IT");
            ViewBag.HRCount = _context.Employees.Count(e => e.Department == "HR");

            return View();
        }
    }

}