using Clinica.Dominio.Comun;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoDisponibilidadEnDia2025(
	MedicoDiaDeLaSemana2025 DiaSemana,
	IReadOnlyList<MedicoFranjaHoraria2025> FranjasHorarias
) {
	public static Result<MedicoDisponibilidadEnDia2025> Crear(MedicoDiaDeLaSemana2025 dia, IEnumerable<MedicoFranjaHoraria2025> franjas) {
		var lista = franjas.OrderBy(f => f.Desde).ToList();

		for (int i = 0; i < lista.Count - 1; i++) {
			if (lista[i].SeSolapaCon(lista[i + 1]))
				return new Result<MedicoDisponibilidadEnDia2025>.Error($"Las franjas del {dia} se solapan.");
		}

		return new Result<MedicoDisponibilidadEnDia2025>.Ok(new(dia, lista));
	}

	public override string ToString() => $"{DiaSemana}: {string.Join(", ", FranjasHorarias.Select(f => f.ToString()))}";
}
