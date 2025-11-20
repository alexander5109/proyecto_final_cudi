using System.Threading.Tasks;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.FunctionalProgramingTools;

namespace Clinica.Dominio.Servicios;

public interface ITurnoAppService{
    Task<Result<Turno2025>> ProgramarTurnoAsync(int pacienteId, int? medicoId, string especialidadUId, System.DateTime fechaYHora);
    Task<Result> CancelarTurnoAsync(int turnoId, string? motivo = null);
    Task<Result> ReprogramarTurnoAsync(int turnoId, System.DateTime nuevaFecha);
    Task<Result<Turno2025>> MarcarComoAtendidoAsync(int turnoId, string informeMedico);
}
