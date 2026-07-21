<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd_Synthetis.Models;

[Table("modelos")]
public class Modelo
{
    [Column("id")]
    public int Id { get; set; }

    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("equipe")]
    public string? Equipe { get; set; }

    [Column("descriçao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("modelo_automacao")]
    public string ModeloAutomacao { get; set; } = string.Empty;

    [Column("documento_modelo")]
    public byte[] DocumentoModelo { get; set; } = [];

    [Column("termografia")]
    public bool Termografia { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [Column("atualizado_em")]
    public DateTime AtualizadoEm { get; set; }
=======
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd_Synthetis.Models;

[Table("modelos")]
public class Modelo
{
    [Column("id")]
    public int Id { get; set; }

    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("equipe")]
    public string? Equipe { get; set; }

    [Column("descriçao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("modelo_automacao")]
    public string ModeloAutomacao { get; set; } = string.Empty;

    [Column("documento_modelo")]
    public byte[] DocumentoModelo { get; set; } = [];

    [Column("termografia")]
    public bool Termografia { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [Column("atualizado_em")]
    public DateTime AtualizadoEm { get; set; }
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}