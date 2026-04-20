using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // 👈 Necessário para usar o AnyAsync()
using Sprint3.Data;
using Sprint3.Models;

namespace Sprint3.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartoesController(AppDbContext context)
    {
        _context = context;
    }

    public class CriarCartaoRequest 
    {
        public long ContaId { get; set; }
        public string TipoCartao { get; set; } = string.Empty;
    }

    [HttpPost("criar")]
    public async Task<IActionResult> CriarCartao([FromBody] CriarCartaoRequest request)
    {
        // 1. VALIDAÇÃO DE DUPLICIDADE (AGORA USANDO A COLUNA NOVA)
        var cartaoExistente = await _context.Cartoes
            .AnyAsync(c => c.ContaId == request.ContaId 
                        && c.TipoCartao == request.TipoCartao // 👈 Valida direto pela coluna nova
                        && c.Ativo == true);

        if (cartaoExistente)
        {
            return BadRequest(new { message = $"Esta conta já possui um Cartão de {request.TipoCartao} ativo." });
        }

        // 2. Gera o número do cartão
        var random = new Random();
        string numeroGerado = "";
        for (int i = 0; i < 12; i++) 
        {
            numeroGerado += random.Next(0, 10).ToString();
        }

        // 3. Monta o objeto (incluindo o TipoCartao)
        var novoCartao = new Cartao 
        {
            ContaId = request.ContaId,
            NumeroCartao = numeroGerado,
            TipoCartao = request.TipoCartao, // 👈 Salva o tipo na nova coluna
            Via = 1,
            Limite = request.TipoCartao == "Credito" ? 1000 : 0,
            Ativo = true,
            DataCriacao = DateTime.Now,
            DataVencimento = DateTime.Now.AddYears(5)
        };

        _context.Cartoes.Add(novoCartao);
        await _context.SaveChangesAsync();

        return Ok(new { 
            message = $"Cartão de {request.TipoCartao} criado com sucesso!", 
            cartao = novoCartao 
        });
    }
}