CREATE PROCEDURE dbo.sp_SelectAtencionesWherePacienteId
    @PacienteId INT
AS
BEGIN
    SELECT *
    FROM dbo.Atencion
    WHERE PacienteId = @PacienteId
    ORDER BY Fecha DESC;
END;
