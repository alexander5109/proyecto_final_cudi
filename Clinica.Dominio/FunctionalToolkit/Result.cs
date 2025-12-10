namespace Clinica.Dominio.FunctionalToolkit;

public readonly struct Unit {
	public static readonly Unit Valor = default;
}
public abstract class Result<T> {
	public sealed class Ok(T valor) : Result<T> {
		public T Valor { get; } = valor;
	}
	public sealed class Error(string mensaje) : Result<T> {
		public string Mensaje { get; } = mensaje;
	}
	public bool IsOk => this is Ok;
	public bool IsError => this is Error;
	public static Result<Unit> Ensure(bool condition, string msg) {
		return condition
			? new Result<Unit>.Ok(Unit.Valor)
			: new Result<Unit>.Error(msg);
	}


}
//
// ==========================================
//  EXTENSIONES FUNCIONALES
// ==========================================
public static class ResultExtensions {

	public static Result<T> ToError<T>(
		this string mensajeError
	) => new Result<T>.Error(mensajeError);


	public static Result<T> ToOk<T>(
		this T valor
	)
	=> new Result<T>.Ok(valor);


	public static Result<T> ToResult<T>(
		this T valor,
		bool condition,
		string mensajeError
	) {
		return condition
			? new Result<T>.Ok(valor)
			: new Result<T>.Error(mensajeError);
	}


	public static Result<T> MapError<T>(
		this Result<T> self,
		Func<string,
			string> mapError
	) {
		return self switch {
			Result<T>.Ok ok => ok,// OK se devuelve tal cual
			Result<T>.Error e => new Result<T>.Error(mapError(e.Mensaje)),// Error se transforma
			_ => throw new InvalidOperationException(),
		};
	}
	public static Result<U> BindWithPrefix<T, U>(
		this Result<T> result,
		string prefixError,
		Func<T, Result<U>> caseOk
	) {
		return result switch {
			Result<T>.Ok o => caseOk(o.Valor),
			Result<T>.Error e => new Result<U>.Error($"{prefixError}{e.Mensaje}"),
			_ => throw new InvalidOperationException()
		};
	}

	public static async Task<Result<U>> BindWithPrefixAsync<T, U>(
		this Result<T> result,
		string prefixError,
		Func<T, Task<Result<U>>> caseOk
	) {
		return result switch {
			Result<T>.Ok ok => await caseOk(ok.Valor),
			Result<T>.Error err => new Result<U>.Error($"{prefixError}{err.Mensaje}"),
			_ => throw new InvalidOperationException("Resultado inválido en BindWithPrefixAsync."),
		};
	}


	public static Result<U> Bind<T, U>(
		this Result<T> result,
		Func<T, Result<U>> caseOk
	) =>
		result switch {
			Result<T>.Ok ok => caseOk(ok.Valor),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};

	public static Result<U> Bind<T, U>(
		this IEnumerable<Result<T>> results,
		Func<IReadOnlyList<T>, Result<U>> caseOk
	) {
		Result<List<T>> combined = results.CombineResults();
		return combined switch {
			Result<List<T>>.Ok ok => caseOk(ok.Valor),
			Result<List<T>>.Error err => new Result<U>.Error(err.Mensaje),
			_ => throw new InvalidOperationException()
		};
	}

	public static void MatchAndDo<T>(
		this Result<T> self,
		Action<T> ok,
		Action<string> error
	) {
		switch (self) {
			case Result<T>.Ok o:
				ok(o.Valor);
				break;
			case Result<T>.Error e:
				error(e.Mensaje);
				break;
		}
	}

	public static U Match<T, U>(
		this Result<T> result, 
		Func<T, U> caseOk, 
		Func<string, U> error
	) => result.MatchAndSet(caseOk, error);

	public static TOut MatchAndSet<T, TOut>(
		this Result<T> self,
		Func<T, TOut> ok,
		Func<string, TOut> error) {
		return self switch {
			Result<T>.Ok o => ok(o.Valor),
			Result<T>.Error e => error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};
	}

	public static Result<List<T>> CombineResults<T>(
		this IEnumerable<Result<T>> results) {
		List<T> list = [];
		foreach (Result<T> r in results) {
			switch (r) {
				case Result<T>.Ok ok:
					list.Add(ok.Valor);
					break;
				case Result<T>.Error err:
					return new Result<List<T>>.Error(err.Mensaje);
			}
		}
		return new Result<List<T>>.Ok(list);
	}


