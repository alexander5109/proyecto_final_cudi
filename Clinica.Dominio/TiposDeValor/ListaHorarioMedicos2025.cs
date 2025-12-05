using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.ListasOrganizadoras;

public readonly record struct ListaHorarioMedicos2025(
	IReadOnlyList<Horario2025> Valores
) : IComoTexto {
	public string ATexto() {
		if (Valores.Count == 0)
			return "Lista de horarios: (vacía)";
		IEnumerable<string> lineas = Valores
			.Select(v => "- " + v.ATexto());
		return "Lista de horarios:\n" + string.Join("\n", lineas);
	}

	public static Result<ListaHorarioMedicos2025> CrearResult(
		IEnumerable<Result<Horario2025>> horariosResult)
		=> horariosResult.Bind(horariosOk =>
			new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(horariosOk))
		);
}
