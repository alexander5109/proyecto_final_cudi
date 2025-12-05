using Clinica.Dominio.Comun;
using Clinica.Dominio.Entidades;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.TiposDeValor;


namespace Clinica.Dominio.Servicios;

public class ServiciosPublicos {

    //Task<Result<Turno2025>> AgendarTurnoAsync(PacienteId pacienteId, MedicoId medicoId, EspecialidadCodigo especialidadCodigo, DateTime desde, DateTime hasta);

    //Task<Result<Turno2025>> CancelarTurnoAsync(TurnoId id, Option<string> motivo);

    //Task<Result<Turno2025>> ReprogramarTurnoAsync(TurnoId id, DateTime nuevaFechaDesde, DateTime nuevaFechaHasta);

    //Task<Result<Turno2025>> MarcarComoAusente(TurnoId id, Option<string> motivo);
    //Task<Result<Turno2025>> MarcarComoConcretado(TurnoId id, Option<string> motivo);


    public static async Task<Result<Usuario2025>> ValidarCredenciales(
        string username,
        string password,
        IRepositorioDomainServiciosPrivados repositorio
    ) {
        Result<Usuario2025> resultadoUsuario =
            await repositorio.SelectUsuarioWhereNombreAsDomain(new NombreUsuario(username));

        return resultadoUsuario switch {
            Result<Usuario2025>.Ok ok =>
                ok.Valor.PasswordMatch(password)
                    ? new Result<Usuario2025>.Ok(ok.Valor)
                    : new Result<Usuario2025>.Error("Usuario o contraseña incorrectos"),

            Result<Usuario2025>.Error err =>
                err, // devolvemos el error tal cual
            _ =>
                new Result<Usuario2025>.Error("Error inesperado validando credenciales")
        };
    }



    public static async Task<Result<IReadOnlyList<Disponibilidad2025>>> SolicitarDisponibilidadesPara(
            EspecialidadCodigo solicitudEspecialidadCodigo,
            DateTime aPartirDeCuando,
            int cuantos,
            IRepositorioDomainServiciosPrivados repositorio
        ) {

		if (cuantos > 50) {
			return new Result<IReadOnlyList<Disponibilidad2025>>.Error("No vamos a producir tantas disponibilidades. Si quiere, adelante la fecha");
		}

		Result<Especialidad2025> solicitudEspecialidadResult = Especialidad2025.CrearPorCodigoInterno(solicitudEspecialidadCodigo);
        if (solicitudEspecialidadResult.IsError) return new Result<IReadOnlyList<Disponibilidad2025>>.Error(solicitudEspecialidadResult.UnwrapAsError());
        Especialidad2025 solicitudEspecialidad = solicitudEspecialidadResult.UnwrapAsOk();


        List<Disponibilidad2025> lista = new(capacity: cuantos);

        await foreach (Result<Disponibilidad2025> dispResult in
            ServiciosPrivados.GenerarDisponibilidades(
                solicitudEspecialidad,
                aPartirDeCuando,
                repositorio)) {
            if (dispResult.IsError) {
                // Propagamos el error aguas arriba
                return new Result<IReadOnlyList<Disponibilidad2025>>
                    .Error(dispResult.UnwrapAsError());
            }

            lista.Add(dispResult.UnwrapAsOk());

            if (lista.Count >= cuantos)
                break;
        }

        if (lista.Count > 0) {
            return new Result<IReadOnlyList<Disponibilidad2025>>.Ok(lista);
        }

        return new Result<IReadOnlyList<Disponibilidad2025>>.Error("No se encontraron disponibilidades.");
    }



