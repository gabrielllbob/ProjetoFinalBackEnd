using Sprint3.DTOs;
using Sprint3.Repositories;

namespace Sprint3.Services;

public interface ITransacaoService
{
    Task ExecutarTransacaoAsync(TransacaoCreateDTO dto);
}

public class TransacaoService : ITransacaoService
{
    private readonly ITransacaoRepository _repository;

    public TransacaoService(ITransacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecutarTransacaoAsync(TransacaoCreateDTO dto)
    {
        await _repository.RealizarTransacaoAsync(
            dto.TipoTransacao,
            dto.Valor,
            dto.ContaOrigemId,
            dto.ContaFinalId
        );
    }
}