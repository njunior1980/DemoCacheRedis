using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DemoCacheRedis.Features.v1.Employees
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employee = await _employeeRepository.GetEmployees();
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}