using System.Text;
using Clinica.Dominio.IRepositorios;
using Clinica.Dominio.Servicios;
using Clinica.Infrastructure.DataAccess;
using Clinica.WebAPI.Servicios;
using Microsoft.IdentityModel.Tokens;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;


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

builder.Services.AddSingleton<RepositorioDapper>(sp => {
    SQLServerConnectionFactory factory = sp.GetRequiredService<SQLServerConnectionFactory>();
    return new RepositorioDapper(factory);
});

// Registrar cada interfaz como alias de la instancia principal
builder.Services.AddSingleton<IRepositorio>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioPacientes>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioMedicos>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioTurnos>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioUsuarios>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioDomainServiciosPrivados>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioHorarios>(sp => sp.GetRequiredService<RepositorioDapper>());



builder.Services.AddSingleton<IServiciosPublicos, ServiciosPublicos>();


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
app.UseMiddleware<UsuarioMiddleware>();

app.MapControllers();

Console.WriteLine("JWT Key cargada: " + "ESTA_ES_UNA_LLAVE_DE_DESARROLLO_CAMBIAR_EN_PRODUCCION_123456789");
Console.WriteLine("Environment: " + app.Environment.EnvironmentName);

app.Run();
