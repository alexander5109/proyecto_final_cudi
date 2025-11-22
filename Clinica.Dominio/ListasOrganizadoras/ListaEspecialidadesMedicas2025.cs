using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.ListasOrganizadoras;

public sealed record ListaEspecialidadesMedicas2025(
	IReadOnlyList<EspecialidadMedica2025> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores is null || Valores.Count == 0)
			return "No hay especialidades asignadas.";

		var sb = new System.Text.StringBuilder();
		sb.AppendLine("Listado de especialidades:");

		foreach (var esp in Valores)
			sb.AppendLine($"  • {esp.ATexto()}");

		return sb.ToString();
	}
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

	// Factory 3: desde una sola especialidad
	public static Result<ListaEspecialidadesMedicas2025> CrearConUnicaEspecialidad(EspecialidadMedica2025 unaSola)
		=> new Result<ListaEspecialidadesMedicas2025>
		.Ok(new ListaEspecialidadesMedicas2025([unaSola])
	);
	public static Result<ListaEspecialidadesMedicas2025> CrearConUnicaEspecialidad(Result<EspecialidadMedica2025> unaSolaResult) 
		=> unaSolaResult.Bind(unaSola => new
			Result<ListaEspecialidadesMedicas2025>.Ok(new ListaEspecialidadesMedicas2025([unaSola]))
		);
}

public readonly record struct EspecialidadCodigoInterno(byte Valor);

public sealed record EspecialidadMedica2025(EspecialidadCodigoInterno CodigoInterno, string Titulo, int DuracionConsultaMinutos) : IComoTexto {
	public string ATexto() => $"{Titulo} (Duración de consulta: {DuracionConsultaMinutos} min)";

	private EspecialidadMedica2025(byte id, string titulo, int duracion) : this(new EspecialidadCodigoInterno(id), titulo, duracion) {
	}

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
	public static readonly IReadOnlyList<EspecialidadMedica2025> Todas = [ClinicoGeneral, Cardiologo, Oftalmologo, Otorrinolaringologo, Psiquiatra, Psicologo, Cirujano, Kinesiologo, Nutricionista, Gastroenterologo, Osteopata, Proctologo, Pediatra, Ginecologo, Traumatologo, Neurologo, Dermatologo];


	// Diccionarios lookup auto-generados

	public static Result<EspecialidadMedica2025> CrearPorCodigoInterno(int? id) {
		if (id is null)
			return new Result<EspecialidadMedica2025>.Error("El CodigoInterno no puede ser nulo.");

		// Buscamos la especialidad exacta por su código
		var esp = Todas.FirstOrDefault(e => e.CodigoInterno.Valor == id.Value);

		return esp is not null
			? new Result<EspecialidadMedica2025>.Ok(esp)
			: new Result<EspecialidadMedica2025>.Error(
				$"No existe la especialidad con CodigoInterno = {id}."
			);
	}



}
