CREATE PROCEDURE dbo.[sp_CreateHorarioMedico]
    @MedicoId INT,
    @DiaSemana INT,
    @HoraDesde TIME(7),
    @HoraHasta TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.HorarioMedico
        (MedicoId, DiaSemana, HoraDesde, HoraHasta)
    VALUES
        (@MedicoId, @DiaSemana, @HoraDesde, @HoraHasta);

    SELECT SCOPE_IDENTITY() AS NuevoId;
END;
GO
