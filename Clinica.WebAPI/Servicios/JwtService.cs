using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinica.Dominio.TiposDeValor;
using Microsoft.IdentityModel.Tokens;

public class JwtService(string jwtKey) {
	public string EmitirJwt(Usuario2025Agg aggrg) {
		JwtSecurityTokenHandler handler = new();

		byte[] key = Encoding.ASCII.GetBytes(jwtKey);

		List<Claim> claims =
		[
			new("userid", aggrg.Id.Valor.ToString()),
			new("username", aggrg.Usuario.UserName.Valor),
			new("role", aggrg.Usuario.EnumRole.ToString())
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
