namespace BackEnd_Synthetis.DTOs.Responses;

public class ModeloResponse
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string Equipe { get; set; } = string.Empty;

    public string ModeloAutomacao { get; set; } = string.Empty;

    public bool Termografia { get; set; }
}