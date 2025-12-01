using System.Text;
using Clinica.Dominio.IRepositorios;
using Clinica.Infrastructure.DataAccess;
using Clinica.WebAPI.Controllers;
using Clinica.WebAPI.RouteConstraint;
using Clinica.WebAPI.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SQLServerConnectionFactory>(sp =>
	new SQLServerConnectionFactory(
		builder.Configuration.GetConnectionString("ClinicaMedica")
			?? throw new InvalidOperationException("Connection string 'ClinicaMedica' not found")
	)
);

// RepositorioDapper (singleton)
builder.Services.AddSingleton<RepositorioInterface>(sp => {
	var factory = sp.GetRequiredService<SQLServerConnectionFactory>();
	return new RepositorioDapper(factory);
});


// JwtService (singleton)
builder.Services.AddSingleton<JwtService>(sp => {
	string? jwtKey = builder.Configuration["Jwt:Key"];
	if (string.IsNullOrWhiteSpace(jwtKey))
		throw new InvalidOperationException("Falta la clave JWT en configuración: 'Jwt:Key'");
	return new JwtService(jwtKey);
});



builder.Services.Configure<RouteOptions>(options => {
	options.ConstraintMap["PacienteId"] = typeof(PacienteIdRouteConstraint);
	options.ConstraintMap["MedicoId"] = typeof(MedicoIdRouteConstraint);
	options.ConstraintMap["TurnoId"] = typeof(TurnoIdRouteConstraint);
	options.ConstraintMap["UsuarioId"] = typeof(UsuarioIdRouteConstraint);
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




builder.Services.AddSwaggerGen(c => {
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Ingrese 'Bearer <token>'"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
	{
		new OpenApiSecurityScheme {
			Reference = new OpenApiReference {
				Type = ReferenceType.SecurityScheme,
				Id = "Bearer"
			}
		},
		Array.Empty<string>()
	}});
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
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
