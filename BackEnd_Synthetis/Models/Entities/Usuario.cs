<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd_Synthetis.Models;

[Table("usuarios")]
public class Usuario
{
    [Column("id")]
    public int Id { get; set; }

    [Column("NomeUsuario")]
    public string NomeUsuario { get; set; } = string.Empty;

    [Column("senha")]
    public string Senha { get; set; } = string.Empty;

    [Column("equipe")]
    public string? Equipe { get; set; }

    [Column("acesso")]
    public int Acesso { get; set; }
=======
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd_Synthetis.Models;

[Table("usuarios")]
public class Usuario
{
    [Column("id")]
    public int Id { get; set; }

    [Column("NomeUsuario")]
    public string NomeUsuario { get; set; } = string.Empty;

    [Column("senha")]
    public string Senha { get; set; } = string.Empty;

    [Column("equipe")]
    public string? Equipe { get; set; }

    [Column("acesso")]
    public int Acesso { get; set; }
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}