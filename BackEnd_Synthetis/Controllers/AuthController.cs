<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using BackEnd_Synthetis.Data;
using BackEnd_Synthetis.DTOs.Requests;
using BackEnd_Synthetis.Security;

namespace BackEnd_Synthetis.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly JwtService _jwtService;

    public AuthController(
        AppDbContext context,
        JwtService jwtService
    )
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromForm] LoginRequest request
    )
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(
                u => u.NomeUsuario == request.NomeUsuario
            );

        if (usuario == null)
        {
            return Unauthorized("Usuário inválido");
        }

        bool senhaValida =
            BCrypt.Net.BCrypt.Verify(
                request.Senha,
                usuario.Senha
            );

        if (!senhaValida)
        {
            return Unauthorized("Senha inválida");
        }

        var token =
            _jwtService.GenerateToken(
                usuario.NomeUsuario
            );

        return Ok(new
        {
            access_token = token,
            token_type = "Bearer",

            usuario = new
            {
                usuario.Id,
                usuario.NomeUsuario,
                usuario.Equipe,
                usuario.Acesso
            }
        });
    }
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using BackEnd_Synthetis.Data;
using BackEnd_Synthetis.DTOs.Requests;
using BackEnd_Synthetis.Security;

namespace BackEnd_Synthetis.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly JwtService _jwtService;

    public AuthController(
        AppDbContext context,
        JwtService jwtService
    )
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromForm] LoginRequest request
    )
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(
                u => u.NomeUsuario == request.NomeUsuario
            );

        if (usuario == null)
        {
            return Unauthorized("Usuário inválido");
        }

        bool senhaValida =
            BCrypt.Net.BCrypt.Verify(
                request.Senha,
                usuario.Senha
            );

        if (!senhaValida)
        {
            return Unauthorized("Senha inválida");
        }

        var token =
            _jwtService.GenerateToken(
                usuario.NomeUsuario
            );

        return Ok(new
        {
            access_token = token,
            token_type = "Bearer",

            usuario = new
            {
                usuario.Id,
                usuario.NomeUsuario,
                usuario.Equipe,
                usuario.Acesso
            }
        });
    }
>>>>>>> c7d2b61b066fb0d744f59c225dac19d723052154
}