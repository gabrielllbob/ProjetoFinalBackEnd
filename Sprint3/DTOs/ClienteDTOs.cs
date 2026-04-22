using System.Text.Json.Serialization;
namespace Sprint3.DTOs;

public class ClienteCreateDTO
{
    public string NomCliente { get; set; } = string.Empty;
    public string NroCPF { get; set; } = string.Empty;
    public string NomEndereco { get; set; } = string.Empty;
    public DateTime DtcNascimento { get; set; }
    [JsonPropertyName("senhaHash")] 
    public string Senha { get; set; } = string.Empty;
    public string Role { get; set; } = "CLIENTE";
}

public class ClienteUpdateDTO : ClienteCreateDTO
{
    public long IdeCliente { get; set; }
    
}