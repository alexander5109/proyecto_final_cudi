using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeIdentificacion;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.IRepositorios;

public interface IRepositorioUsuarios {
	Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId2025 id);
	Task<Result<UsuarioId2025>> InsertUsuarioReturnId(Usuario2025 instance);
	Task<Result<IEnumerable<UsuarioDbModel>>> SelectUsuarios();
	Task<Result<UsuarioDbModel?>> SelectUsuarioWhereId(UsuarioId2025 id);
	Task<Result<UsuarioDbModel>> SelectUsuarioProfileWhereUsername(UserName2025 username);
	Task<Result<UsuarioDbModel>> UpdateUsuarioWhereId(UsuarioId2025 id, Usuario2025Edicion instance);
}
