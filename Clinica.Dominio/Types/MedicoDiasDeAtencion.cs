using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Types;

public readonly record struct MedicoDiasDeAtencion {
	private readonly List<HorarioMedico> _horarios;

	private MedicoDiasDeAtencion(List<HorarioMedico> horarios) => _horarios = horarios;

	public static Result<MedicoDiasDeAtencion> Crear(IEnumerable<HorarioMedico> horarios) {
		var list = horarios.ToList();

		// Validar solapamiento
		//var solapados = list
		//	.GroupBy(h => h.DiaSemana.DiaNombre)
		//	.Any(g => g.Any(x => g.Any(y => x != y && x.Desde < y.Hasta && y.Desde < x.Hasta)));

		//if (solapados)
		//	return new Result<MedicoDiasDeAtencion>.Error("Los horarios de atención se superponen.");

		return new Result<MedicoDiasDeAtencion>.Ok(new(list));
	}

	public IReadOnlyList<HorarioMedico> Horarios => _horarios;
}
