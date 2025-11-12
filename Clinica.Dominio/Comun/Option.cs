namespace Clinica.Dominio.Comun;
public abstract record Option<T> {
	private Option() { }

	public sealed record Some(T Value) : Option<T>;
	public sealed record None : Option<T>;

	public R Match<R>(Func<T, R> onSome, Func<R> onNone) =>
		this switch {
			Some s => onSome(s.Value),
			None => onNone(),
			_ => throw new InvalidOperationException()
		};
}

public static class Option {
	public static Option<T> Some<T>(T value) => new Option<T>.Some(value);
	public static Option<T> None<T>() => new Option<T>.None();

	// Helpers funcionales opcionales:
	public static Option<R> Map<T, R>(this Option<T> opt, Func<T, R> map) =>
		opt switch {
			Option<T>.Some s => new Option<R>.Some(map(s.Value)),
			_ => new Option<R>.None()
		};

	public static Option<R> Bind<T, R>(this Option<T> opt, Func<T, Option<R>> bind) =>
		opt switch {
			Option<T>.Some s => bind(s.Value),
			_ => new Option<R>.None()
		};
}
