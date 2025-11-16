using Clinica.Dominio.Comun;
using Clinica.Dominio.Tipos;

namespace Clinica.Dominio.Entidades;

public readonly record struct Paciente2025(
	NombreCompleto2025 NombreCompleto,
	DniArgentino2025 Dni,
	Contacto2025 Contacto,
	DomicilioArgentino2025 Domicilio,
	FechaDeNacimiento2025 FechaNacimiento,
	FechaIngreso2025 FechaIngreso
) {
	public static Result<Paciente2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<DniArgentino2025> dniResult,
		Result<Contacto2025> contactoResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<FechaDeNacimiento2025> fechaNacimientoResult,
		Result<FechaIngreso2025> fechaIngresoResult
	)
	=>
		from nombre in nombreResult
		from dni in dniResult
		from contacto in contactoResult
		from domicilio in domicilioResult
		from fechaNac in fechaNacimientoResult
		from fechaIng in fechaIngresoResult
		select new Paciente2025(
			nombre,
			dni,
			contacto,
			domicilio,
			fechaNac,
			fechaIng
		);
}
