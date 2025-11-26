using System.Globalization;
using Clinica.Dominio.Comun;
using Clinica.Dominio.ListasOrganizadoras;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;

public record struct MedicoId(int Valor);
public record Medico2025(
	MedicoId Id,
	NombreCompleto2025 NombreCompleto,
	ListaEspecialidadesMedicas2025 Especialidades,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	ContactoTelefono2025 Telefono,
	ListaHorarioMedicos2025 ListaHorarios,
	FechaRegistro2025 FechaIngreso,
	bool HaceGuardias
) {
	public static Result<Medico2025> Crear(
		MedicoId id,
		Result<NombreCompleto2025> nombreResult,
		Result<ListaEspecialidadesMedicas2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<ContactoTelefono2025> telefonoResult,
		Result<ListaHorarioMedicos2025> horariosResult,
		Result<FechaRegistro2025> fechaIngresoResult,
		bool haceGuardia
	) =>
		from nombre in nombreResult
		from esp in especialidadResult
		from dni in dniResult
		from dom in domicilioResult
		from tel in telefonoResult
		from horarios in horariosResult
		from fechaIng in fechaIngresoResult
		select new Medico2025(
			id,
			nombre,
			esp,
			dni,
			dom,
			tel,
			horarios,
			fechaIng,
			haceGuardia
		);
}