CREATE PROCEDURE sp_SelectUsuarioWhereId
    @Id INT
AS
BEGIN
    SELECT *
    FROM Usuario
    WHERE Id = @Id;
END
