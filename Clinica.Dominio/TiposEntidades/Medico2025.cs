using Clinica.Dominio.FunctionalToolkit;

namespace Clinica.Dominio.TiposDeValor;

public record struct MedicoId(int Valor) {
	public static Result<MedicoId> CrearResult(MedicoId? id) =>
		(id is MedicoId idGood && idGood.Valor >= 0)
		? new Result<MedicoId>.Ok(idGood)
		: new Result<MedicoId>.Error("El id no puede ser nulo o negativo.");
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

public record Medico2025Agg(MedicoId Id, Medico2025 Medico) {
	public static Medico2025Agg Crear(MedicoId id, Medico2025 medico) => new(id, medico);
	public static Result<Medico2025Agg> CrearResult(
		Result<MedicoId> idResult,
		Result<Medico2025> medicoResult
	)
		=> from id in idResult
		   from paciente in medicoResult
		   select new Medico2025Agg(
			   id,
			   paciente
		   );

}

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
	DateTime FechaIngreso,
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
		DateTime fechaIngreso,
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
		select new Medico2025(
			id,
			nombre,
			esp,
			dni,
			dom,
			tel,
			email,
			horarios,
			fechaIngreso,
			haceGuardia
		);
}