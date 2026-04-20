using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;

namespace Sprint3.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContasController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContasController(AppDbContext context)
    {
        _context = context;
    }

    // --- 1. BUSCAR AS CONTAS (AGORA TRAZENDO OS CARTÕES JUNTO) ---
    [HttpGet("minhas-contas/{cpf}")]
    public async Task<IActionResult> GetContas(string cpf)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.NroCPF == cpf);

        if (cliente == null) return NotFound(new { message = "Cliente não encontrado." });

        var contas = await _context.Contas
            .Where(c => c.ClienteId == cliente.IdeCliente && c.Ativo == true) 
            .Select(c => new {
                id = c.Id,
                numeroConta = c.Id.ToString(), 
                tipoConta = c.TipoConta, 
                saldo = c.Saldo,
                // 👇 NOVO: Buscando os cartões desta conta específica 👇
                cartoes = _context.Cartoes
                    .Where(cartao => cartao.ContaId == c.Id && cartao.Ativo)
                    .Select(cartao => new {
                        numeroCartao = cartao.NumeroCartao,
                        tipoCartao = cartao.TipoCartao, // 👈 INCLUIR ESTA LINHA
                        via = cartao.Via,
                        dataVencimento = cartao.DataVencimento
                    }).ToList()
            })
            .ToListAsync();

        return Ok(contas);
    }

    // --- CLASSE AUXILIAR PARA RECEBER DADOS DO FRONT ---
    public class CriarContaRequest 
    {
        public string Cpf { get; set; } = string.Empty;
        public string TipoConta { get; set; } = string.Empty;
        public double SaldoInicial { get; set; }
    }

    // --- 2. CRIAR CONTA USANDO A SUA STORED PROCEDURE ---
    [HttpPost("criar")]
    public async Task<IActionResult> CriarConta([FromBody] CriarContaRequest request)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.NroCPF == request.Cpf);
        if (cliente == null) return NotFound(new { message = "Cliente não encontrado." });

        // 👇 NOVA VALIDAÇÃO: Verifica se o cliente já possui esse tipo de conta ATIVA
        var contaExistente = await _context.Contas
            .AnyAsync(c => c.ClienteId == cliente.IdeCliente && c.TipoConta == request.TipoConta && c.Ativo == true);

        if (contaExistente)
        {
            return BadRequest(new { message = $"Você já possui uma Conta {request.TipoConta} aberta." });
        }

        await _context.Database.ExecuteSqlRawAsync(
            "CALL spInsConta({0}, {1}, {2})", 
            request.SaldoInicial, request.TipoConta, cliente.IdeCliente
        );

        return Ok(new { message = $"Conta {request.TipoConta} criada com sucesso!" });
    }
}