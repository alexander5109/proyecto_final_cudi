using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct Medico2025 {
	public NombreCompleto2025 NombreCompleto { get; }
	public MedicoEspecialidad2025 Especialidad { get; }
	public DniArgentino2025 Dni { get; }
	public DomicilioArgentino2025 Domicilio { get; }
	public Contacto2025 Contacto { get; }
	public MedicoAgenda2025 Agenda { get; }
	public FechaIngreso2025 FechaIngreso { get; init; }
	public MedicoSueldoMinimoGarantizado2025 SueldoMinimoGarantizado { get; }
	public bool HaceGuardias { get; }

	// 🔒 Constructor privado — solo el método Crear puede usarlo
	private Medico2025(
		NombreCompleto2025 nombre,
		MedicoEspecialidad2025 especialidad,
		DniArgentino2025 dni,
		DomicilioArgentino2025 domicilio,
		Contacto2025 contacto,
		MedicoAgenda2025 agenda,
		FechaIngreso2025 fechaDeIngreso,
		MedicoSueldoMinimoGarantizado2025 sueldo,
		bool guardias
	) {
		NombreCompleto = nombre;
		Especialidad = especialidad;
		Dni = dni;
		Domicilio = domicilio;
		Contacto = contacto;
		Agenda = agenda;
		FechaIngreso = fechaDeIngreso;
		SueldoMinimoGarantizado = sueldo;
		HaceGuardias = guardias;
	}

	// 🏭 Factory controlada
	public static Result<Medico2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<MedicoEspecialidad2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<Contacto2025> contactoResult,
		Result<MedicoAgenda2025> agendaResult,
		Result<FechaIngreso2025> fechaIngresoResult,
		Result<MedicoSueldoMinimoGarantizado2025> sueldoResult,
		bool guardias
	)
		=> nombreResult.Bind(nombreOk =>
		   especialidadResult.Bind(espOk =>
		   dniResult.Bind(dniOk =>
		   domicilioResult.Bind(domOk =>
		   contactoResult.Bind(contOk =>
		   fechaIngresoResult.Bind(fechaIngOk =>
		   sueldoResult.Bind(sueldoOk =>
		   agendaResult.Map(agendaOk =>
			   new Medico2025(
				   nombreOk,
				   espOk,
				   dniOk,
				   domOk,
				   contOk,
				   agendaOk,
				   fechaIngOk,
				   sueldoOk,
				   guardias
			   )
		   ))))))));
}
