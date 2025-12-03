using Clinica.Dominio.Comun;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.WebAPI.Controllers;

public static class ControllerExtentions {
	public static async Task<IActionResult> SafeExecute<T>(
		this ControllerBase controller,
		PermisoSistema permiso,
		Func<Task<Result<T>>> action,
		string? notFoundMessage = null
	) {
		if (controller.HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return controller.Unauthorized();

		if (!usuario.HasPermission(permiso))
			return controller.Forbid();

		Result<T> result = await action();

		if (result.IsError)
			return controller.Problem(result.UnwrapAsError());

		T value = result.UnwrapAsOk();

		// Si el tipo T es nullable y vino null → NotFound
		if (value is null)
			return controller.NotFound(new { mensaje = notFoundMessage ?? "Entidad no encontrada." });

		return controller.Ok(value);
	}

	public static async Task<IActionResult> SafeExecuteWithDomain<TDto, TDomain, TResult>(
		this ControllerBase controller,
		PermisoSistema permiso,
		TDto dto,
		Func<TDto, Result<TDomain>> toDomain,
		Func<TDomain, Task<Result<TResult>>> action,
		string? notFoundMessage = null
	) {
		if (controller.HttpContext.Items["Usuario"] is not Usuario2025 usuario)
			return controller.Unauthorized();

		if (!usuario.HasPermission(permiso))
			return controller.Forbid();

		Result<TDomain> dom = toDomain(dto);

		if (dom.IsError)
			return controller.BadRequest(new { error = dom.UnwrapAsError() });

		var result = await action(dom.UnwrapAsOk());

		if (result.IsError)
			return controller.Problem(result.UnwrapAsError());

		TResult value = result.UnwrapAsOk();

		if (value is null)
			return controller.NotFound(new { mensaje = notFoundMessage ?? "Entidad no encontrada" });

		return controller.Ok(value);
	}


}