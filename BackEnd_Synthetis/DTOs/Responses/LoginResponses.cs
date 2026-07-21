<<<<<<< HEAD
namespace BackEnd_Synthetis.DTOs.Responses;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public UsuarioResponse Usuario { get; set; } = null!;
=======
namespace BackEnd_Synthetis.DTOs.Responses;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public UsuarioResponse Usuario { get; set; } = null!;
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}