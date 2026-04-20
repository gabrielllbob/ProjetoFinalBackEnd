using Microsoft.EntityFrameworkCore;
using Sprint3.Data;

namespace Sprint3.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly AppDbContext _context;

    public TransacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task RealizarTransacaoAsync(string tipo, decimal valor, long? origem, long? destino)
    {
        tipo = tipo.Trim();

        await _context.Database.ExecuteSqlRawAsync(
            "CALL spInsTransacao({0}, {1}, {2}, {3})",
            tipo,
            valor,
            origem,
            destino
        );
    }
}