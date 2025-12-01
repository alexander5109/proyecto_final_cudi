using Clinica.Dominio.IRepositorios;
using Clinica.Infrastructure.DataAccess;
using Clinica.WebAPI.RouteConstraint;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

// SQLServerConnectionFactory (singleton)
builder.Services.AddSingleton<SQLServerConnectionFactory>(sp =>
	new SQLServerConnectionFactory(
		builder.Configuration.GetConnectionString("ClinicaMedica")
			?? throw new InvalidOperationException("Connection string 'ClinicaMedica' not found")
	)
);

// RepositorioDapper (singleton)
builder.Services.AddSingleton<RepositorioInterface>(sp => {
	var factory = sp.GetRequiredService<SQLServerConnectionFactory>();

	string? jwtKey = builder.Configuration["Jwt:Key"];
	if (string.IsNullOrWhiteSpace(jwtKey))
		throw new InvalidOperationException("Falta la clave JWT en configuración: 'Jwt:Key'");

	return new RepositorioDapper(factory, jwtKey);
});


builder.Services.Configure<RouteOptions>(options => {
	options.ConstraintMap["PacienteId"] = typeof(PacienteIdRouteConstraint);
	options.ConstraintMap["MedicoId"] = typeof(MedicoIdRouteConstraint);
	options.ConstraintMap["TurnoId"] = typeof(TurnoIdRouteConstraint);
});

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



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => {
		options.TokenValidationParameters = new TokenValidationParameters {
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
			)
		};
	});

builder.Services.AddAuthorization();

// ------------------------------------------------------------
// Build app
// ------------------------------------------------------------
WebApplication app = builder.Build();

// ------------------------------------------------------------
// 4. Middleware pipeline
// ------------------------------------------------------------
if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

Console.WriteLine("JWT Key cargada: " + builder.Configuration["Jwt:Key"]);

app.Run();
