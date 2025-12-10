using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record PacienteDbModel(
		PacienteId Id,
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo2025 ProvinciaCodigo,
		string Telefono,
		string Email,
		DateTime FechaNacimiento
	) {
		public PacienteDbModel()
			: this(default, "", "", "", default, "", "", default, "", "", default) { }
	}

	public static PacienteDbModel ToModel(this Paciente2025Agg aggrg) {
		return new PacienteDbModel(
			Id: aggrg.Id,
			Dni: aggrg.Paciente.Dni.Valor,
			Nombre: aggrg.Paciente.NombreCompleto.NombreValor,
			Apellido: aggrg.Paciente.NombreCompleto.ApellidoValor,
			FechaIngreso: aggrg.Paciente.FechaIngreso,
			Domicilio: aggrg.Paciente.Domicilio.DireccionValor,
			Localidad: aggrg.Paciente.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo: aggrg.Paciente.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono: aggrg.Paciente.Contacto.Telefono.Valor,
			Email: aggrg.Paciente.Contacto.Email.Valor,
			FechaNacimiento: aggrg.Paciente.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue)
		);
	}



	public static Result<Paciente2025Agg> ToDomainAgg(this PacienteDbModel dbModel) {
		return Paciente2025Agg.CrearResult(
			PacienteId.CrearResult(dbModel.Id.Valor),
			Paciente2025.CrearResult(
				NombreCompleto2025.CrearResult(dbModel.Nombre, dbModel.Apellido),
				DniArgentino2025.CrearResult(dbModel.Dni),
				Contacto2025.CrearResult(
				ContactoEmail2025.CrearResult(dbModel.Email),
				ContactoTelefono2025.CrearResult(dbModel.Telefono)),
				DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(
					dbModel.Localidad,
					ProvinciaArgentina2025.CrearResultPorCodigo(
						dbModel.ProvinciaCodigo)
					)
				, dbModel.Domicilio),
				FechaDeNacimiento2025.CrearResult(dbModel.FechaNacimiento),
				dbModel.FechaIngreso
			)
		);
	}


}