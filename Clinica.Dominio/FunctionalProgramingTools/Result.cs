using static Clinica.Dominio.FunctionalProgramingTools.Result;

namespace Clinica.Dominio.FunctionalProgramingTools;

//
// ==========================================
//  BASE (sin genéricos)
// ==========================================
public abstract class Result {
	public sealed class Ok : Result { }

	public sealed class Error(string mensaje) : Result {
        public string Mensaje { get; } = mensaje;
    }

	public bool IsOk => this is Ok;
	public bool IsError => this is Error;

	// ✅ Métodos estáticos sin colisiones
	public static Result Success() => new Ok();
	public static Result Failure(string mensaje) => new Error(mensaje);

	public void Switch(Action ok, Action<string> error) {
		switch (this) {
			case Ok: ok(); break;
			case Error e: error(e.Mensaje); break;
			default: throw new InvalidOperationException();
		}
	}

	public TOut Match<TOut>(Func<TOut> ok, Func<string, TOut> error)
		=> this switch {
			Ok => ok(),
			Error e => error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};



	// ✅ Métodos auxiliares para versión genérica
	public static Result<T> Success<T>(T value) => new Result<T>.Ok(value);
	public static Result<T> Failure<T>(string message) => new Result<T>.Error(message);
}



//
// ==========================================
//  VERSION GENÉRICA (Result<T>)
// ==========================================
public abstract class Result<T> {
	public sealed class Ok(T valor) : Result<T> {
        public T Valor { get; } = valor;
    }

	public sealed class Error(string mensaje) : Result<T> {
        public string Mensaje { get; } = mensaje;
    }

	public bool IsOk => this is Ok;
	public bool IsError => this is Error;

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



	public static T GetOrRaise<T>(this Result<T> result) =>
		result.Match(
			ok => ok, // this "ok" IS the T value
			err => throw new InvalidOperationException(err)
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
		var combined = results.CombineResults();
		return combined switch {
			Result<List<T>>.Ok ok => func(ok.Valor),
			Result<List<T>>.Error err => new Result<U>.Error(err.Mensaje),
			_ => throw new InvalidOperationException()
		};
	}

	// CombineResults: IEnumerable<Result<T>> → Result<List<T>>
	public static Result<List<T>> CombineResults<T>(
		this IEnumerable<Result<T>> results) {
		var list = new List<T>();
		foreach (var r in results) {
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

	// MapToNonGeneric: permite degradar Result<T> → Result
	public static Result MapToNonGeneric<T>(this Result<T> r) =>
		r switch {
			Result<T>.Ok => new Result.Ok(),
			Result<T>.Error e => new Result.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};
}
public static class ResultLinqExtensions {
	// SELECT  (equivalente a Map)
	public static Result<U> Select<T, U>(
		this Result<T> r,
		Func<T, U> f
	) => r.Map(f);

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
