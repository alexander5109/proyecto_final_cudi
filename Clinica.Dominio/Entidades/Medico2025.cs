using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct Medico2025 {
	public NombreCompleto2025 NombreCompleto { get; }
	public MedicoEspecialidad2025 Especialidad { get; }
	public DniArgentino2025 Dni { get; }
	public DomicilioArgentino2025 Domicilio { get; }
	public Contacto2025Telefono Telefono { get; }
	public IReadOnlyList<HorarioMedico> Horarios { get; }
	public FechaIngreso2025 FechaIngreso { get; init; }
	public MedicoSueldoMinimoGarantizado2025 SueldoMinimoGarantizado { get; }
	public bool HaceGuardias { get; }

	public string HorariosToString() {
		return string.Join(", ", Horarios.Select(h => h.ToString()));
	}

	private Medico2025(
		NombreCompleto2025 nombre,
		MedicoEspecialidad2025 especialidad,
		DniArgentino2025 dni,
		DomicilioArgentino2025 domicilio,
		Contacto2025Telefono telefono,
		IReadOnlyList<HorarioMedico> horarios,
		FechaIngreso2025 fechaDeIngreso,
		MedicoSueldoMinimoGarantizado2025 sueldo,
		bool guardias
	) {
		NombreCompleto = nombre;
		Especialidad = especialidad;
		Dni = dni;
		Domicilio = domicilio;
		Telefono = telefono;
		Horarios = horarios;
		FechaIngreso = fechaDeIngreso;
		SueldoMinimoGarantizado = sueldo;
		HaceGuardias = guardias;
	}

	public static Result<Medico2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<MedicoEspecialidad2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<Contacto2025Telefono> telefonoResult,
		IReadOnlyList<Result<HorarioMedico>> horariosResult,
		Result<FechaIngreso2025> fechaIngresoResult,
		Result<MedicoSueldoMinimoGarantizado2025> sueldoResult,
		bool haceGuardia
	)
		=> nombreResult.Bind(nombreOk =>
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
