CREATE PROCEDURE [dbo].[sp_SelectTurnosWhereMedicosIds]
    @MedicoIds IntListType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Turno
    WHERE MedicoId IN (SELECT Valor FROM @MedicoIds);
END;
GO
