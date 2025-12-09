CREATE PROCEDURE dbo.sp_SelectTurnos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM dbo.[Turno]
	
	ORDER BY FechaHoraAsignadaDesde, OutcomeEstado ASC
	;
END;
GO