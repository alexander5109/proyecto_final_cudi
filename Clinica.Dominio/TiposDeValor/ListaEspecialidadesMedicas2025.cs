using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;
using System.Text;

namespace Clinica.Dominio.ListasOrganizadoras;

public sealed record ListaEspecialidadesMedicas2025(
	IReadOnlyList<EspecialidadMedica2025> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores is null || Valores.Count == 0)
			return "No hay especialidades asignadas.";

        StringBuilder sb = new();
		sb.AppendLine("Listado de especialidades:");

		foreach (EspecialidadMedica2025 esp in Valores)
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

