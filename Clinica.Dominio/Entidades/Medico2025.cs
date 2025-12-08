using Clinica.Dominio.Comun;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;

public record struct MedicoId(int Valor) {
	public static Result<MedicoId> CrearResult(int? id) =>
		id is int idGood
		? new Result<MedicoId>.Ok(new MedicoId(idGood))
		: new Result<MedicoId>.Error("El id no puede ser nulo.");
	public static MedicoId Crear(int id) => new(id);
	public static bool TryParse(string? s, out MedicoId id) {
		if (int.TryParse(s, out int value)) {
			id = new MedicoId(value);
			return true;
		}

		id = default;
		return false;
	}
	public override string ToString() => Valor.ToString();
}


//public struct HaceGuardia(bool Valor);

public record Medico2025Agg(MedicoId Id, Medico2025 Medico);

public record Medico2025(
	//ListaEspecialidadesMedicas2025 Especialidades,

	MedicoId Id,
	NombreCompleto2025 NombreCompleto,
	Especialidad2025 EspecialidadUnica,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	ContactoTelefono2025 Telefono,
	ContactoEmail2025 Email,
	ListaHorarioMedicos2025 ListaHorarios,
	FechaRegistro2025 FechaIngreso,
	bool HaceGuardiasValor
) {
    public static Result<Medico2025> CrearResult(
		Result<MedicoId> idResult,
		Result<NombreCompleto2025> nombreResult,
		Result<Especialidad2025> especialidadResult,
		//Result<ListaEspecialidadesMedicas2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<ContactoTelefono2025> telefonoResult,
		Result<ContactoEmail2025> emailResult,
		Result<ListaHorarioMedicos2025> horariosResult,
		Result<FechaRegistro2025> fechaIngresoResult,
		bool haceGuardia
	) =>
		from id in idResult
		from nombre in nombreResult
		from esp in especialidadResult
		from dni in dniResult
		from dom in domicilioResult
		from tel in telefonoResult
		from email in emailResult
		from horarios in horariosResult
		from fechaIng in fechaIngresoResult
		select new Medico2025(
			id,
			nombre,
			esp,
			dni,
			dom,
			tel,
			email,
			horarios,
			fechaIng,
			haceGuardia
		);
}