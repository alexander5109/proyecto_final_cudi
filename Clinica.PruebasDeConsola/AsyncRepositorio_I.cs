using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;

namespace Clinica.PruebasDeConsola;

public interface AsyncRepositorio_I {
	static abstract Task<List<Result<Paciente2025>>> GetPacientes();
	static abstract Task<List<Medico2025>> GetMedicos();
	static abstract Task<ListaTurnosHistorial2025> GetTurnos();
}