using Clinica.Dominio.TiposDeValor;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.DataPersistencia.ModelDtos;

public class MedicoDto {
	public int Id { get; init; }              // INT → int
	public string Name { get; init; } = "";
	public string LastName { get; init; } = "";
	public string Dni { get; init; } = "";    // NCHAR(8) → string
	public string Provincia { get; init; } = "";
	public string Domicilio { get; init; } = "";
	public string Localidad { get; init; } = "";
	public int EspecialidadCodigoInterno { get; init; }  // INT in Medico table // (You used "Especialidad" string, but DB returns an INT)
	public string Telefono { get; init; } = "";
	public bool Guardia { get; init; }        // BIT → bool
	public DateTime FechaIngreso { get; init; } // DATETIME → DateTime
	public double SueldoMinimoGarantizado { get; init; } // FLOAT(53) = SQL double precision → C# double // (NOT decimal)

	public List<HorarioMedicoDto> Horarios { get; init; } = new();


	public static MedicoDto FromSQLReader(SqlDataReader reader) {
		// --- 1) Parsear horarios JSON ---
		var horarios = new List<HorarioMedicoDto>();

		if (reader["Horarios"] is string horariosJson &&
			!string.IsNullOrWhiteSpace(horariosJson)) {
			try {
				var array = JArray.Parse(horariosJson);

				foreach (var token in array) {
					horarios.Add(new HorarioMedicoDto {
						Id = int.Parse(token["Id"].ToString()),
						MedicoId = int.Parse(token["MedicoId"].ToString()),
						DiaSemana = token["DiaSemana"].ToString(),
						Desde = token["HoraDesde"].ToString(),
						Hasta = token["HoraHasta"].ToString(),
					});
				}
			} catch {
				// JSON corrupto → dejar lista vacía
			}
		}

		// --- 2) Construir DTO principal ---
		return new MedicoDto {
			Id = reader.GetInt32(reader.GetOrdinal("Id")),
			Name = reader["Name"]?.ToString() ?? "",
			LastName = reader["LastName"]?.ToString() ?? "",
			Dni = reader["Dni"]?.ToString() ?? "",
			Provincia = reader["Provincia"]?.ToString() ?? "",
			Domicilio = reader["Domicilio"]?.ToString() ?? "",
			Localidad = reader["Localidad"]?.ToString() ?? "",
			EspecialidadCodigoInterno = reader.GetInt32(reader.GetOrdinal("EspecialidadCodigoInterno")),
			Telefono = reader["Telefono"]?.ToString() ?? "",
			Guardia = reader["Guardia"] != DBNull.Value && reader.GetBoolean(reader.GetOrdinal("Guardia")),
			FechaIngreso = reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
			SueldoMinimoGarantizado = reader["SueldoMinimoGarantizado"] != DBNull.Value
				? Convert.ToDouble(reader["SueldoMinimoGarantizado"])
				: 0.0,
			Horarios = horarios
		};
	}



	public static MedicoDto FromDomain(Medico2025 medicoDomain, int medicoId)
		=> new MedicoDto {
			Id = medicoId,
			Name = medicoDomain.NombreCompleto.Nombre,
			LastName = medicoDomain.NombreCompleto.Apellido,
			Dni = medicoDomain.Dni.Valor,
			Provincia = medicoDomain.Domicilio.Localidad.Provincia.Nombre,
			Domicilio = medicoDomain.Domicilio.Direccion,
			Localidad = medicoDomain.Domicilio.Localidad.Nombre,
			EspecialidadCodigoInterno = medicoDomain.Especialidad.CodigoInterno.Valor,
			Telefono = medicoDomain.Telefono.Valor,
			Guardia = medicoDomain.HaceGuardias,
			FechaIngreso = medicoDomain.FechaIngreso.Valor,
			SueldoMinimoGarantizado = medicoDomain.SueldoMinimoGarantizado.Valor,
			Horarios = medicoDomain.ListaHorarios.Valores.Select(h => HorarioMedicoDto.FromDomain(h)).ToList()
		};
	public static Result<Medico2025> ToDomain(MedicoDto medicoDto)
		=> Medico2025.Crear(
			NombreCompleto2025.Crear(medicoDto.Name, medicoDto.LastName),
			EspecialidadMedica2025.CrearPorCodigoInterno(medicoDto.EspecialidadCodigoInterno),
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

