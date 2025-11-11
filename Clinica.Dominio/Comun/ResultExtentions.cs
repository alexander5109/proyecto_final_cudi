using Clinica.Dominio.Comun;

public static class ResultExtensions {


	// 🔹 Combine para dos resultados
	public static Result<TResult> Combine<T1, T2, TResult>(
		Result<T1> r1,
		Result<T2> r2,
		Func<T1, T2, TResult> projector) {
		if (r1 is Result<T1>.Error e1)
			return new Result<TResult>.Error(e1.Mensaje);
		if (r2 is Result<T2>.Error e2)
			return new Result<TResult>.Error(e2.Mensaje);

		var v1 = ((Result<T1>.Ok)r1).Value;
		var v2 = ((Result<T2>.Ok)r2).Value;

		return new Result<TResult>.Ok(projector(v1, v2));
	}

	// 🔹 Combine para tres resultados
	public static Result<TResult> Combine<T1, T2, T3, TResult>(
		Result<T1> r1,
		Result<T2> r2,
		Result<T3> r3,
		Func<T1, T2, T3, TResult> projector) {
		if (r1 is Result<T1>.Error e1)
			return new Result<TResult>.Error(e1.Mensaje);
		if (r2 is Result<T2>.Error e2)
			return new Result<TResult>.Error(e2.Mensaje);
		if (r3 is Result<T3>.Error e3)
			return new Result<TResult>.Error(e3.Mensaje);

		var v1 = ((Result<T1>.Ok)r1).Value;
		var v2 = ((Result<T2>.Ok)r2).Value;
		var v3 = ((Result<T3>.Ok)r3).Value;

		return new Result<TResult>.Ok(projector(v1, v2, v3));
	}

	// 🔹 Combine para cuatro resultados
	public static Result<TResult> Combine<T1, T2, T3, T4, TResult>(
		Result<T1> r1,
		Result<T2> r2,
		Result<T3> r3,
		Result<T4> r4,
		Func<T1, T2, T3, T4, TResult> projector) {
		if (r1 is Result<T1>.Error e1)
			return new Result<TResult>.Error(e1.Mensaje);
		if (r2 is Result<T2>.Error e2)
			return new Result<TResult>.Error(e2.Mensaje);
		if (r3 is Result<T3>.Error e3)
			return new Result<TResult>.Error(e3.Mensaje);
		if (r4 is Result<T4>.Error e4)
			return new Result<TResult>.Error(e4.Mensaje);

		var v1 = ((Result<T1>.Ok)r1).Value;
		var v2 = ((Result<T2>.Ok)r2).Value;
		var v3 = ((Result<T3>.Ok)r3).Value;
		var v4 = ((Result<T4>.Ok)r4).Value;

		return new Result<TResult>.Ok(projector(v1, v2, v3, v4));
	}

	// 🔹 Combine para cinco resultados (como el caso de Paciente2025)
	public static Result<TResult> Combine<T1, T2, T3, T4, T5, TResult>(
		Result<T1> r1,
		Result<T2> r2,
		Result<T3> r3,
		Result<T4> r4,
		Result<T5> r5,
		Func<T1, T2, T3, T4, T5, TResult> projector) {
		if (r1 is Result<T1>.Error e1)
			return new Result<TResult>.Error(e1.Mensaje);
		if (r2 is Result<T2>.Error e2)
			return new Result<TResult>.Error(e2.Mensaje);
		if (r3 is Result<T3>.Error e3)
			return new Result<TResult>.Error(e3.Mensaje);
		if (r4 is Result<T4>.Error e4)
			return new Result<TResult>.Error(e4.Mensaje);
		if (r5 is Result<T5>.Error e5)
			return new Result<TResult>.Error(e5.Mensaje);

		var v1 = ((Result<T1>.Ok)r1).Value;
		var v2 = ((Result<T2>.Ok)r2).Value;
		var v3 = ((Result<T3>.Ok)r3).Value;
		var v4 = ((Result<T4>.Ok)r4).Value;
		var v5 = ((Result<T5>.Ok)r5).Value;

		return new Result<TResult>.Ok(projector(v1, v2, v3, v4, v5));
	}


	// Combina múltiples Results en uno solo, acumulando errores
	public static Result<(T1, T2, T3, T4, T5)> Combine<T1, T2, T3, T4, T5>(
			Result<T1> r1, Result<T2> r2, Result<T3> r3, Result<T4> r4, Result<T5> r5) {
		var errores = new List<string>();

		if (r1 is Result<T1>.Error e1) errores.Add(e1.Mensaje);
		if (r2 is Result<T2>.Error e2) errores.Add(e2.Mensaje);
		if (r3 is Result<T3>.Error e3) errores.Add(e3.Mensaje);
		if (r4 is Result<T4>.Error e4) errores.Add(e4.Mensaje);
		if (r5 is Result<T5>.Error e5) errores.Add(e5.Mensaje);

		if (errores.Count > 0)
			return new Result<(T1, T2, T3, T4, T5)>.Error(string.Join("; ", errores));

		return new Result<(T1, T2, T3, T4, T5)>.Ok((
			((Result<T1>.Ok)r1).Value,
			((Result<T2>.Ok)r2).Value,
			((Result<T3>.Ok)r3).Value,
			((Result<T4>.Ok)r4).Value,
			((Result<T5>.Ok)r5).Value
		));
	}

	// Mapea un Ok<T> a Ok<R>, propagando error si no lo es
	public static Result<R> Map<T, R>(this Result<T> r, Func<T, R> f) =>
		r switch {
			Result<T>.Ok ok => new Result<R>.Ok(f(ok.Value)),
			Result<T>.Error err => new Result<R>.Error(err.Mensaje),
			_ => throw new NotImplementedException()
		};

}
