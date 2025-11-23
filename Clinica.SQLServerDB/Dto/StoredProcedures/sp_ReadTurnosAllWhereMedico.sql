CREATE PROCEDURE [dbo].sp_ReadTurnosAllWhereMedico
	@MedicoId INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [dbo].[Turno]
	WHERE MedicoId = @MedicoId;
END;
GO