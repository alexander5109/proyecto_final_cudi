using System.Text.Json;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.DbModels;

public static partial class DbModels {
	public record MedicoDbModel(
		MedicoId Id,
		EspecialidadCodigo EspecialidadCodigo,
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo ProvinciaCodigo,
		string Telefono,
		string Email,
		bool HaceGuardias,
		string? HorariosJson
	) {
		public MedicoDbModel()
			: this(default!, default, "", "", "", default, "", "", default, "", "", default, null) { }
	}

	public static MedicoDbModel ToModel(this Medico2025Agg aggrg) {
		return new MedicoDbModel(
			Id: aggrg.Id,
			EspecialidadCodigo: aggrg.Medico.EspecialidadUnica.Codigo,
			Dni: aggrg.Medico.Dni.Valor,
			Nombre: aggrg.Medico.NombreCompleto.NombreValor,
			Apellido: aggrg.Medico.NombreCompleto.ApellidoValor,
			FechaIngreso: aggrg.Medico.FechaIngreso,
			Domicilio: aggrg.Medico.Domicilio.DireccionValor,
			Localidad: aggrg.Medico.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo: aggrg.Medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono: aggrg.Medico.Telefono.Valor,
			Email: aggrg.Medico.Email.Valor,
			HaceGuardias: aggrg.Medico.HaceGuardiasValor,
			HorariosJson: JsonSerializer.Serialize(aggrg.Medico.ListaHorarios.ToString()) //Cualquier cosa estaba haciedno aca.
		);
	}
	public static MedicoDbModel ToModel(this Medico2025 instance, MedicoId id) {
		return new MedicoDbModel(
			Id: id,
			EspecialidadCodigo: instance.EspecialidadUnica.Codigo,
			Dni: instance.Dni.Valor,
			Nombre: instance.NombreCompleto.NombreValor,
			Apellido: instance.NombreCompleto.ApellidoValor,
			FechaIngreso: instance.FechaIngreso,
			Domicilio: instance.Domicilio.DireccionValor,
			Localidad: instance.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo: instance.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono: instance.Telefono.Valor,
			Email: instance.Email.Valor,
			HaceGuardias: instance.HaceGuardiasValor,
			HorariosJson: JsonSerializer.Serialize(instance.ListaHorarios.ToString()) //Cualquier cosa estaba haciedno aca.
		);
	}

	public static Result<Medico2025Agg> ToDomainAgg(this MedicoDbModel dbModel) {
		string json = string.IsNullOrWhiteSpace(dbModel.HorariosJson) ? "[]" : dbModel.HorariosJson;
		List<Horario2025> horariosDto = JsonSerializer.Deserialize<List<Horario2025>>(json)
			?? [];
		return Medico2025Agg.CrearResult(
			MedicoId.CrearResult(dbModel.Id),
			Medico2025.CrearResult(
				NombreCompleto2025.CrearResult(dbModel.Nombre, dbModel.Apellido),

				Especialidad2025.CrearResult(dbModel.EspecialidadCodigo),

				DniArgentino2025.CrearResult(dbModel.Dni),

				DomicilioArgentino2025.CrearResult(
					LocalidadDeProvincia2025.CrearResult(dbModel.Localidad, ProvinciaArgentina2025.CrearResultPorCodigo(dbModel.ProvinciaCodigo))
					, dbModel.Domicilio
				),
				ContactoTelefono2025.CrearResult(dbModel.Telefono),
				ContactoEmail2025.CrearResult(dbModel.Email),
				ListaHorarioMedicos2025.CrearResult(horariosDto),
				dbModel.FechaIngreso,
				dbModel.HaceGuardias
			)
		);
	}









}
