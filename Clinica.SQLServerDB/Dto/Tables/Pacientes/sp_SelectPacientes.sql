CREATE PROCEDURE dbo.sp_SelectPacientes
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM dbo.Paciente;
END;
GO

