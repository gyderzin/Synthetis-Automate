<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using BackEnd_Synthetis.Data;
using BackEnd_Synthetis.Models;
namespace BackEnd_Synthetis.Services;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using BackEnd_Synthetis.DTOs.Requests;
using System.Text.Json;

public class RelatorioService
{
    private readonly AppDbContext _context;
    private readonly DocumentoService _documentoService;
    private readonly IConfiguration _configuration;

    public RelatorioService(
        AppDbContext context,
        DocumentoService documentoService,
        IConfiguration configuration
    )
    {
        _context = context;
        _documentoService = documentoService;
        _configuration = configuration;
    }

    // --------------------------------------------------------
    // RECUPERAR RELATÓRIOS
    // --------------------------------------------------------
    public async Task<object>
        RecuperarRelatorios(
            string currentUser,
            string? equipe
        )
    {
        var query =
            _context.Relatorios
                .Where(r => r.Emissor == currentUser);

        if (!string.IsNullOrEmpty(equipe))
        {
            query =
                query.Where(r => r.Equipe == equipe);
        }

        var relatorios =
            await query
                .OrderByDescending(r => r.EmitidoEm)
                .ToListAsync();

        if (!relatorios.Any())
        {
            return false;
        }

        return relatorios.Select(r => new
        {
            id = r.Id,
            modelo = r.Modelo,
            emissor = r.Emissor,
            equipe = r.Equipe,
            nome_arquivo = r.NomeArquivo,
            emitido_em = r.EmitidoEm,
            item_pendente = r.ItemPendente
        });
    }

    // --------------------------------------------------------
    // GERAR RELATÓRIO
    // --------------------------------------------------------
    public async Task<object>
        GerarRelatorio(
            GerarRelatorioRequest request
        )
    {
        var modelo =
            await _context.Modelos.FindAsync(
                request.ModeloId
            );

        if (
            modelo == null
            || modelo.DocumentoModelo == null
        )
        {
            return new
            {
                erro =
                    "Modelo não encontrado"
            };
        }

        // --------------------------------------------------------
        // CARREGA DOCX
        // --------------------------------------------------------
        var stream =
            new MemoryStream();

        await stream.WriteAsync(
            modelo.DocumentoModelo
        );

        stream.Position = 0;

        // --------------------------------------------------------
        // SUBSTITUI TEXTOS
        // --------------------------------------------------------
        _documentoService.SubstituirTexto(
            stream,
            request.Dados
        );

        // --------------------------------------------------------
        // SUBSTITUI IMAGENS
        // --------------------------------------------------------
        _documentoService.SubstituirImagens(
            stream,
            request.Dados
        );

        // --------------------------------------------------------
        // INSERE PENDÊNCIAS
        // --------------------------------------------------------
        _documentoService.InserirPendencias(
            stream,
            request.Pendencias,
            request.ChavePendencia ?? ""
        );

        // --------------------------------------------------------
        // OBTÉM DIRETÓRIO DE ARMAZENAMENTO
        // --------------------------------------------------------
        var pasta =
            _configuration["Storage:Relatorios"]
            ?? Path.Combine(
                AppContext.BaseDirectory,
                "relatorios"
            );

        // --------------------------------------------------------
        // GARANTE QUE A PASTA EXISTE
        // --------------------------------------------------------
        Directory.CreateDirectory(pasta);

        // --------------------------------------------------------
        // GERA NOME FÍSICO
        // --------------------------------------------------------
        var nomeArquivoFisico =
            $"{Guid.NewGuid()}.docx";

        var caminhoFinal =
            Path.Combine(
                pasta,
                nomeArquivoFisico
            );

        // --------------------------------------------------------
        // SALVA DOCUMENTO
        // --------------------------------------------------------
        await File.WriteAllBytesAsync(
            caminhoFinal,
            stream.ToArray()
        );

        // --------------------------------------------------------
        // NOME EXIBIÇÃO
        // --------------------------------------------------------
        var nomeRelatorio =
            !string.IsNullOrWhiteSpace(
                request.NomeRelatorio
            )
                ? request.NomeRelatorio
                : modelo.Titulo;

        // --------------------------------------------------------
        // SERIALIZA ITENS PENDENTES
        // --------------------------------------------------------
        var itensPendentesJson =
            request.ItensPendentes != null
                ? JsonSerializer.Serialize(
                    request.ItensPendentes
                )
                : null;

        // --------------------------------------------------------
        // SALVA BANCO
        // --------------------------------------------------------
        var novoRelatorio =
            new Relatorio
            {
                Modelo =
                    modelo.Titulo,

                Emissor =
                    request.Responsavel,

                Equipe =
                    modelo.Equipe
                    ?? string.Empty,

                NomeArquivo =
                    nomeRelatorio,

                CaminhoArquivo =
                    caminhoFinal,

                ItemPendente =
                    itensPendentesJson,

                EmitidoEm =
                    DateTime.UtcNow
                        .AddHours(-4)
            };

        _context.Relatorios.Add(
            novoRelatorio
        );

        await _context.SaveChangesAsync();

        return new
        {
            status = "ok",

            mensagem =
                "Relatório armazenado no disco com sucesso"
        };
    }
    // EXCLUIR RELATÓRIOS
    public async Task<bool> ExcluirRelatorio(
    int id,
    string usuario
)
    {
        var relatorio =
            await _context.Relatorios
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.Emissor == usuario);

