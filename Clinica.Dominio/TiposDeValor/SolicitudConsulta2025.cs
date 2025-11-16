using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.TiposDeValor;

public readonly record struct SolicitudConsulta2025(
    Paciente2025 Paciente,
    MedicoEspecialidad2025 Especialidad,
    DateTime FechaDeseada
)
{
    public static Result<SolicitudConsulta2025> Crear(Result<Paciente2025> pacienteResult, Result<MedicoEspecialidad2025> especialidadResult, DateTime? fechaDeseada)
    {
        if (pacienteResult is Result<Paciente2025>.Error pErr)
            return new Result<SolicitudConsulta2025>.Error(pErr.Mensaje);
        if (especialidadResult is Result<MedicoEspecialidad2025>.Error eErr)
            return new Result<SolicitudConsulta2025>.Error(eErr.Mensaje);
        if (fechaDeseada is null)
            return new Result<SolicitudConsulta2025>.Error("La fecha deseada es obligatoria.");

        var paciente = ((Result<Paciente2025>.Ok)pacienteResult).Valor;
        var especialidad = ((Result<MedicoEspecialidad2025>.Ok)especialidadResult).Valor;

        if (fechaDeseada.Value < DateTime.Now.Date)
            return new Result<SolicitudConsulta2025>.Error("La fecha deseada no puede ser en el pasado.");

        return new Result<SolicitudConsulta2025>.Ok(new SolicitudConsulta2025(paciente, especialidad, fechaDeseada.Value));
    }
}
