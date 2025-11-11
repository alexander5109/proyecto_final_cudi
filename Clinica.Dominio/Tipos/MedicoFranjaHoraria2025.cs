using Clinica.Dominio.Comun;
namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoFranjaHoraria2025(
	TimeOnly Desde,
	TimeOnly Hasta
)  {
	public static Result<MedicoFranjaHoraria2025> Crear(TimeOnly desde, TimeOnly hasta) {
		if (desde >= hasta)
			return new Result<MedicoFranjaHoraria2025>.Error("La hora de inicio debe ser anterior a la de fin.");

		return new Result<MedicoFranjaHoraria2025>.Ok(new(desde, hasta));
	}

	public bool SeSolapaCon(MedicoFranjaHoraria2025 otra)
		=> Desde < otra.Hasta && otra.Desde < Hasta;

	public override string ToString() => $"{Desde:HH:mm}–{Hasta:HH:mm}";
}
