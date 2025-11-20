using System.Globalization;
using System.Collections.Immutable;
using Clinica.Dominio.FunctionalProgramingTools;

namespace Clinica.Dominio.Entidades;

public static partial class Entidades {

	public sealed class DiaSemana2025 {
		public DayOfWeek Valor { get; }
		public string Nombre { get; }   // en español
		public int Numero { get; }      // 0..6

		private DiaSemana2025(DayOfWeek valor, string nombre) {
			Valor = valor;
			Nombre = nombre;
			Numero = (int)valor;
		}

		// ============================================================
		//           INSTANCIAS ESTÁTICAS — "ENUM RICO"
		// ============================================================

		public static readonly DiaSemana2025 Lunes = new(DayOfWeek.Monday, "Lunes");
		public static readonly DiaSemana2025 Martes = new(DayOfWeek.Tuesday, "Martes");
		public static readonly DiaSemana2025 Miercoles = new(DayOfWeek.Wednesday, "Miércoles");
		public static readonly DiaSemana2025 Jueves = new(DayOfWeek.Thursday, "Jueves");
		public static readonly DiaSemana2025 Viernes = new(DayOfWeek.Friday, "Viernes");
		public static readonly DiaSemana2025 Sabado = new(DayOfWeek.Saturday, "Sábado");
		public static readonly DiaSemana2025 Domingo = new(DayOfWeek.Sunday, "Domingo");

		private static readonly IReadOnlyList<DiaSemana2025> _todos =
		[
			Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo
		];

		public static IReadOnlyList<DiaSemana2025> Todos => _todos;

		// Maps para búsquedas rápidas
		private static readonly ImmutableDictionary<DayOfWeek, DiaSemana2025> _porEnum;
		private static readonly ImmutableDictionary<int, DiaSemana2025> _porNumero;
		private static readonly ImmutableDictionary<string, DiaSemana2025> _porNombre;

		static DiaSemana2025() {
			_porEnum = _todos.ToImmutableDictionary(x => x.Valor);
			_porNumero = _todos.ToImmutableDictionary(x => x.Numero);
			_porNombre = _todos.ToImmutableDictionary(
				x => x.Nombre.ToLowerInvariant(),
				x => x
			);
		}

		// ============================================================
		//                 FACTORÍAS (compatibles con tests)
		// ============================================================

		public static Result<DiaSemana2025> Crear(DayOfWeek input) {
			return new Result<DiaSemana2025>.Ok(_porEnum[input]);
		}

		public static Result<DiaSemana2025> Crear(int input) {
			return _porNumero.TryGetValue(input, out var d)
				? new Result<DiaSemana2025>.Ok(d)
				: new Result<DiaSemana2025>.Error("El número debe estar entre 0 y 6.");
		}

		public static Result<DiaSemana2025> Crear(string input) {
			if (string.IsNullOrWhiteSpace(input))
				return new Result<DiaSemana2025>.Error("El nombre no puede ser vacío.");

			var norm = input.Trim().ToLowerInvariant();

			if (_porNombre.TryGetValue(norm, out var d))
				return new Result<DiaSemana2025>.Ok(d);

			// Intento adicional (ES-ES)
			try {
				var es = new CultureInfo("es-ES");
				foreach (var dia in _todos) {
					if (es.DateTimeFormat.GetDayName(dia.Valor)
						.Equals(norm, StringComparison.OrdinalIgnoreCase))
						return new Result<DiaSemana2025>.Ok(dia);
				}
			} catch { }

			return new Result<DiaSemana2025>.Error($"'{input}' no corresponde a un día válido.");
		}

		public override string ToString() => $"{Nombre} ({Valor})";
	}
}
