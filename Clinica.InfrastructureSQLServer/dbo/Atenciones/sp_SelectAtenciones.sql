CREATE PROCEDURE dbo.sp_SelectAtenciones
AS
BEGIN
    SELECT *
    FROM dbo.Atencion
    ORDER BY Fecha DESC;
END;
