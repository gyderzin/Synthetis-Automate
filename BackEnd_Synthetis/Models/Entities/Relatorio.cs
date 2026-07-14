using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd_Synthetis.Models;

[Table("relatorios")]
public class Relatorio
{
    [Column("id")]
    public int Id { get; set; }

    [Column("modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Column("emissor")]
    public string Emissor { get; set; } = string.Empty;

    [Column("equipe")]
    public string Equipe { get; set; } = string.Empty;

    [Column("nome_arquivo")]
    public string NomeArquivo { get; set; } = string.Empty;

    [Column("item_pendente")]
    public string? ItemPendente { get; set; }

    [Column("caminho_arquivo")]
    public string CaminhoArquivo { get; set; } = string.Empty;

    [Column("emitido_em")]
    public DateTime EmitidoEm { get; set; }
}