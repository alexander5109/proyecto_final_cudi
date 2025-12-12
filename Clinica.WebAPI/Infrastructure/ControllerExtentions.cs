using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions {

	// ------------------------------
	// Obtener rol
	// ------------------------------
	private static bool TryGetUsuarioRole(
		this ControllerBase controller,
		out UsuarioRoleCodigo role
	) {
		role = default;

		var usuario = controller.HttpContext.Items["Usuario"];
		if (usuario is null) return false;

		var roleString = usuario.GetType().GetProperty("Role")?.GetValue(usuario)?.ToString();

		return Enum.TryParse(roleString, out role);
	}

	// ------------------------------
	// Helpers de errores
	// ------------------------------
	private static ApiResult<T> UsuarioNoAutorizado<T>()
		=> new ApiResult<T>.Error(
			new ApiError("No se encontró información del usuario.", StatusCodes.Status401Unauthorized)
		);

	private static ApiResult<T> PermisoDenegado<T>()
		=> new ApiResult<T>.Error(
			new ApiError("No posee permisos para acceder a este recurso.", StatusCodes.Status403Forbidden)
		);


	// =====================================================================
	// 🔥 SAFEEXECUTE (RETORNA TIPADO)
	// =====================================================================
	public static async Task<ActionResult<T>> SafeExecute<T>(
		this ControllerBase controller,
		ILogger logger,
		PermisosAccionesCodigo permiso,
		Func<Task<Result<T>>> action,
		string? notFoundMessage = null
	) {
		// 1️⃣ Usuario
		if (!controller.TryGetUsuarioRole(out UsuarioRoleCodigo role))
			return controller.ToActionResult(UsuarioNoAutorizado<T>());

		// 2️⃣ Permiso
		if (!role.TienePermisosPara(permiso))
			return controller.ToActionResult(PermisoDenegado<T>());

		try {
            // 3️⃣ Ejecutar acción
            Result<T> result = await action();
            ApiResult<T> apiResult = result.ToApi();

			// 4️⃣ Caso especial: Ok pero null
			if (apiResult.IsOk && (apiResult as ApiResult<T>.Ok)!.Value is null) {
				logger.LogInformation("Recurso no encontrado: {Mensaje}", notFoundMessage);
				return controller.NotFound(new {
					mensaje = notFoundMessage ?? "Entidad no encontrada."
				});
			}

			return controller.ToActionResult(apiResult);
		} catch (Exception ex) {
			logger.LogError(ex, "Error inesperado en SafeExecute");
			return controller.ToActionResult(
				new ApiResult<T>.Error(
					new ApiError(
						"Error inesperado al procesar la solicitud",
						StatusCodes.Status500InternalServerError,
						ex.Message
					)
				)
			);
		}
	}


	// =====================================================================
	// 🔥 SAFEEXECUTE WITH DOMAIN (RETORNA TIPADO)
	// =====================================================================
	public static Task<ActionResult<TResult>> SafeExecuteWithDomain<TDto, TDomain, TResult>(
		this ControllerBase controller,
		ILogger logger,
		PermisosAccionesCodigo permiso,
		TDto dto,
		Func<TDto, Result<TDomain>> toDomain,
		Func<TDomain, Task<Result<TResult>>> action,
		string? notFoundMessage = null
	) =>
		controller.SafeExecute<TResult>(
			logger,
			permiso,
			async () => {
                Result<TDomain> dom = toDomain(dto);
				return dom.IsError
					? new Result<TResult>.Error(dom.UnwrapAsError())
					: await action(dom.UnwrapAsOk());
			},
			notFoundMessage
		);


	// =====================================================================
	// 🔥 SAFEEXECUTE API (RETORNA TIPADO)
	// =====================================================================
	public static Task<ActionResult<T>> SafeExecuteApi<T>(
		this ControllerBase controller,
		ILogger logger,
		PermisosAccionesCodigo permiso,
		Func<Task<ApiResult<T>>> operation,
		string? notFoundMessage = null
	) =>
		controller.SafeExecute<T>(
			logger,
			permiso,
			async () => {
                ApiResult<T> api = await operation();

				if (api.IsOk)
					return new Result<T>.Ok((api as ApiResult<T>.Ok)!.Value);

				return new Result<T>.Error((api as ApiResult<T>.Error)!.ErrorInfo.Message);
			},
			notFoundMessage
		);
}
