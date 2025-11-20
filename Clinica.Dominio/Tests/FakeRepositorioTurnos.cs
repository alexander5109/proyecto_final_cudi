using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Clinica.Dominio.FunctionalProgramingTools;
using Clinica.Dominio.Servicios;
using Clinica.Dominio.Entidades;

namespace Clinica.Dominio.Tests.Escenarios;

public class FakeRepositorioTurnos : IRepositorioTurnos {
	private readonly List<Turno2025> _turnos = new();

	public Result<IReadOnlyList<Turno2025>> ObtenerTurnosPorMedicoDni(string medicoDni, DateTime desde, DateTime hasta) => new Result<IReadOnlyList<Turno2025>>.Ok(_turnos.Where(t => t.MedicoAsignado is not null && t.MedicoAsignado.Dni.Valor == medicoDni && t.FechaYHora >= desde && t.FechaYHora <= hasta).ToList());

	public Result<IReadOnlyList<Turno2025>> ObtenerTurnosPorEspecialidad(string especialidadTitulo, DateTime desde, DateTime hasta) => new Result<IReadOnlyList<Turno2025>>.Ok(_turnos.Where(t => t.Especialidad.Titulo == especialidadTitulo && t.FechaYHora >= desde && t.FechaYHora <= hasta).ToList());

	public Result<Turno2025> Guardar(Turno2025 turno) { _turnos.Add(turno); return new Result<Turno2025>.Ok(turno); }
	public Result<bool> Eliminar(Turno2025 turno) { _turnos.RemoveAll(t => t.FechaYHora == turno.FechaYHora && t.Paciente.Dni.Valor == turno.Paciente.Dni.Valor); return new Result<bool>.Ok(true); }
}
