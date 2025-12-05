CREATE PROCEDURE dbo.sp_SelectMedicosWhereEspecialidadCode
    @EspecialidadCodigoInterno INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
       *
    FROM dbo.Medico
    WHERE EspecialidadCodigoInterno = @EspecialidadCodigoInterno;
END;
GO
