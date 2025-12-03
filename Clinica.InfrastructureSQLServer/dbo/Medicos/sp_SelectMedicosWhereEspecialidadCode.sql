CREATE PROCEDURE sp_SelectMedicosWhereEspecialidadCode
    @EspecialidadCodigoInterno INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        Id
    FROM Medico
    WHERE EspecialidadCodigoInterno = @EspecialidadCodigoInterno;
END;
GO
