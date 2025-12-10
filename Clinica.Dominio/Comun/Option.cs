namespace Clinica.Dominio.Comun;

public readonly struct Option<T> {
	public bool HasValor { get; }
	public T Valor { get; }
	private Option(T value) {
		HasValor = true;
		Valor = value;
	}
	public static Option<T> None => new();    // Sin valor
	public static Option<T> Some(T value) => new(value);
}

public static class OptionExtensions {
	public static TResult Match<T, TResult>(
		this Option<T> option,
		Func<T, TResult> some,
		Func<TResult> none
	) {
		if (option.HasValor)
			return some(option.Valor);

		return none();
	}

	public static Option<T> ToOption<T>(this T? value) where T : struct
		=> value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None;

	public static Option<string> ToOption(this string? value)
		=> string.IsNullOrWhiteSpace(value)
			? Option<string>.None
			: Option<string>.Some(value);

}

//public abstract record Option<T> {
//	private Option() { }

//	public sealed record Some(T Valor) : Option<T>;
//	public sealed record None : Option<T>;

//	public R MatchAndSet<R>(Func<T, R> onSome, Func<R> onNone) =>
//		this switch {
//			Some s => onSome(s.Valor),
//			None => onNone(),
//			_ => throw new InvalidOperationException()
//		};
//}

//public static class Option {
//	public static Option<T> Some<T>(T value) => new Option<T>.Some(value);
//	public static Option<T> None<T>() => new Option<T>.None();

//	// Helpers funcionales opcionales:
//	public static Option<R> Map<T, R>(this Option<T> opt, Func<T, R> map) =>
//		opt switch {
//			Option<T>.Some s => new Option<R>.Some(map(s.Valor)),
//			_ => new Option<R>.None()
//		};

//	public static Option<R> Bind<T, R>(this Option<T> opt, Func<T, Option<R>> bind) =>
//		opt switch {
//			Option<T>.Some s => bind(s.Valor),
//			_ => new Option<R>.None()
//		};
//}
