using Dapper;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.Comun;
using Clinica.DataPersistencia.Mapeadores;
using Clinica.DataPersistencia.Infrastructure;
using Clinica.DataPersistencia.ModelsDto;

namespace Clinica.DataPersistencia.Repositorios;

public class MedicoRepository {
	private readonly DbConnectionFactory _connectionFactory;

	public MedicoRepository(DbConnectionFactory connectionFactory) {
		_connectionFactory = connectionFactory;
	}

	// ------------------- CREATE -------------------
	public async Task<Result<string>> InsertAsync(Medico2025 medico) {
		var dto = medico.ToDto(Guid.NewGuid().ToString());

		const string sql = @"
			INSERT INTO MedicoDto
			(Id, Name, LastName, Dni, Provincia, Domicilio, Localidad,
			 Especialidad, EspecialidadRama, Telefono, Guardia,
			 FechaIngreso, SueldoMinimoGarantizado)
			VALUES
			(@Id, @Name, @LastName, @Dni, @Provincia, @Domicilio, @Localidad,
			 @Especialidad, @EspecialidadRama, @Telefono, @Guardia,
			 @FechaIngreso, @SueldoMinimoGarantizado);";

		try {
			using var conn = _connectionFactory.CreateConnection();
			await conn.ExecuteAsync(sql, dto);

			// Inserción de horarios médicos
			foreach (var horario in dto.Horarios) {
				horario.MedicoId = dto.Id;
				await conn.ExecuteAsync(@"
					INSERT INTO HorarioMedicoDto (MedicoId, DiaSemana, Desde, Hasta)
					VALUES (@MedicoId, @DiaSemana, @Desde, @Hasta);", horario);
			}

			return new Result<string>.Ok(dto.Id);
		} catch (Exception ex) {
			return new Result<string>.Error($"Error al insertar médico: {ex.Message}");
		}
	}

	// ------------------- READ -------------------
	public async Task<Result<Medico2025>> GetByIdAsync(string id) {
		const string sqlMedico = "SELECT * FROM MedicoDto WHERE Id = @Id";
		const string sqlHorarios = "SELECT * FROM HorarioMedicoDto WHERE MedicoId = @Id";

		using var conn = _connectionFactory.CreateConnection();
		var medicoDto = await conn.QuerySingleOrDefaultAsync<MedicoDto>(sqlMedico, new { Id = id });

		if (medicoDto is null)
			return new Result<Medico2025>.Error("Médico no encontrado.");

		var horarios = (await conn.QueryAsync<HorarioMedicoDto>(sqlHorarios, new { Id = id })).ToList();
		medicoDto.Horarios = horarios;

		return medicoDto.ToDomain();
	}

	// ------------------- UPDATE -------------------
	public async Task<Result> UpdateAsync(string id, Medico2025 medico) {
		var dto = medico.ToDto(id);

		const string sql = @"
			UPDATE MedicoDto SET
				Name = @Name,
				LastName = @LastName,
				Dni = @Dni,
				Provincia = @Provincia,
				Domicilio = @Domicilio,
				Localidad = @Localidad,
				Especialidad = @Especialidad,
				EspecialidadRama = @EspecialidadRama,
				Telefono = @Telefono,
				Guardia = @Guardia,
				FechaIngreso = @FechaIngreso,
				SueldoMinimoGarantizado = @SueldoMinimoGarantizado
			WHERE Id = @Id;";

		try {
			using var conn = _connectionFactory.CreateConnection();
			await conn.ExecuteAsync(sql, dto);

			// Podés borrar e insertar los horarios si son pocos (simplificación)
			await conn.ExecuteAsync("DELETE FROM HorarioMedicoDto WHERE MedicoId = @Id", new { Id = dto.Id });
			foreach (var horario in dto.Horarios) {
				horario.MedicoId = dto.Id;
				await conn.ExecuteAsync(@"
					INSERT INTO HorarioMedicoDto (MedicoId, DiaSemana, Desde, Hasta)
					VALUES (@MedicoId, @DiaSemana, @Desde, @Hasta);", horario);
			}

			return new Result.Ok();
		} catch (Exception ex) {
			return new Result.Error($"Error al actualizar médico: {ex.Message}");
		}
	}

	// ------------------- DELETE -------------------
	public async Task<Result> DeleteAsync(string id) {
		using var conn = _connectionFactory.CreateConnection();
		using var tx = conn.BeginTransaction();
		try {
			await conn.ExecuteAsync("DELETE FROM HorarioMedicoDto WHERE MedicoId = @Id", new { Id = id }, tx);
			await conn.ExecuteAsync("DELETE FROM MedicoDto WHERE Id = @Id", new { Id = id }, tx);
			tx.Commit();
			return new Result.Ok();
		} catch (Exception ex) {
			tx.Rollback();
			return new Result.Error($"Error al eliminar médico: {ex.Message}");
		}
	}
}
