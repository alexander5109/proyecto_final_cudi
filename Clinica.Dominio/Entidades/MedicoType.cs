using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct MedicoType(
	NombreCompletoType NombreCompleto,
	MedicoEspecialidadType Especialidad,
	DniArgentinoType Dni,
	DomicilioArgentinoType Domicilio,
	ContactoTelefonoType Telefono,
	IReadOnlyList<HorarioMedicoType> Horarios,
	FechaIngresoType FechaIngreso,
	MedicoSueldoMinimoType SueldoMinimoGarantizado,
	bool HaceGuardias
);


public static class Medico {
	public static string HorariosToString(this MedicoType medico) => string.Join(", ", medico.Horarios.Select(h => h.ToString()));
	public static Result<MedicoType> Create(
		Result<NombreCompletoType> nombreResult,
		Result<MedicoEspecialidadType> especialidadResult,
		Result<DniArgentinoType> dniResult,
		Result<DomicilioArgentinoType> domicilioResult,
		Result<ContactoTelefonoType> telefonoResult,
		IReadOnlyList<Result<HorarioMedicoType>> horariosResult,
		Result<FechaIngresoType> fechaIngresoResult,
		Result<MedicoSueldoMinimoType> sueldoResult,
		bool haceGuardia
	) =>
		nombreResult.Bind(nombreOk =>
		especialidadResult.Bind(espOk =>
		dniResult.Bind(dniOk =>
		domicilioResult.Bind(domOk =>
		telefonoResult.Bind(contOk =>
		horariosResult.Bind(horariosOk =>
		fechaIngresoResult.Bind(fechaIngOk =>
		sueldoResult.Map(sueldoOk =>
			new MedicoType(
				nombreOk,
				espOk,
				dniOk,
				domOk,
				contOk,
				horariosOk,
				fechaIngOk,
				sueldoOk,
				haceGuardia
			)
		))))))));

}