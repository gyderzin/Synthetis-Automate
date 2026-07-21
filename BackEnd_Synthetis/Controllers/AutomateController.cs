using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BackEnd_Synthetis.DTOs.Requests;
using BackEnd_Synthetis.Services;

namespace BackEnd_Synthetis.Controllers;

[ApiController]
[Route("automate")]
public class AutomateController : ControllerBase
{
    private readonly RelatorioService _relatorioService;

    public AutomateController(
        RelatorioService relatorioService
    )
    {
        _relatorioService =
            relatorioService;
    }

    // --------------------------------------------------------
    // GERAR DOC
    // --------------------------------------------------------
    [Authorize]
    [HttpPost("gerar-doc")]
    public async Task<IActionResult>
     GerarDocumento(
         [FromBody]
        GerarRelatorioRequest request
     )
    {
        request.Responsavel =
            User.Identity?.Name ?? "";

        var response =
            await _relatorioService
                .GerarRelatorio(request);

        return Ok(response);
    }
    // --------------------------------------------------------
    // BAIXAR RELATÓRIO
    // --------------------------------------------------------
    [Authorize]
    [HttpGet("baixarRelatorio/{relatorioId}")]
    public async Task<IActionResult>
        BaixarRelatorio(
            int relatorioId
        )
    {
        var currentUser =
            User.Identity?.Name;

        if (string.IsNullOrEmpty(currentUser))
        {
            return Unauthorized();
        }

        try
        {
            var relatorio =
                await _relatorioService
                    .ObterRelatorioParaDownload(
                        relatorioId,
                        currentUser
                    );

            if (relatorio == null)
            {
                return NotFound(
                    new
                    {
                        detail =
                            "Relatório não encontrado"
                    });
            }

            if (
                string.IsNullOrEmpty(
                    relatorio.CaminhoArquivo
                )
                || !System.IO.File.Exists(
                    relatorio.CaminhoArquivo
                )
            )
            {
                return NotFound(
                    new
                    {
                        detail =
                            "Arquivo não encontrado no servidor"
                    });
            }

            var nomeDownload =
                relatorio.NomeArquivo;

            if (
                !nomeDownload
                    .ToLower()
                    .EndsWith(".docx")
            )
            {
                nomeDownload += ".docx";
            }

            var stream =
                new FileStream(
                    relatorio.CaminhoArquivo,
                    FileMode.Open,
                    FileAccess.Read
                );

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                nomeDownload
            );
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(
                403,
                new
                {
                    detail = "Acesso negado"
                });
        }
    }
    // --------------------------------------------------------
    // RESOLVER ITENS PENDENTES
    // --------------------------------------------------------
    [HttpPost("resolver-itens-pendentes")]
    public async Task<IActionResult>
 ResolverItensPendentes(
     [FromBody]
    ResolverPendenciasRequest request
 )
    {
        var currentUser =
            User.Identity?.Name;

        if (
            string.IsNullOrWhiteSpace(
                currentUser
            )
        )
        {
            return Unauthorized();
        }

        var resultado =
            await _relatorioService
                .ResolverPendencias(
                    request,
                    currentUser
                );

        return Ok(resultado);
    }
}