using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Cors;

namespace EmployeeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors]

public class EmployeeController : ControllerBase
{
    
    private readonly AppDbContext _context;
    public EmployeeController(AppDbContext context)
    {
         _context = context;
    }
    [HttpGet]
    public IActionResult Get()
    {
        var employees = _context.Employees.ToList();
        return Ok(employees);
    }
    [HttpGet("{id}")]
     public IActionResult GetById(int id)
     {
         var employee = _context.Employees.Find(id);
         if (employee == null)
             return NotFound();


         return Ok(employee);
     }
     [HttpPost]
     public IActionResult Create([FromBody] EmployeeModel employee)
     {
         _context.Employees.Add(employee);
         _context.SaveChanges();
         return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
     }
     [HttpPut("{id}")]
     public IActionResult Update(int id, [FromBody] EmployeeModel updatedEmployee)
     {
         var existingEmployee = _context.Employees.Find(id);
         if (existingEmployee == null)
             return NotFound();


         existingEmployee.firstName = updatedEmployee.firstName;
         existingEmployee.lastname = updatedEmployee.lastname;
         existingEmployee.position = updatedEmployee.position;
         // Update other properties


         _context.SaveChanges();
         return NoContent();
     }
     [HttpPatch("{id}")]
     public IActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<EmployeeModel> patchDoc){
        if(patchDoc==null || id<=0){
            BadRequest();
        }
        var existingEmployee = _context.Employees.Where(s => s.Id == id).FirstOrDefault();
        if(existingEmployee == null){
            return NotFound();
        }
        var emp = new EmployeeModel{
            Id = existingEmployee.Id,
            firstName = existingEmployee.firstName,
            lastname = existingEmployee.lastname,
            position = existingEmployee.position,
        };
        if(patchDoc != null){
            patchDoc.ApplyTo(emp, ModelState);
        }

        if(ModelState.IsValid){
            return BadRequest(ModelState);
        }
        existingEmployee.firstName = emp.firstName;
        existingEmployee.lastname = emp.lastname;
        existingEmployee.position = emp.position;
        _context.SaveChanges();
        return NoContent();
     }
     [HttpDelete("{id}")]
     public IActionResult Delete(int id)
     {
         var employeeToRemove = _context.Employees.Find(id);
         if (employeeToRemove == null)
             return NotFound();
             _context.Employees.Remove(employeeToRemove);
             _context.SaveChanges();
             return NoContent();
     }
}