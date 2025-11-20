using Clinica.Dominio.Comun;
using Clinica.Dominio.TiposDeValor;

namespace Clinica.Dominio.Entidades;

public record Medico2025WithId(
    Medico2025 Medico,
    int Id
);

public record Medico2025(
    NombreCompleto2025 NombreCompleto,
    EspecialidadMedica2025 Especialidad,
    DniArgentino2025 Dni,
    DomicilioArgentino2025 Domicilio,
    ContactoTelefono2025 Telefono,
    ListaHorarioMedicos2025 ListaHorarios,
    FechaIngreso2025 FechaIngreso,
    MedicoSueldoMinimo2025 SueldoMinimoGarantizado,
    bool HaceGuardias
) {
    public static Result<Medico2025> Crear(
        Result<NombreCompleto2025> nombreResult,
        Result<EspecialidadMedica2025> especialidadResult,
        Result<DniArgentino2025> dniResult,
        Result<DomicilioArgentino2025> domicilioResult,
        Result<ContactoTelefono2025> telefonoResult,
        Result<ListaHorarioMedicos2025> horariosResult,
        Result<FechaIngreso2025> fechaIngresoResult,
        Result<MedicoSueldoMinimo2025> sueldoResult,
        bool haceGuardia
    ) =>
        from nombre in nombreResult
        from esp in especialidadResult
        from dni in dniResult
        from dom in domicilioResult
        from tel in telefonoResult
        from horarios in horariosResult
        from fechaIng in fechaIngresoResult
        from sueldo in sueldoResult
        select new Medico2025(
            nombre,
            esp,
            dni,
            dom,
            tel,
            horarios,
            fechaIng,
            sueldo,
            haceGuardia
        );
}