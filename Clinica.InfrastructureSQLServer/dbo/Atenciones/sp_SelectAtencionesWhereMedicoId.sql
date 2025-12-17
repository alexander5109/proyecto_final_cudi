CREATE PROCEDURE dbo.sp_SelectAtencionesWhereMedicoId
    @MedicoId INT
AS
BEGIN
    SELECT *
    FROM dbo.Atencion
    WHERE MedicoId = @MedicoId
    ORDER BY Fecha DESC;
END;
