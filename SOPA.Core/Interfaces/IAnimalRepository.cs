using SOPA.Core.Models;

namespace SOPA.Core.Interfaces
{
    public interface IAnimalRepository
    {
        Task<IEnumerable<Animal>> ObterTodosAsync();
        Task<Animal?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Animal animal);
        Task AtualizarAsync(Animal animal);
        Task DeletarAsync(int id);
    }
}