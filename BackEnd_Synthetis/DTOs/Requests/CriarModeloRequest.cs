using Microsoft.AspNetCore.Mvc;

namespace BackEnd_Synthetis.DTOs.Requests;

public class CriarModeloRequest
{
    [FromForm(Name = "titulo")]
    public string Titulo { get; set; } = string.Empty;

    [FromForm(Name = "equipe")]
    public string Equipe { get; set; } = string.Empty;

    [FromForm(Name = "descriçao")]
    public string Descricao { get; set; } = string.Empty;

    [FromForm(Name = "modelo_automacao")]
    public string ModeloAutomacao { get; set; } = string.Empty;

    [FromForm(Name = "documento_modelo")]
    public IFormFile DocumentoModelo { get; set; } = null!;

    [FromForm(Name = "termografia")]
    public bool Termografia { get; set; }
}