CREATE VIEW dbo.[PacienteListView] AS
SELECT
    Id,
    Dni,
    Nombre,
    Apellido,
    Email,
    Telefono
FROM dbo.Paciente;
GO
