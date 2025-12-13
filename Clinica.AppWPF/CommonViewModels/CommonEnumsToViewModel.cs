using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposExtensiones;
// ================================================================
// VIEWMODELS PARA SUB-COLLECTIONS
// ===========================================

public static class CommonEnumsToViewModel {
	public record ProvinciaVmItem(
		ProvinciaCodigo Codigo,
		string Nombre
	);
	public static ProvinciaVmItem ToViewModel(this ProvinciaCodigo enumm) => new(Codigo: enumm, Nombre: enumm.ATexto());
	public static ProvinciaVmItem ToViewModel(this ProvinciaArgentina2025 domain) => new(Codigo: domain.CodigoInternoValor, Nombre: domain.NombreValor);


}
