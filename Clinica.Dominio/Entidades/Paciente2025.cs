using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;
namespace Clinica.Dominio.Entidades;

public readonly record struct Paciente2025 {
	private Paciente2025(
		NombreCompleto2025 nombreCompleto,
		DniArgentino2025 dni,
		Contacto2025 contacto,
		DomicilioArgentino2025 domicilio,
		FechaDeNacimiento2025 fechaNacimiento,
		FechaDeIngreso2025 fechaIngreso
	) {
		NombreCompleto = nombreCompleto;
		Dni = dni;
		Contacto = contacto;
		Domicilio = domicilio;
		FechaNacimiento = fechaNacimiento;
		FechaIngreso = fechaIngreso;
	}

	public NombreCompleto2025 NombreCompleto { get; }
	public DniArgentino2025 Dni { get; }
	public Contacto2025 Contacto { get; }
	public DomicilioArgentino2025 Domicilio { get; }
	public FechaDeNacimiento2025 FechaNacimiento { get; }
	public FechaDeIngreso2025 FechaIngreso { get; }

	// ✅ Factory controlada
	public static Result<Paciente2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<DniArgentino2025> dniResult,
		Result<Contacto2025> contactoResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<FechaDeNacimiento2025> fechaNacimientoResult,
		Result<FechaDeIngreso2025> fechaIngresoResult)
		=> nombreResult.Bind(nombreOk =>
		   dniResult.Bind(dniOk =>
		   contactoResult.Bind(contactoOk =>
		   domicilioResult.Bind(domicilioOk =>
		   fechaNacimientoResult.Bind(fechaNacOk =>
		   fechaIngresoResult.Map(fechaIngOk =>
			   new Paciente2025(
				   nombreOk,
				   dniOk,
				   contactoOk,
				   domicilioOk,
				   fechaNacOk,
				   fechaIngOk
			   )
		   ))))));
}
