using Clinica.Infrastructure.Persistencia;
using Clinica.Infrastructure.ServiciosAsync;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1. Load configuration (appsettings.json + environment config)
// ------------------------------------------------------------
IConfiguration config = builder.Configuration;
// No necesitas nada más: builder.Configuration ya incluye:
// - appsettings.json
// - appsettings.{Environment}.json
// - secrets.json (si aplica)
// - variables de entorno
// ------------------------------------------------------------

// ------------------------------------------------------------
// 2. Add services to the container
// ------------------------------------------------------------
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------------------------------
// 3. Dependency Injection: Database + ServiciosPublicosAsync
// ------------------------------------------------------------

// SqlConnectionFactory (singleton)
builder.Services.AddSingleton<SqlConnectionFactory>(sp =>
	new SqlConnectionFactory(
		config.GetConnectionString("ClinicaMedica")
			?? throw new InvalidOperationException("Connection string 'ClinicaMedica' not found")
	)
);

// BaseDeDatosRepositorio (scoped)
builder.Services.AddScoped<BaseDeDatosRepositorio>();

// ServiciosPublicosAsync (scoped)
builder.Services.AddScoped<ServiciosPublicosAsync>();

// ------------------------------------------------------------
// (Optional) CORS – if you will call API from WPF or browser
// ------------------------------------------------------------
builder.Services.AddCors(options => {
	options.AddPolicy("AllowAll", policy => {
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

// ------------------------------------------------------------
// Build app
// ------------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------------
// 4. Middleware pipeline
// ------------------------------------------------------------
if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
