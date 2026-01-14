using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.EF;    // Pagination

namespace EmployeeManagementSystem.Controllers
{
    // Only Admins can access this controller
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly EmployeeDbContext _context;

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        // ====================================================================
        // GET: Employees
        // List all non-deleted employees, with optional search and pagination
        // ====================================================================
        public async Task<IActionResult> Index(string search, int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var employeesQuery = _context.Employees
                .Where(e => !e.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                employeesQuery = employeesQuery.Where(e =>
                    e.FullName.Contains(search) ||
                    e.Department.Contains(search));
            }

            employeesQuery = employeesQuery.OrderBy(e => e.FullName);

            var pagedEmployees = await employeesQuery.ToPagedListAsync(pageNumber, pageSize);

            ViewBag.CurrentFilter = search;
            return View(pagedEmployees);
        }

        // ============================================================
        // GET: Employees/Trash
        // Show all soft-deleted employees (with search + pagination)
        // ============================================================
        public async Task<IActionResult> Trash(string search, int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var deletedQuery = _context.Employees
                .Where(e => e.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                deletedQuery = deletedQuery.Where(e =>
                    e.FullName.Contains(search) ||
                    e.Department.Contains(search));
            }

            deletedQuery = deletedQuery.OrderBy(e => e.FullName);

            var pagedDeleted = await deletedQuery.ToPagedListAsync(pageNumber, pageSize);

            ViewBag.CurrentFilter = search;
            return View(pagedDeleted);
        }

        // ==================================
        // GET: Employees/Details/5
        // ==================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // =======================================
        // GET: Employees/Create
        // =======================================
        public IActionResult Create()
        {
            return View();
        }

        // =======================================
        // POST: Employees/Create
        // =======================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Department,Position,Email,Salary")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.CreatedBy = User.Identity.Name;
                employee.CreatedAt = DateTime.UtcNow;

                _context.Add(employee);

                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Action = "Created Employee",
                    EntityName = employee.FullName,
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // =========================================
        // GET: Employees/Edit/5
        // =========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // =========================================
        // POST: Employees/Edit/5
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Department,Position,Email,Salary")] Employee employee)
        {
            if (id != employee.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.Employees.FindAsync(id);
                    if (existingEmployee == null)
                        return NotFound();

                    existingEmployee.FullName = employee.FullName;
                    existingEmployee.Department = employee.Department;
                    existingEmployee.Position = employee.Position;
                    existingEmployee.Email = employee.Email;
                    existingEmployee.Salary = employee.Salary;

                    _context.ActivityLogs.Add(new ActivityLog
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Action = "Updated Employee",
                        EntityName = existingEmployee.FullName,
                        Timestamp = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // =======================================
        // GET: Employees/Delete/5
        // =======================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // =======================================
        // POST: Employees/Delete/5
        // =======================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            employee.IsDeleted = true;

            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Action = "Deleted Employee",
                EntityName = employee.FullName,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =======================================
        // POST: Employees/Restore/5
        // =======================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            employee.IsDeleted = false;

            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Action = "Restored Employee",
                EntityName = employee.FullName,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Trash));
        }

        // ==================================
        // Helper: Check if employee exists
        // ==================================
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
