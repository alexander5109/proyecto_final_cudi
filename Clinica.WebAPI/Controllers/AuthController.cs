using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Infrastructure.Servicios;
using Microsoft.AspNetCore.Mvc;
using static Clinica.WebAPI.DTOs.DtosWebAPI;

namespace Clinica.WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase {
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequestDto dto) {
		Result<UsuarioBase2025> resultado = await authService.ValidarCredenciales(dto.Username, dto.Password);

		if (resultado.IsError) {
			return Unauthorized(new { error = ((Result<UsuarioBase2025>.Error)resultado).Mensaje });
		}

		UsuarioBase2025 usuario = ((Result<UsuarioBase2025>.Ok)resultado).Valor;

		string token = authService.EmitirJwt(usuario);

		return Ok(new LoginResponseDto(
			usuario.UserName.Valor,
			usuario switch {
				Usuario2025Nivel1Admin => "Admin",
				Usuario2025Nivel2Secretaria => "Secretaria",
				_ => "Desconocido"
			},
			token
		));
	}
}
