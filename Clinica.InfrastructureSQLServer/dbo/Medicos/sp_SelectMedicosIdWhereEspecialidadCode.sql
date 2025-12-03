CREATE PROCEDURE dbo.sp_SelectMedicosIdWhereEspecialidadCode
    @EspecialidadCodigoInterno INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        Id
    FROM dbo.Medico
    WHERE EspecialidadCodigoInterno = @EspecialidadCodigoInterno;
END;
GO
