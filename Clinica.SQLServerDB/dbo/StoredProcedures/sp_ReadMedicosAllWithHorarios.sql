CREATE PROCEDURE sp_ReadMedicosAllWithHorarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        M.Id,
        M.Dni,
        M.Name,
        M.LastName,
        M.FechaIngreso,
        M.Domicilio,
        M.Localidad,
        M.Provincia,
        M.Telefono,
        M.Especialidad,
        M.EspecialidadRama,
        M.Guardia,
        M.SueldoMinimoGarantizado,

        H.Id AS HorarioId,
        H.DiaSemana,
        CASE H.DiaSemana
            WHEN 1 THEN N'Lunes'
            WHEN 2 THEN N'Martes'
            WHEN 3 THEN N'Miércoles'
            WHEN 4 THEN N'Jueves'
            WHEN 5 THEN N'Viernes'
            WHEN 6 THEN N'Sábado'
            WHEN 7 THEN N'Domingo'
        END AS DiaSemana2025,
        H.HoraDesde,
        H.HoraHasta
    FROM 
        dbo.Medico AS M
        LEFT JOIN dbo.HorarioMedico AS H
            ON M.Id = H.MedicoId
    ORDER BY 
        M.Id, H.DiaSemana, H.HoraDesde;
END;
GO
