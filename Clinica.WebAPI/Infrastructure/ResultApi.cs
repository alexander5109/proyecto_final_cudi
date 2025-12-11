using Clinica.Dominio.FunctionalToolkit;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.WebAPI.Infrastructure;

public record ApiError(
	string Message,
	int StatusCode,
	string? Detail = null
);

public abstract class ApiResult<T> {
	public sealed class Ok(T value) : ApiResult<T> {
        public T Value { get; } = value;
    }

	public sealed class Error(ApiError errorInfo) : ApiResult<T> {
        public ApiError ErrorInfo { get; } = errorInfo;
    }

	public bool IsOk => this is Ok;
	public bool IsError => this is Error;
}

public static class ResultToApiAdapter {
	public static ApiResult<T> ToApi<T>(this Result<T> result, int statusCodeOnError = 400) {
		return result switch {
			Result<T>.Ok ok =>
				new ApiResult<T>.Ok(ok.Valor),

			Result<T>.Error err =>
				new ApiResult<T>.Error(
					new ApiError(
						Message: err.Mensaje,
						StatusCode: statusCodeOnError,
						Detail: ""
					)
				),

			_ => throw new InvalidOperationException()
		};
	}
}


public static class ApiResultExtensions {

	public static ApiResult<TOut> Bind<TIn, TOut>(
		this ApiResult<TIn> result,
		Func<TIn, ApiResult<TOut>> next
	) {
		return result switch {
			ApiResult<TIn>.Ok ok => next(ok.Value),
			ApiResult<TIn>.Error err => new ApiResult<TOut>.Error(err.ErrorInfo),
			_ => throw new InvalidOperationException()
		};
	}
	public static async Task<ApiResult<TOut>> BindAsync<TIn, TOut>(
		this ApiResult<TIn> result,
		Func<TIn, Task<ApiResult<TOut>>> next
	) {
		return result switch {
			ApiResult<TIn>.Ok ok => await next(ok.Value),
			ApiResult<TIn>.Error err =>
				new ApiResult<TOut>.Error(err.ErrorInfo),
			_ => throw new InvalidOperationException()
		};
	}
	public static TResult Match<T, TResult>(
		this ApiResult<T> result,
		Func<T, TResult> ok,
		Func<ApiError, TResult> error
	) => result switch {
		ApiResult<T>.Ok o => ok(o.Value),
		ApiResult<T>.Error e => error(e.ErrorInfo),
		_ => throw new InvalidOperationException()
	};


	public static ActionResult ToActionResult<T>(
		this ControllerBase controller,
		ApiResult<T> result
	) {
		return result.Match(
			ok => controller.Ok(ok),
			err => controller.Problem(
				detail: err.Detail,
				statusCode: err.StatusCode,
				title: err.Message
			)
		);
	}


}
