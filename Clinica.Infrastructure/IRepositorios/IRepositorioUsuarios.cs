using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioUsuarios {
	Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id);
	Task<Result<UsuarioId>> InsertUsuarioReturnId(Usuario2025 instance);
	Task<Result<IEnumerable<UsuarioDbModel>>> SelectUsuarios();
	Task<Result<UsuarioDbModel?>> SelectUsuarioWhereId(UsuarioId id);
	Task<Result<UsuarioDto>> UpdateUsuarioWhereId(UsuarioId id, Usuario2025 instance);
	Task<Result<UsuarioDbModel>> SelectUsuarioProfileWhereUsername(UserName username);
}
