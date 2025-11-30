CREATE PROCEDURE [dbo].sp_SelectPacientesListView
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PacienteListView;
END;
GO
