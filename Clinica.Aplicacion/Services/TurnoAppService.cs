using System.Threading.Tasks;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Repositorios;

namespace Clinica.Aplicacion.Services;

public class TurnoAppService : ITurnoAppService
{
    private readonly IRepositorioTurnos _repoTurnos;
    private readonly IRepositorioMedicos _repoMedicos;

    public TurnoAppService(IRepositorioTurnos repoTurnos, IRepositorioMedicos repoMedicos)
    {
        _repoTurnos = repoTurnos;
        _repoMedicos = repoMedicos;
    }

    public Task<Result<Turno2025>> ProgramarTurnoAsync(int pacienteId, int? medicoId, string especialidadUId, System.DateTime fechaYHora)
    {
        // TODO: implement using repository lookups and domain Programar logic
        return Task.FromResult(Result.Failure<Turno2025>("Not implemented"));
    }

    public Task<Result> CancelarTurnoAsync(int turnoId, string? motivo = null)
    {
        return Task.FromResult(Result.Failure("Not implemented"));
    }

    public Task<Result> ReprogramarTurnoAsync(int turnoId, System.DateTime nuevaFecha)
    {
        return Task.FromResult(Result.Failure("Not implemented"));
    }

    public Task<Result<Turno2025>> MarcarComoAtendidoAsync(int turnoId, string informeMedico)
    {
        return Task.FromResult(Result.Failure<Turno2025>("Not implemented"));
    }
}
