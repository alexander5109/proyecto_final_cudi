using Clinica.AppWPF.Infrastructure;
using Clinica.Dominio.TiposDeEntidad;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.UsuarioSecretaria;


public static class SecretariaPacienteMiniViewModels {
	public record ProvinciaVmItem(
		ProvinciaCodigo2025 Codigo,
		string Nombre
	);
	public static ProvinciaVmItem ToViewModel(this ProvinciaCodigo2025 enumm) => new(Codigo: enumm, Nombre: enumm.ATexto());
	public static ProvinciaVmItem ToViewModel(this ProvinciaArgentina2025 domain) => new(Codigo: domain.CodigoInternoValor, Nombre: domain.NombreValor);


	public static SecretariaPacienteFormularioViewModel ToViewModel(this PacienteDbModel model)
		=> new SecretariaPacienteFormularioViewModel {
			Id = model.Id,
			Dni = model.Dni,
			Nombre = model.Nombre,
			Apellido = model.Apellido,
			FechaIngreso = model.FechaIngreso,
			Email = model.Email,
			Telefono = model.Telefono,
			FechaNacimiento = model.FechaNacimiento,
			Domicilio = model.Domicilio,
			Localidad = model.Localidad,
			Provincia = model.ProvinciaCodigo.ToViewModel()
		};
	public static ResultWpf<Paciente2025> ToDomain(this SecretariaPacienteFormularioViewModel viewModel) {
		return Paciente2025.CrearResult(
			//PacienteId.CrearResult(viewModel.Id),
			NombreCompleto2025.CrearResult(viewModel.Nombre, viewModel.Apellido),
			DniArgentino2025.CrearResult(viewModel.Dni),
			Contacto2025.CrearResult(
				ContactoEmail2025.CrearResult(viewModel.Email),
				ContactoTelefono2025.CrearResult(viewModel.Telefono)
			),
			DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(viewModel.Localidad, ProvinciaArgentina2025.CrearResultPorCodigo(viewModel.Provincia?.Codigo)),
				viewModel.Domicilio
			),
			FechaDeNacimiento2025.CrearResult(viewModel.FechaNacimiento),
			viewModel.FechaIngreso
		).ToWpf();
	}
}
