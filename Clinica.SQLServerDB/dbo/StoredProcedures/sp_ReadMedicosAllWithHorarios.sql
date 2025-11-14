CREATE PROCEDURE sp_ReadMedicosAllWithHorarios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        M.Id,
        M.Name,
        M.LastName,
        (
            SELECT *
            FROM HorarioMedico H
            WHERE H.MedicoId = M.Id
            FOR JSON PATH
        ) AS Horarios
    FROM Medico M
    FOR JSON PATH;
END;
GO
