CREATE PROCEDURE dbo.sp_InsertHorarioReturnId
    @MedicoId     INT,
    @DiaSemana    TINYINT,
    @HoraDesde    TIME(0),
    @HoraHasta    TIME(0),
    @VigenteDesde DATE,
    @VigenteHasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Medico WHERE Id = @MedicoId)
    BEGIN
        RAISERROR('El Médico especificado no existe.', 16, 1);
        RETURN;
    END

    IF (@DiaSemana < 0 OR @DiaSemana > 6)
    BEGIN
        RAISERROR('DiaSemana debe estar entre 0 y 6.', 16, 1);
        RETURN;
    END

    IF (@HoraDesde >= @HoraHasta)
    BEGIN
        RAISERROR('HoraDesde debe ser menor que HoraHasta.', 16, 1);
        RETURN;
    END

    IF (@VigenteHasta IS NOT NULL AND @VigenteDesde > @VigenteHasta)
    BEGIN
        RAISERROR('VigenteDesde debe ser menor o igual que VigenteHasta.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Horario (
        MedicoId,
        DiaSemana,
        HoraDesde,
        HoraHasta,
        VigenteDesde,
        VigenteHasta
    )
    VALUES (
        @MedicoId,
        @DiaSemana,
        @HoraDesde,
        @HoraHasta,
        @VigenteDesde,
        @VigenteHasta
    );

    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO
