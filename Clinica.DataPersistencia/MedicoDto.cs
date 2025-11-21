using Clinica.Dominio.TiposDeValor;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;


namespace Clinica.DataPersistencia;

public record MedicoDto(
	int Id,
	string Nombre,
	string Apellido,
	string Dni,
	DateTime FechaIngreso,
	bool HaceGuardias,
	double SueldoMinimo
) {
	/*
	public static MedicoDto FromDomain(Medico2025 medicoDomain, int medicoId) => new MedicoDto(
		medicoId,
		medicoDomain.NombreCompleto.Nombre,
		medicoDomain.NombreCompleto.Apellido,
		medicoDomain.Dni.Valor,
		medicoDomain.FechaIngreso.Valor,
		medicoDomain.HaceGuardias,
		medicoDomain.SueldoMinimoGarantizado.Valor
	);

	public static Result<Medico2025> ToDomain(MedicoDto medicoDto, List<MedicoEspecialidadDto> especialidadDtos)
		=> Medico2025.Crear(
			NombreCompleto2025.Crear(medicoDto.Nombre, medicoDto.Apellido),

				ListaEspecialidadesMedicas2025.Crear([
					EspecialidadMedica2025.CrearPorCodigoInterno(1),
					EspecialidadMedica2025.CrearPorCodigoInterno(2),
				]),
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
			Nombre = reader["Name"]?.ToString() ?? "",
			Apellido = reader["LastName"]?.ToString() ?? "",
			Dni = reader["Dni"]?.ToString() ?? "",
			Provincia = reader["Provincia"]?.ToString() ?? "",
			Domicilio = reader["Domicilio"]?.ToString() ?? "",
			Localidad = reader["Localidad"]?.ToString() ?? "",
			Telefono = reader["Telefono"]?.ToString() ?? "",
			Guardia = reader["Guardia"] != DBNull.Value && reader.GetBoolean(reader.GetOrdinal("Guardia")),
			FechaIngreso = reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
			SueldoMinimoGarantizado = reader["SueldoMinimoGarantizado"] != DBNull.Value
				? Convert.ToDouble(reader["SueldoMinimoGarantizado"])
				: 0.0,
			Horarios = horarios
		};
	}

}
public record MedicoEspecialidadDto(
	int MedicoId,
	string Especialidad
);
public record MedicoHorarioDto(
	int MedicoId,
	int DiaSemana,
	TimeSpan HoraDesde,
	TimeSpan HoraHasta
) {
	public static MedicoHorarioDto FromDomain(HorarioMedico2025 horarioMedicoDomain)
		=> new() {
			MedicoId = null,
			DiaSemana = horarioMedicoDomain.DiaSemana.Valor,
			HoraDesde = horarioMedicoDomain.Desde.Valor,
			HoraHasta = horarioMedicoDomain.Hasta.Valor,
		};

}
public record MedicoContactoDto(
	int MedicoId,
	string Telefono
);
public record MedicoDomicilioDto(
	int MedicoId,
	string Calle,
	string Numero,
	string Localidad
);


	*/

}