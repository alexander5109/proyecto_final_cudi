using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;

namespace Clinica.Dominio.Politicas;

public static class PoliticaDeAtencionMedica {

	public static Result<Unit> PuedeDiagnosticarTurno(
		TurnoEstadoEnum estadoTurno,
		DateTime fechaTurno,
		bool tieneAtencion
	) {
		if (estadoTurno != TurnoEstadoEnum.Concretado)
			return new Result<Unit>.Error("El paciente aún no se presentó en la clínica.");

		if (DateTime.Today < fechaTurno.Date)
			return new Result<Unit>.Error("El diagnóstico solo puede cargarse el día del turno o después.");

		if (tieneAtencion)
			return new Result<Unit>.Error("Este turno ya tiene un diagnóstico registrado.");

		return new Result<Unit>.Ok(Unit.Valor);
	}
}
