using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;
using Clinica.Dominio.Comun;

namespace Clinica.DataPersistencia.Mapeadores;

public static class MedicoMapper {
	public static HorarioMedicoDto ToDto(this HorarioMedicoType horario)
		=> new HorarioMedicoDto {
			DiaSemana = (int)horario.DiaSemana.Valor,
			Desde = horario.Desde.Valor.ToTimeSpan(),
			Hasta = horario.Hasta.Valor.ToTimeSpan(),
			MedicoId = null
		};

	public static Result<HorarioMedicoType> ToDomain(this HorarioMedicoDto dto)
		=> HorarioMedico2025.Crear(
			DiaSemana2025.Crear(dto.DiaSemana),
			HorarioHora2025.Crear(TimeOnly.FromTimeSpan(dto.Desde)),
			HorarioHora2025.Crear(TimeOnly.FromTimeSpan(dto.Hasta))
		);
	public static Result<MedicoType> ToDomain(this MedicoDto dto)
		=> Medico2025.Crear(
			NombreCompleto2025.Crear(dto.Name, dto.LastName),
			MedicoEspecialidad2025.Crear(dto.Especialidad, dto.EspecialidadRama),
			DniArgentino2025.Crear(dto.Dni),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(
					dto.Localidad,
					ProvinciaArgentina2025.Crear("Buenos Aires")
				),
				dto.Domicilio
			),
			ContactoTelefono2025.Crear(dto.Telefono),
			[.. dto.Horarios.Select(h => h.ToDomain())], 
			FechaIngreso2025.Crear(DateOnly.FromDateTime(dto.FechaIngreso)),
			MedicoSueldoMinimo2025.Crear(dto.SueldoMinimoGarantizado),
			dto.Guardia
		);


	public static MedicoDto ToDto(this MedicoType medico, string id)
		=> new MedicoDto {
			Id = id,
			Name = medico.NombreCompleto.Nombre,
			LastName = medico.NombreCompleto.Apellido,
			Dni = medico.Dni.Valor,
			Provincia = medico.Domicilio.Localidad.Provincia.Nombre,
			Domicilio = medico.Domicilio.Direccion,
			Localidad = medico.Domicilio.Localidad.Nombre,
			Especialidad = medico.Especialidad.Titulo,
			EspecialidadRama = medico.Especialidad.Rama,
			Telefono = medico.Telefono.Valor,
			Guardia = medico.HaceGuardias,
			FechaIngreso = medico.FechaIngreso.Valor.ToDateTime(TimeOnly.MinValue),
			SueldoMinimoGarantizado = medico.SueldoMinimoGarantizado.Valor,
			Horarios = medico.Horarios.Select(h => h.ToDto()).ToList()
		};

}
