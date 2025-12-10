using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.WebAPI.Infrastructure;


public static class ControllerExtensions {
	// --------------------------------------------------------
	// Helper para extraer rol del usuario desde el contexto
	// --------------------------------------------------------
	private static bool TryGetUsuarioRole(this ControllerBase controller, out UsuarioRoleCodigo roleEnum) {
		roleEnum = default;

		var usuario = controller.HttpContext.Items["Usuario"];
		if (usuario is null) return false;

		var roleString = usuario.GetType().GetProperty("Role")?.GetValue(usuario)?.ToString();
		if (!Enum.TryParse<UsuarioRoleCodigo>(roleString, out roleEnum)) return false;

		return true;
	}

	// --------------------------------------------------------
	// Helper para retornar 401
	// --------------------------------------------------------
	private static ApiResult<T> UsuarioNoAutorizado<T>(string message = "No se encontró información del usuario en el contexto.")
		=> new ApiResult<T>.Error(
			new ApiError(message, StatusCodes.Status401Unauthorized)
		);

	// --------------------------------------------------------
	// Helper para retornar 403
	// --------------------------------------------------------
	private static ApiResult<T> PermisoDenegado<T>()
		=> new ApiResult<T>.Error(
			new ApiError("No posee permisos para acceder a este recurso.", StatusCodes.Status403Forbidden)
		);

	// --------------------------------------------------------
	// Función unificada
	// --------------------------------------------------------
	public static async Task<IActionResult> SafeExecute<T>(
		this ControllerBase controller,
		ILogger logger,
		PermisosAccionesCodigo permiso,
		Func<Task<Result<T>>> action,  // <-- sin pasar ControllerBase
		string? notFoundMessage = null
	) {
		// 1️⃣ Validar usuario
		if (!controller.TryGetUsuarioRole(out var roleEnum))
			return controller.ToActionResult(UsuarioNoAutorizado<T>());

		// 2️⃣ Validar permiso
		if (!roleEnum.TienePermisosPara(permiso))
			return controller.ToActionResult(PermisoDenegado<T>());

		try {
			// 3️⃣ Ejecutar acción
			Result<T> result = await action();

			ApiResult<T> apiResult = result.ToApi();

			// 4️⃣ Manejar NotFound si T es nullable y valor es null
			if (apiResult.IsOk && (apiResult as ApiResult<T>.Ok)!.Value is null) {
				logger.LogInformation("Recurso no encontrado. {Mensaje}", notFoundMessage);
				return controller.NotFound(new { mensaje = notFoundMessage ?? "Entidad no encontrada." });
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

	// --------------------------------------------------------
	// Variante con DTO -> Domain -> Acción de dominio
	// --------------------------------------------------------
	public static async Task<IActionResult> SafeExecuteWithDomain<TDto, TDomain, TResult>(
		this ControllerBase controller,
		ILogger logger,
		PermisosAccionesCodigo permiso,
		TDto dto,
		Func<TDto, Result<TDomain>> toDomain,
		Func<TDomain, Task<Result<TResult>>> action,
		string? notFoundMessage = null
	) {
		return await controller.SafeExecute<TResult>(
			logger,
			permiso,
			async () => {
				// Convertir DTO a dominio
				Result<TDomain> dom = toDomain(dto);
				if (dom.IsError)
					return new Result<TResult>.Error(dom.UnwrapAsError());

				// Ejecutar acción de dominio
				return await action(dom.UnwrapAsOk());
			},
			notFoundMessage
		);
	}

	// --------------------------------------------------------
	// Variante mínima para APIs que retornan ApiResult directamente
	// --------------------------------------------------------
	public static async Task<IActionResult> SafeExecuteApi<T>(
		this ControllerBase controller,
		ILogger logger,
		PermisosAccionesCodigo permiso,
		Func<Task<ApiResult<T>>> operation
	) {
		return await controller.SafeExecute<T>(
			logger,
			permiso,
			async () => {
				ApiResult<T> r = await operation();
				return r.IsOk
					? new Result<T>.Ok((r as ApiResult<T>.Ok)!.Value)
					: new Result<T>.Error((r as ApiResult<T>.Error)!.ErrorInfo.Message);
			}
		);
	}





}
