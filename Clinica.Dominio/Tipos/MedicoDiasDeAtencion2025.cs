using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoDiasDeAtencion2025 {
	private readonly List<HorarioMedico2025> _horarios;

	private MedicoDiasDeAtencion2025(List<HorarioMedico2025> horarios) => _horarios = horarios;

	public static Result<MedicoDiasDeAtencion2025> Crear(IEnumerable<HorarioMedico2025> horarios) {
		var list = horarios.ToList();

		// Validar solapamiento
		//var solapados = list
		//	.GroupBy(h => h.DiaSemana.DiaNombre)
		//	.Any(g => g.Any(x => g.Any(y => x != y && x.Desde < y.Hasta && y.Desde < x.Hasta)));

		//if (solapados)
		//	return new Result<MedicoDiasDeAtencion>.Error("Los horarios de atención se superponen.");

		return new Result<MedicoDiasDeAtencion2025>.Ok(new(list));
	}

	public IReadOnlyList<HorarioMedico2025> Horarios => _horarios;
}
