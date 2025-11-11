CREATE PROCEDURE [dbo].[sp_ReadMedicosAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [dbo].[Medico];
END;
GO