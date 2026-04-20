using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService; // 👈 Injeta o serviço

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.NroCPF == dto.Cpf);
        
        if (cliente == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, cliente.SenhaHash)) 
            return Unauthorized(new { message = "CPF ou senha inválidos." });

        // 🔐 Agora gera o token real!
        var token = _jwtService.GerarToken(cliente.NroCPF, cliente.Role.ToString());

        return Ok(new {
            token = token, 
            role = cliente.Role.ToString(),
            nome = cliente.NomCliente,
            cpf = cliente.NroCPF
        });
    }
}