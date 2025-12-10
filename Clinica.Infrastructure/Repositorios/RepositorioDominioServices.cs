using System.Data;
using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using static Clinica.Dominio.IInterfaces.QueryModels;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.Repositorios;


public class RepositorioDominioServices(SQLServerConnectionFactory factory) : RepositorioBase(factory), IRepositorioDominioServices {



	Task<Result<Turno2025Agg>> IRepositorioDomainServiciosPrivados.UpdateTurnoWhereId(TurnoId id, Turno2025 instance) => ((IRepositorioTurnos)this).UpdateTurnoWhereId(id, instance);
	Task<Result<IEnumerable<MedicoId>>> IRepositorioDominioServices.SelectMedicosIdWhereEspecialidadCodigo(EspecialidadCodigo code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoId>(
				"sp_SelectMedicosIdWhereEspecialidadCodigo",
				new { EspecialidadCodigo = code },
				commandType: CommandType.StoredProcedure
			);
		});







	Task<Result<TurnoId>> IRepositorioDomainServiciosPrivados.InsertTurnoReturnId(Turno2025 instance) => ((IRepositorioTurnos)this).InsertTurnoReturnId(instance);







	Task<Result<IEnumerable<TurnoQM>>> IRepositorioDomainServiciosPrivados.SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
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



	Task<Result<IEnumerable<HorarioMedicoQM>>> IRepositorioDomainServiciosPrivados.SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
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





	//Task<Result<Usuario2025Agg>> IRepositorioDomainServiciosPrivados.SelectUsuarioWhereIdAsDomain(UsuarioId id)
	//	=> TryAsync(async conn => {
	//		UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
	//			"sp_SelectUsuarioWhereId",
	//			new { Id = id.Valor },
	//			commandType: CommandType.StoredProcedure
	//		) ?? throw new Exception($"Usuario con Id={id.Valor} no encontrado.");
	//		Result<Usuario2025> map = dto.ToDomain();
	//		if (map.IsError)
	//			throw new Exception($"Erro de dominio: Usuario con Id={id.Valor} no cumple las reglas del dominio: \n{map.UnwrapAsError()}");
	//		return Usuario2025Agg.Crear(id, map.UnwrapAsOk());
	//	});



	Task<Result<Turno2025Agg>> IRepositorioDomainServiciosPrivados
		.SelectTurnoWhereIdAsDomain(TurnoId id)
		=> TryAsyncAndMap<TurnoDbModel, Turno2025Agg>(
			query: conn => conn.QuerySingleOrDefaultAsync<TurnoDbModel>(
				"sp_SelectTurnoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			),

			mapper: dto => {
				if (dto is null)
					return new Result<Turno2025Agg>.Error(
						$"Turno con id {id} no encontrado."
					);

				// ToDomainAgg() → Result<Turno2025>
				return dto.ToDomain().BindWithPrefix(
					$"Error de dominio en turno {id}: ",
					turnoOk => new Result<Turno2025Agg>.Ok(Turno2025Agg.Crear(id, turnoOk))
				);
			}
		);











}

