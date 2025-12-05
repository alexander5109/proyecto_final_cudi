using Clinica.AppWPF.ViewModels;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.Dtos.ApiDtos;

namespace Clinica.AppWPF.Pacientes;

public static class ConverterExtentions {



	public static PacienteUpdateViewModel ToViewModel(this PacienteDto dto)
		=> new PacienteUpdateViewModel {
			Id = dto.Id.Valor,
			Dni = dto.Dni,
			Nombre = dto.Nombre,
			Apellido = dto.Apellido,
			FechaIngreso = dto.FechaIngreso,
			Email = dto.Email,
			Telefono = dto.Telefono,
			FechaNacimiento = dto.FechaNacimiento,
			Domicilio = dto.Domicilio,
			Localidad = dto.Localidad,
			ProvinciaCodigo = dto.ProvinciaCodigo
		};


	public static Result<Paciente2025> ToDomain(this PacienteUpdateViewModel viewModel) {
		return Paciente2025.CrearResult(
			PacienteId.CrearResult(viewModel.Id),
			NombreCompleto2025.CrearResult(viewModel.Nombre, viewModel.Apellido),
			DniArgentino2025.CrearResult(viewModel.Dni),
			Contacto2025.CrearResult(
				ContactoEmail2025.CrearResult(viewModel.Email),
				ContactoTelefono2025.CrearResult(viewModel.Telefono)
			),
			DomicilioArgentino2025.CrearResult(
				LocalidadDeProvincia2025.CrearResult(viewModel.Localidad, ProvinciaArgentina2025.CrearResultPorCodigo(viewModel.ProvinciaCodigo)),
				viewModel.Domicilio
			),
			FechaDeNacimiento2025.CrearResult(viewModel.FechaNacimiento),
			FechaRegistro2025.CrearResult(viewModel.FechaIngreso)
		);
	}

}
