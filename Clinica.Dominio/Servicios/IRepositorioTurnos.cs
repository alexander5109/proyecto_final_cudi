using Clinica.Dominio.Entidades;
using Clinica.Dominio.FunctionalProgramingTools;
using System.Collections.Generic;

namespace Clinica.Dominio.Servicios;

public interface IRepositorioTurnos
{
    Result<IReadOnlyList<Turno2025>> ObtenerTurnosPorMedicoDni(string medicoDni, DateTime desde, DateTime hasta);
    Result<IReadOnlyList<Turno2025>> ObtenerTurnosPorEspecialidad(string especialidadTitulo, DateTime desde, DateTime hasta);

    Result<Turno2025> Guardar(Turno2025 turno);
    Result<bool> Eliminar(Turno2025 turno);
}
