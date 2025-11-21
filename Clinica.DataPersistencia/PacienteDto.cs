using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Microsoft.Data.SqlClient;

namespace Clinica.DataPersistencia;

public record PacienteDto(
    int Id,
    string Dni,
    string Name,
    string LastName,
    DateTime FechaIngreso,
    string Domicilio,
    string Localidad,
    int ProvinciaCodigo,
    string Telefono,
    string Email,
    DateTime FechaNacimiento
) {
    // Para Dapper necesita un constructor vacío
    public PacienteDto() : this(0, "", "", "", DateTime.MinValue, "", "", 0, "", "", DateTime.MinValue) { }


    public Result<Paciente2025> ToDomain() {
        var nombre = NombreCompleto2025.Crear(Name, LastName);
        var dni = DniArgentino2025.Crear(Dni);
        var contacto = Contacto2025.Crear(ContactoEmail2025.Crear(Email), ContactoTelefono2025.Crear(Telefono));
        var domicilio = DomicilioArgentino2025.Crear(LocalidadDeProvincia2025.Crear(Localidad, ProvinciaArgentina2025.CrearPorCodigo(ProvinciaCodigo)), Domicilio);
        var fechaNacimiento = FechaDeNacimiento2025.Crear(DateOnly.FromDateTime(FechaNacimiento));
        var ingreso = FechaIngreso2025.Crear(FechaIngreso);

        return Paciente2025.Crear(
            nombre,
            dni,
            contacto,
            domicilio,
            fechaNacimiento,
            ingreso
        );
    }


    public static PacienteDto FromDomain(Paciente2025 p, int id = 0) {
        return new PacienteDto(
            Id: id,
            Dni: p.Dni.Valor,
            Name: p.NombreCompleto.Nombre,
            LastName: p.NombreCompleto.Apellido,
            FechaIngreso: p.FechaIngreso.Valor,
            Domicilio: p.Domicilio.Direccion,
            Localidad: p.Domicilio.Localidad.Nombre,
            ProvinciaCodigo: p.Domicilio.Localidad.Provincia.CodigoInterno,
            Telefono: p.Contacto.Telefono.Valor,
            Email: p.Contacto.Email.Valor,
            FechaNacimiento: p.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue)
        );
    }


    public static PacienteDto FromSqlReader(SqlDataReader reader) {
        return new PacienteDto(
            Id: reader.GetInt32(reader.GetOrdinal("Id")),
            Dni: reader.GetString(reader.GetOrdinal("Dni")).Trim(),
            Name: reader.GetString(reader.GetOrdinal("Name")),
            LastName: reader.GetString(reader.GetOrdinal("LastName")),
            FechaIngreso: reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
            Domicilio: reader.GetString(reader.GetOrdinal("Domicilio")),
            Localidad: reader.GetString(reader.GetOrdinal("Localidad")),
            ProvinciaCodigo: reader.GetInt32(reader.GetOrdinal("ProvinciaCodigo")),
            Telefono: reader.GetString(reader.GetOrdinal("Telefono")),
            Email: reader.GetString(reader.GetOrdinal("Email")),
            FechaNacimiento: reader.GetDateTime(reader.GetOrdinal("FechaNacimiento"))
        );
    }


}