using BCrypt.Net;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories;

namespace Sprint3.Services;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ListarTodosAsync();
    Task<Cliente?> ObterPorIdAsync(long id); // Na interface do service, pode deixar assim mesmo
    Task AdicionarAsync(ClienteCreateDTO dto);
    Task AtualizarAsync(ClienteUpdateDTO dto);
    Task DeletarAsync(long id);
}

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Cliente>> ListarTodosAsync() => await _repository.GetAllAsync();

    public async Task<Cliente?> ObterPorIdAsync(long id) 
    {
        // 👇 AQUI ESTAVA O ERRO! Agora ele chama o nome correto do repositório 👇
        return await _repository.GetByIdAsync(id);
    }

    public async Task AdicionarAsync(ClienteCreateDTO dto)
    {
        string hashSeguro = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
        
        // 👇 Remove pontos, traços e espaços do CPF
        string cpfNumerico = new string(dto.NroCPF.Where(char.IsDigit).ToArray());

        var cliente = new Cliente {
            NomCliente = dto.NomCliente, 
            NroCPF = cpfNumerico, // 👈 Salva apenas números
            NomEndereco = dto.NomEndereco, 
            DtcNascimento = dto.DtcNascimento,
            SenhaHash = hashSeguro,
            Role = dto.Role ?? "CLIENTE"
        };

        await _repository.CreateAsync(cliente);
    }

    public async Task AtualizarAsync(ClienteUpdateDTO dto)
    {
        var cliente = new Cliente {
            IdeCliente = dto.IdeCliente, 
            NomCliente = dto.NomCliente, 
            NroCPF = dto.NroCPF, 
            NomEndereco = dto.NomEndereco, 
            DtcNascimento = dto.DtcNascimento
        };
        await _repository.UpdateAsync(cliente);
    }

    public async Task DeletarAsync(long id) => await _repository.DeleteAsync(id);
}