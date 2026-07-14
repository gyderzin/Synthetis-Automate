namespace BackEnd_Synthetis.DTOs.Requests;

public class LoginRequest
{
    public string NomeUsuario { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;
}