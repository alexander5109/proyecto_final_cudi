using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Tipos;

namespace Clinica.DataPersistencia.ModelDtos;
public record class MedicoDto {
	public required string? Id { get; set; } = string.Empty;
	public required string Name { get; set; } = string.Empty;
	public required string LastName { get; set; } = string.Empty;
	public required string Dni { get; set; } = string.Empty;
	public required string Provincia { get; set; } = string.Empty;
	public required string Domicilio { get; set; } = string.Empty;
	public required string Localidad { get; set; } = string.Empty;
	public required string Especialidad { get; set; } = string.Empty;
	public required string EspecialidadRama { get; set; } = string.Empty;
	public required string Telefono { get; set; } = string.Empty;
	public required bool Guardia { get; set; }
	public required string FechaIngreso { get; set; }
	public required decimal SueldoMinimoGarantizado { get; set; }
	public required List<HorarioMedicoDto> Horarios { get; set; } = [];



	public static MedicoDto FromDomain(Medico2025 medicoDomain)
		=> new MedicoDto {
			Id = null,
			Name = medicoDomain.NombreCompleto.Nombre,
			LastName = medicoDomain.NombreCompleto.Apellido,
			Dni = medicoDomain.Dni.Valor,
			Provincia = medicoDomain.Domicilio.Localidad.Provincia.Nombre,
			Domicilio = medicoDomain.Domicilio.Direccion,
			Localidad = medicoDomain.Domicilio.Localidad.Nombre,
			Especialidad = medicoDomain.Especialidad.Titulo,
			EspecialidadRama = medicoDomain.Especialidad.Rama,
			Telefono = medicoDomain.Telefono.Valor,
			Guardia = medicoDomain.HaceGuardias,
			FechaIngreso = medicoDomain.FechaIngreso.Valor.ToString(),
			SueldoMinimoGarantizado = medicoDomain.SueldoMinimoGarantizado.Valor,
			Horarios = medicoDomain.ListaHorarios.Valores.Select(h => HorarioMedicoDto.FromDomain(h)).ToList();
		};
	public static Result<Medico2025> ToDomain(MedicoDto medicoDto)
		=> Medico2025.Crear(
			NombreCompleto2025.Crear(medicoDto.Name, medicoDto.LastName),
			MedicoEspecialidad2025.Crear(medicoDto.Especialidad, medicoDto.EspecialidadRama),
			DniArgentino2025.Crear(medicoDto.Dni),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(
					medicoDto.Localidad,
					ProvinciaArgentina2025.Crear(medicoDto.Provincia)
				),
				medicoDto.Domicilio
			),
			ContactoTelefono2025.Crear(medicoDto.Telefono),
			ListaHorarioMedicos2025.Crear(medicoDto.Horarios.Select(h => h.ToDomain())),
			FechaIngreso2025.Crear(medicoDto.FechaIngreso),
			MedicoSueldoMinimo2025.Crear(medicoDto.SueldoMinimoGarantizado),
			medicoDto.Guardia
		);
}

