CREATE PROCEDURE dbo.sp_SelectMedicos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM dbo.Medico M;
END;
GO
