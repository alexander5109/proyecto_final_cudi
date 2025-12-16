CREATE PROCEDURE dbo.sp_SelectAtencionesPorPaciente
    @PacienteId INT
AS
BEGIN
    SELECT *
    FROM dbo.Atencion
    WHERE PacienteId = @PacienteId
    ORDER BY Fecha DESC;
END;
