using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRSystem.Data;
using HRSystem.Models.DTOs;

namespace HRSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly HRContext _context;

        public EmployeesController(HRContext context)
        {
            _context = context;
        }

        // ────────────────────────────────────────────────────────────
        // GET /api/employees
        // Returns a summary list of all employees.
        // ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeSummaryDto>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Select(e => new EmployeeSummaryDto
                {
                    EmployeeId     = e.EmployeeId,
                    FirstName      = e.FirstName,
                    LastName       = e.LastName,
                    Email          = e.Email,
                    JobTitle       = e.JobTitle,
                    DepartmentName = e.Department!.Name
                })
                .ToListAsync();

            return Ok(employees);
        }

        // ────────────────────────────────────────────────────────────
        // GET /api/employees/{id}
        // Returns full details for a single employee.
        // ────────────────────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDetailDto>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.EmployeeId == id)
                .Select(e => new EmployeeDetailDto
                {
                    EmployeeId     = e.EmployeeId,
                    FirstName      = e.FirstName,
                    LastName       = e.LastName,
                    Email          = e.Email,
                    Phone          = e.Phone,
                    JobTitle       = e.JobTitle,
                    HireDate       = e.HireDate,
                    DepartmentId   = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound(new { message = $"Employee with ID {id} was not found." });

            return Ok(employee);
        }

        // ────────────────────────────────────────────────────────────
        // GET /api/employees/search?name=sarah
        // Searches employees by first or last name (partial match).
        // ────────────────────────────────────────────────────────────
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EmployeeSummaryDto>>> SearchEmployees([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "The 'name' query parameter is required." });

            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name))
                .Select(e => new EmployeeSummaryDto
                {
                    EmployeeId     = e.EmployeeId,
                    FirstName      = e.FirstName,
                    LastName       = e.LastName,
                    Email          = e.Email,
                    JobTitle       = e.JobTitle,
                    DepartmentName = e.Department!.Name
                })
                .ToListAsync();

            return Ok(employees);
        }
    }
}
