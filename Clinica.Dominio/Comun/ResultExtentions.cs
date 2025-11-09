namespace Clinica.Dominio.Comun;

public static class ResultExtensions {
	public static Result<TResult> Bind<T, TResult>(
		this Result<T> result, Func<T, Result<TResult>> func) {
		return result switch {
			Result<T>.Ok ok => func(ok.Value),
			Result<T>.Error err => new Result<TResult>.Error(err.Message),
			_ => throw new InvalidOperationException()
		};
	}

	public static Result<TResult> Map<T, TResult>(
		this Result<T> result, Func<T, TResult> func) {
		return result switch {
			Result<T>.Ok ok => new Result<TResult>.Ok(func(ok.Value)),
			Result<T>.Error err => new Result<TResult>.Error(err.Message),
			_ => throw new InvalidOperationException()
		};
	}

	public static Result<TResult> Combine<T1, T2, TResult>(
		Result<T1> r1, Result<T2> r2, Func<T1, T2, TResult> combine) {
		if (r1 is Result<T1>.Error e1)
			return new Result<TResult>.Error(e1.Message);
		if (r2 is Result<T2>.Error e2)
			return new Result<TResult>.Error(e2.Message);

		var v1 = ((Result<T1>.Ok)r1).Value;
		var v2 = ((Result<T2>.Ok)r2).Value;
		return new Result<TResult>.Ok(combine(v1, v2));
	}
}