    public static async Task<Result<Turno2025>> SolicitarTurnoEnLaPrimeraDisponibilidad(
        PacienteId pacienteId,
        EspecialidadCodigo solicitudEspecialidadCodigo,
        DateTime solicitudFechaCreacionRaw,
        IRepositorioDomainServiciosPrivados repositorio
    ) {

        Result<FechaRegistro2025> fechaRresult = FechaRegistro2025.Crear(solicitudFechaCreacionRaw);
        if (fechaRresult.IsError) return new Result<Turno2025>.Error(fechaRresult.UnwrapAsError());
        FechaRegistro2025 solicitudFechaCreacion = fechaRresult.UnwrapAsOk();




        Result<Especialidad2025> solicitudEspecialidadResult = Especialidad2025.CrearPorCodigoInterno(solicitudEspecialidadCodigo);
        if (solicitudEspecialidadResult.IsError) return new Result<Turno2025>.Error(solicitudEspecialidadResult.UnwrapAsError());
        Especialidad2025 solicitudEspecialidad = solicitudEspecialidadResult.UnwrapAsOk();


        // 1. Buscar próxima disponibilidad
        Result<Disponibilidad2025> dispResult =
            await ServiciosPrivados.EncontrarProximaDisponibilidad(
                solicitudEspecialidad,
                solicitudFechaCreacion.Valor,
                repositorio
            );

        if (dispResult is Result<Disponibilidad2025>.Error errDisp)
            return new Result<Turno2025>.Error(errDisp.Mensaje);

        Disponibilidad2025 disp = ((Result<Disponibilidad2025>.Ok)dispResult).Valor;

        // 2. CrearResult turno provisorio desde el dominio
        Result<Turno2025> turnoResult = Turno2025.ProgramarNuevo(
            new TurnoId(-1),            // provisional
            pacienteId,
            solicitudFechaCreacion,
            disp
        );

        if (turnoResult is Result<Turno2025>.Error errTurno)
            return new Result<Turno2025>.Error(errTurno.Mensaje);

        Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)turnoResult).Valor;

        // 3. Persistir en la BD (insert que devuelve el TurnoId)
        Result<TurnoId> insertResult = await repositorio.InsertTurnoReturnId(turnoProvisorio);

        if (insertResult is Result<TurnoId>.Error errPersist)
            return new Result<Turno2025>.Error($"Error al persistir el nuevo turno: {errPersist.Mensaje}");

        TurnoId idReal = ((Result<TurnoId>.Ok)insertResult).Valor;

        // 4. Devolver el turno ya con su ID real seteado
        return new Result<Turno2025>.Ok(turnoProvisorio with { Id = idReal });
    }



    public static async Task<Result<Turno2025>> SolicitarReprogramacionALaPrimeraDisponibilidad(
        TurnoId turnoOriginalId,
        DateTime outcomeFecha,
        string outcomeComentario,
        IRepositorioDomainServiciosPrivados repositorio
    ) {
        Result<Turno2025> turnoOriginalResult = await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId);
        if (turnoOriginalResult.IsError) return turnoOriginalResult;
        //if (turnoOriginalResult.IsError) return new Result<Turno2025>.Error($"No se encontró el turno original: {turnoOriginalResult.UnwrapAsError()}");
        Turno2025 turnoOriginal = turnoOriginalResult.UnwrapAsOk();


        Result<Turno2025> canceladoResult = turnoOriginal.SetOutcome(TurnoOutcomeEstado2025.Reprogramado, outcomeFecha, outcomeComentario);
        if (canceladoResult.IsError) return canceladoResult;
        //if (canceladoResult.IsError) return new Result<Turno2025>.Error($"No se puede cancelar el turno: {canceladoResult.UnwrapAsError()}");
        Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;


        Result<Unit> updateResult = await repositorio.UpdateTurnoWhereId(turnoCancelado);
        if (updateResult.IsError) return new Result<Turno2025>.Error($"Error al persistir la cancelación del turno: \n\t{updateResult.UnwrapAsError()}");


        Result<Disponibilidad2025> dispResult = await ServiciosPrivados.EncontrarProximaDisponibilidad(
            turnoOriginal.Especialidad,
            outcomeFecha,
            repositorio
        );

        if (dispResult is Result<Disponibilidad2025>.Error e3)
            return new Result<Turno2025>.Error(e3.Mensaje);

        Disponibilidad2025 disponibilidad = ((Result<Disponibilidad2025>.Ok)dispResult).Valor;


        Result<Turno2025> provResult = turnoCancelado.Reprogramar(
            disponibilidad,
            new TurnoId(-1) // placeholder hasta persistencia
        );

        if (provResult is Result<Turno2025>.Error e4)
            return new Result<Turno2025>.Error(e4.Mensaje);

        Turno2025 turnoProvisorio = ((Result<Turno2025>.Ok)provResult).Valor;


        Result<TurnoId> insertResult = await repositorio.InsertTurnoReturnId(turnoProvisorio);

        if (insertResult is Result<TurnoId>.Error e5)
            return new Result<Turno2025>.Error(
                $"Error al persistir el nuevo turno reprogramado: {e5.Mensaje}"
            );

        TurnoId idReal = ((Result<TurnoId>.Ok)insertResult).Valor;


        return new Result<Turno2025>.Ok(
            turnoProvisorio with { Id = idReal }
        );
    }



    public static async Task<Result<Turno2025>> SolicitarCancelacion(
        TurnoId turnoOriginalId,
        DateTime outcomeFecha,
        string outcomeComentario,
        IRepositorioDomainServiciosPrivados repositorio
    ) {
        Result<Turno2025> turnoOriginalResult = await repositorio.SelectTurnoWhereIdAsDomain(turnoOriginalId);
        if (turnoOriginalResult.IsError) return turnoOriginalResult;
        //if (turnoOriginalResult.IsError) return new Result<Turno2025>.Error($"No se encontró el turno original: {turnoOriginalResult.UnwrapAsError()}");
        Turno2025 turnoOriginal = turnoOriginalResult.UnwrapAsOk();

        // 1. Aplicar regla de dominio para cancelar
        Result<Turno2025> canceladoResult = turnoOriginal.SetOutcome(
            TurnoOutcomeEstado2025.Cancelado,
            outcomeFecha,
            outcomeComentario
        );

        if (canceladoResult is Result<Turno2025>.Error e1)
            return new Result<Turno2025>.Error(e1.Mensaje);

        Turno2025 turnoCancelado = ((Result<Turno2025>.Ok)canceladoResult).Valor;

        // 2. Guardar cambios (IO)
        Result<Unit> updateResult = await repositorio.UpdateTurnoWhereId(turnoCancelado);

        if (updateResult is Result<Unit>.Error e2)
            return new Result<Turno2025>.Error(
                $"Error al persistir la cancelación del turno: {e2.Mensaje}"
            );

        return new Result<Turno2025>.Ok(turnoCancelado);
    }
}