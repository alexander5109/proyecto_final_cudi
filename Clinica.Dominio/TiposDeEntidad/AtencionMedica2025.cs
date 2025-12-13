using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.TiposDeEntidad;

/*
public sealed class AtencionMedica {
	public AtencionId Id { get; }
	public TurnoId TurnoId { get; }
	public MedicoId MedicoId { get; }
	public DateTime Fecha { get; }

	public Diagnostico Diagnostico { get; private set; }
	public Observaciones Observaciones { get; private set; }

	public void RegistrarDiagnostico(...) { }
}
*/

// ✔️ Para la siguiente iteración

// Crear AtencionMedica

// Relación 1–1 con Turno

// UI exclusiva para médicos


// “El turno modela la logística.
// Cuando se concreta, nace una atención médica, que es una entidad clínica distinta con reglas y permisos propios.”