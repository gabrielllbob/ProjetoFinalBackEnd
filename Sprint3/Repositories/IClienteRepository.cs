using Sprint3.Models;

namespace Sprint3.Repositories;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task CreateAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(long id);
    Task<Cliente?> GetByIdAsync(long id);
}