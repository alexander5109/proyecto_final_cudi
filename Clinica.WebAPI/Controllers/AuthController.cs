using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.IRepositorios;
using Clinica.Shared.ApiDtos;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.WebAPI.Controllers;



[ApiController]
[Route("auth")]
public class AuthController(
	IRepositorioUsuarios repositorio,
	ILogger<HorariosController> logger,
	JwtService jwtService
) : ControllerBase {




	[HttpPost("login")]
	public async Task<ActionResult<UsuarioAutenticadoDbModel>> Login([FromBody] UsuarioLoginRequestDto dto) {

		Result<UsuarioAutenticadoDbModel> result = await ServicioAuth.ValidarCredenciales(
			dto.Username,
			dto.UserPassword,
			repositorio
		);

		// MatchAndSet -> produce directamente un IActionResult
		return result.MatchAndSet<UsuarioAutenticadoDbModel, ActionResult>(
			ok => Ok(new UsuarioLoginResponseDto(
				ok.UserName,
				ok.EnumRole,
				jwtService.EmitirJwt(ok) // ver punto 3 abajo
			)),
			err => Unauthorized(new { error = err })
		);
	}


}




public class AuthMiddleware(RequestDelegate next) {
	private readonly RequestDelegate _next = next;

	public async Task Invoke(HttpContext context) {
		ClaimsPrincipal user = context.User;

		if (user.Identity?.IsAuthenticated == true) {
			int userId = int.Parse(user.FindFirst("userid")!.Value);
			string username = user.FindFirst("username")!.Value;
			string role = user.FindFirst("role")!.Value;

			// solo info necesaria, no agregados completos
			context.Items["Usuario"] = new {
				Id = userId,
				Username = username,
				Role = role
			};
		}

		await _next(context);
	}
}



public class JwtService(string jwtKey) {
	public string EmitirJwt(UsuarioAutenticadoDbModel usuarioAutenticado) {
		JwtSecurityTokenHandler handler = new();

		byte[] key = Encoding.ASCII.GetBytes(jwtKey);

		List<Claim> claims = [
			new("userid", usuarioAutenticado.Id.Valor.ToString()),
			new("username", usuarioAutenticado.UserName),
			new("role", usuarioAutenticado.EnumRole.ToString())
		];

		SecurityTokenDescriptor descriptor = new() {
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddHours(8),
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature
			)
		};

		SecurityToken token = handler.CreateToken(descriptor);
		return handler.WriteToken(token);
	}

}



public static class ServicioAuth {

	public static async Task<Result<UsuarioAutenticadoDbModel>> ValidarCredenciales(
		string username,
		string passwordRaw,
		IRepositorioUsuarios repo
	) {
		Result<UsuarioDbModel> dbResult = await repo.SelectUsuarioProfileWhereUsername(new UserName(username));

		if (dbResult.IsError) {
			return "Usuario o contraseña incorrectos.".ToError<UsuarioAutenticadoDbModel>();
		}
		UsuarioDbModel db = dbResult.UnwrapAsOk();
		// VALIDAR PASSWORD
		if (!ContraseñaHasheada.RawIdenticalToHashed(passwordRaw, db.PasswordHash))
			return "Usuario o contraseña incorrectos.".ToError<UsuarioAutenticadoDbModel>();

		// CONSTRUIR RESULTADO
		return new UsuarioAutenticadoDbModel(
			db.Id,
			db.UserName,
			db.EnumRole
		).ToOk();
	}
}





