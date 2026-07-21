using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BackEnd_Synthetis.DTOs.Responses;
using BackEnd_Synthetis.Services;
using BackEnd_Synthetis.DTOs.Requests;

namespace BackEnd_Synthetis.Controllers;

[ApiController]
[Route("app")]
public class AppController : ControllerBase
{
    private readonly ModeloService _modeloService;

    private readonly RelatorioService _relatorioService;

    private readonly WordTemplateService _wordService;

    public AppController(
        ModeloService modeloService,
        RelatorioService relatorioService,
        WordTemplateService wordService
    )
    {
        _modeloService = modeloService;
        _relatorioService = relatorioService;
        _wordService = wordService;
    }

    // --------------------------------------------------------
    // LISTA MODELOS
    // --------------------------------------------------------
    [Authorize]
    [HttpGet("modelos")]
    public async Task<IActionResult> ListarModelos(
        [FromQuery] string equipe
    )
    {
        var response =
            await _modeloService
                .ListarPorEquipe(equipe);

        return Ok(response);
    }

    // --------------------------------------------------------
    // RECUPERA RELATÓRIOS
    // --------------------------------------------------------
    [Authorize]
    [HttpGet("recuperarRelatorios")]
    public async Task<IActionResult>
        RecuperarRelatorios(
            [FromQuery] string? equipe
        )
    {
        var currentUser =
            User.Identity?.Name;

        if (string.IsNullOrEmpty(currentUser))
        {
            return Unauthorized();
        }

        var response =
            await _relatorioService
                .RecuperarRelatorios(
                    currentUser,
                    equipe
                );

        return Ok(response);
    }

    // --------------------------------------------------------
    // EXTRAI VARIÁVEIS DO WORD
    // --------------------------------------------------------
    [Authorize]
    [HttpPost("extrair-variaveis")]
    public IActionResult ExtrairVariaveis(
        IFormFile file
    )
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(
                new
                {
                    erro = "Arquivo não enviado"
                });
        }

        if (!file.FileName.EndsWith(".docx"))
        {
            return BadRequest(
                new
                {
                    erro =
                        "Formato inválido. Envie um arquivo .docx"
                });
        }

        using var stream =
            file.OpenReadStream();

        var variaveis =
            _wordService.ExtrairVariaveis(stream);

        return Ok(
            new VariaveisResponse
            {
                Variaveis = variaveis
            });
    }

    // --------------------------------------------------------
    // NOVO MODELO
    // --------------------------------------------------------
    [Authorize]
    [HttpPost("novoModelo")]
    public async Task<IActionResult>
        CriarModelo(
            [FromForm]
            CriarModeloRequest request
        )
    {
        var response =
            await _modeloService
                .CriarModelo(request);

        return Ok(response);
    }
    // --------------------------------------------------------
    // EXCLUI MODELO
    // --------------------------------------------------------
    [Authorize]
    [HttpDelete("deleteModelo/{id}")]
    public async Task<IActionResult> ExcluirModelo(int id)
    {
        var resultado = await _modeloService.ExcluirModelo(id);

        if (!resultado)
        {
            return NotFound(new
            {
                mensagem = "Modelo não encontrado."
            });
        }

        return Ok(new
        {
            mensagem = "Modelo excluído com sucesso."
        });
    }

    [Authorize]
    [HttpDelete("deleteRelatorio/{id}")]
    public async Task<IActionResult> ExcluirRelatorio(int id)
    {
        var currentUser =
            User.Identity?.Name;

        if (string.IsNullOrEmpty(currentUser))
        {
            return Unauthorized();
        }

        var response =
            await _relatorioService
                .ExcluirRelatorio(
                    id,
                    currentUser
                );

        if (!response)
        {
            return NotFound();
        }

        return Ok();
    }

}