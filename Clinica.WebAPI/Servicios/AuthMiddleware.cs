using System.Security.Claims;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.WebAPI.Servicios;




public class AuthMiddleware {
	private readonly RequestDelegate _next;

	public AuthMiddleware(RequestDelegate next)
		=> _next = next;

	public async Task Invoke(HttpContext context, IRepositorio repo) {
		ClaimsPrincipal user = context.User;

		if (user.Identity is { IsAuthenticated: true }) {
			string? idClaim = user.FindFirst("userid")?.Value;
			if (int.TryParse(idClaim, out int id)) {
				Result<Usuario2025Agg> result = await repo.SelectUsuarioWhereIdAsDomain(new UsuarioId(id));

				if (result.IsOk) {
					context.Items["Usuario"] = result.UnwrapAsOk();
				}
			}
		}

		await _next(context);
	}
}
