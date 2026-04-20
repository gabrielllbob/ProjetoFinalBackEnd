using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services;

namespace Sprint3.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var clientes = await _clienteService.ListarTodosAsync();
        return Ok(clientes);
    }

    // 👇 MÉTODO NOVO ADICIONADO AQUI 👇
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id);
        
        if (cliente == null) 
        {
            return NotFound(new { message = "Cliente não encontrado." });
        }
        
        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ClienteCreateDTO dto)
    {
        try {
            await _clienteService.AdicionarAsync(dto);
            return StatusCode(201, new { message = "Cliente criado com sucesso via Procedure!" });
        } catch (Exception ex) {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ClienteUpdateDTO dto)
    {
        await _clienteService.AtualizarAsync(dto);
        return Ok(new { message = "Cliente atualizado com sucesso!" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _clienteService.DeletarAsync(id);
        return NoContent();
    }
}