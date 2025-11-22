CREATE PROCEDURE [dbo].[sp_ReadPacientesAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [dbo].Paciente;
END;
GO