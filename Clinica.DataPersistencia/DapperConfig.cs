//using Clinica.DataPersistencia.Mapeadores.TypeHandlers;
using Clinica.Dominio.Tipos;
using Dapper;
using static Dapper.SqlMapper;

namespace Clinica.DataPersistencia;

public static class DapperConfig {
	private static bool _initialized = false;

	public static void Initialize() {
		if (_initialized) return;
		_initialized = true;

		// 🔹 Registramos handlers personalizados
		//SqlMapper.AddTypeHandler(new ProvinciaTypeHandler());
		//SqlMapper.AddTypeHandler(new DniTypeHandler());
		//SqlMapper.AddTypeHandler(new TelefonoTypeHandler());
	}
}
