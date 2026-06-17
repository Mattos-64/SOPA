using Microsoft.EntityFrameworkCore;
using SOPA.Api.Data;
using SOPA.Core.Models;
using SOPA.Core.Interfaces;

namespace SOPA.Api.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly AppDbContext _context;

        public AppDbContext Context => _context;

        public AnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Animal>> ObterTodosAsync() => await _context.Animais.ToListAsync();

        public async Task<Animal?> ObterPorIdAsync(int id) => await _context.Animais.FindAsync(id);

        public async Task AdicionarAsync(Animal animal)
        {
            await _context.Animais.AddAsync(animal);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Animal animal)
        {
            _context.Animais.Update(animal);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(int id)
        {
            var animal = await ObterPorIdAsync(id);
            if (animal != null)
            {
                _context.Animais.Remove(animal);
                await _context.SaveChangesAsync();
            }
        }
    }
}