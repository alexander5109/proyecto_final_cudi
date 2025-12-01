using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.Dominio.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace Clinica.WebAPI.Servicios;

public class JwtService(string jwtKey) {
	public string EmitirJwt(UsuarioBase2025 usuario) {
		JwtSecurityTokenHandler handler = new();
		//string jwtKey = 
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
}