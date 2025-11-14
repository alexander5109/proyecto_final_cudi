using Clinica.Dominio.Comun;
using System.Collections.Immutable;

namespace Clinica.Dominio.Entidades;

public readonly record struct MedicoHorarios2025 {
	public IReadOnlyList<HorarioMedico> Items { get; }
	private MedicoHorarios2025(IReadOnlyList<HorarioMedico> items){
		Items = items;
	}

	public static Result<MedicoHorarios2025> Crear(IReadOnlyList<Result<HorarioMedico>> horariosResult)
	{
		List<string> errores = new List<string>();
		List<HorarioMedico> horariosOk = new List<HorarioMedico>();
		foreach (Result<HorarioMedico> horarioResult in horariosResult)
		{
			if (horarioResult.IsSuccess)
			{
				horariosOk.Add(horarioResult.Valor);
			}
			else
			{
				errores.AddRange(horarioResult.Errors);
			}
		}
		if (errores.Count > 0)
		{
			return Result.Failure<MedicoHorarios2025>(errores);
		}
		return Result.Success(new MedicoHorarios2025(horariosOk.ToImmutableList()));
	}
}
