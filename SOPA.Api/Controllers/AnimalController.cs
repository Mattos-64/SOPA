using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOPA.Api.Data;
using SOPA.Core.Models;
namespace SOPA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimaisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimaisController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Animais
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimais()
        {
            return await _context.Animais
                .Where(a => a.TenantId == 1)
                .ToListAsync();
        }

        // GET: api/Animais/5
        [HttpGet("{Id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _context.Animais.FindAsync(id);

            if (animal == null || animal.TenantId != 1)
            {
                return NotFound(" !! Animal não identificado !! ");
            }

            return animal;
        }

        // POST: api/Animais
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<Animal>> PostAnimal([FromBody] Animal animal)
        {
            // Forçamos o TenantId como 1 por segurança, ignorando o que vier do front por enquanto
            animal.TenantId = 1;
            animal.DataCadastro = DateTime.Now;

            _context.Animais.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        // PUT: api/Animais/5
        [HttpPut("{Id}")]
        public async Task<ActionResult> PutAnimal(int id, Animal animal)
        {
            if (id != animal.Id)
            {
                return BadRequest("ID informado não coincide ");
            }

            if (animal.TenantId != 1)
            {
                return Unauthorized("Acesso negado para este registro ");
            }

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/Animais/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animais.FindAsync(id);
            if (animal == null || animal.TenantId != 1)
            {
                return NotFound();
            }
            _context.Animais.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnimalExists(int id)
        {
            return _context.Animais.Any(e => e.Id == id && e.TenantId == 1);
        }
    }
}
