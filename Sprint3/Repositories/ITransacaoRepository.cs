namespace Sprint3.Repositories;

public interface ITransacaoRepository
{
    Task RealizarTransacaoAsync(string tipo, decimal valor, long? origem, long? destino);
}