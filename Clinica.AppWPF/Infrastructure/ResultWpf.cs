using System.Windows;
using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.AppWPF.Infrastructure;

public readonly struct UnitWpf {
	public static readonly UnitWpf Valor = default;
}
public abstract class ResultWpf<T> {
	public sealed class Ok(T valor) : ResultWpf<T> {
        public T Valor { get; } = valor;
    }

	public sealed class Error(ErrorInfo info) : ResultWpf<T> {
        public ErrorInfo Info { get; } = info;
    }

	public bool IsOk => this is Ok;
	public bool IsError => this is Error;
}

public record ErrorInfo(
	string Mensaje,
	MessageBoxImage Icono = MessageBoxImage.Error,
	string? Detalle = null,
	int? HttpStatus = null
);

public static class ResultToWpfAdapter {

	//Sin referencias todavia
	public static ResultWpf<T> ToWpf<T>(this Result<T> result) {
		return result switch {
			Result<T>.Ok ok => new ResultWpf<T>.Ok(ok.Valor),
			Result<T>.Error err => new ResultWpf<T>.Error(
				new ErrorInfo(err.Mensaje, MessageBoxImage.Warning)
			),
			_ => throw new InvalidOperationException()
		};
	}
}
public static class WpfResultExtensions {


	public static Task<ResultWpf<TOut>> Bind<TIn, TOut>(
		this ResultWpf<TIn> result,
		Func<TIn, Task<ResultWpf<TOut>>> next
	) {
		return result switch {
			ResultWpf<TIn>.Ok ok =>
				next(ok.Valor),

			ResultWpf<TIn>.Error err =>
				Task.FromResult<ResultWpf<TOut>>(
					new ResultWpf<TOut>.Error(err.Info)
				),

			_ => throw new InvalidOperationException()
		};
	}

	public static void MatchAndDo<T>(
		this ResultWpf<T> result,
		Action<T> ok,
		Action<ErrorInfo> error
	) {
		switch (result) {
			case ResultWpf<T>.Ok o:
				ok(o.Valor);
				break;

			case ResultWpf<T>.Error e:
				error(e.Info);
				break;
		}
	}

	public static TResult MatchAndSet<T, TResult>(
		this ResultWpf<T> result,
		Func<T, TResult> ok,
		Func<ErrorInfo, TResult> error
	) {
		return result switch {
			ResultWpf<T>.Ok o => ok(o.Valor),
			ResultWpf<T>.Error e => error(e.Info),
			_ => throw new InvalidOperationException()
		};
	}

	public static ResultWpf<TOut> MatchTo<TIn, TOut>(
		this ResultWpf<TIn> result,
		Func<TIn, ResultWpf<TOut>> ok,
		Func<ErrorInfo, ResultWpf<TOut>> error
	) {
		return result switch {
			ResultWpf<TIn>.Ok o => ok(o.Valor),
			ResultWpf<TIn>.Error e => error(e.Info),
			_ => throw new InvalidOperationException()
		};
	}


	public static void ShowMessageBox(this ErrorInfo error) {
		string mensaje = error.Mensaje;

		if (!string.IsNullOrWhiteSpace(error.Detalle))
			mensaje += $"\n\nDetalles:\n{error.Detalle}";

		MessageBox.Show(
			mensaje,
			"Error",
			MessageBoxButton.OK,
			error.Icono
		);
	}

}


