

using System.Net;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.DataPersistencia;

public record TurnoDto(
	int Id,
	DateTime FechaDeCreacion,
	int PacienteId,
	int MedicoId,
	int EspecialidadCodigo,
	DateTime FechaHoraAsignadaDesde,
	DateTime FechaHoraAsignadaHasta,
	byte OutcomeEstado,
	DateTime? OutcomeFecha,
	string? OutcomeComentario
) {
	public Result<Turno2025> ToDomain() {
		throw new NotImplementedException();
		//return Turno2025.Crear(
		//	nombre,
		//	dni,
		//	contacto,
		//	domicilio,
		//	fechaNacimiento,
		//	ingreso
		//);
	}
}