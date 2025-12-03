using System.Text;
using Clinica.Dominio.IRepositorios;
using Clinica.Infrastructure.DataAccess;
using Clinica.WebAPI.Servicios;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using static Clinica.Infrastructure.DataAccess.IRepositorioInterfaces;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;
builder.Services.AddControllers();



builder.Services.AddOpenApi();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SQLServerConnectionFactory>(sp =>
	new SQLServerConnectionFactory(
		builder.Configuration.GetConnectionString("ClinicaMedica")
			?? throw new InvalidOperationException("Connection string 'ClinicaMedica' not found")
	)
);

builder.Services.AddSingleton<RepositorioDapper>(sp => {
    var factory = sp.GetRequiredService<SQLServerConnectionFactory>();
    return new RepositorioDapper(factory);
});

// Registrar cada interfaz como alias de la instancia principal
builder.Services.AddSingleton<IRepositorio>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioPacientes>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioMedicos>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioTurnos>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioUsuarios>(sp => sp.GetRequiredService<RepositorioDapper>());
builder.Services.AddSingleton<IRepositorioDomain>(sp => sp.GetRequiredService<RepositorioDapper>());


// JwtService (singleton)
builder.Services.AddSingleton<JwtService>(sp => {
	string? jwtKey = builder.Configuration["Jwt:Key"];
	if (string.IsNullOrWhiteSpace(jwtKey))
		throw new InvalidOperationException("Falta la clave JWT en configuración: 'Jwt:Key'");
	return new JwtService(jwtKey);
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
				Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])
			)
		};
	});

builder.Services.AddAuthorization();


builder.Services.AddSwaggerGen(options => {
	options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme {
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Description = "Ingrese 'Bearer <token>'"
	});
	options.AddSecurityRequirement(document => new OpenApiSecurityRequirement {
		[new OpenApiSecuritySchemeReference("bearer", document)] = []
	});
});



WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UsuarioMiddleware>();

app.MapControllers();

Console.WriteLine("JWT Key cargada: " + builder.Configuration["Jwt:Key"]);

app.Run();
