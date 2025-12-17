using System;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DBModels.DbModels;

namespace Clinica.Shared.ApiDtos;

public record AtencionDto(
	int TurnoId,
	int PacienteId,
	int MedicoId,
	string Observaciones
) {
	public Result<Atencion2025> ToDomain() => Atencion2025.CrearResult(
			pacienteId: PacienteId2025.Crear(PacienteId),
			medicoId: MedicoId2025.Crear(MedicoId),
			turnoId: TurnoId2025.Crear(TurnoId),
			fecha: DateTime.Now,
			observaciones: Observaciones
		);
}

// DTO específico para modificar solo observaciones
public record ModificarObservacionDto(
	string Observaciones
) {
	public string ToDomain() => Observaciones;
}
