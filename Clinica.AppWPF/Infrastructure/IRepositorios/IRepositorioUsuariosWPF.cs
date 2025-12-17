using Clinica.Dominio.TiposDeAgregado;
using Clinica.Dominio.TiposDeEntidad;
using Clinica.Dominio.TiposDeEnum;
using Clinica.Dominio.TiposDeIdentificacion;
using static Clinica.Shared.DbModels.DbModels;

namespace Clinica.AppWPF.Infrastructure.IRepositorios;



public interface IRepositorioUsuariosWPF {
	Task<ResultWpf<UnitWpf>> DeleteUsuarioWhereId(UsuarioId2025 id);
	Task<ResultWpf<UsuarioId2025>> InsertUsuarioReturnId(Usuario2025 instance);
	Task<ResultWpf<UnitWpf>> UpdateUsuarioWhereId(Usuario2025EdicionAgg instance);
	Task<List<UsuarioDbModel>> SelectUsuarios();
	Task<UsuarioDbModel?> SelectUsuarioProfileWhereUsername(string username);
	Task<IReadOnlyCollection<AccionesDeUsuarioEnum>> SelectAccionesDeUsuarioWhereEnumRole(UsuarioRoleEnum enumRole);
	Task<IReadOnlyCollection<AccionesDeUsuarioEnum>> SelectAccionesDeUsuario();


}
