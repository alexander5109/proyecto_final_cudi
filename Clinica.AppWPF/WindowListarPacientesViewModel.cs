using System;
using System.Collections.Generic;
using System.Text;

namespace Clinica.AppWPF;

public record PacienteListDto(
	int Id,
	string Dni,
	string Nombre,
	string Apellido,
	string Email,
	string Telefono
);
public record TurnoListDto(
	int Id,
	TimeSpan Hora,
	DateTime Fecha,
	byte EspecialidadCodigoInterno,
	byte Estado,
	int MedicoId
);

public record MedicoListDto(
	string Dni,
	string Nombre,
	string Apellido,
	byte EspecialidadCodigoInterno
);
