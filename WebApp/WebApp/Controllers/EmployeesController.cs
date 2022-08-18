using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMongoCollection<Employee> _collection;

        public EmployeesController(IOptions<AppSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Employee>(settings.Value.CollectionName);
        }

        [HttpPost("addEmployee")]
        public async Task<IActionResult> Insert([FromBody] Employee emp)
        {
            await _collection.InsertOneAsync(emp);
            return Ok();
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _collection.Find(_ => true).ToListAsync();
            var employeesDto = new List<EmployeesDTO>();
            employees.ForEach(emp =>
            {
                employeesDto.Add(new EmployeesDTO() { Name = emp.Name, Surname = emp.Surname});
            });
            return Ok(employeesDto);
        }

        [HttpGet("getByName")]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var emp = await _collection.Find(emp => emp.Name == name).FirstOrDefaultAsync();
            return Ok(new EmployeesDTO() {Name = emp.Name, Surname = emp.Surname});
        }
    }
}
