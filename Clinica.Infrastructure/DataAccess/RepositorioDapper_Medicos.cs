using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.TiposDeValor;
using Dapper;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;



public class MedicosRepositorioDapper(SQLServerConnectionFactory factory) : RepositorioDapperBase(factory), IMedicosRepositorio {
    Task<Result<Unit>> IMedicosRepositorio.DeleteMedicoWhereId(MedicoId id)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_DeleteMedicoWhereId",
				new {
					Id = id.Valor,
				},
				commandType: CommandType.StoredProcedure
			);
		});

	Task<Result<MedicoId>> IMedicosRepositorio.InsertMedicoReturnId(Medico2025 medico)
		=> TryAsync(async conn => {
			return new MedicoId(await conn.ExecuteScalarAsync<int>(
				"sp_InsertMedicoReturnId",
				new {
					Nombre = medico.NombreCompleto.NombreValor,
					Apellido = medico.NombreCompleto.ApellidoValor,
					Dni = medico.Dni.Valor,
					ProvinciaCodigo = medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
					Domicilio = medico.Domicilio.DireccionValor,
					Localidad = medico.Domicilio.Localidad.NombreValor,
					EspecialidadCodigoInterno = medico.EspecialidadUnica.CodigoInternoValor,
					Telefono = medico.Telefono.Valor,
					Email = medico.Email.Valor,
					Guardia = medico.HaceGuardiasValor,
					FechaIngreso = medico.FechaIngreso.Valor
				},
				commandType: CommandType.StoredProcedure
			));
		});

	Task<Result<IEnumerable<MedicoDbModel>>> IMedicosRepositorio.SelectMedicos()
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoDbModel>("sp_SelectMedicos", commandType: CommandType.StoredProcedure);
		});


	Task<Result<IEnumerable<MedicoDbModel>>> IMedicosRepositorio.SelectMedicosWhereEspecialidadCode(EspecialidadCodigo2025 code)
		=> TryAsync(async conn => {
			return await conn.QueryAsync<MedicoDbModel>("sp_SelectMedicosWhereEspecialidadCode", new { EspecialidadCodigoInterno = code }, commandType: CommandType.StoredProcedure);
		});

	Task<Result<MedicoDbModel?>> IMedicosRepositorio.SelectMedicoWhereId(MedicoId id)
		=> TryAsync(async conn => {
			return await conn.QuerySingleOrDefaultAsync<MedicoDbModel>("sp_SelectMedicoWhereId",
			new { MedicIdoId = id.Valor }, commandType: CommandType.StoredProcedure);
		});

	Task<Result<Unit>> IMedicosRepositorio.UpdateMedicoWhereId(Medico2025 medico)
		=> TryAsyncVoid(async conn => {
			await conn.ExecuteAsync(
				"sp_UpdateMedico",
				new {
					Id = medico.Id.Valor,
					Nombre = medico.NombreCompleto.NombreValor,
					Apellido = medico.NombreCompleto.ApellidoValor,
					Dni = medico.Dni.Valor,
					ProvinciaCodigo = medico.Domicilio.Localidad.Provincia.CodigoInternoValor,
					Domicilio = medico.Domicilio.DireccionValor,
					Localidad = medico.Domicilio.Localidad.NombreValor,
					EspecialidadCodigoInterno = medico.EspecialidadUnica.CodigoInternoValor,
					Telefono = medico.Telefono.Valor,
					Email = medico.Email.Valor,
					Guardia = medico.HaceGuardiasValor,
					FechaIngreso = medico.FechaIngreso.Valor
				},
				commandType: CommandType.StoredProcedure
			);
		});
}