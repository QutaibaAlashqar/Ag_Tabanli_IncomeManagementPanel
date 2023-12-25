using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxesController : ControllerBase
    {
        private readonly DataContext _context;

        public BoxesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/boxes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Box>>> GetBoxes()
        {
            return await _context.Boxes.ToListAsync();
        }

        // GET api/boxes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Box>> GetBox(int id)
        {
            var box = await _context.Boxes.FindAsync(id);
            if (box == null)
            {
                return NotFound();
            }
            return box;
        }

        // POST api/boxes
        [HttpPost]
        public async Task<ActionResult<Box>> PostBox([FromBody] Box box)
        {
            _context.Boxes.Add(box);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBox), new { id = box.BoxID }, box);
        }

        // PUT api/boxes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBox(int id, [FromBody] Box box)
        {
            if (id != box.BoxID)
            {
                return BadRequest();
            }
            _context.Entry(box).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/boxes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBox(int id)
        {
            var box = await _context.Boxes.FindAsync(id);
            if (box == null)
            {
                return NotFound();
            }
            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
