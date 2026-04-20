namespace Sprint3.DTOs;

public class ClienteCreateDTO
{
    public string NomCliente { get; set; } = string.Empty;
    public string NroCPF { get; set; } = string.Empty;
    public string NomEndereco { get; set; } = string.Empty;
    public DateTime DtcNascimento { get; set; }
    
    // 🔐 Adicione estes dois campos para o cadastro funcionar:
    public string Senha { get; set; } = string.Empty; 
    public string Role { get; set; } = "CLIENTE";
}

public class ClienteUpdateDTO : ClienteCreateDTO
{
    public long IdeCliente { get; set; }
    
    // Dica: No Update, você pode decidir se a senha é obrigatória ou não.
    // Se não for trocar a senha na edição geral, pode deixar como opcional.
}