	public static Result<V> SelectMany<T, U, V>(
		this Result<T> r,
		Func<T, Result<U>> bind,
		Func<T, U, V> project
	) {
		// result.Bind(t => bind(t).Map(u => project(t, u)))
		return r.Bind(t =>
			bind(t).Map(u => project(t, u))
		);
	}
	public static Result<U> Map<T, U>(this Result<T> r, Func<T, U> f) =>
		r switch {
			Result<T>.Ok ok => new Result<U>.Ok(f(ok.Valor)),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};

	public static async Task<Result<V>> SelectManyAsync<T, U, V>(
		this Result<T> r,
		Func<T, Task<Result<U>>> bindAsync,
		Func<T, U, V> project
	) {
		switch (r) {
			case Result<T>.Ok ok:
				Result<U> uResult = await bindAsync(ok.Valor);
				return uResult switch {
					Result<U>.Ok uOk => new Result<V>.Ok(project(ok.Valor, uOk.Valor)),
					Result<U>.Error uErr => new Result<V>.Error(uErr.Mensaje),
					_ => throw new InvalidOperationException(),
				};
			case Result<T>.Error e:
				return new Result<V>.Error(e.Mensaje);
			default:
				throw new InvalidOperationException();
		}
	}

	// ==========================================
	//  UNSAFE NOOB FUNCTIONS
	// ==========================================
	public static string UnwrapAsError<T>(this Result<T> result) =>
		result.MatchAndSet(
			ok => throw new InvalidOperationException(
				$"Expected Error result but got Ok({ok})"
			),
			mensaje => mensaje
		);
	public static T UnwrapAsOk<T>(this Result<T> result) =>
		result.MatchAndSet(
			ok => ok,
			mensaje => throw new InvalidOperationException(mensaje)
		);

	public static T GetOrRaise<T>(this Result<T> result) =>
		result.MatchAndSet(
			ok => {
				Console.WriteLine($"Succesfully asserting {typeof(T).Name}");
				return ok;
			},
			mensaje => {
				Console.WriteLine(mensaje);
				Console.Write("Presione enter para lanzar la excepcion: ");
				Console.ReadLine();
				throw new InvalidOperationException(mensaje);
			}
		);

	public static Result<T> PrintAndContinue<T>(this Result<T> result, string? label = null) {
		string prefix = label is null ? "" : $"{label}: ";

		result.MatchAndDo(
			ok => {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"{prefix}OK → {typeof(T).Name} succeded");
				Console.ResetColor();
			},
			err => {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"{prefix}ERROR → {err}");
				Console.ResetColor();
			}
		);

		return result;
	}

}




public static class ResultExtensionsASync {




	public static async Task<Result<U>> BindAsync<T, U>(
		this Task<Result<T>> task,
		Func<T, Task<Result<U>>> func
	) {
		Result<T> result = await task;
		return result is Result<T>.Ok ok
			? await func(ok.Valor)
			: new Result<U>.Error(((Result<T>.Error)result).Mensaje);
	}

	public static async Task<Result<U>> MapAsync<T, U>(
		this Task<Result<T>> task,
		Func<T, U> map
	) {
		Result<T> result = await task;
		return result is Result<T>.Ok ok
			? new Result<U>.Ok(map(ok.Valor))
			: new Result<U>.Error(((Result<T>.Error)result).Mensaje);
	}
	public static async Task<Result<T>> RequireOkAsync<T>(
	this Task<Result<T>> task) {
		Result<T> result = await task;
		return result;
	}
	public static async Task<Result<T>> OrFailAsync<T>(
	this Task<Result<T>> task,
	string? overrideMessage = null) {
		Result<T> result = await task;

		return result switch {
			Result<T>.Ok ok => ok,
			Result<T>.Error err => new Result<T>.Error(
				overrideMessage ?? err.Mensaje
			),
			_ => throw new InvalidOperationException()
		};
	}
	public static async IAsyncEnumerable<T> SelectOk<T>(
	this IAsyncEnumerable<Result<T>> stream) {
		await foreach (Result<T> r in stream) {
			switch (r) {
				case Result<T>.Ok ok:
					yield return ok.Valor;
					break;

				case Result<T>.Error err:
					throw new Exception(err.Mensaje); // o devolver Result<...>
			}
		}
	}
}