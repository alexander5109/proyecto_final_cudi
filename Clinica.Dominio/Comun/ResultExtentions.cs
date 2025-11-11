namespace Clinica.Dominio.Comun;

public static class ResultExtensions {

	public static Result<U> Map<T, U>(this Result<T> r, Func<T, U> f) =>
		r switch {
			Result<T>.Ok ok => new Result<U>.Ok(f(ok.Value)),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};

	public static Result<U> Bind<T, U>(this Result<T> r, Func<T, Result<U>> f) =>
		r switch {
			Result<T>.Ok ok => f(ok.Value),
			Result<T>.Error e => new Result<U>.Error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};

	public static Result<TResult> Combine<T1, T2, TResult>(
		Result<T1> r1, Result<T2> r2, Func<T1, T2, TResult> combine) {
		if (r1 is Result<T1>.Error e1)
			return new Result<TResult>.Error(e1.Mensaje);
		if (r2 is Result<T2>.Error e2)
			return new Result<TResult>.Error(e2.Mensaje);

		var v1 = ((Result<T1>.Ok)r1).Value;
		var v2 = ((Result<T2>.Ok)r2).Value;
		return new Result<TResult>.Ok(combine(v1, v2));
	}
}
