using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.WebAPI.Infrastructure;

public static class ControllerExtentions {


	public static async Task<IActionResult> SafeExecuteApi<T>(
		this ControllerBase controller,
		ILogger logger,
		PermisoSistema permiso,
		Func<Task<ApiResult<T>>> operation
	) {
		ApiResult<T> result;

		// -----------------------
		// 1. Validar usuario
		// -----------------------
		if (controller.HttpContext.Items["Usuario"] is not Usuario2025Agg usuario) {
			result = new ApiResult<T>.Error(
				new ApiError(
					Message: "No se encontró información del usuario en el contexto.",
					StatusCode: StatusCodes.Status401Unauthorized
				)
			);

			return controller.ToActionResult(result);
		}

		// -----------------------
		// 2. Validar permiso
		// -----------------------
		if (!usuario.Usuario.HasPermission(permiso)) {
			result = new ApiResult<T>.Error(
				new ApiError(
					Message: "No posee permisos para acceder a este recurso.",
					StatusCode: StatusCodes.Status403Forbidden
				)
			);

			return controller.ToActionResult(result);
		}

		// -----------------------
		// 3. Ejecutar operación
		// -----------------------
		try {
			result = await operation();
		} catch (Exception ex) {
			logger.LogError(ex, "Error inesperado en SafeExecuteApi");

			result = new ApiResult<T>.Error(
				new ApiError(
					Message: "Error inesperado al procesar la solicitud",
					Detail: ex.Message,
					StatusCode: StatusCodes.Status500InternalServerError
				)
			);
		}

		return controller.ToActionResult(result);
	}



	public static async Task<IActionResult> SafeExecute<T>(
		this ControllerBase controller,
		ILogger logger,
		PermisoSistema permiso,
		Func<Task<Result<T>>> action,
		string? notFoundMessage = null
	) {
		// Validación de usuario
		if (controller.HttpContext.Items["Usuario"] is not Usuario2025Agg aggrgg) {
			logger.LogWarning("Token válido pero no se encontró Usuario en HttpContext.");
			return controller.Unauthorized();
		}

		// Validación de permisos
		if (!aggrgg.Usuario.HasPermission(permiso)) {
			logger.LogWarning("Usuario {UserId} intentó acceder sin permiso {Permiso}.",
				aggrgg.Id.Valor, permiso);
			return controller.Forbid();
		}

		// Ejecutar acción
		Result<T> result = await action();

		if (result.IsError) {
			logger.LogError("Error ejecutando acción: {Error}", result.UnwrapAsError());
			return controller.Problem(result.UnwrapAsError());
		}

		T value = result.UnwrapAsOk();

		// Caso NotFound
		if (value is null) {
			logger.LogInformation("Recurso no encontrado. {Mensaje}", notFoundMessage);
			return controller.NotFound(new { mensaje = notFoundMessage ?? "Entidad no encontrada." });
		}

		logger.LogDebug("Operación SafeExecute exitosa: {Tipo}", typeof(T).Name);
		return controller.Ok(value);
	}




	public static async Task<IActionResult> SafeExecuteWithDomain<TDto, TDomain, TResult>(
		this ControllerBase controller,
		ILogger logger,
		PermisoSistema permiso,
		TDto dto,
		Func<TDto, Result<TDomain>> toDomain,
		Func<TDomain, Task<Result<TResult>>> action,
		string? notFoundMessage = null
	) {
		if (controller.HttpContext.Items["Usuario"] is not Usuario2025Agg aggrg) {
			logger.LogWarning("Token válido pero no se encontró Usuario en HttpContext.");
			return controller.Unauthorized();
		}

		if (!aggrg.Usuario.HasPermission(permiso)) {
			logger.LogWarning("Usuario {UserId} intentó acceder sin permiso {Permiso}.",
				aggrg.Id.Valor, permiso);
			return controller.Forbid();
		}

		Result<TDomain> dom = toDomain(dto);

		if (dom.IsError) {
			logger.LogInformation("Error mapeando DTO a Domain: {Error}", dom.UnwrapAsError());
			return controller.BadRequest(new { error = dom.UnwrapAsError() });
		}

        Result<TResult> result = await action(dom.UnwrapAsOk());

		if (result.IsError) {
			logger.LogError("Error en acción de dominio: {Error}", result.UnwrapAsError());
			return controller.Problem(result.UnwrapAsError());
		}

		TResult value = result.UnwrapAsOk();

		if (value is null) {
			logger.LogInformation("Recurso no encontrado. {Mensaje}", notFoundMessage);
			return controller.NotFound(new { mensaje = notFoundMessage ?? "Entidad no encontrada" });
		}

		logger.LogDebug("Operación exitosa: {Tipo}", typeof(TResult).Name);
		return controller.Ok(value);
	}


}