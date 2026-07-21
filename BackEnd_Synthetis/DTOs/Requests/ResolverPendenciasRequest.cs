using System.Text.Json.Serialization;

namespace BackEnd_Synthetis.DTOs.Requests;

public class ResolverPendenciasRequest
{
    [JsonPropertyName("relatorio_id")]
    public int RelatorioId { get; set; }

    [JsonPropertyName("imagens")]
    public Dictionary<string, string> Imagens
        { get; set; } = new();
}