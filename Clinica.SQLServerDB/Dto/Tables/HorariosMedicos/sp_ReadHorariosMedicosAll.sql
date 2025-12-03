CREATE PROCEDURE dbo.[sp_ReadHorariosMedicosAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM dbo.HorarioMedico;
END;
GO