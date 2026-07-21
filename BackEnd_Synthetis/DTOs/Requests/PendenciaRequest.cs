<<<<<<< HEAD
namespace BackEnd_Synthetis.DTOs.Requests;

public class PendenciaRequest
{
    public string Titulo { get; set; }
        = string.Empty;

    public string? Descricao { get; set; }

    public string? Descrição
    {
        get => Descricao;
        set => Descricao = value;
    }

    public string? Imagem { get; set; }
=======
namespace BackEnd_Synthetis.DTOs.Requests;

public class PendenciaRequest
{
    public string Titulo { get; set; }
        = string.Empty;

    public string? Descricao { get; set; }

    public string? Descrição
    {
        get => Descricao;
        set => Descricao = value;
    }

    public string? Imagem { get; set; }
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}