namespace Clinica.Dominio.Comun; 
public abstract record Result<T> {
	public sealed record Ok(T Value) : Result<T>;
	public sealed record Error(string Mensaje) : Result<T>;
}
