using System.Security.Claims;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;

namespace Clinica.WebAPI.Servicios;

public class UsuarioMiddleware {
	private readonly RequestDelegate _next;

	public UsuarioMiddleware(RequestDelegate next)
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
