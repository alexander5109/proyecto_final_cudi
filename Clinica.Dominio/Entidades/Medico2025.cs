using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;





public record struct Medico2025(
	NombreCompleto2025 Nombre,
	MedicoEspecialidad2025 Especialidad,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	Contacto2025 Contacto,
	MedicoAgenda2025 Agenda
) : IValidate<Contacto2025> {
	public static Result<Medico2025> Crear(
		string nombre,
		string apellido,
		string dni,
		string telefono,
		string email,
		string provincia,
		string localidad,
		string direccion,
		string especialidad,
		string rama,
		MedicoAgenda2025 agenda
	) {
		var nombreRes = NombreCompleto2025.Crear(nombre, apellido);
		if (nombreRes is Result<NombreCompleto2025>.Error e1) return new Result<Medico2025>.Error(e1.Mensaje);

		var dniRes = DniArgentino2025.Crear(dni);
		if (dniRes is Result<DniArgentino2025>.Error e2) return new Result<Medico2025>.Error(e2.Mensaje);

		var contactoRes = Contacto2025.Crear(telefono, email);
		if (contactoRes is Result<Contacto2025>.Error e3) return new Result<Medico2025>.Error(e3.Mensaje);

		var domicilioRes = DomicilioArgentino2025.Crear(provincia, localidad, direccion);
		if (domicilioRes is Result<DomicilioArgentino2025>.Error e4) return new Result<Medico2025>.Error(e4.Mensaje);

		var especialidadRes = MedicoEspecialidad2025.Crear(especialidad, rama);
		if (especialidadRes is Result<MedicoEspecialidad2025>.Error e5) return new Result<Medico2025>.Error(e5.Mensaje);

		// (Se podría validar que la especialidad esté habilitada en esa provincia, si aplica)

		return new Result<Medico2025>.Ok(new Medico2025(
			((Result<NombreCompleto2025>.Ok)nombreRes).Value,
			((Result<MedicoEspecialidad2025>.Ok)especialidadRes).Value,
			((Result<DniArgentino2025>.Ok)dniRes).Value,
			((Result<DomicilioArgentino2025>.Ok)domicilioRes).Value,
			((Result<Contacto2025>.Ok)contactoRes).Value,
			agenda
		));
	}
}
