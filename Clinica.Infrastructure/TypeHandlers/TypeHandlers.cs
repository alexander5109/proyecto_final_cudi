using System.Data;
using Clinica.Dominio.TiposDeIdentificacion;
using Dapper;

namespace Clinica.Infrastructure.TypeHandlers;

public class TurnoIdHandler : SqlMapper.TypeHandler<TurnoId2025> {
	public override void SetValue(IDbDataParameter parameter, TurnoId2025 value) {
		parameter.Value = value.Valor; // store as int in DB
	}

	public override TurnoId2025 Parse(object value) {
		return TurnoId2025.Crear(Convert.ToInt32(value)); // read from DB as int
	}
}

public class MedicoIdHandler : SqlMapper.TypeHandler<MedicoId2025> {
	public override void SetValue(IDbDataParameter parameter, MedicoId2025 value) {
		parameter.Value = value.Valor; // store as int in DB
	}
	public override MedicoId2025 Parse(object value) {
		return MedicoId2025.Crear(Convert.ToInt32(value)); // read from DB as int
	}
}

public class HorarioIdHandler : SqlMapper.TypeHandler<HorarioId2025> {
	public override void SetValue(IDbDataParameter parameter, HorarioId2025 value) {
		parameter.Value = value.Valor; // store as int in DB
	}
	public override HorarioId2025 Parse(object value) {
		return HorarioId2025.Crear(Convert.ToInt32(value)); // read from DB as int
	}
}

public class PacienteIdHandler : SqlMapper.TypeHandler<PacienteId2025> {
	public override void SetValue(IDbDataParameter parameter, PacienteId2025 value) {
		parameter.Value = value.Valor; // store as int in DB
	}

	public override PacienteId2025 Parse(object value) {
		return PacienteId2025.Crear(Convert.ToInt32(value)); // read from DB as int
	}
}



public class UsuarioIdHandler : SqlMapper.TypeHandler<UsuarioId2025> {
	public override void SetValue(IDbDataParameter parameter, UsuarioId2025 value) {
		parameter.Value = value.Valor; // store as int in DB
	}

	public override UsuarioId2025 Parse(object value) {
		return UsuarioId2025.Crear(Convert.ToInt32(value)); // read from DB as int
	}
}

