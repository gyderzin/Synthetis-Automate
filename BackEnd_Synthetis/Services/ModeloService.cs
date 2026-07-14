using Microsoft.EntityFrameworkCore;
using BackEnd_Synthetis.Data;
using BackEnd_Synthetis.DTOs.Responses;
using BackEnd_Synthetis.DTOs.Requests;
using BackEnd_Synthetis.Models;
namespace BackEnd_Synthetis.Services;

public class ModeloService
{
    private readonly AppDbContext _context;

    public ModeloService(AppDbContext context)
    {
        _context = context;
    }

    // --------------------------------------------------------
    // LISTAR MODELOS
    // --------------------------------------------------------
    public async Task<List<ModeloResponse>>
        ListarPorEquipe(string equipe)
    {
        var modelos = await _context.Modelos
            .Where(m => m.Equipe == equipe)
            .ToListAsync();

        return modelos.Select(m => new ModeloResponse
        {
            Id = m.Id,
            Titulo = m.Titulo ?? string.Empty,
            Descricao = m.Descricao ?? string.Empty,
            Equipe = m.Equipe ?? string.Empty,
            ModeloAutomacao = m.ModeloAutomacao ?? string.Empty,
            Termografia = m.Termografia
        }).ToList();
    }
    // --------------------------------------------------------
    // CRIAR MODELO
    // --------------------------------------------------------
    public async Task<object>
        CriarModelo(
            CriarModeloRequest request
        )
    {
        if (
            request.DocumentoModelo == null
            || request.DocumentoModelo.Length == 0
        )
        {
            return new
            {
                erro = "Arquivo não enviado"
            };
        }

        if (
            !request.DocumentoModelo
                .FileName
                .EndsWith(".docx")
        )
        {
            return new
            {
                erro =
                    "O arquivo deve estar no formato .docx"
            };
        }

        byte[] conteudoDoc;

        using (
            var memoryStream =
                new MemoryStream()
        )
        {
            await request.DocumentoModelo
                .CopyToAsync(memoryStream);

            conteudoDoc =
                memoryStream.ToArray();
        }

        var novoModelo = new Modelo
        {
            Titulo = request.Titulo.Trim(),

            Equipe = request.Equipe.Trim(),

            Descricao = request.Descricao.Trim(),

            ModeloAutomacao =
             request.ModeloAutomacao.Trim(),

            DocumentoModelo = conteudoDoc,

            Termografia = request.Termografia,

            CriadoEm = DateTime.UtcNow,

            AtualizadoEm = DateTime.UtcNow
        };

        _context.Modelos.Add(novoModelo);

        await _context.SaveChangesAsync();

        return new
        {
            mensagem =
                "Modelo criado com sucesso",

            id = novoModelo.Id
        };
    }
    public async Task<bool> ExcluirModelo(int id)
    {
        var modelo = await _context.Modelos.FindAsync(id);

        if (modelo == null)
            return false;

        _context.Modelos.Remove(modelo);

        await _context.SaveChangesAsync();

        return true;
    }
}