using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Clinica.DataPersistencia.ModelDtos;

public record class MedicoDto {
	public required string Id { get; set; } = string.Empty;
	public required string Name { get; set; } = string.Empty;
	public required string LastName { get; set; } = string.Empty;
	public required string Dni { get; set; } = string.Empty;
	public required string Provincia { get; set; } = string.Empty;
	public required string Domicilio { get; set; } = string.Empty;
	public required string Localidad { get; set; } = string.Empty;
	public required string Especialidad { get; set; } = string.Empty;
	public required string Telefono { get; set; } = string.Empty;
	public required bool Guardia { get; set; }
	public required string FechaIngreso { get; set; }
	public required decimal SueldoMinimoGarantizado { get; set; }
	public required List<HorarioMedicoDto> Horarios { get; set; } = new List<HorarioMedicoDto>();


	public static MedicoDto FromSQLReader(SqlDataReader reader) {
		// 1. Leer JSON de horarios
		var horariosJson = reader["Horarios"] as string;

		var horarios = new List<HorarioMedicoDto>();

		if (!string.IsNullOrWhiteSpace(horariosJson)) {
			try {
				var array = JArray.Parse(horariosJson);

				foreach (var token in array) {
					horarios.Add(new HorarioMedicoDto {
						DiaSemana = token["DiaSemana"]?.ToString() ?? "",
						Desde = token["HoraDesde"]?.ToString() ?? "",
						Hasta = token["HoraHasta"]?.ToString() ?? "",
						MedicoId = token["MedicoId"]?.ToString() // puede ser null
					});
				}
			} catch (Exception) {
				// Si el JSON está corrupto, no cortamos la lectura del médico
				// pero dejamos lista vacía.
			}
		}

		// 2. Construir DTO principal
		return new MedicoDto {
			Id = reader["Id"]?.ToString() ?? "",
			Name = reader["Name"]?.ToString() ?? "",
			LastName = reader["LastName"]?.ToString() ?? "",
			Dni = reader["Dni"]?.ToString() ?? "",
			Provincia = reader["Provincia"]?.ToString() ?? "",
			Domicilio = reader["Domicilio"]?.ToString() ?? "",
			Localidad = reader["Localidad"]?.ToString() ?? "",
			Especialidad = reader["Especialidad"]?.ToString() ?? "",
			Telefono = reader["Telefono"]?.ToString() ?? "",
			Guardia = reader["Guardia"] != DBNull.Value && Convert.ToBoolean(reader["Guardia"]),
			FechaIngreso = reader["FechaIngreso"]?.ToString() ?? "",
			SueldoMinimoGarantizado = reader["SueldoMinimoGarantizado"] == DBNull.Value
										 ? 0
										 : Convert.ToDecimal(reader["SueldoMinimoGarantizado"]),
			Horarios = horarios
		};
	}


	public static MedicoDto FromDomain(Medico2025 medicoDomain, string medicoId)
		=> new MedicoDto {
			Id = medicoId,
			Name = medicoDomain.NombreCompleto.Nombre,
			LastName = medicoDomain.NombreCompleto.Apellido,
			Dni = medicoDomain.Dni.Valor,
			Provincia = medicoDomain.Domicilio.Localidad.Provincia.Nombre,
			Domicilio = medicoDomain.Domicilio.Direccion,
			Localidad = medicoDomain.Domicilio.Localidad.Nombre,
			Especialidad = medicoDomain.Especialidad.Titulo,
			Telefono = medicoDomain.Telefono.Valor,
			Guardia = medicoDomain.HaceGuardias,
			FechaIngreso = medicoDomain.FechaIngreso.Valor.ToString(),
			SueldoMinimoGarantizado = medicoDomain.SueldoMinimoGarantizado.Valor,
			Horarios = medicoDomain.ListaHorarios.Valores.Select(h => HorarioMedicoDto.FromDomain(h)).ToList()
		};
	public static Result<Medico2025> ToDomain(MedicoDto medicoDto)
		=> Medico2025.Crear(
			NombreCompleto2025.Crear(medicoDto.Name, medicoDto.LastName),
			EspecialidadMedica2025.Crear(medicoDto.Especialidad),
			DniArgentino2025.Crear(medicoDto.Dni),
			DomicilioArgentino2025.Crear(
				LocalidadDeProvincia2025.Crear(
					medicoDto.Localidad,
					ProvinciaArgentina2025.Crear(medicoDto.Provincia)
				),
				medicoDto.Domicilio
			),
			ContactoTelefono2025.Crear(medicoDto.Telefono),
			ListaHorarioMedicos2025.Crear(medicoDto.Horarios.Select(h => HorarioMedicoDto.ToDomain(h))),
			FechaIngreso2025.Crear(medicoDto.FechaIngreso),
			MedicoSueldoMinimo2025.Crear(medicoDto.SueldoMinimoGarantizado),
			medicoDto.Guardia
		);
}

