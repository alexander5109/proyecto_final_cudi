namespace Clinica.Dominio.Comun;

public static class ResultExtensions {

	public static Result<U> Map<T, U>(this Result<T> r, Func<T, U> f) =>
		r switch {
			Result<T>.Ok ok => new Result<U>.Ok(f(ok.Value)),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};



	public static Result<U> Bind<T, U>(
		this IReadOnlyList<Result<T>> results,
		Func<IReadOnlyList<T>, Result<U>> func) {
		// simplemente reusa la versión IEnumerable
		return ((IEnumerable<Result<T>>)results).Bind(func);
	}


	public static Result<U> Bind<T, U>(this Result<T> r, Func<T, Result<U>> f) =>
		r switch {
			Result<T>.Ok ok => f(ok.Value),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};


	public static Result<U> Bind<T, U>(
		this IEnumerable<Result<T>> results,
		Func<IReadOnlyList<T>, Result<U>> func) {
		var combined = results.CombineResults();

		return combined switch {
			Result<List<T>>.Ok ok => func(ok.Value),
			Result<List<T>>.Error err => new Result<U>.Error(err.Mensaje),
			_ => throw new InvalidOperationException()
		};
	}

	public static Result<List<T>> CombineResults<T>(
		this IEnumerable<Result<T>> results) {
		var list = new List<T>();
		foreach (var r in results) {
			switch (r) {
				case Result<T>.Ok ok:
					list.Add(ok.Value);
					break;
				case Result<T>.Error err:
					return new Result<List<T>>.Error(err.Mensaje);
			}
		}
		return new Result<List<T>>.Ok(list);
	}
}