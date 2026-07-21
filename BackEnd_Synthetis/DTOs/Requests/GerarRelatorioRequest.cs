using System.Text.Json.Serialization;

namespace BackEnd_Synthetis.DTOs.Requests;

public class GerarRelatorioRequest
{
    [JsonPropertyName("modelo_id")]
    public int ModeloId { get; set; }

    [JsonPropertyName("equipamento")]
    public string Equipamento { get; set; } = string.Empty;

    [JsonPropertyName("dados")]
    public Dictionary<string, object> Dados { get; set; }
        = new();

    [JsonPropertyName("responsavel")]
    public string Responsavel { get; set; }
        = string.Empty;

    [JsonPropertyName("pendencias")]
    public List<PendenciaRequest> Pendencias { get; set; }
        = new();

    [JsonPropertyName("chavePendencia")]
    public string? ChavePendencia { get; set; }

    [JsonPropertyName("itens_pendentes")]
    public object? ItensPendentes { get; set; }

    [JsonPropertyName("nome_relatorio")]
    public string? NomeRelatorio { get; set; }
    
}