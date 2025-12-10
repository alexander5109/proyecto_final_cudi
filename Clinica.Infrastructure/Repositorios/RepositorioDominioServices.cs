using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Shared.ApiDtos;
using Dapper;
using static Clinica.Dominio.IInterfaces.QueryModels;
using static Clinica.Shared.ApiDtos.TurnoDtos;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioDominioServices(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioDominioServices {



	Task<Result<Turno2025>> IRepositorioDominioServices.UpdateTurnoWhereId(TurnoId id, Turno2025 instance) => ((IRepositorioTurnos)this).UpdateTurnoWhereId(id, instance);
	Task<Result<IEnumerable<MedicoId>>> IRepositorioDominioServices.SelectMedicosIdWhereEspecialidadCodigo(EspecialidadCodigo code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoId>(
				"sp_SelectMedicosIdWhereEspecialidadCodigo",
				new { EspecialidadCodigo = code },
				commandType: CommandType.StoredProcedure
			);
		});







	Task<Result<TurnoId>> IRepositorioDominioServices.InsertTurnoReturnId(Turno2025 instance) => ((IRepositorioTurnos)this).InsertTurnoReturnId(instance);







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
		=> TryAsyncAndMap<TurnoDto, Turno2025>(
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

