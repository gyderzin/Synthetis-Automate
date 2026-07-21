namespace BackEnd_Synthetis.DTOs.Responses;

public class UsuarioResponse
{
    public int Id { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;

    public string? Equipe { get; set; }

    public int Acesso { get; set; }
}