        if (relatorio == null)
        {
            return false;
        }

        var caminho =
    relatorio.CaminhoArquivo;

        if (
            !string.IsNullOrWhiteSpace(caminho)
            &&
            File.Exists(caminho)
        )
        {
            File.Delete(caminho);
        }

        _context.Relatorios.Remove(relatorio);

        await _context.SaveChangesAsync();

        return true;
    }
    // RESOLVER PENDENCIAS
    public async Task<Relatorio?>
        ObterRelatorioParaDownload(
            int relatorioId,
            string currentUser
        )
    {
        var relatorio =
            await _context.Relatorios
                .FirstOrDefaultAsync(
                    r => r.Id == relatorioId
                );

        if (relatorio == null)
        {
            return null;
        }

        if (relatorio.Emissor != currentUser)
        {
            throw new UnauthorizedAccessException(
                "Acesso negado"
            );
        }

        return relatorio;
    }
    public async Task<object>
    ResolverPendencias(
        ResolverPendenciasRequest request,
        string currentUser
    )
    {
        var relatorio =
            await _context.Relatorios
                .FirstOrDefaultAsync(
                    r =>
                        r.Id == request.RelatorioId
                        && r.Emissor == currentUser
                );

        if (relatorio == null)
        {
            return new
            {
                erro = "Relatório não encontrado"
            };
        }

        var caminho =
     relatorio.CaminhoArquivo;

        if (
            string.IsNullOrWhiteSpace(caminho)
            ||
            !File.Exists(caminho)
        )
        {
            return new
            {
                erro = "Arquivo não encontrado"
            };
        }

        Console.WriteLine(
            $"ARQUIVO: {caminho}"
         );

        // =========================================
        // ABRE DOCUMENTO
        // =========================================

        using var document =
      WordprocessingDocument.Open(
          caminho,
          true
      );

        var body =
            document.MainDocumentPart?
                .Document?
                .Body;

        if (body == null)
        {
            return new
            {
                erro = "Body não encontrado"
            };
        }

        // =========================================
        // LISTA TODOS OS PARÁGRAFOS
        // =========================================

        var paragraphs =
            body.Descendants<Paragraph>();

        foreach (var item in request.Imagens)
        {
            var chaveRecebida =
                item.Key;

            var imagemBase64 =
                item.Value;

            Console.WriteLine(
                $"CHAVE RECEBIDA FRONT: {chaveRecebida}"
            );

            var placeholder =
             $"{{{{{chaveRecebida}}}}}"
                 .Replace("–", "-")
                 .Replace("—", "-");

            Console.WriteLine(
                $"PLACEHOLDER GERADO: {placeholder}"
            );

            foreach (var paragraph in paragraphs)
            {
                var texto =
                    paragraph.InnerText
                        .Replace("–", "-")
                        .Replace("—", "-");

                Console.WriteLine(
                    $"PARAGRAFO: {texto}"
                );

                if (
                    texto.Contains(
                        placeholder
                    )
                )
                {
                    Console.WriteLine(
                        "PLACEHOLDER ENCONTRADO!"
                    );

                    _documentoService
                        .InserirImagemNoParagrafo(
                            paragraph,
                            document.MainDocumentPart!,
                            imagemBase64
                        );
                }
            }
        }

        document.MainDocumentPart?
    .Document?
    .Save();

        document.Dispose();

        // =========================================
        // ATUALIZA BANCO
        // =========================================

        relatorio.ItemPendente = null;

        relatorio.EmitidoEm =
            DateTime.UtcNow
                .AddHours(-4);

        await _context.SaveChangesAsync();

        return new
        {
            status = "ok"
        };
    }
