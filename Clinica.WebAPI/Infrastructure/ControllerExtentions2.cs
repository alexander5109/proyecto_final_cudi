using Clinica.Dominio.TiposDeValor;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.WebAPI.Infrastructure;

public static class ControllerExtentions2 {


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



}
