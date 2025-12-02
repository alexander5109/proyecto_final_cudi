namespace Clinica.Dominio.Comun;

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
	public void Match(Action<T> ok, Action<string> error) {
		switch (this) {
			case Ok o: ok(o.Valor); break;
			case Error e: error(e.Mensaje); break;
		}
	}
	public TOut Match<TOut>(Func<T, TOut> ok, Func<string, TOut> error)
		=> this switch {
			Ok o => ok(o.Valor),
			Error e => error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};

	public void Switch(Action<T> ok, Action<string> error) {
		switch (this) {
			case Ok o: ok(o.Valor); break;
			case Error e: error(e.Mensaje); break;
			default: throw new InvalidOperationException();
		}
	}
}
//
// ==========================================
//  EXTENSIONES FUNCIONALES
// ==========================================
public static class ResultExtensions {

	public static Result<T> PrintAndContinue<T>(this Result<T> result, string? label = null) {
		string prefix = label is null ? "" : $"{label}: ";

		result.Match(
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



	//UNSAFE
	public static string UnwrapAsError<T>(this Result<T> result) =>
		result.Match(
			ok => throw new InvalidOperationException(
				$"Expected Error result but got Ok({ok})"
			),
			mensaje => mensaje
		);


	//UNSAFE
	public static T UnwrapAsOk<T>(this Result<T> result) =>
		result.Match(
			ok => ok,
			mensaje => throw new InvalidOperationException(mensaje)
		);

	public static T GetOrRaise<T>(this Result<T> result) =>
		result.Match(
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


	// Map: Result<T> → Result<U>
	public static Result<U> Map<T, U>(this Result<T> r, Func<T, U> f) =>
		r switch {
			Result<T>.Ok ok => new Result<U>.Ok(f(ok.Valor)),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};

	// Bind: Result<T> → Result<U>
	public static Result<U> Bind<T, U>(this Result<T> r, Func<T, Result<U>> f) =>
		r switch {
			Result<T>.Ok ok => f(ok.Valor),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};

	// Bind (para colecciones)
	public static Result<U> Bind<T, U>(
		this IReadOnlyList<Result<T>> results,
		Func<IReadOnlyList<T>, Result<U>> func)
		=> ((IEnumerable<Result<T>>)results).Bind(func);

	public static Result<U> Bind<T, U>(
		this IEnumerable<Result<T>> results,
		Func<IReadOnlyList<T>, Result<U>> func) {
		Result<List<T>> combined = results.CombineResults();
		return combined switch {
			Result<List<T>>.Ok ok => func(ok.Valor),
			Result<List<T>>.Error err => new Result<U>.Error(err.Mensaje),
			_ => throw new InvalidOperationException()
		};
	}

	// CombineResults: IEnumerable<Result<T>> → Result<List<T>>
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


	// SELECT MANY (equivalente a Bind)
	public static Result<V> SelectMany<T, U, V>(
		this Result<T> r,
		Func<T, Result<U>> bind,
		Func<T, U, V> project
	) {
		// r.Bind(t => bind(t).Map(u => project(t, u)))
		return r.Bind(t =>
			bind(t).Map(u => project(t, u))
		);
	}
}