# Solucion
dotnet new sln -n Clinica

# Create projects
dotnet new classlib -n Clinica.Dominio
dotnet new webapi -n Clinica.WebAPI
dotnet new blazorserver -n Clinica.BlazorServer
dotnet new sqlproj -n Clinica.Database
