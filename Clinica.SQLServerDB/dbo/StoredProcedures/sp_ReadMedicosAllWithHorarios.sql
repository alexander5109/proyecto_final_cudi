CREATE PROCEDURE sp_ReadMedicosAllWithHorarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        M.Id,
        M.EspecialidadCodigoInterno,
        M.Dni,
        M.Name,
        M.LastName,
        M.FechaIngreso,
        M.Domicilio,
        M.Localidad,
        M.Provincia,
        M.Telefono,
        M.Guardia,
        M.SueldoMinimoGarantizado,

        (
            SELECT 
                H.Id,
                H.MedicoId,
                H.DiaSemana,
                H.HoraDesde,
                H.HoraHasta
            FROM HorarioMedico H
            WHERE H.MedicoId = M.Id
            FOR JSON PATH
        ) AS Horarios
    FROM Medico M;  -- NOTICE: NO "FOR JSON PATH" HERE
END;
GO
