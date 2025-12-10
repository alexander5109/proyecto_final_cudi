using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.TiposDeValor;
using static Clinica.Shared.ApiDtos.UsuarioAuthDtos;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.Infrastructure.DataAccess;

public static partial class IRepositorioInterfaces {
    public interface IRepositorioUsuarios {
		Task<Result<Unit>> DeleteUsuarioWhereId(UsuarioId id);
		Task<Result<UsuarioId>> InsertUsuarioReturnId(Usuario2025 instance);
		Task<Result<IEnumerable<UsuarioDbModel>>> SelectUsuarios();
		Task<Result<UsuarioDbModel?>> SelectUsuarioWhereId(UsuarioId id);
		Task<Result<Unit>> UpdateUsuarioWhereId(UsuarioId id, Usuario2025 instance);
		Task<Result<UsuarioPerfilDto>> SelectUsuarioProfileWhereUsername(UserName username); //need domain entitiy because this is not really data to query, it's data that immediatly needs domain methods.
	}
}
