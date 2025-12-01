using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.Servicios;
using Clinica.WebAPI.Servicios;
using Microsoft.AspNetCore.Mvc;
using static Clinica.Shared.Dtos.ApiDtos;
using static Clinica.Shared.Dtos.DomainDtos;

namespace Clinica.WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(RepositorioInterface repository, JwtService jwtService) : ControllerBase {
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequestDto dto) {
		Result<UsuarioBase2025> resultado = await ServiciosPublicos.ValidarCredenciales(dto.Username, dto.Password, repository);

		if (resultado.IsError) {
			return Unauthorized(new { error = ((Result<UsuarioBase2025>.Error)resultado).Mensaje });
		}

		UsuarioBase2025 usuario = ((Result<UsuarioBase2025>.Ok)resultado).Valor;

		string token = jwtService.EmitirJwt(usuario);

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
