using Clinica.Dominio.Comun;
using Clinica.Dominio.Extentions;

namespace Clinica.Dominio.Entidades;

public readonly record struct Turno2025(
	Medico2025 Medico,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	TimeSpan DuracionMinutos
) {

	public static Result<Turno2025> Crear(Result<Medico2025> medicoResult, Result<Paciente2025> pacienteResult, DateTime? fechaYHora, int? duracionMinutos) {
		// 🟡 Validar resultados previos (early return)
		if (medicoResult is Result<Medico2025>.Error medicoError)
			return new Result<Turno2025>.Error($"Error en médico: {medicoError.Mensaje}");

		if (pacienteResult is Result<Paciente2025>.Error pacienteError)
			return new Result<Turno2025>.Error($"Error en paciente: {pacienteError.Mensaje}");

		// 🟢 Desempaquetar entidades válidas
		var medico = ((Result<Medico2025>.Ok)medicoResult).Valor;
		var paciente = ((Result<Paciente2025>.Ok)pacienteResult).Valor;

		// 🕒 Validar fecha
		if (fechaYHora is null)
			return new Result<Turno2025>.Error("La fecha y hora del turno es obligatoria.");

		if (fechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("El turno no puede ser en el pasado.");

		// ⏱️ Duración
		var duracion = TimeSpan.FromMinutes(duracionMinutos ?? 40);

		if (duracion.TotalMinutes is < 10 or > 120)
			return new Result<Turno2025>.Error("La duración del turno no es razonable (10–120 min).");

		// 👩‍⚕️ Validar disponibilidad del médico
		bool disponible = medico.ListaHorarios.TienenDisponibilidad(fechaYHora.Value, duracion);
		if (!disponible)
			return new Result<Turno2025>.Error("El médico no atiende en ese horario.");

		// ✅ Si todo está bien
		return new Result<Turno2025>.Ok(new Turno2025(medico, paciente, fechaYHora.Value, duracion));
	}


}
