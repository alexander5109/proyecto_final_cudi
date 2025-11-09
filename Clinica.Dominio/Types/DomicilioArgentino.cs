namespace Clinica.Dominio.Types;

public record struct DomicilioArgentino {
	LocalidadDeProvincia Localidad;
	ProvinciaDeArgentina Provincia;
	string Direccion;
}


