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
}