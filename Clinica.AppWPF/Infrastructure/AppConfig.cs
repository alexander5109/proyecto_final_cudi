using System.IO;
using Microsoft.Extensions.Configuration;

namespace Clinica.AppWPF.Infrastructure;

public static class AppConfig {
	private static IConfigurationRoot? _config;

	public static IConfigurationRoot Config {
		get {
			if (_config is not null)
				return _config;

            IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			// Si existe un appsettings.Development.json, lo lee automáticamente
			if (File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.Development.json")))
				builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

			_config = builder.Build();
			return _config;
		}
	}
}