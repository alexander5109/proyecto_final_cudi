CREATE PROCEDURE sp_ReadMedicosAllWithHorarios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        M.Id,
        M.EspecialidadCodigoInterno,
        M.Dni,
        M.Name AS Nombre,
        M.LastName AS Apellido,
        M.FechaIngreso,
        M.Domicilio,
        M.Localidad,
        M.ProvinciaCodigo,
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
        ) AS HorariosJson    -- <--- cambiar nombre del alias
    FROM Medico M;
END;
GO
