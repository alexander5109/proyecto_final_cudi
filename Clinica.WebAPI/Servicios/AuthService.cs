using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Clinica.Infrastructure.DataAccess;
using Microsoft.IdentityModel.Tokens;
namespace Clinica.WebAPI.Servicios;

public class AuthService(BaseDeDatosRepositorio repo, string jwtKey) {
	public async Task<Result<UsuarioBase2025>> ValidarCredenciales(string username, string password) {
		// 1. Buscar usuario
		Result<UsuarioBase2025> resultadoUsuario = await repo.SelectUsuarioWhereNombre(new NombreUsuario(username));

		if (resultadoUsuario.IsError)
			return resultadoUsuario; // pasa error

		UsuarioBase2025 usuario = ((Result<UsuarioBase2025>.Ok)resultadoUsuario).Valor;

		// 2. Comparar hashes (SHA256 de ejemplo)
		string passwordHash = ComputeSha256(password);

		if (passwordHash != usuario.UserPassword.Valor)
			return new Result<UsuarioBase2025>.Error("Contraseña incorrecta");

		return new Result<UsuarioBase2025>.Ok(usuario);
	}

	public string EmitirJwt(UsuarioBase2025 usuario) {
		JwtSecurityTokenHandler handler = new();
		byte[] key = Encoding.ASCII.GetBytes(jwtKey);

		List<Claim> claims = [
			new("userid", usuario.UserId.Valor.ToString()),
			new("username", usuario.UserName.Valor),
			new("role", usuario switch {
				Usuario2025Nivel1Admin => "Admin",
				Usuario2025Nivel2Secretaria => "Secretaria",
				_ => "Desconocido"
			})
		];

		SecurityTokenDescriptor tokenDescriptor = new() {
			Subject = new ClaimsIdentity(claims),

			Expires = DateTime.UtcNow.AddHours(8),

			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature
			)
		};

		SecurityToken token = handler.CreateToken(tokenDescriptor);
		return handler.WriteToken(token);
	}

	private static string ComputeSha256(string raw) {
		byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
		return Convert.ToHexString(bytes);
	}
}
