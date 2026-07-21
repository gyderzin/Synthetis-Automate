namespace BackEnd_Synthetis.DTOs.Responses;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public UsuarioResponse Usuario { get; set; } = null!;
}