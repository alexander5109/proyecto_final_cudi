using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Dapper;
using static Clinica.Shared.Dtos.DbModels;

namespace Clinica.Infrastructure.DataAccess;



public class PacientesRepositorioDapper(SQLServerConnectionFactory factory) : RepositorioDapperBase(factory), IPacientesRepositorio {
    public Task<Result<IEnumerable<PacienteDbModel>>> SelectPacientes()
        => TryAsync(async conn => {
            return await conn.QueryAsync<PacienteDbModel>("sp_SelectPacientes", commandType: CommandType.StoredProcedure);
        });
    public Task<Result<PacienteDbModel?>> SelectPacienteWhereId(PacienteId id)
        => TryAsync(async conn => {
            return await conn.QuerySingleOrDefaultAsync<PacienteDbModel>(
                "sp_SelectPacienteWhereId",
                new { Id = id.Valor },
                commandType: CommandType.StoredProcedure
            );
        });


    public Task<Result<PacienteId>> InsertPacienteReturnId(Paciente2025 paciente)
        => TryAsync(async conn => {
            return new PacienteId(await conn.ExecuteScalarAsync<int>(
                "sp_InsertPacienteReturnId",
                new {
                    Dni = paciente.Dni.Valor,
                    Nombre = paciente.NombreCompleto.NombreValor,
                    Apellido = paciente.NombreCompleto.ApellidoValor,
                    FechaIngreso = paciente.FechaIngreso.Valor,
                    Email = paciente.Contacto.Email.Valor,
                    Telefono = paciente.Contacto.Telefono.Valor,
                    FechaNacimiento = paciente.FechaNacimiento.Valor,
                    Domicilio = paciente.Domicilio.DireccionValor,
                    Localidad = paciente.Domicilio.Localidad.NombreValor,
                    ProvinciaCodigo = paciente.Domicilio.Localidad.Provincia.CodigoInternoValor,
                },
                commandType: CommandType.StoredProcedure
            ));
        });

    public Task<Result<Unit>> UpdatePacienteWhereId(Paciente2025 paciente)
        => TryAsyncVoid(async conn => {
            await conn.ExecuteAsync(
                "sp_UpdatePaciente",
                new {
                    Id = paciente.Id.Valor,
                    Dni = paciente.Dni.Valor,
                    Nombre = paciente.NombreCompleto.NombreValor,
                    Apellido = paciente.NombreCompleto.ApellidoValor,
                    FechaIngreso = paciente.FechaIngreso.Valor,
                    Email = paciente.Contacto.Email.Valor,
                    Telefono = paciente.Contacto.Telefono.Valor,
                    FechaNacimiento = paciente.FechaNacimiento.Valor,
                    Domicilio = paciente.Domicilio.DireccionValor,
                    Localidad = paciente.Domicilio.Localidad.NombreValor,
                    ProvinciaCodigo = paciente.Domicilio.Localidad.Provincia.CodigoInternoValor
                },
                commandType: CommandType.StoredProcedure
            );
        });


    public Task<Result<Unit>> DeletePacienteWhereId(PacienteId id)
        => TryAsyncVoid(async conn => {
            await conn.ExecuteAsync(
                "sp_DeletePacienteWhereId",
                new { Id = id.Valor },
                commandType: CommandType.StoredProcedure
            );
        });

}