=======
using Microsoft.EntityFrameworkCore;
using BackEnd_Synthetis.Data;
using BackEnd_Synthetis.Models;
namespace BackEnd_Synthetis.Services;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using BackEnd_Synthetis.DTOs.Requests;
using System.Text.Json;

public class RelatorioService
{
    private readonly AppDbContext _context;
    private readonly DocumentoService _documentoService;
    private readonly IConfiguration _configuration;

    public RelatorioService(
        AppDbContext context,
        DocumentoService documentoService,
        IConfiguration configuration
    )
    {
        _context = context;
        _documentoService = documentoService;
        _configuration = configuration;
    }

    // --------------------------------------------------------
    // RECUPERAR RELATÓRIOS
    // --------------------------------------------------------
    public async Task<object>
        RecuperarRelatorios(
            string currentUser,
            string? equipe
        )
    {
        var query =
            _context.Relatorios
                .Where(r => r.Emissor == currentUser);

        if (!string.IsNullOrEmpty(equipe))
        {
            query =
                query.Where(r => r.Equipe == equipe);
        }

        var relatorios =
            await query
                .OrderByDescending(r => r.EmitidoEm)
                .ToListAsync();

        if (!relatorios.Any())
        {
            return false;
        }

        return relatorios.Select(r => new
        {
            id = r.Id,
            modelo = r.Modelo,
            emissor = r.Emissor,
            equipe = r.Equipe,
            nome_arquivo = r.NomeArquivo,
            emitido_em = r.EmitidoEm,
            item_pendente = r.ItemPendente
        });
    }

    // --------------------------------------------------------
    // GERAR RELATÓRIO
    // --------------------------------------------------------
    public async Task<object>
        GerarRelatorio(
            GerarRelatorioRequest request
        )
    {
        var modelo =
            await _context.Modelos.FindAsync(
                request.ModeloId
            );

        if (
            modelo == null
            || modelo.DocumentoModelo == null
        )
        {
            return new
            {
                erro =
                    "Modelo não encontrado"
            };
        }

        // --------------------------------------------------------
        // CARREGA DOCX
        // --------------------------------------------------------
        var stream =
            new MemoryStream();

        await stream.WriteAsync(
            modelo.DocumentoModelo
        );

        stream.Position = 0;

        // --------------------------------------------------------
        // SUBSTITUI TEXTOS
        // --------------------------------------------------------
        _documentoService.SubstituirTexto(
            stream,
            request.Dados
        );

        // --------------------------------------------------------
        // SUBSTITUI IMAGENS
        // --------------------------------------------------------
        _documentoService.SubstituirImagens(
            stream,
            request.Dados
        );

        // --------------------------------------------------------
        // INSERE PENDÊNCIAS
        // --------------------------------------------------------
        _documentoService.InserirPendencias(
            stream,
            request.Pendencias,
            request.ChavePendencia ?? ""
        );

        // --------------------------------------------------------
        // OBTÉM DIRETÓRIO DE ARMAZENAMENTO
        // --------------------------------------------------------
        var pasta =
            _configuration["Storage:Relatorios"]
            ?? Path.Combine(
                AppContext.BaseDirectory,
                "relatorios"
            );

        // --------------------------------------------------------
        // GARANTE QUE A PASTA EXISTE
        // --------------------------------------------------------
        Directory.CreateDirectory(pasta);

        // --------------------------------------------------------
        // GERA NOME FÍSICO
        // --------------------------------------------------------
        var nomeArquivoFisico =
            $"{Guid.NewGuid()}.docx";

        var caminhoFinal =
            Path.Combine(
                pasta,
                nomeArquivoFisico
            );

        // --------------------------------------------------------
        // SALVA DOCUMENTO
        // --------------------------------------------------------
        await File.WriteAllBytesAsync(
            caminhoFinal,
            stream.ToArray()
        );

        // --------------------------------------------------------
        // NOME EXIBIÇÃO
        // --------------------------------------------------------
        var nomeRelatorio =
            !string.IsNullOrWhiteSpace(
                request.NomeRelatorio
            )
                ? request.NomeRelatorio
                : modelo.Titulo;

        // --------------------------------------------------------
        // SERIALIZA ITENS PENDENTES
        // --------------------------------------------------------
        var itensPendentesJson =
            request.ItensPendentes != null
                ? JsonSerializer.Serialize(
                    request.ItensPendentes
                )
                : null;

        // --------------------------------------------------------
        // SALVA BANCO
        // --------------------------------------------------------
        var novoRelatorio =
            new Relatorio
            {
                Modelo =
                    modelo.Titulo,

                Emissor =
                    request.Responsavel,

                Equipe =
                    modelo.Equipe
                    ?? string.Empty,

                NomeArquivo =
                    nomeRelatorio,

                CaminhoArquivo =
                    caminhoFinal,

                ItemPendente =
                    itensPendentesJson,

                EmitidoEm =
                    DateTime.UtcNow
                        .AddHours(-4)
            };

        _context.Relatorios.Add(
            novoRelatorio
        );

        await _context.SaveChangesAsync();

        return new
        {
            status = "ok",

            mensagem =
                "Relatório armazenado no disco com sucesso"
        };
    }
    // EXCLUIR RELATÓRIOS
    public async Task<bool> ExcluirRelatorio(
    int id,
    string usuario
)
    {
        var relatorio =
            await _context.Relatorios
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.Emissor == usuario);

