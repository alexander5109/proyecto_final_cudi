using System.Data;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using static Clinica.Dominio.IRepositorios.QueryModels;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public class DominioRepositorioYTurnosDapper(SQLServerConnectionFactory factory) : RepositorioDapperBase(factory), IRepositorioDomain, ITurnosRepositorio {
	public Task<Result<Unit>> UpdateTurnoWhereId(Turno2025 turno)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateTurnoWhereId",
				new {
					Id = turno.Id.Valor,
					OutcomeEstado = turno.OutcomeEstado.Codigo,
					OutcomeFecha = turno.OutcomeFechaOption.Valor,
					OutcomeComentario = turno.OutcomeComentarioOption.Valor
				},
				commandType: CommandType.StoredProcedure
			);
		});


	public async Task<Result<TurnoId>> InsertTurnoReturnId(Turno2025 turno)
		=> await TryAsync(async conn => {
			DynamicParameters parameters = new();
			parameters.Add("@FechaDeCreacion", turno.FechaDeCreacion.Valor);
			parameters.Add("@PacienteId", turno.PacienteId.Valor);
			parameters.Add("@MedicoId", turno.MedicoId.Valor);
			parameters.Add("@EspecialidadCodigo", turno.Especialidad.CodigoInternoValor);
			parameters.Add("@FechaHoraAsignadaDesde", turno.FechaHoraAsignadaDesdeValor);
			parameters.Add("@FechaHoraAsignadaHasta", turno.FechaHoraAsignadaHastaValor);
			parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
			await conn.ExecuteAsync(
				"sp_InsertTurnoReturnId",
				parameters,
				commandType: CommandType.StoredProcedure
			);
			int newId = parameters.Get<int>("@NewId");
			return new TurnoId(newId);
		});

	Task<Result<Usuario2025>> IRepositorioDomain.SelectUsuarioWhereNombre(NombreUsuario nombre)
		//ESTA FUNCION ES LA UNICA QUE TIENE PERMISO PARA CONVERTIR A TODOMAIN PORQUE IMPLEMENTA UNA INTERFAZ DESDE EL DOMINIO, PARA EL DOMINIO. 
		//QUISIERA SEPARAR EN 2 CLASES DISTINTAS PERO COMPARTEN ALGUNOS METODOS COMUNES, ASI QUE ESTA CLASE IMPLEMENTA 2 INTERFACES (QUE TIENEN FIRMAS EN COMUN)
		=> TryAsync(async conn => {
			UsuarioDbModel? dto = await conn.QuerySingleOrDefaultAsync<UsuarioDbModel>(
				"sp_SelectUsuarioWhereNombre",
				new { NombreUsuario = nombre.Valor },
				commandType: CommandType.StoredProcedure
			) ?? throw new Exception($"Usuario con NombreUsuario={nombre.Valor} no encontrado.");
			Result<Usuario2025> map = dto.ToDomain();
			if (map.IsError)
				throw new Exception($"Error creando Usuario2025 desde UsuarioDbModel (NombreUsuario={nombre.Valor}): {map.UnwrapAsError()}");

			return map.UnwrapAsOk();
		});




	Task<Result<IEnumerable<MedicoId>>> IRepositorioDomain.SelectMedicosIdWhereEspecialidadCode(EspecialidadCodigo2025 code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoId>("sp_SelectMedicosIdWhereEspecialidadCode", new { EspecialidadCodigoInterno = code }, commandType: CommandType.StoredProcedure);
		});



	public Task<Result<IEnumerable<TurnoQM>>> SelectTurnosProgramadosBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoQM>("sp_SelectTurnosProgramadosBetweenFechasWhereMedicoId",
			new { MedicoId = medicoId.Valor, 
				FechaDesde = fechaDesde, 
				FechaHasta = fechaHasta 
			}, commandType: CommandType.StoredProcedure);
		});



	public Task<Result<IEnumerable<HorarioMedicoQM>>> SelectHorariosVigentesBetweenFechasWhereMedicoId(MedicoId medicoId, DateTime fechaDesde, DateTime fechaHasta)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<HorarioMedicoQM>(
				"sp_SelectHorariosVigentesBetweenFechasWhereMedicoId",
				new { MedicoId = medicoId.Valor, 
					FechaDesde = fechaDesde.Date, 
					FechaHasta = fechaHasta.Date 
				},
				commandType: CommandType.StoredProcedure
			);
		});


	//----------------------------------------------implementaciones de turnosInterface

	public Task<Result<TurnoDbModel?>> SelectTurnoWhereId(TurnoId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<TurnoDbModel>(
				"sp_SelectTurnoWhereId",
				new { Id = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});

	public Task<Result<Unit>> DeleteTurnoWhereId(TurnoId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteTurnoWhereId",
				new { Id = id.Valor, },
				commandType: CommandType.StoredProcedure
			);
		});


	public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWherePacienteId(PacienteId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnosWherePacienteId",
				new { PacienteId = id.Valor },
				commandType: CommandType.StoredProcedure
			);
		});


	public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnos()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>(
				"sp_SelectTurnos",
				commandType: CommandType.StoredProcedure
			);
		});

	public Task<Result<IEnumerable<TurnoDbModel>>> SelectTurnosWhereMedicoId(MedicoId id)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<TurnoDbModel>("sp_SelectTurnosWhereMedicoId", new { Id = id.Valor, }, commandType: CommandType.StoredProcedure);
		});


}