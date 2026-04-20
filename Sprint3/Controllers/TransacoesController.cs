using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // 👈 Importante para o SqlQueryRaw
using Sprint3.Data; // 👈 Onde está seu AppDbContext
using Sprint3.DTOs;
using Sprint3.Services;

namespace Sprint3.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _service;
    private readonly AppDbContext _context; // 👈 Adicione o contexto aqui se for chamar direto

    public TransacoesController(ITransacaoService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpPost("realizar")]
    public async Task<IActionResult> RealizarOperacao([FromBody] TransacaoCreateDTO dto)
    {
        try
        {
            await _service.ExecutarTransacaoAsync(dto);
            return Ok(new { message = "Operação realizada com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // 🚀 NOVO ENDPOINT PARA O EXTRATO
    [HttpGet("extrato/{contaId}")]
    public async Task<IActionResult> GetExtrato(long contaId)
    {
        try
        {
            // O SqlQueryRaw mapeia o resultado da Procedure para uma lista
            // Note: Se você não tiver uma classe ExtratoDTO, pode criar uma simples 
            // com as colunas: selTransacao, nroValor, dtcTransacao, clienteOrigem, clienteDestino
            var extrato = await _context.Database
                .SqlQueryRaw<ExtratoDTO>("CALL spSelTransacoesCliente({0})", contaId)
                .ToListAsync();

            return Ok(extrato);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Erro ao buscar extrato: " + ex.Message });
        }
    }
}