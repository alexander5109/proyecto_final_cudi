using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.TiposDeEntidad;

public record Medico2025(
	//ListaEspecialidadesMedicas2025 Especialidades,
	//MedicoId2025 Id,
	NombreCompleto2025 NombreCompleto,
	Especialidad2025 EspecialidadUnica,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	Telefono2025 Telefono,
	Email2025 Email,
	//ListaHorarioMedicos2025 ListaHorarios,
	DateTime FechaIngreso,
	bool HaceGuardiasValor
) {
    public static Result<Medico2025> CrearResult(
		//Result<MedicoId2025> idResult,
		Result<NombreCompleto2025> nombreResult,
		Result<Especialidad2025> especialidadResult,
		//Result<ListaEspecialidadesMedicas2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<Telefono2025> telefonoResult,
		Result<Email2025> emailResult,
		//Result<ListaHorarioMedicos2025> horariosResult,
		DateTime fechaIngreso,
		bool haceGuardia
	) =>
		//from id in idResult
		from nombre in nombreResult
		from esp in especialidadResult
		from dni in dniResult
		from dom in domicilioResult
		from tel in telefonoResult
		from email in emailResult
		//from horarios in horariosResult
		select new Medico2025(
			//id,
			nombre,
			esp,
			dni,
			dom,
			tel,
			email,
			//horarios,
			fechaIngreso,
			haceGuardia
		);
}