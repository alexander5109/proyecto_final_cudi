CREATE PROCEDURE [dbo].[sp_UpdateHorarioMedico]
    @Id INT,
    @DiaSemana INT,
    @HoraDesde TIME(7),
    @HoraHasta TIME(7)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].HorarioMedico
    SET 
        DiaSemana = @DiaSemana,
        HoraDesde = @HoraDesde,
        HoraHasta = @HoraHasta
    WHERE [Id] = @Id;
END;
GO
