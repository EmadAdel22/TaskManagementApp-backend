using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.Models;



namespace TaskManagement.Controllers
{
    
        [Route("api/[controller]")]
        [ApiController]
        public class TasksController : ControllerBase
        {
            private readonly AppDbContext _context;

            public TasksController(AppDbContext context)
            {
                _context = context;
            }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByid(int id)
        {
             var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound("not found id");
            return Ok(task);

        }

        [HttpPost]
            public async Task<IActionResult> Create(TaskItems task)
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return Ok(task);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, TaskItems task)
            {
                if (id != task.Id)
                    return BadRequest("this id not exist");

                _context.Entry(task).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(task);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                    return NotFound("not found id");

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    
}
