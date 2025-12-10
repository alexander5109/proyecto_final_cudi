using System.Text;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.Servicios;
using Clinica.Infrastructure.IRepositorios;
using Clinica.Infrastructure.Repositorios;
using Clinica.WebAPI.Controllers;
using Microsoft.IdentityModel.Tokens;
using static Clinica.WebAPI.Controllers.AuthMiddleware;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;
builder.Services.AddControllers();



//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SQLServerConnectionFactory>(sp =>
	new SQLServerConnectionFactory(
		builder.Configuration.GetConnectionString("ClinicaMedica")
			?? throw new InvalidOperationException("Connection string 'ClinicaMedica' not found")
	)
);

// Primero registrás las implementaciones
builder.Services.AddSingleton<RepositorioPacientes>();
builder.Services.AddSingleton<RepositorioTurnos>();
builder.Services.AddSingleton<RepositorioMedicos>();
builder.Services.AddSingleton<RepositorioUsuarios>();
builder.Services.AddSingleton<RepositorioDominioServices>();
builder.Services.AddSingleton<RepositorioHorarios>();

// Luego mapeás interfaces → implementación
builder.Services.AddSingleton<IRepositorioPacientes, RepositorioPacientes>();
builder.Services.AddSingleton<IRepositorioTurnos, RepositorioTurnos>();
builder.Services.AddSingleton<IRepositorioMedicos, RepositorioMedicos>();
builder.Services.AddSingleton<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddSingleton<IRepositorioDominioServices, RepositorioDominioServices>();
builder.Services.AddSingleton<IRepositorioHorarios, RepositorioHorarios>();

// Servicios públicos
builder.Services.AddSingleton<IServiciosDeDominio, ServiciosPublicos>();


// JwtService (singleton)
builder.Services.AddSingleton<JwtService>(sp => {
	// string? jwtKey = builder.Configuration["Jwt:Key"];
	// if (string.IsNullOrWhiteSpace(jwtKey))
	// throw new InvalidOperationException("Falta la clave JWT en configuración: 'Jwt:Key'");
	return new JwtService("ESTA_ES_UNA_LLAVE_DE_DESARROLLO_CAMBIAR_EN_PRODUCCION_123456789");
});


builder.Services.AddCors(options => {
	options.AddPolicy("AllowAll", policy => {
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

builder.Services.AddAuthentication("Bearer")

	.AddJwtBearer("Bearer", options => {
		options.TokenValidationParameters = new TokenValidationParameters {
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.ASCII.GetBytes("ESTA_ES_UNA_LLAVE_DE_DESARROLLO_CAMBIAR_EN_PRODUCCION_123456789")
			)
		};
		options.MapInboundClaims = false;
	});

builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddSwaggerGen(options => {
//	options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme {
//		Type = SecuritySchemeType.Http,
//		Scheme = "bearer",
//		BearerFormat = "JWT",
//		Description = "Ingrese 'Bearer <token>'"
//	});
//	options.AddSecurityRequirement(document => new OpenApiSecurityRequirement {
//		[new OpenApiSecuritySchemeReference("bearer", document)] = []
//	});
//});
//DESABILADO EN NET 8.0. TOTAL NO VOY A USAR SWAGGER AHORA.
builder.Services.AddSwaggerGen(options => {
	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {
		Title = "Clinica Web API",
		Version = "v1"
	});

	// 🔒 Definición de autenticación con JWT (Bearer)
	options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
		Description = "Ingrese el token JWT así: **Bearer {token}**",
		Name = "Authorization",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});

	// 🔑 Requerimiento global: Swagger enviará el token si lo cargás una vez
	options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
	{
		{
			new Microsoft.OpenApi.Models.OpenApiSecurityScheme
			{
				Reference = new Microsoft.OpenApi.Models.OpenApiReference
				{
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

WebApplication app = builder.Build();

//if (app.Environment.IsDevelopment()) {
//app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
//}

if (app.Environment.IsDevelopment() == false) {
	// NO llamar a app.UseHttpsRedirection();
} else {
	app.UseHttpsRedirection();
}



app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

Console.WriteLine("JWT Key cargada: " + "ESTA_ES_UNA_LLAVE_DE_DESARROLLO_CAMBIAR_EN_PRODUCCION_123456789");
Console.WriteLine("Environment: " + app.Environment.EnvironmentName);

app.Run();
