using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Servicios;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IRepositorio repositorio, JwtService jwtService, ILogger<AuthController> logger)
	: ControllerBase {
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UsuarioLoginRequestDto dto) {
		Result<Usuario2025Agg> resultado = await ServiciosPublicos.ValidarCredenciales(
			dto.Username,
			dto.UserPassword,
			repositorio
		);
		IActionResult response = null!;
		resultado.MatchAndDo(
			okContent => response = Ok(new UsuarioLoginResponseDto(
				okContent.Usuario.NombreUsuario.Valor,
				okContent.Usuario.EnumRole,
				jwtService.EmitirJwt(okContent.Usuario)
			)),
			errMsj => response = Unauthorized(new { error = errMsj })
		);
		return response;
	}

}
