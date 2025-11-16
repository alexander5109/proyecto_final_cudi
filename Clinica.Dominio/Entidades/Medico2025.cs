using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Entidades;
public readonly record struct MedicoId2025(string Valor);

public readonly record struct Medico2025WithId(
	Medico2025 Medico,
	MedicoId2025 Id
);


public readonly record struct Medico2025(
	NombreCompleto2025 NombreCompleto,
	EspecialidadMedica2025 Especialidad,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	ContactoTelefono2025 Telefono,
	ListaHorarioMedicos2025 ListaHorarios,
	FechaIngreso2025 FechaIngreso,
	MedicoSueldoMinimo2025 SueldoMinimoGarantizado,
	bool HaceGuardias
) {



	//public static Medico2025 CrearFromRaw(
	//		string nombre, 
	//		string apellido,
	//		string dni, 
	//		string especialidadTitulo, 
	//		DayOfWeek dia, 
	//		TimeOnly desde, 
	//		TimeOnly hasta
	//) {
	//	var nombreRes = NombreCompleto2025.Programar(nombre, apellido);
	//	var espRes = EspecialidadMedica2025.Programar(especialidadTitulo, EspecialidadMedica2025.RamasDisponibles.First());
	//	var dniRes = DniArgentino2025.Programar(dni);
	//	var domRes = DomicilioArgentino2025.Programar(LocalidadDeProvincia2025.Programar("Localidad", ProvinciaArgentina2025.Programar("Buenos Aires")), "Calle 1");
	//	var telRes = ContactoTelefono2025.Programar("+5491123456789");
	//	var horariosRes = ListaHorarioMedicos2025.Programar([
	//			HorarioMedico2025.Programar(DiaSemana2025.Programar(dia),
	//			HorarioHora2025.Programar(desde),
	//			HorarioHora2025.Programar(hasta))
	//			.Match(ok => ok, err => throw new Exception(err)) ]
	//	);
	//	var fechaIng = FechaIngreso2025.Programar(DateTime.Today);
	//	var sueldo = MedicoSueldoMinimo2025.Programar(250000m);

	//	var medRes = Medico2025.Programar(nombreRes, espRes, dniRes, domRes, telRes, horariosRes, fechaIng, sueldo, false);
	//	return medRes.Match(m => m, e => throw new Exception(e));
	//}


	public static Result<Medico2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<EspecialidadMedica2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<ContactoTelefono2025> telefonoResult,
		Result<ListaHorarioMedicos2025> horariosResult,
		Result<FechaIngreso2025> fechaIngresoResult,
		Result<MedicoSueldoMinimo2025> sueldoResult,
		bool haceGuardia
	) =>
		from nombre in nombreResult
		from esp in especialidadResult
		from dni in dniResult
		from dom in domicilioResult
		from tel in telefonoResult
		from horarios in horariosResult
		from fechaIng in fechaIngresoResult
		from sueldo in sueldoResult
		select new Medico2025(
			nombre,
			esp,
			dni,
			dom,
			tel,
			horarios,
			fechaIng,
			sueldo,
			haceGuardia
		);

}