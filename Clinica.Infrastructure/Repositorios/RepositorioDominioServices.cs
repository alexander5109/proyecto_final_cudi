using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using Clinica.Shared.DbModels;
using Dapper;
using static Clinica.Dominio.IInterfaces.QueryModels;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;
using static Clinica.Shared.ApiDtos.TurnoDtos;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioDominioServices(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioDominioServices {


	Task<Result<Turno2025>> IRepositorioDominioServices.UpdateTurnoWhereIdAndReturnAsDomain(
		TurnoId id,
		Turno2025 instance
	)
		=> TryAsync(async conn => {
            // Enviar parámetros al SP
            TurnoDbModel parametros = instance.ToModel(id);

			// Ejecutar SP que devuelve int RowsAffected
			int rowsAffected = await conn.ExecuteScalarAsync<int>(
				"sp_UpdateTurnoWhereId",
				parametros,
				commandType: CommandType.StoredProcedure
			);

			if (rowsAffected == 0)
				throw new Exception("No se actualizó ningún turno.");
			// → TryAsync lo convertirá en Result.Error automáticamente

			// Si hubo cambios, devolver el DTO actualizado
			// (tenemos todo en la instancia, ya que la DB solo actualiza)
			return instance;
		});



	Task<Result<IEnumerable<MedicoId>>> IRepositorioDominioServices.SelectMedicosIdWhereEspecialidadCodigo(EspecialidadEnum code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoId>(
				"sp_SelectMedicosIdWhereEspecialidadCodigo",
				new { EspecialidadCodigo = code },
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<TurnoQM>>> IRepositorioDominioServices.SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoQM>(
				"sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId",
				new {
					MedicoId = medicoId.Valor,
					FechaDesde = fechaDesde,
					FechaHasta = fechaHasta
				},
				commandType: CommandType.StoredProcedure
			);
		});



	Task<Result<IEnumerable<HorarioMedicoQM>>> IRepositorioDominioServices.SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<HorarioMedicoQM>(
				"sp_SelectHorariosVigentesBetweenFechasWhereMedicoId",
				new {
					MedicoId = medicoId.Valor,
					FechaDesde = fechaDesde.Date,
					FechaHasta = fechaHasta.Date
				},
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<Usuario2025>> IRepositorioDominioServices.SelectUsuarioWhereIdAsDomain(UsuarioId id)
		=> TryResultAsync(async conn => {
			UsuarioDto? dto =
				await conn.QuerySingleOrDefaultAsync<UsuarioDto>(
					"sp_SelectUsuarioWhereId",
					new { Id = id.Valor },
					commandType: CommandType.StoredProcedure
				);

			if (dto is null)
				return new Result<Usuario2025>.Error($"Usuario con Id={id} no encontrado.");

			return dto.ToDomain(); // ESTE devuelve Result<Usuario2025>
		});


	Task<Result<Turno2025>> IRepositorioDominioServices.SelectTurnoWhereIdAsDomain(TurnoId id)
		=> TryAsyncAndMap(
			query: conn => conn.QuerySingleOrDefaultAsync<TurnoDto>(
				"sp_SelectTurnoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			),

			mapper: dto => {
				if (dto is null)
					return new Result<Turno2025>.Error(
						$"Turno con id {id} no encontrado."
					);

				// ToDomainAgg() → Result<Turno2025>
				return dto.ToDomain().BindWithPrefix(
					$"Error de dominio en turno {id}: ",
					turnoOk => new Result<Turno2025>.Ok(turnoOk)
				);
			}
		);











}

