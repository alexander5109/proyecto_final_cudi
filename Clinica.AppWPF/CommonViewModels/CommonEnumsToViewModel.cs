using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposExtensiones;
// ================================================================
// VIEWMODELS PARA SUB-COLLECTIONS
// ===========================================


namespace Clinica.AppWPF.CommonViewModels;

public static class CommonEnumsToViewModel {
	public record ProvinciaVmItem(
		ProvinciaEnum Codigo,
		string Nombre
	);
	public static ProvinciaVmItem ToViewModel(this ProvinciaEnum enumm) => new(Codigo: enumm, Nombre: enumm.ATexto());
	public static ProvinciaVmItem ToViewModel(this ProvinciaArgentina2025 domain) => new(Codigo: domain.CodigoInternoValor, Nombre: domain.NombreValor);





	public record EspecialidadViewModel(
		EspecialidadEnum Codigo,
		string NombreEspecialidad,
		int Duracion
	);
	public static EspecialidadViewModel ToViewModel(this Especialidad2025 instance) {
		return new(
			Codigo: instance.Codigo,
			NombreEspecialidad: instance.Titulo,
			Duracion: instance.DuracionConsultaMinutos
		);

	}
	public static EspecialidadViewModel ToViewModel(this EspecialidadEnum enumm) {
        Especialidad2025 instance = Especialidad2025.Representar(enumm);
		return new(
			Codigo: instance.Codigo,
			NombreEspecialidad: instance.Titulo,
			Duracion: instance.DuracionConsultaMinutos
		);

	}



}
