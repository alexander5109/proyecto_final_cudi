using System.Text.Json;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Shared.ApiDtos;

public static class MedicoDtos {

	public record MedicDto(
		EspecialidadCodigo EspecialidadCodigo,
		string Dni,
		string Nombre,
		string Apellido,
		DateTime FechaIngreso,
		string Domicilio,
		string Localidad,
		ProvinciaCodigo2025 ProvinciaCodigo,
		string Telefono,
		string Email,
		bool HaceGuardias,
		string? HorariosJson
	) {
		public MedicDto()
			: this(default, "", "", "", default, "", "", default, "", "", default, null) { }
	}


	public static MedicDto ToDto(this Medico2025 medico) {
		return new MedicDto(
			EspecialidadCodigo: medico.EspecialidadUnica.Codigo,
			Dni: medico.Dni.Valor,
			Nombre: medico.NombreCompleto.NombreValor,
			Apellido: medico.NombreCompleto.ApellidoValor,
			FechaIngreso: medico.FechaIngreso,
			Domicilio: medico.Domicilio.DireccionValor,
			Localidad: medico.Domicilio.Localidad.NombreValor,
			ProvinciaCodigo: medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
			Telefono: medico.Telefono.Valor,
			Email: medico.Email.Valor,
			HaceGuardias: medico.HaceGuardiasValor,
			HorariosJson: JsonSerializer.Serialize(medico.ListaHorarios.ToString()) //Cualquier cosa estaba haciedno aca.
		);
	}

}
