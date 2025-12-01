using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Clinica.Dominio.Entidades;
using Dapper;

namespace Clinica.Infrastructure.TypeHandlers;

public class TurnoIdHandler : SqlMapper.TypeHandler<TurnoId> {
	public override void SetValue(IDbDataParameter parameter, TurnoId value) {
		parameter.Value = value.Valor; // store as int in DB
	}

	public override TurnoId Parse(object value) {
		return new TurnoId(Convert.ToInt32(value)); // read from DB as int
	}
}

public class MedicoIdHandler : SqlMapper.TypeHandler<MedicoId> {
	public override void SetValue(IDbDataParameter parameter, MedicoId value) {
		parameter.Value = value.Valor; // store as int in DB
	}
	public override MedicoId Parse(object value) {
		return new MedicoId(Convert.ToInt32(value)); // read from DB as int
	}
}

public class PacienteIdHandler : SqlMapper.TypeHandler<PacienteId> {
	public override void SetValue(IDbDataParameter parameter, PacienteId value) {
		parameter.Value = value.Valor; // store as int in DB
	}

	public override PacienteId Parse(object value) {
		return new PacienteId(Convert.ToInt32(value)); // read from DB as int
	}
}



public class UsuarioIdHandler : SqlMapper.TypeHandler<UsuarioId> {
	public override void SetValue(IDbDataParameter parameter, UsuarioId value) {
		parameter.Value = value.Valor; // store as int in DB
	}

	public override UsuarioId Parse(object value) {
		return new UsuarioId(Convert.ToInt32(value)); // read from DB as int
	}
}

