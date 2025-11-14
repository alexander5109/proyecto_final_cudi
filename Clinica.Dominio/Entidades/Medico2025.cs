using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct Medico2025(
	NombreCompleto2025 NombreCompleto,
	MedicoEspecialidad2025 Especialidad,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	ContactoTelefono2025 Telefono,
	ListaHorarioMedicos2025 ListaHorarios,
	FechaIngreso2025 FechaIngreso,
	MedicoSueldoMinimo2025 SueldoMinimoGarantizado,
	bool HaceGuardias
){
	public static Result<Medico2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<MedicoEspecialidad2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<ContactoTelefono2025> telefonoResult,
		Result<ListaHorarioMedicos2025> horariosResult,
		Result<FechaIngreso2025> fechaIngresoResult,
		Result<MedicoSueldoMinimo2025> sueldoResult,
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
			new Medico2025(
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