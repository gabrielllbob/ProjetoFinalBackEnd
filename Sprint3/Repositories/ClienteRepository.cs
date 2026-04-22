using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;

namespace Sprint3.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.ToListAsync();
    }

    // 👇 O nome tem que ser igual ao da interface 👇
    public async Task<Cliente?> GetByIdAsync(long id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task CreateAsync(Cliente c)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "CALL spInsCliente({0}, {1}, {2}, {3}, {4}, {5})", 
            c.NomCliente, c.NroCPF, c.NomEndereco, c.DtcNascimento, c.SenhaHash, c.Role);
    }

    public async Task UpdateAsync(Cliente c)
    {
        await _context.Database.ExecuteSqlRawAsync(
            "CALL spEdtCliente({0}, {1}, {2}, {3}, {4})", 
            c.IdeCliente, c.NomCliente, c.NroCPF, c.NomEndereco, c.DtcNascimento);
    }

    public async Task DeleteAsync(long id)
    {
        await _context.Database.ExecuteSqlRawAsync("CALL spDelCliente({0})", id);
    }
}