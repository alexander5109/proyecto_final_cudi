using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeValor;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;

namespace Clinica.WebAPI.Controllers;




public static class ServicioAuth {
	public static async Task<Result<Usuario2025Agg>> ValidarCredenciales(string username, string password, IRepositorioUsuarios repositorio) {
		Result<Usuario2025Agg> resultadoUsuario =
			await repositorio.SelectUsuarioWhereNombreAsDomain(new UserName(username));

		return resultadoUsuario.MatchAndSet(
			okValue => okValue.Usuario.PasswordMatch(password)
						? new Result<Usuario2025Agg>.Ok(okValue)
						: new Result<Usuario2025Agg>.Error("Usuario o contraseña incorrectos"),
			err => resultadoUsuario
		);
	}
}



[ApiController]
[Route("auth")]
public class AuthController(IRepositorio repositorio, IServiciosDeDominio servicios, JwtService jwtService, ILogger<AuthController> logger)
	: ControllerBase {
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UsuarioLoginRequestDto dto) {
		Result<Usuario2025Agg> resultado = await ServicioAuth.ValidarCredenciales(
			dto.Username,
			dto.UserPassword,
			repositorio
		);
		IActionResult response = null!;
		resultado.MatchAndDo(
			okContent => response = Ok(new UsuarioLoginResponseDto(
				okContent.Usuario.UserName.Valor,
				okContent.Usuario.EnumRole,
				jwtService.EmitirJwt(okContent)
			)),
			errMsj => response = Unauthorized(new { error = errMsj })
		);
		return response;
	}

}
