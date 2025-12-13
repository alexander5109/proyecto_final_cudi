CREATE PROCEDURE dbo.sp_SelectMedicoWithHorariosWhereId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1
        M.Id,
        M.EspecialidadCodigo,
        M.Dni,
        M.Nombre AS Nombre,
        M.Apellido AS Apellido,
        M.FechaIngreso,
        M.Domicilio,
        M.Localidad,
        M.ProvinciaCodigo,
        M.Telefono,
        M.Email,
        M.Guardia,
        (
            SELECT 
                H.Id,
                H.MedicoId,
                H.DiaSemana,
                H.HoraDesde,
                H.HoraHasta,
                H.VigenteDesde,
                H.VigenteHasta
            FROM dbo.Horario H
            WHERE H.MedicoId = M.Id
            FOR JSON PATH
        ) AS HorariosJson    -- <--- cambiar nombre del alias
    FROM dbo.Medico M
    WHERE M.Id = @Id;
END;
GO
