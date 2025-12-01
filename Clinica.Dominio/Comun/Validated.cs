namespace Clinica.Dominio.Comun;

public abstract class Validated<T> {
	public sealed class Valid(T value) : Validated<T> {
		public T Value { get; } = value;
	}

	public sealed class Invalid(IReadOnlyList<string> errors) : Validated<T> {
		public IReadOnlyList<string> Errors { get; } = errors;
	}

	public bool IsValid => this is Valid;
	public bool IsInvalid => this is Invalid;


}


public static class ValidatedExtensions {
	public static Validated<T> ToValidated<T>(this Result<T> r) =>
		r switch {
			Result<T>.Ok ok => new Validated<T>.Valid(ok.Valor),
			Result<T>.Error err => new Validated<T>.Invalid(new[] { err.Mensaje }),
			_ => throw new InvalidOperationException()
		};



	public static Validated<(T1, T2)> Combine<T1, T2>(
		this Validated<T1> v1,
		Validated<T2> v2
	) {
		if (v1 is Validated<T1>.Valid ok1 &&
			v2 is Validated<T2>.Valid ok2)
			return new Validated<(T1, T2)>.Valid((ok1.Value, ok2.Value));

		var errors = new List<string>();

		if (v1 is Validated<T1>.Invalid e1)
			errors.AddRange(e1.Errors);

		if (v2 is Validated<T2>.Invalid e2)
			errors.AddRange(e2.Errors);

		return new Validated<(T1, T2)>.Invalid(errors);
	}


	public static Validated<(T1, T2, T3)> Combine<T1, T2, T3>(Validated<T1> a, Validated<T2> b, Validated<T3> c) {
		return Combine(a, Combine(b, c))
			.Map(t => (t.Item1, t.Item2.Item1, t.Item2.Item2));
	}
	public static Validated<U> Map<T, U>(this Validated<T> v, Func<T, U> f) {
		return v switch {
			Validated<T>.Valid ok => new Validated<U>.Valid(f(ok.Value)),
			Validated<T>.Invalid err => new Validated<U>.Invalid(err.Errors),
			_ => throw new InvalidOperationException()
		};
	}
}

public static class ValidatedCombine {
	public static Validated<(T1, T2, T3, T4, T5, T6, T7)> Combine<T1, T2, T3, T4, T5, T6, T7>(
		Validated<T1> v1,
		Validated<T2> v2,
		Validated<T3> v3,
		Validated<T4> v4,
		Validated<T5> v5,
		Validated<T6> v6,
		Validated<T7> v7) {
		var errors = new List<string>();

		T1? t1 = default;
		T2? t2 = default;
		T3? t3 = default;
		T4? t4 = default;
		T5? t5 = default;
		T6? t6 = default;
		T7? t7_ = default;

		// v1
		if (v1 is Validated<T1>.Valid ok1)
			t1 = ok1.Value;
		else if (v1 is Validated<T1>.Invalid e1)
			errors.AddRange(e1.Errors);

		// v2
		if (v2 is Validated<T2>.Valid ok2)
			t2 = ok2.Value;
		else if (v2 is Validated<T2>.Invalid e2)
			errors.AddRange(e2.Errors);

		// v3
		if (v3 is Validated<T3>.Valid ok3)
			t3 = ok3.Value;
		else if (v3 is Validated<T3>.Invalid e3)
			errors.AddRange(e3.Errors);

		// v4
		if (v4 is Validated<T4>.Valid ok4)
			t4 = ok4.Value;
		else if (v4 is Validated<T4>.Invalid e4)
			errors.AddRange(e4.Errors);

		// v5
		if (v5 is Validated<T5>.Valid ok5)
			t5 = ok5.Value;
		else if (v5 is Validated<T5>.Invalid e5)
			errors.AddRange(e5.Errors);

		// v6
		if (v6 is Validated<T6>.Valid ok6)
			t6 = ok6.Value;
		else if (v6 is Validated<T6>.Invalid e6)
			errors.AddRange(e6.Errors);

		// v7
		if (v7 is Validated<T7>.Valid ok7)
			t7_ = ok7.Value;
		else if (v7 is Validated<T7>.Invalid e7)
			errors.AddRange(e7.Errors);

		if (errors.Count > 0)
			return new Validated<(T1, T2, T3, T4, T5, T6, T7)>.Invalid(errors);

		return new Validated<(T1, T2, T3, T4, T5, T6, T7)>.Valid(
			(t1!, t2!, t3!, t4!, t5!, t6!, t7_!)
		);
	}
}