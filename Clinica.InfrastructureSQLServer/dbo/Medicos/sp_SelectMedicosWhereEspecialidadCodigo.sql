CREATE PROCEDURE dbo.sp_SelectMedicosWhereEspecialidadCodigo
    @EspecialidadCodigo INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
       *
    FROM dbo.Medico
    WHERE EspecialidadCodigo = @EspecialidadCodigo;
END;
GO
