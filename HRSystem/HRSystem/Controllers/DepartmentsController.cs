using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRSystem.Data;
using HRSystem.Models.DTOs;

namespace HRSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly HRContext _context;

        public DepartmentsController(HRContext context)
        {
            _context = context;
        }

        // ────────────────────────────────────────────────────────────
        // GET /api/departments
        // Returns all departments with their employees.
        // ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departments = await _context.Departments
     .Include(d => d.Employees)
     .ToListAsync();

            var result = departments.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Location = d.Location,
                Employees = d.Employees.Select(e => new EmployeeSummaryDto
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    JobTitle = e.JobTitle,
                    DepartmentName = d.Name
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // ────────────────────────────────────────────────────────────
        // GET /api/departments/{id}
        // Returns a single department with its employees.
        // ────────────────────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _context.Departments
    .Include(d => d.Employees)
    .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
                return NotFound(new { message = $"Department with ID {id} was not found." });

            var result = new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Location = department.Location,
                Employees = department.Employees.Select(e => new EmployeeSummaryDto
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    JobTitle = e.JobTitle,
                    DepartmentName = department.Name
                }).ToList()
            };

            return Ok(result);
        }
    }
}
