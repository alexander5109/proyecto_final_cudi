using Clinica.Dominio.Comun;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.WebAPI.Controllers;

public static class EndpointsService {
	public static async Task<IActionResult> SafeExecute<T>(this ControllerBase controller, PermisoSistema permiso, Func<Task<Result<T>>> action) {
		if (controller.HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return controller.Unauthorized();

		if (!usuario.HasPermission(permiso))
			return controller.Forbid();

		var result = await action();
		return result.Match<IActionResult>(
			ok => controller.Ok(ok),
			err => controller.Problem(err)
		);
	}

}
