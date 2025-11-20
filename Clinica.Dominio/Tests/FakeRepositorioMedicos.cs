using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Clinica.Dominio.FunctionalProgramingTools;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.Tests.Escenarios;

public class FakeRepositorioMedicos(List<Medico2025> medicos) : IRepositorioMedicos {
	public Result<IReadOnlyList<Medico2025>> ObtenerMedicosPorEspecialidad(string especialidadTitulo) => new Result<IReadOnlyList<Medico2025>>.Ok(medicos.Where(m => m.Especialidad.Titulo == especialidadTitulo).ToList());
	public Result<Medico2025> ObtenerPorDni(string dni) => new Result<Medico2025>.Ok(medicos.First(m => m.Dni.Valor == dni));
	public Result<Medico2025> Guardar(Medico2025 medico) => new Result<Medico2025>.Ok(medico);
	public Result<bool> EliminarPorDni(string dni) => new Result<bool>.Ok(true);
}
