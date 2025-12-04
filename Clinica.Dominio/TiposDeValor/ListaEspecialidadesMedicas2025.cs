using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;
using System.Text;

namespace Clinica.Dominio.ListasOrganizadoras;

public sealed record ListaEspecialidadesMedicas2025(
	IReadOnlyList<Especialidad2025> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores is null || Valores.Count == 0)
			return "No hay especialidades asignadas.";

        StringBuilder sb = new();
		sb.AppendLine("Listado de especialidades:");

		foreach (Especialidad2025 esp in Valores)
			sb.AppendLine($"  • {esp.ATexto()}");

		return sb.ToString();
	}
	// Factory 1: desde una lista de Result<Especialidad>
	public static Result<ListaEspecialidadesMedicas2025> Crear(IReadOnlyList<Result<Especialidad2025>> results)
		=> results.Bind(list =>
			new Result<ListaEspecialidadesMedicas2025>.Ok(
				new ListaEspecialidadesMedicas2025(list)
			)
		);

	// Factory 2: desde una lista ya validada de Especialidad2025
	public static Result<ListaEspecialidadesMedicas2025> Crear(IReadOnlyList<Especialidad2025> okList)
		=> new Result<ListaEspecialidadesMedicas2025>.Ok(
			new ListaEspecialidadesMedicas2025(okList)
		);

	// Factory 3: desde una sola especialidad
	public static Result<ListaEspecialidadesMedicas2025> CrearConUnicaEspecialidad(Especialidad2025 unaSola)
		=> new Result<ListaEspecialidadesMedicas2025>
		.Ok(new ListaEspecialidadesMedicas2025([unaSola])
	);
	public static Result<ListaEspecialidadesMedicas2025> CrearConUnicaEspecialidad(Result<Especialidad2025> unaSolaResult) 
		=> unaSolaResult.Bind(unaSola => new
			Result<ListaEspecialidadesMedicas2025>.Ok(new ListaEspecialidadesMedicas2025([unaSola]))
		);
}

