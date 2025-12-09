using System.Text.Json;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.Dtos;

public static class ApiDtos {


	public record SolicitarCancelacionDto(
		TurnoId TurnoId,
		DateTime OutcomeFecha,
		string OutcomeComentario
	);
	public record SolicitarReprogramacionDto(
		TurnoId TurnoId,
		DateTime OutcomeFecha,
		string OutcomeComentario
	);

	public record SolicitarTurnoPrimeraDispDto(
		PacienteId PacienteId,
		EspecialidadCodigo Especialidad,
		DateTime FechaCreacion
	);



	public record UsuarioLoginRequestDto(string Username, string UserPassword);
	public record UsuarioLoginResponseDto(string Username, UsuarioEnumRole EnumRole, string Token);










	public record PacienteApiDto(
		PacienteId Id,
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo2025 ProvinciaCodigo,
		string Telefono,
		string Email,
		DateTime FechaNacimiento
	) {
		public PacienteApiDto()
			: this(default!, "", "", "", default, "", "", default, "", "", default) { }
	}
	public static Result<Paciente2025Agg> ToDomainAgg(this PacienteApiDto pacientedto) {
		return Paciente2025Agg.CrearResult(
			PacienteId.CrearResult(pacientedto.Id.Valor),
			Paciente2025.CrearResult(
				NombreCompleto2025.CrearResult(pacientedto.Nombre, pacientedto.Apellido),
				DniArgentino2025.CrearResult(pacientedto.Dni),
				Contacto2025.CrearResult(
				ContactoEmail2025.CrearResult(pacientedto.Email),
				ContactoTelefono2025.CrearResult(pacientedto.Telefono)),
				DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(
					pacientedto.Localidad,
					ProvinciaArgentina2025.CrearResultPorCodigo(
						pacientedto.ProvinciaCodigo)
					)
				, pacientedto.Domicilio),
				FechaDeNacimiento2025.CrearResult(pacientedto.FechaNacimiento),
				pacientedto.FechaIngreso
			)
		);
	}
	public static Result<Paciente2025> ToDomain(this PacienteApiDto pacientedto) {
		return Paciente2025.CrearResult(
			//PacienteId.CrearResult(pacientedto.Id.Valor),
			NombreCompleto2025.CrearResult(pacientedto.Nombre, pacientedto.Apellido),
			DniArgentino2025.CrearResult(pacientedto.Dni),
			Contacto2025.CrearResult(
			ContactoEmail2025.CrearResult(pacientedto.Email),
			ContactoTelefono2025.CrearResult(pacientedto.Telefono)),
			DomicilioArgentino2025.CrearResult(
			LocalidadDeProvincia2025.CrearResult(
				pacientedto.Localidad,
				ProvinciaArgentina2025.CrearResultPorCodigo(
					pacientedto.ProvinciaCodigo)
				)
			, pacientedto.Domicilio),
			FechaDeNacimiento2025.CrearResult(pacientedto.FechaNacimiento),
			pacientedto.FechaIngreso
		);
	}

	public static PacienteApiDto ToDto(this Paciente2025 paciente) {
		return new PacienteApiDto {
			Dni = paciente.Dni.Valor,
			Nombre = paciente.NombreCompleto.NombreValor,
			Apellido = paciente.NombreCompleto.ApellidoValor,
			FechaIngreso = paciente.FechaIngreso,
			Domicilio = paciente.Domicilio.DireccionValor,
			Localidad = paciente.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo = paciente.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono = paciente.Contacto.Telefono.Valor,
			Email = paciente.Contacto.Email.Valor,
			FechaNacimiento = paciente.FechaNacimiento.Valor.ToDateTime(TimeOnly.MaxValue),
		};
	}
	public static PacienteApiDto ToDto(this Paciente2025Agg agg) {
		return new PacienteApiDto {
			Id = agg.Id,
			Dni = agg.Paciente.Dni.Valor,
			Nombre = agg.Paciente.NombreCompleto.NombreValor,
			Apellido = agg.Paciente.NombreCompleto.ApellidoValor,
			FechaIngreso = agg.Paciente.FechaIngreso,
			Domicilio = agg.Paciente.Domicilio.DireccionValor,
			Localidad = agg.Paciente.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo = agg.Paciente.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono = agg.Paciente.Contacto.Telefono.Valor,
			Email = agg.Paciente.Contacto.Email.Valor,
			FechaNacimiento = agg.Paciente.FechaNacimiento.Valor.ToDateTime(TimeOnly.MaxValue),
		};
	}




	public record TurnoDto(
		//TurnoId Id,
		DateTime FechaCreacionSolicitud,
		PacienteId PacienteId,
		MedicoId MedicoId,
		EspecialidadCodigo EspecialidadCodigo,
		DateTime FechaHoraAsignadaDesde,
		DateTime FechaHoraAsignadaHasta,
		TurnoEstadoCodigo OutcomeEstado,
		DateTime? OutcomeFecha,
		string? OutcomeComentario
	) {
		// Constructor sin parámetros requerido por algunos ORMs/serializadores
		public TurnoDto() : this(default, default!, default!, default!, default, default, default!, default, default) { }
	};

	public static TurnoDto ToDto(this Turno2025 turno) {
		return new TurnoDto(
			turno.FechaDeCreacion,
			turno.PacienteId,
			turno.MedicoId,
			turno.Especialidad.Codigo,
			turno.FechaHoraAsignadaDesdeValor,
			turno.FechaHoraAsignadaHastaValor,
			turno.OutcomeEstado,
			turno.OutcomeFechaOption.Match(d => d, () => (DateTime?)null),
			turno.OutcomeComentarioOption.Match(s => s, () => (string?)null)
		);
	}
	//public static Result<Turno2025> ToDomain(this TurnoDto turnoDto) {
	//	return Turno2025.CrearResult(
	//		//TurnoId.CrearResult(turnoDto.Id.Valor),
	//		FechaRegistro2025.CrearResult(turnoDto.FechaCreacionSolicitud),
	//		PacienteId.CrearResult(turnoDto.PacienteId.Valor),
	//		MedicoId.CrearResult(turnoDto.MedicoId.Valor),
	//		Especialidad2025.CrearResult(turnoDto.EspecialidadCodigo),
	//		turnoDto.FechaHoraAsignadaDesde,
	//		turnoDto.FechaHoraAsignadaHasta,
	//		turnoDto.OutcomeEstado,
	//		turnoDto.OutcomeFecha.ToOption(),
	//		turnoDto.OutcomeComentario.ToOption()
	//	);
	//}
	public record UsuarioDto(
		//UsuarioId Id,
		string NombreUsuario,
		string PasswordHash,
		UsuarioEnumRole EnumRole
	) {
		// Constructor sin parámetros requerido por Dapper / serializadores
		public UsuarioDto() : this("", "", default) { }
	}
	public static Result<Usuario2025> ToDomain(this UsuarioDto usuario)
		=> Usuario2025.CrearResult(usuario.NombreUsuario, usuario.PasswordHash, usuario.EnumRole);

	public static UsuarioDto ToDto(this Usuario2025 entidad) {
		return new UsuarioDto(entidad.NombreUsuario.Valor, entidad.PasswordHash.Valor, entidad.EnumRole);
	}


	public record HorarioDto(
		MedicoId MedicoId,
		DayOfWeek DiaSemana,
		TimeSpan HoraDesde,
		TimeSpan HoraHasta,
		DateTime VigenteDesde,
		DateTime VigenteHasta
	) {
		public HorarioDto()
			: this(default, default, default, default, default, default) { }
	}
	public static HorarioDto ToDto(this Horario2025 instance) {
		return new HorarioDto {
			MedicoId = instance.MedicoId,
			DiaSemana = instance.DiaSemana,
			HoraDesde = instance.HoraDesde.Valor.ToTimeSpan(),
			HoraHasta = instance.HoraHasta.Valor.ToTimeSpan(),
			VigenteDesde = instance.VigenteDesde.ToDateTime(TimeOnly.MaxValue),
			VigenteHasta = instance.VigenteHasta.ToDateTime(TimeOnly.MaxValue)
		};
	}

	public static Result<Horario2025> ToDomain(this HorarioDto horarioDto) {
		return Horario2025.CrearResult(
			horarioDto.MedicoId,
			horarioDto.DiaSemana,
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraDesde)),
			new HorarioHora2025(TimeOnly.FromTimeSpan(horarioDto.HoraHasta)),
			new DateOnly(2014, 1, 1),
			new DateOnly(2026, 1, 30)
		);
	}

	public record MedicoDto(
		MedicoId Id,
		EspecialidadCodigo EspecialidadCodigo,
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo2025 ProvinciaCodigo,
		string Telefono,
		string Email,
		bool HaceGuardias,
		string? HorariosJson
	) {
		public MedicoDto()
			: this(default!, default, "", "", "", default, "", "", default, "", "", default, null) { }
	}


	public static Result<Medico2025> ToDomain(this MedicoDto medicoDto) {
		string json = string.IsNullOrWhiteSpace(medicoDto.HorariosJson) ? "[]" : medicoDto.HorariosJson;
		List<HorarioDto> horariosDto = JsonSerializer.Deserialize<List<HorarioDto>>(json)
			?? [];
		return Medico2025.CrearResult(
			MedicoId.CrearResult(medicoDto.Id.Valor),
			NombreCompleto2025.CrearResult(medicoDto.Nombre, medicoDto.Apellido),
			//ListaEspecialidadesMedicas2025.CrearConUnicaEspecialidad(
			Especialidad2025.CrearResult(medicoDto.EspecialidadCodigo),
			DniArgentino2025.CrearResult(medicoDto.Dni),
			DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(
					medicoDto.Localidad,
					ProvinciaArgentina2025.CrearResultPorCodigo(medicoDto.ProvinciaCodigo)),
				medicoDto.Domicilio
			),
			ContactoTelefono2025.CrearResult(medicoDto.Telefono),
			ContactoEmail2025.CrearResult(medicoDto.Email),
			ListaHorarioMedicos2025.CrearResult(horariosDto.Select(x => x.ToDomain())),
			medicoDto.FechaIngreso,
			medicoDto.HaceGuardias
		);
	}


	public static MedicoDto ToDto(this Medico2025Agg aggrg) {
		return new MedicoDto {
			Id = aggrg.Medico.Id,
			EspecialidadCodigo = aggrg.Medico.EspecialidadUnica.Codigo,
			Dni = aggrg.Medico.Dni.Valor,
			Nombre = aggrg.Medico.NombreCompleto.NombreValor,
			Apellido = aggrg.Medico.NombreCompleto.ApellidoValor,
			FechaIngreso = aggrg.Medico.FechaIngreso,
			Domicilio = aggrg.Medico.Domicilio.DireccionValor,
			Localidad = aggrg.Medico.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo = aggrg.Medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono = aggrg.Medico.Telefono.Valor,
			Email = aggrg.Medico.Email.Valor,
			HaceGuardias = aggrg.Medico.HaceGuardiasValor,
			HorariosJson = JsonSerializer.Serialize(aggrg.Medico.ToDto())
		};
	}

	public static MedicoDto ToDto(this Medico2025 medico) {
		return new MedicoDto {
			Id = medico.Id,
			EspecialidadCodigo = medico.EspecialidadUnica.Codigo,
			Dni = medico.Dni.Valor,
			Nombre = medico.NombreCompleto.NombreValor,
			Apellido = medico.NombreCompleto.ApellidoValor,
			FechaIngreso = medico.FechaIngreso,
			Domicilio = medico.Domicilio.DireccionValor,
			Localidad = medico.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo = medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono = medico.Telefono.Valor,
			Email = medico.Email.Valor,
			HaceGuardias = medico.HaceGuardiasValor,
			HorariosJson = JsonSerializer.Serialize(medico.ToDto())
		};
	}


	//public record PacienteListDto(
	//	PacienteId Id,
	//	string Dni,
	//	string Username,
	//	string Apellido,
	//	string Email,
	//	string Telefono
	//);
	//public record TurnoListDto(
	//	TurnoId Id,
	//	TimeSpan Hora,
	//	DateTime Fecha,
	//	EspecialidadCodigo EspecialidadCodigo,
	//	TurnoEstadoCodigo Estado,
	//	MedicoId MedicoId
	//);

	//public record MedicoListDto(
	//	string Dni,
	//	string Username,
	//	string Apellido,
	//	EspecialidadCodigo EspecialidadCodigo
	//);










	public record ReprogramarTurnoRequestDto(
		DateTime NuevaFechaDesde,
		DateTime NuevaFechaHasta
	);


	public record CrearTurnoRequestDto(
		PacienteId PacienteId,
		MedicoId MedicoId,
		EspecialidadCodigo EspecialidadCodigo,
		DateTime Desde,
		DateTime Hasta
	);

	public record CancelarTurnoRequest(
		DateTime OutcomeFecha,
		string OutcomeComentario
	);

	//public record DisponibilidadDTO(
	//	MedicoId MedicoId,
	//	DateTime FechaInicio,
	//	DateTime FechaFin,
	//	EspecialidadCodigo EspecialidadCodigo
	//);


	public record ReprogramarTurnoRequest(
		DateTime OutcomeFecha,
		string OutcomeComentario
	);

	public record SolicitarTurnoRequest(
		PacienteId PacienteId,
		string Especialidad   // string para no acoplar la API al enum interno
	);

}
