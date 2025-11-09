namespace Clinica.Dominio.Comun;
public abstract class Result<T> {

	public sealed class Ok : Result<T> {
		public T Value { get; }
		public Ok(T value) => Value = value;
	}

	public sealed class Error : Result<T> {
		public string Mensaje { get; }
		public Error(string message) => Mensaje = message;
	}

	public bool IsOk => this is Ok;
	public bool IsError => this is Error;

	public TOut Match<TOut>(Func<T, TOut> ok, Func<string, TOut> error) =>
		this switch {
			Ok o => ok(o.Value),
			Error e => error(e.Mensaje),
			_ => throw new InvalidOperationException()
		};
}


