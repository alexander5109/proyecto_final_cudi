using System.Collections.Immutable;
using Clinica.Dominio.FunctionalProgramingTools;

namespace Clinica.Dominio.Entidades;

public readonly record struct EspecialidadCodigoInterno(int Valor);

public static partial class Entidades {

	public sealed record ListaEspecialidadesMedicas2025(
		IReadOnlyList<EspecialidadMedica2025> Valores
	) {
		// Factory 1: desde una lista de Result<Especialidad>
		public static Result<ListaEspecialidadesMedicas2025> Crear(IReadOnlyList<Result<EspecialidadMedica2025>> results)
			=> results.Bind(list =>
				new Result<ListaEspecialidadesMedicas2025>.Ok(
					new ListaEspecialidadesMedicas2025(list)
				)
			);

		// Factory 2: desde una lista ya validada de EspecialidadMedica2025
		public static Result<ListaEspecialidadesMedicas2025> Crear(IReadOnlyList<EspecialidadMedica2025> okList)
			=> new Result<ListaEspecialidadesMedicas2025>.Ok(
				new ListaEspecialidadesMedicas2025(okList)
			);
	}


	public sealed class EspecialidadMedica2025 {
		public EspecialidadCodigoInterno CodigoInterno { get; }
		public string Titulo { get; }
		public int DuracionConsultaMinutos { get; }

		private EspecialidadMedica2025(int id, string titulo, int duracion) {
			CodigoInterno = new EspecialidadCodigoInterno(id);
			Titulo = titulo;
			DuracionConsultaMinutos = duracion;
		}

		// =====================================================================
		//        INSTANCIAS ESTÁTICAS — EL “ENUM” ENRIQUECIDO DEL DOMINIO
		// =====================================================================

		public static readonly EspecialidadMedica2025 ClinicoGeneral = new(1, "Clínico General", 30);
		public static readonly EspecialidadMedica2025 Cardiologo = new(2, "Cardiólogo", 40);
		public static readonly EspecialidadMedica2025 Oftalmologo = new(3, "Oftalmólogo", 20);
		public static readonly EspecialidadMedica2025 Otorrinolaringologo = new(4, "Otorrinolaringólogo", 25);
		public static readonly EspecialidadMedica2025 Psiquiatra = new(5, "Psiquiatra", 50);
		public static readonly EspecialidadMedica2025 Psicologo = new(6, "Psicólogo", 50);
		public static readonly EspecialidadMedica2025 Cirujano = new(7, "Cirujano", 60);
		public static readonly EspecialidadMedica2025 Kinesiologo = new(8, "Kinesiólogo", 30);
		public static readonly EspecialidadMedica2025 Nutricionista = new(9, "Nutricionista", 30);
		public static readonly EspecialidadMedica2025 Gastroenterologo = new(10, "Gastroenterólogo", 40);
		public static readonly EspecialidadMedica2025 Osteopata = new(11, "Osteópata", 30);
		public static readonly EspecialidadMedica2025 Proctologo = new(12, "Proctólogo", 30);
		public static readonly EspecialidadMedica2025 Pediatra = new(13, "Pediatra", 25);
		public static readonly EspecialidadMedica2025 Ginecologo = new(14, "Ginecólogo", 35);
		public static readonly EspecialidadMedica2025 Traumatologo = new(15, "Traumatólogo", 40);
		public static readonly EspecialidadMedica2025 Neurologo = new(16, "Neurólogo", 45);
		public static readonly EspecialidadMedica2025 Dermatologo = new(17, "Dermatólogo", 20);

		// Colección privada de TODAS
		private static readonly IReadOnlyList<EspecialidadMedica2025> _todas =
		[
			ClinicoGeneral, Cardiologo, Oftalmologo, Otorrinolaringologo,
			Psiquiatra, Psicologo, Cirujano, Kinesiologo, Nutricionista,
			Gastroenterologo, Osteopata, Proctologo, Pediatra, Ginecologo,
			Traumatologo, Neurologo, Dermatologo
		];

		public static IReadOnlyList<EspecialidadMedica2025> Todas =>
			_todas;

		// Diccionarios lookup auto-generados
		private static readonly ImmutableDictionary<EspecialidadCodigoInterno, EspecialidadMedica2025> _porId;
		private static readonly ImmutableDictionary<string, EspecialidadMedica2025> _porTitulo;

		static EspecialidadMedica2025() {
			_porId = _todas.ToImmutableDictionary(e => e.CodigoInterno, e => e);
			_porTitulo = _todas.ToImmutableDictionary(e => e.Titulo, e => e, StringComparer.OrdinalIgnoreCase);
		}

		// ======================================================
		//   API: CrearPorTitulo / CrearPorCodigoInterno
		// ======================================================

		public static Result<EspecialidadMedica2025> CrearPorTitulo(string? titulo) {
			if (string.IsNullOrWhiteSpace(titulo))
				return new Result<EspecialidadMedica2025>.Error("El título no puede estar vacío.");

			return _porTitulo.TryGetValue(titulo.Trim(), out var esp)
				? new Result<EspecialidadMedica2025>.Ok(esp)
				: new Result<EspecialidadMedica2025>.Error($"La especialidad '{titulo}' no está soportada.");
		}

		public static Result<EspecialidadMedica2025> CrearPorCodigoInterno(int? id) {
			if (id is null)
				return new Result<EspecialidadMedica2025>.Error("El CodigoInterno no puede ser nulo.");

			var key = new EspecialidadCodigoInterno(id.Value);

			return _porId.TryGetValue(key, out var esp)
				? new Result<EspecialidadMedica2025>.Ok(esp)
				: new Result<EspecialidadMedica2025>.Error($"No existe la especialidad con CodigoInterno = {id}.");
		}

		public override string ToString() =>
			$"{Titulo} ({DuracionConsultaMinutos} min)";
	}
}