        if (relatorio == null)
        {
            return false;
        }

        var caminho =
    relatorio.CaminhoArquivo;

        if (
            !string.IsNullOrWhiteSpace(caminho)
            &&
            File.Exists(caminho)
        )
        {
            File.Delete(caminho);
        }

        _context.Relatorios.Remove(relatorio);

        await _context.SaveChangesAsync();

        return true;
    }
    // RESOLVER PENDENCIAS
    public async Task<Relatorio?>
        ObterRelatorioParaDownload(
            int relatorioId,
            string currentUser
        )
    {
        var relatorio =
            await _context.Relatorios
                .FirstOrDefaultAsync(
                    r => r.Id == relatorioId
                );

        if (relatorio == null)
        {
            return null;
        }

        if (relatorio.Emissor != currentUser)
        {
            throw new UnauthorizedAccessException(
                "Acesso negado"
            );
        }

        return relatorio;
    }
    public async Task<object>
    ResolverPendencias(
        ResolverPendenciasRequest request,
        string currentUser
    )
    {
        var relatorio =
            await _context.Relatorios
                .FirstOrDefaultAsync(
                    r =>
                        r.Id == request.RelatorioId
                        && r.Emissor == currentUser
                );

        if (relatorio == null)
        {
            return new
            {
                erro = "Relatório não encontrado"
            };
        }

        var caminho =
     relatorio.CaminhoArquivo;

        if (
            string.IsNullOrWhiteSpace(caminho)
            ||
            !File.Exists(caminho)
        )
        {
            return new
            {
                erro = "Arquivo não encontrado"
            };
        }

        Console.WriteLine(
            $"ARQUIVO: {caminho}"
         );

        // =========================================
        // ABRE DOCUMENTO
        // =========================================

        using var document =
      WordprocessingDocument.Open(
          caminho,
          true
      );

        var body =
            document.MainDocumentPart?
                .Document?
                .Body;

        if (body == null)
        {
            return new
            {
                erro = "Body não encontrado"
            };
        }

        // =========================================
        // LISTA TODOS OS PARÁGRAFOS
        // =========================================

        var paragraphs =
            body.Descendants<Paragraph>();

        foreach (var item in request.Imagens)
        {
            var chaveRecebida =
                item.Key;

            var imagemBase64 =
                item.Value;

            Console.WriteLine(
                $"CHAVE RECEBIDA FRONT: {chaveRecebida}"
            );

            var placeholder =
             $"{{{{{chaveRecebida}}}}}"
                 .Replace("–", "-")
                 .Replace("—", "-");

            Console.WriteLine(
                $"PLACEHOLDER GERADO: {placeholder}"
            );

            foreach (var paragraph in paragraphs)
            {
                var texto =
                    paragraph.InnerText
                        .Replace("–", "-")
                        .Replace("—", "-");

                Console.WriteLine(
                    $"PARAGRAFO: {texto}"
                );

                if (
                    texto.Contains(
                        placeholder
                    )
                )
                {
                    Console.WriteLine(
                        "PLACEHOLDER ENCONTRADO!"
                    );

                    _documentoService
                        .InserirImagemNoParagrafo(
                            paragraph,
                            document.MainDocumentPart!,
                            imagemBase64
                        );
                }
            }
        }

        document.MainDocumentPart?
    .Document?
    .Save();

        document.Dispose();

        // =========================================
        // ATUALIZA BANCO
        // =========================================

        relatorio.ItemPendente = null;

        relatorio.EmitidoEm =
            DateTime.UtcNow
                .AddHours(-4);

        await _context.SaveChangesAsync();

        return new
        {
            status = "ok"
        };
    }
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}