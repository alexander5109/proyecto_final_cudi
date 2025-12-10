using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.IRepositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.WebAPI.Controllers;



public class AuthMiddleware {
	private readonly RequestDelegate _next;

	public AuthMiddleware(RequestDelegate next)
		=> _next = next;

	public async Task Invoke(HttpContext context) {
		ClaimsPrincipal user = context.User;

		if (user.Identity?.IsAuthenticated == true) {
			context.Items["UsuarioId"] = int.Parse(user.FindFirst("userid")!.Value);
			context.Items["UserName"] = user.FindFirst("username")!.Value;
			context.Items["Role"] = user.FindFirst("role")!.Value;
		}

		await _next(context);
	}

}


public class JwtService(string jwtKey) {
	public string EmitirJwt(UsuarioAutenticado u) {
		JwtSecurityTokenHandler handler = new();

		byte[] key = Encoding.ASCII.GetBytes(jwtKey);

		List<Claim> claims =
		[
			new("userid", u.Id.Valor.ToString()),
		new("username", u.UserName),
		new("role", u.EnumRole.ToString())
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

	public static async Task<Result<UsuarioAutenticado>> ValidarCredenciales(
		string username,
		string passwordRaw,
		IRepositorioUsuarios repo
	) {
		// Paso 1: Buscar usuario en DB
		Result<UsuarioDbModel> rDb =
			await repo.SelectUsuarioProfileWhereUsername(new UserName(username));

		// Paso 2: DBModel → Aggregate
		Result<Usuario2025Agg> rAgg =
			rDb.Bind(db => db.ToDomainAgg());

		// Paso 3: Validar password
		Result<Usuario2025Agg> rPassword =
			rAgg.Bind(agg =>
				agg.Usuario.PasswordHash.IgualA(passwordRaw)
					? agg.ToOk()
					: "Usuario o contraseña incorrectos.".ToError<Usuario2025Agg>()
			);

		// Paso 4: Convertir → UsuarioAutenticado
		return rPassword.Map(agg =>
			new UsuarioAutenticado(
				agg.Id,
				agg.Usuario.UserName.Valor,
				agg.Usuario.EnumRole
			)
		);
	}
}





[ApiController]
[Route("auth")]
public class AuthController(
	IRepositorioUsuarios repositorio,
	JwtService jwtService
) : ControllerBase {

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UsuarioLoginRequestDto dto) {

		Result<UsuarioAutenticado> result =
			await ServicioAuth.ValidarCredenciales(
				dto.Username,
				dto.UserPassword,
				repositorio
			);

		// MatchAndSet -> produce directamente un IActionResult
		return result.MatchAndSet<UsuarioAutenticado, IActionResult>(
			ok => Ok(new UsuarioLoginResponseDto(
				ok.UserName,
				ok.EnumRole,
				jwtService.EmitirJwt(ok) // ver punto 3 abajo
			)),
			err => Unauthorized(new { error = err })
		);
	}
}
