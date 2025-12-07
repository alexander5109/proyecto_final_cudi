CREATE PROCEDURE dbo.sp_SelectMedicosIdWhereEspecialidadCodigo
    @EspecialidadCodigo INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        Id
    FROM dbo.Medico
    WHERE EspecialidadCodigo = @EspecialidadCodigo;
END;
GO
