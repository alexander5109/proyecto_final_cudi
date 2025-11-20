using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using Clinica.Dominio.FunctionalProgramingTools;

namespace Clinica.Dominio.Entidades;

public enum TurnoEstado2025 {
	Programado,
	Reprogramado,
	Cancelado,
	Atendido
}

public record Turno2025(
	Medico2025? MedicoAsignado,
	Paciente2025 Paciente,
	DateTime FechaYHora,
	EspecialidadMedica2025 Especialidad,
	TurnoEstado2025 Estado
) {

	public static Result<Turno2025> Programar(Result<Medico2025> medicoResult, Result<Paciente2025> pacienteResult, Result<EspecialidadMedica2025> especialidadResult, DateTime? fechaYHora) {
		// Validar entradas
		if (medicoResult is Result<Medico2025>.Error medicoError)
			return new Result<Turno2025>.Error($"Error en médico: {medicoError.Mensaje}");

		if (pacienteResult is Result<Paciente2025>.Error pacienteError)
			return new Result<Turno2025>.Error($"Error en paciente: {pacienteError.Mensaje}");

		if (especialidadResult is Result<EspecialidadMedica2025>.Error espError)
			return new Result<Turno2025>.Error($"Error en especialidad: {espError.Mensaje}");

		var medico = ((Result<Medico2025>.Ok)medicoResult).Valor;
		var paciente = ((Result<Paciente2025>.Ok)pacienteResult).Valor;
		var especialidad = ((Result<EspecialidadMedica2025>.Ok)especialidadResult).Valor;

		if (fechaYHora is null)
			return new Result<Turno2025>.Error("La fecha y hora del turno es obligatoria.");

		if (fechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("El turno no puede ser en el pasado.");

		var duracion = TimeSpan.FromMinutes(especialidad.DuracionConsultaMinutos);
		if (duracion.TotalMinutes is < 5 or > 240)
			return new Result<Turno2025>.Error("La duración derivada de la especialidad no es razonable.");

		// Validar disponibilidad del médico asignado según sus horarios
		bool disponible = medico.ListaHorarios.TienenDisponibilidad(fechaYHora.Value, duracion);
		if (!disponible)
			return new Result<Turno2025>.Error("El médico no atiende en ese horario.");

		return new Result<Turno2025>.Ok(new Turno2025(medico, paciente, fechaYHora.Value, especialidad, TurnoEstado2025.Programado));
	}

	public Result<Turno2025> Cancelar(string? motivo = null) {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("El turno ya está cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("No se puede cancelar un turno ya atendido.");
		return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Cancelado });
	}

	public Result<Turno2025> MarcarComoAtendido() {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("No se puede marcar como atendido un turno cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("El turno ya está marcado como atendido.");
		if (FechaYHora > DateTime.Now.AddMinutes(15))
			return new Result<Turno2025>.Error("No se puede marcar como atendido antes de la hora programada (falta más de 15 minutos).");
		return new Result<Turno2025>.Ok(this with { Estado = TurnoEstado2025.Atendido });
	}

	public Result<Turno2025> Reprogramar(DateTime nuevaFechaYHora, IEnumerable<Turno2025>? otrosTurnosDelMedico = null) {
		if (Estado == TurnoEstado2025.Cancelado)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno cancelado.");
		if (Estado == TurnoEstado2025.Atendido)
			return new Result<Turno2025>.Error("No se puede reprogramar un turno ya atendido.");

		if (nuevaFechaYHora < DateTime.Now)
			return new Result<Turno2025>.Error("La nueva fecha no puede ser en el pasado.");

		var duracion = TimeSpan.FromMinutes(Especialidad.DuracionConsultaMinutos);
		if (duracion.TotalMinutes is < 5 or > 240)
			return new Result<Turno2025>.Error("La duración derivada de la especialidad no es razonable.");

		// Disponibilidad del médico asignado según sus horarios
		if (MedicoAsignado is null)
			return new Result<Turno2025>.Error("No hay médico asignado al turno para verificar disponibilidad.");

		if (!MedicoAsignado.ListaHorarios.TienenDisponibilidad(nuevaFechaYHora, duracion))
			return new Result<Turno2025>.Error("El médico no atiende en el nuevo horario.");

		// Verificar solapamiento con otros turnos del mismo médico
		if (otrosTurnosDelMedico is not null) {
			var comienzo = nuevaFechaYHora;
			var fin = nuevaFechaYHora.Add(duracion);
			foreach (var ot in otrosTurnosDelMedico) {
				if (ot.MedicoAsignado is null) continue;
				if (ot.MedicoAsignado.Dni == this.MedicoAsignado.Dni && ot.FechaYHora == this.FechaYHora)
					continue; // ignorar propio

				if (ot.MedicoAsignado.Dni == this.MedicoAsignado.Dni) {
					var otherStart = ot.FechaYHora;
					var otherEnd = ot.FechaYHora.Add(TimeSpan.FromMinutes(ot.Especialidad.DuracionConsultaMinutos));
					if (comienzo < otherEnd && otherStart < fin)
						return new Result<Turno2025>.Error("El nuevo horario se solapa con otro turno del médico.");
				}
			}
		}

		return new Result<Turno2025>.Ok(this with { FechaYHora = nuevaFechaYHora, Estado = TurnoEstado2025.Reprogramado });
	}

}

public record DisponibilidadSlot(DateTime FechaHora, int? MedicoId, string MedicoDisplay);


public record DisponibilidadEspecialidad2025(
	EspecialidadMedica2025 Especialidad,
	TimeSpan DuracionConsulta,
	IReadOnlyList<DisponibilidadDia2025> DiasDisponibles
);


public readonly record struct EspecialidadCodigoInterno(int Valor);

public sealed class EspecialidadMedica2025 {
	// ============================
	//  Fields / Properties
	// ============================
	public EspecialidadCodigoInterno CodigoInterno { get; }
	public string Titulo { get; }
	public int DuracionConsultaMinutos { get; }

	public static List<EspecialidadMedica2025> TodasLasSoportadas => [.. PorId.Values];

	private EspecialidadMedica2025(
		EspecialidadCodigoInterno codigoInterno,
		string titulo,
		int duracionMin
	) {
		CodigoInterno = codigoInterno;
		Titulo = titulo;
		DuracionConsultaMinutos = duracionMin;
	}

	// ============================
	//  Static Data (Domain Truth)
	// ============================

	public static readonly string[] Titulos = [
		"Clínico General",
		"Cardiólogo",
		"Oftalmólogo",
		"Otorrinolaringólogo",
		"Psiquiatra",
		"Psicólogo",
		"Cirujano",
		"Kinesiólogo",
		"Nutricionista",
		"Gastroenterólogo",
		"Osteópata",
		"Proctólogo",
		"Pediatra",
		"Ginecólogo",
		"Traumatólogo",
		"Neurólogo",
		"Dermatólogo"
	];

	private static readonly Dictionary<string, int> DuracionesPorEspecialidad =
		new(StringComparer.OrdinalIgnoreCase) {
			["Clínico General"] = 30,
			["Cardiólogo"] = 40,
			["Oftalmólogo"] = 20,
			["Otorrinolaringólogo"] = 25,
			["Psiquiatra"] = 50,
			["Psicólogo"] = 50,
			["Cirujano"] = 60,
			["Kinesiólogo"] = 30,
			["Nutricionista"] = 30,
			["Gastroenterólogo"] = 40,
			["Osteópata"] = 30,
			["Proctólogo"] = 30,
			["Pediatra"] = 25,
			["Ginecólogo"] = 35,
			["Traumatólogo"] = 40,
			["Neurólogo"] = 45,
			["Dermatólogo"] = 20
		};


	// ============================================
	//  Bidirectional Dictionary (ID <-> Especialidad)
	// ============================================

	// Domain ID starts from 1…N, fixed permanently.
	private static readonly ImmutableDictionary<EspecialidadCodigoInterno, EspecialidadMedica2025> _fromId;
	private static readonly ImmutableDictionary<string, EspecialidadMedica2025> _fromTitulo;

	public static IReadOnlyDictionary<EspecialidadCodigoInterno, EspecialidadMedica2025> PorId => _fromId;
	public static IReadOnlyDictionary<string, EspecialidadMedica2025> PorTitulo => _fromTitulo;

	static EspecialidadMedica2025() {
		var byIdBuilder = ImmutableDictionary.CreateBuilder<EspecialidadCodigoInterno, EspecialidadMedica2025>();
		var byTituloBuilder = ImmutableDictionary.CreateBuilder<string, EspecialidadMedica2025>(StringComparer.OrdinalIgnoreCase);

		int idCounter = 1;
		foreach (var titulo in Titulos) {
			int dur = DuracionesPorEspecialidad.TryGetValue(titulo, out var x) ? x : 30;

			var esp = new EspecialidadMedica2025(
				new EspecialidadCodigoInterno(idCounter),
				titulo,
				dur
			);

			byIdBuilder.Add(esp.CodigoInterno, esp);
			byTituloBuilder.Add(esp.Titulo, esp);

			idCounter++;
		}

		_fromId = byIdBuilder.ToImmutable();
		_fromTitulo = byTituloBuilder.ToImmutable();
	}

	// ======================================================
	// Public API: Creation (Lookup only, no dynamic creation)
	// ======================================================

	public static Result<EspecialidadMedica2025> CrearPorTitulo(string? titulo) {
		if (string.IsNullOrWhiteSpace(titulo))
			return new Result<EspecialidadMedica2025>.Error("El título no puede estar vacío.");

		if (_fromTitulo.TryGetValue(titulo.Trim(), out var esp))
			return new Result<EspecialidadMedica2025>.Ok(esp);

		return new Result<EspecialidadMedica2025>.Error($"La especialidad '{titulo}' no está soportada por el dominio.");
	}

	public static Result<EspecialidadMedica2025> CrearPorCodigoInterno(int? id) {
		if (id is null)
			return new Result<EspecialidadMedica2025>.Error("El CodigoInterno no puede ser nulo.");
		var key = new EspecialidadCodigoInterno((int)id);

		if (_fromId.TryGetValue(key, out var esp))
			return new Result<EspecialidadMedica2025>.Ok(esp);

		return new Result<EspecialidadMedica2025>.Error($"No existe la especialidad con CodigoInterno = {id}.");
	}

	// Convenience factory to keep older tests/usage working
	public static Result<EspecialidadMedica2025> Crear(string? titulo) => CrearPorTitulo(titulo);

	// ======================================================
	// Utility
	// ======================================================

	public override string ToString() => $"{Titulo} ({DuracionConsultaMinutos} min)";
}

public record Medico2025(
	NombreCompleto2025 NombreCompleto,
	EspecialidadMedica2025 Especialidad,
	DniArgentino2025 Dni,
	DomicilioArgentino2025 Domicilio,
	ContactoTelefono2025 Telefono,
	ListaHorarioMedicos2025 ListaHorarios,
	FechaIngreso2025 FechaIngreso,
	MedicoSueldoMinimo2025 SueldoMinimoGarantizado,
	bool HaceGuardias
) {
	public static Result<Medico2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<EspecialidadMedica2025> especialidadResult,
		Result<DniArgentino2025> dniResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<ContactoTelefono2025> telefonoResult,
		Result<ListaHorarioMedicos2025> horariosResult,
		Result<FechaIngreso2025> fechaIngresoResult,
		Result<MedicoSueldoMinimo2025> sueldoResult,
		bool haceGuardia
	) =>
		from nombre in nombreResult
		from esp in especialidadResult
		from dni in dniResult
		from dom in domicilioResult
		from tel in telefonoResult
		from horarios in horariosResult
		from fechaIng in fechaIngresoResult
		from sueldo in sueldoResult
		select new Medico2025(
			nombre,
			esp,
			dni,
			dom,
			tel,
			horarios,
			fechaIng,
			sueldo,
			haceGuardia
		);
}
public record Paciente2025(
	NombreCompleto2025 NombreCompleto,
	DniArgentino2025 Dni,
	Contacto2025 Contacto,
	DomicilioArgentino2025 Domicilio,
	FechaDeNacimiento2025 FechaNacimiento,
	FechaIngreso2025 FechaIngreso
) {
	public static Result<Paciente2025> Crear(
		Result<NombreCompleto2025> nombreResult,
		Result<DniArgentino2025> dniResult,
		Result<Contacto2025> contactoResult,
		Result<DomicilioArgentino2025> domicilioResult,
		Result<FechaDeNacimiento2025> fechaNacimientoResult,
		Result<FechaIngreso2025> fechaIngresoResult
	)
	=>
		from nombre in nombreResult
		from dni in dniResult
		from contacto in contactoResult
		from domicilio in domicilioResult
		from fechaNac in fechaNacimientoResult
		from fechaIng in fechaIngresoResult
		select new Paciente2025(
			nombre,
			dni,
			contacto,
			domicilio,
			fechaNac,
			fechaIng
		);
}

public record struct Contacto2025(
	ContactoEmail2025 Email,
	ContactoTelefono2025 Telefono
) {
	public static Result<Contacto2025> Crear(Result<ContactoEmail2025> emailResult, Result<ContactoTelefono2025> telResult)
		=> emailResult.Bind(emailOk => telResult.Map(telOk => new Contacto2025(emailOk, telOk)));
}



public readonly record struct ContactoTelefono2025(
	string Valor
) {
	public static Result<ContactoTelefono2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoTelefono2025>.Error("El teléfono no puede estar vacío.");
		if (!Regex.IsMatch(input, @"^\+?\d{6,15}$"))
			return new Result<ContactoTelefono2025>.Error("Teléfono inválido.");

		return new Result<ContactoTelefono2025>.Ok(new ContactoTelefono2025(input.Trim()));
	}
}


public readonly record struct DisponibilidadDia2025(
	DateTime Fecha,
	IReadOnlyList<FranjaDisponible2025> Franjas
);


public readonly record struct DiaSemana2025(
	DayOfWeek Valor
//, string NombreDia
) {

	public static Result<DiaSemana2025> Crear(DayOfWeek input) {
		return new Result<DiaSemana2025>.Ok(new DiaSemana2025(input));
	}

	public static Result<DiaSemana2025> Crear(DiaSemana2025 input) {
		return new Result<DiaSemana2025>.Ok(input);
	}
	public static Result<DiaSemana2025> Crear(int input) {
		if (input < 0 || input > 6)
			return new Result<DiaSemana2025>.Error("El número del día de la semana debe estar entre 0 (domingo) y 6 (sábado).");
		return new Result<DiaSemana2025>.Ok(new DiaSemana2025((DayOfWeek)input));
	}
	public static Result<DiaSemana2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DiaSemana2025>.Error("El nombre del día no puede estar vacío.");
		string normalized = input.Trim().ToLowerInvariant();

		if (Extensiones.DiasDeLaSemanaToEnum.TryGetValue(normalized, out var day))
			return new Result<DiaSemana2025>.Ok(new DiaSemana2025(day));

		// Intento adicional: cultura
		try {
			var culture = new CultureInfo("es-ES");
			for (int i = 0; i < 7; i++) {
				if (culture.DateTimeFormat.GetDayName((DayOfWeek)i).Equals(normalized, StringComparison.InvariantCultureIgnoreCase))
					return new Result<DiaSemana2025>.Ok(new DiaSemana2025((DayOfWeek)i));
			}
		} catch { }

		return new Result<DiaSemana2025>.Error($"'{input}' no corresponde a un día válido.");
	}

	public static readonly DiaSemana2025[] Los7DiaSemana2025 = [
		new(DayOfWeek.Monday), //Value 1
		new(DayOfWeek.Tuesday), //Value 2
		new(DayOfWeek.Wednesday),//Value 3
		new(DayOfWeek.Thursday),//Value 4
		new(DayOfWeek.Friday),//Value 5
		new(DayOfWeek.Saturday), //Value 6
		new(DayOfWeek.Sunday), //Value 0
	];

	public static readonly string[] Los7StringDias = [
		DayOfWeek.Monday.AEspañol(), //Value 1
		DayOfWeek.Tuesday.AEspañol(), //Value 2
		DayOfWeek.Wednesday.AEspañol(),//Value 3
		DayOfWeek.Thursday.AEspañol(),//Value 4
		DayOfWeek.Friday.AEspañol(),//Value 5
		DayOfWeek.Saturday.AEspañol(), //Value 6
		DayOfWeek.Sunday.AEspañol(), //Value 0
	];

	public static readonly DayOfWeek[] Los7EnumDias = [
		DayOfWeek.Monday, //Value 1
		DayOfWeek.Tuesday, //Value 2
		DayOfWeek.Wednesday,//Value 3
		DayOfWeek.Thursday,//Value 4
		DayOfWeek.Friday,//Value 5
		DayOfWeek.Saturday, //Value 6
		DayOfWeek.Sunday, //Value 0
	];
}
public readonly record struct DniArgentino2025(
	string Valor
) {
	public static Result<DniArgentino2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<DniArgentino2025>.Error("El DNI no puede estar vacío.");
		var normalized = input.Trim();
		if (!Regex.IsMatch(normalized, @"^\d{1,8}$"))
			return new Result<DniArgentino2025>.Error("El DNI debe contener hasta 8 dígitos numéricos.");
		return new Result<DniArgentino2025>.Ok(new DniArgentino2025(normalized));
	}
}

public readonly record struct DomicilioArgentino2025(
	LocalidadDeProvincia2025 Localidad,
	string Direccion
) {
	public static Result<DomicilioArgentino2025> Crear(Result<LocalidadDeProvincia2025> localidadResult, string? direccionTexto) {
		if (string.IsNullOrWhiteSpace(direccionTexto))
			return new Result<DomicilioArgentino2025>.Error("La dirección no puede estar vacía");


		if (localidadResult is Result<LocalidadDeProvincia2025>.Error localidadError)
			return new Result<DomicilioArgentino2025>.Error(localidadError.Mensaje);

		var localidad = ((Result<LocalidadDeProvincia2025>.Ok)localidadResult).Valor;

		return new Result<DomicilioArgentino2025>.Ok(
			new DomicilioArgentino2025(
				localidad,
				Normalize(direccionTexto)
			)
		);
	}
	public static string Normalize(string value) => value.Trim();
}

public readonly record struct FechaDeNacimiento2025(
	DateOnly Valor
) {
	public static readonly DateOnly Hoy = DateOnly.FromDateTime(DateTime.Now);

	public static Result<FechaDeNacimiento2025> Crear(DateOnly fecha) {
		if (fecha > Hoy)
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede ser futura.");
		if (fecha < Hoy.AddYears(-120))
			return new Result<FechaDeNacimiento2025>.Error("Edad no válida (más de 120 años).");

		return new Result<FechaDeNacimiento2025>.Ok(new(fecha));
	}

	public static Result<FechaDeNacimiento2025> Crear(DateTime? fecha) {
		if (fecha is null) {
			return new Result<FechaDeNacimiento2025>.Error("La fecha de ingreso no puede estar vacía.");
		}
		DateOnly dateOnly = DateOnly.FromDateTime(fecha.Value);
		return Crear(dateOnly);
	}

	public static Result<FechaDeNacimiento2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaDeNacimiento2025>.Error("La fecha de nacimiento no puede estar vacía.");

		// Soportar varios formatos razonables
		string[] formatos = [
			"dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy", "M/d/yyyy", "dd-MM-yyyy"
		];

		if (DateTime.TryParseExact(input.Trim(), formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
			return Crear(dt);

		if (DateTime.TryParse(input.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt2))
			return Crear(dt2);

		return new Result<FechaDeNacimiento2025>.Error("Formato de fecha inválido.");
	}

	// --- Edad aproximada ---
	public static int Edad(FechaDeNacimiento2025 fecha) {
		var hoy = DateOnly.FromDateTime(DateTime.Now);
		int edad = hoy.Year - fecha.Valor.Year;
		if (hoy < fecha.Valor.AddYears(edad)) edad--;
		return edad;
	}

}



public readonly record struct FechaIngreso2025(DateTime Valor) {
	public static readonly DateTime Ahora = DateTime.Now;

	// ==============================
	// -------- Factories ----------
	// ==============================

	public static Result<FechaIngreso2025> Crear(DateTime fecha) {
		// VALIDACIONES DE DOMINIO
		if (fecha > Ahora)
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede ser futura.");

		if (fecha < Ahora.AddYears(-30))
			return new Result<FechaIngreso2025>.Error("Hace 30 años no existía la clínica.");

		return new Result<FechaIngreso2025>.Ok(new FechaIngreso2025(fecha));
	}

	public static Result<FechaIngreso2025> Crear(DateTime? fecha) {
		if (fecha is null)
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede estar vacía.");

		return Crear(fecha.Value);
	}

	// Compatibilidad: DateOnly → DateTime
	public static Result<FechaIngreso2025> Crear(DateOnly fecha)
		=> Crear(fecha.ToDateTime(TimeOnly.MinValue));

	public static Result<FechaIngreso2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<FechaIngreso2025>.Error("La fecha de ingreso no puede estar vacía.");

		input = input.Trim();

		// Formatos razonables de fecha y fecha+hora
		string[] formatos =
		{
			"dd/MM/yyyy",
			"dd/MM/yyyy HH:mm",
			"dd/MM/yyyy HH:mm:ss",

			"yyyy-MM-dd",
			"yyyy-MM-dd HH:mm",
			"yyyy-MM-dd HH:mm:ss",
			"yyyy-MM-ddTHH:mm",
			"yyyy-MM-ddTHH:mm:ss",

			"d/M/yyyy",
			"d/M/yyyy HH:mm",
			"M/d/yyyy",
			"M/d/yyyy HH:mm"
		};

		// Intento 1: parseo exacto
		if (DateTime.TryParseExact(
				input,
				formatos,
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out var dtExact)) {
			return Crear(dtExact);
		}

		// Intento 2: parseo flexible según cultura del usuario
		if (DateTime.TryParse(
				input,
				CultureInfo.CurrentCulture,
				DateTimeStyles.AssumeLocal,
				out var dtCulture)) {
			return Crear(dtCulture);
		}

		return new Result<FechaIngreso2025>.Error("Formato de fecha inválido.");
	}

	// ==============================
	// -------- Helpers ------------
	// ==============================

	public override string ToString()
		=> Valor.ToString("yyyy-MM-dd HH:mm:ss");
}

public readonly record struct HorarioHora2025(
	TimeOnly Valor
) {
	public static Result<HorarioHora2025> Crear(string input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<HorarioHora2025>.Error("La hora no puede estar vacía.");

		if (TimeOnly.TryParseExact(
				input.Trim(),
				new[] { "HH:mm", "H:mm" },
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out var time))
			return new Result<HorarioHora2025>.Ok(new HorarioHora2025(time));

		return new Result<HorarioHora2025>.Error($"'{input}' no es una hora válida.");
	}

	public static Result<HorarioHora2025> Crear(TimeOnly value)
		=> new Result<HorarioHora2025>.Ok(new HorarioHora2025(value));

}
public readonly record struct FranjaDisponible2025(
	TimeOnly Desde,
	TimeOnly Hasta,
	int TurnosPosibles
);

public readonly record struct ListaHorarioMedicos2025(
	IReadOnlyList<HorarioMedico2025> Valores
) {
	public static Result<ListaHorarioMedicos2025> Crear(IReadOnlyList<Result<HorarioMedico2025>> horariosResult)
		=> horariosResult.Bind(horariosOk =>
			new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(horariosOk))
		);
	// ✅ 1. Factory desde lista de HorarioMedico2025 ya válidos
	public static Result<ListaHorarioMedicos2025> Crear(
		IEnumerable<HorarioMedico2025> valores) {
		if (valores is null)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de horarios no puede ser nula.");

		return new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(valores.ToList()));
	}

	// ✅ 2. Factory desde lista de Result<HorarioMedico2025>
	public static Result<ListaHorarioMedicos2025> Crear(
		IEnumerable<Result<HorarioMedico2025>> resultados) {
		if (resultados is null)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de resultados no puede ser nula.");

		var errores = resultados.OfType<Result<HorarioMedico2025>.Error>().ToList();
		if (errores.Count != 0) {
			var mensaje = string.Join(" | ", errores.Select(e => e.Mensaje));
			return new Result<ListaHorarioMedicos2025>.Error($"Errores en horarios: {mensaje}");
		}

		var valoresOk = resultados
			.OfType<Result<HorarioMedico2025>.Ok>()
			.Select(r => r.Valor)
			.ToList();

		return new Result<ListaHorarioMedicos2025>.Ok(new ListaHorarioMedicos2025(valoresOk));
	}

	// ✅ 3. Factory “desde strings” (útil para CSV, test o carga directa)
	public static Result<ListaHorarioMedicos2025> Crear(
		IEnumerable<(string Dia, string Desde, string Hasta)> entradas) {
		if (entradas is null)
			return new Result<ListaHorarioMedicos2025>.Error("La lista de entradas no puede ser nula.");

		var resultados = entradas
			.Select(e => HorarioMedico2025.Crear(e.Dia, e.Desde, e.Hasta))
			.ToList();

		return Crear(resultados); // Reutiliza la factory #2
	}

	// ✅ 4. Versión vacía explícita
	public static Result<ListaHorarioMedicos2025> CrearVacia()
		=> new Result<ListaHorarioMedicos2025>.Ok(
			new ListaHorarioMedicos2025([])
		);
}

public readonly record struct HorarioMedico2025(
	DiaSemana2025 DiaSemana,
	HorarioHora2025 Desde,
	HorarioHora2025 Hasta
) {
	// ✅ Versión simple (usa tipos ya validados)
	public static Result<HorarioMedico2025> Crear(
		DiaSemana2025 dia,
		HorarioHora2025 desde,
		HorarioHora2025 hasta) {
		if (desde.Valor >= hasta.Valor)
			return new Result<HorarioMedico2025>.Error("La hora de inicio debe ser anterior a la hora de fin.");

		return new Result<HorarioMedico2025>.Ok(new HorarioMedico2025(dia, desde, hasta));
	}

	// ✅ Versión que toma los Result<SubTipo> — ideal para los mappers
	public static Result<HorarioMedico2025> Crear(
		Result<DiaSemana2025> diaResult,
		Result<HorarioHora2025> desdeResult,
		Result<HorarioHora2025> hastaResult)
		=> diaResult.Bind(diaOk =>
		   desdeResult.Bind(desdeOk =>
		   hastaResult.Bind(hastaOk =>
			   Crear(diaOk, desdeOk, hastaOk))));

	// ✅ Versión “desde strings” — útil para tests, carga desde BD, o CSV
	public static Result<HorarioMedico2025> Crear(string dia, string desde, string hasta)
		=> DiaSemana2025.Crear(dia).Bind(diaOk =>
		   HorarioHora2025.Crear(desde).Bind(desdeOk =>
		   HorarioHora2025.Crear(hasta).Bind(hastaOk =>
			   Crear(diaOk, desdeOk, hastaOk))));
}


public readonly record struct LocalidadDeProvincia2025(
	string Nombre,
	ProvinciaArgentina2025 Provincia
) {
	public static Result<LocalidadDeProvincia2025> Crear(string? nombreLocalidad, Result<ProvinciaArgentina2025> provinciaResult) {
		if (string.IsNullOrWhiteSpace(nombreLocalidad))
			return new Result<LocalidadDeProvincia2025>.Error("El nombre de la localidad no puede estar vacío.");

		if (provinciaResult is Result<ProvinciaArgentina2025>.Error err)
			return new Result<LocalidadDeProvincia2025>.Error($"Provincia inválida: {err.Mensaje}");

		var provincia = ((Result<ProvinciaArgentina2025>.Ok)provinciaResult).Valor;

		return new Result<LocalidadDeProvincia2025>.Ok(new(nombreLocalidad, provincia));
	}
	public static string Normalize(string nombreLocalidad) => char.ToUpper(nombreLocalidad[0]) + nombreLocalidad[1..].ToLower();
}


public readonly record struct MedicoSueldoMinimo2025(
	double Valor
) {
	public const double MINIMO = 200_000;
	public const double MAXIMO = 5_000_000;
	public static Result<MedicoSueldoMinimo2025> Crear(double? input) {
		return input switch {
			null => new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede estar vacío."),
			< 0 => new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede ser negativo."),
			< MINIMO => new Result<MedicoSueldoMinimo2025>.Error($"El sueldo mínimo razonable es {MINIMO:N0}."),
			> MAXIMO => new Result<MedicoSueldoMinimo2025>.Error($"El sueldo ingresado ({input}) es excesivamente alto."),
			_ => new Result<MedicoSueldoMinimo2025>.Ok(new MedicoSueldoMinimo2025(input.Value))
		};
	}
	public static Result<MedicoSueldoMinimo2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede estar vacío.");

		var normalized = input.Trim();

		if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor) &&
			!double.TryParse(normalized, NumberStyles.Float, CultureInfo.CurrentCulture, out valor)) {
			return new Result<MedicoSueldoMinimo2025>.Error($"Valor inválido: '{input}'. Debe ser un número.");
		}

		if (valor < 0)
			return new Result<MedicoSueldoMinimo2025>.Error("El sueldo no puede ser negativo.");

		if (valor < MINIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo mínimo razonable es {MINIMO.ToString("N0")}.");

		if (valor > MAXIMO)
			return new Result<MedicoSueldoMinimo2025>.Error($"El sueldo ingresado ({valor.ToString("N0")}) es excesivamente alto.");

		return new Result<MedicoSueldoMinimo2025>.Ok(new MedicoSueldoMinimo2025(valor));
	}

}

public readonly record struct NombreCompleto2025(
	string Nombre,
	string Apellido
) {
	static readonly int MaxLongitud = 100; // razonable, pero configurable
	public static Result<NombreCompleto2025> Crear(string? nombre, string? apellido) {
		if (string.IsNullOrWhiteSpace(nombre))
			return new Result<NombreCompleto2025>.Error("El nombre no puede estar vacío.");

		if (string.IsNullOrWhiteSpace(apellido))
			return new Result<NombreCompleto2025>.Error("El apellido no puede estar vacío.");

		string nombreNorm = Normalize(nombre);
		string apellidoNorm = Normalize(apellido);

		if (nombreNorm.Length > MaxLongitud)
			return new Result<NombreCompleto2025>.Error($"El nombre es demasiado largo (máximo {MaxLongitud} caracteres).");

		if (apellidoNorm.Length > MaxLongitud)
			return new Result<NombreCompleto2025>.Error($"El apellido es demasiado largo (máximo {MaxLongitud} caracteres).");

		return new Result<NombreCompleto2025>.Ok(new(nombreNorm, apellidoNorm));
	}

	public static string Normalize(string input) => input.Trim(); //evneutalmente podria capitalizar palabras
}

public readonly record struct ProvinciaArgentina2025(
	string Nombre
) {
	public static readonly HashSet<string> _provinciasValidas =
		new(StringComparer.OrdinalIgnoreCase){
			"Buenos Aires",
			"Ciudad Autónoma de Buenos Aires",
			"Catamarca",
			"Chaco",
			"Chubut",
			"Córdoba",
			"Corrientes",
			"Entre Ríos",
			"Formosa",
			"Jujuy",
			"La Pampa",
			"La Rioja",
			"Mendoza",
			"Misiones",
			"Neuquén",
			"Río Negro",
			"Salta",
			"San Juan",
			"San Luis",
			"Santa Cruz",
			"Santa Fe",
			"Santiago del Estero",
			"Tierra del Fuego",
			"Tucumán"
		};

	public static Result<ProvinciaArgentina2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ProvinciaArgentina2025>.Error("La provincia no puede estar vacía.");

		string normalizado = Normalizar(input);

		if (!_provinciasValidas.Contains(normalizado))
			return new Result<ProvinciaArgentina2025>.Error($"Provincia inválida: '{input}'.");

		return new Result<ProvinciaArgentina2025>.Ok(new ProvinciaArgentina2025(normalizado));
	}

	public static IReadOnlyCollection<string> ProvinciasValidas() => _provinciasValidas.ToList().AsReadOnly();
	public static string Normalizar(string content)
		=> content.Trim();
	public static bool EsValida(string input) => _provinciasValidas.Contains(Normalizar(input));
}



public readonly record struct SolicitudDeTurno(
	Paciente2025 Paciente,
	EspecialidadMedica2025 Especialidad,
	DateTime FechaDeseada
) {
	public static Result<SolicitudDeTurno> Crear(Result<Paciente2025> pacienteResult, Result<EspecialidadMedica2025> especialidadResult, DateTime? fechaDeseada) {
		if (pacienteResult is Result<Paciente2025>.Error pErr)
			return new Result<SolicitudDeTurno>.Error(pErr.Mensaje);
		if (especialidadResult is Result<EspecialidadMedica2025>.Error eErr)
			return new Result<SolicitudDeTurno>.Error(eErr.Mensaje);
		if (fechaDeseada is null)
			return new Result<SolicitudDeTurno>.Error("La fecha deseada es obligatoria.");

		var paciente = ((Result<Paciente2025>.Ok)pacienteResult).Valor;
		var especialidad = ((Result<EspecialidadMedica2025>.Ok)especialidadResult).Valor;

		if (fechaDeseada.Value < DateTime.Now.Date)
			return new Result<SolicitudDeTurno>.Error("La fecha deseada no puede ser en el pasado.");

		return new Result<SolicitudDeTurno>.Ok(new SolicitudDeTurno(paciente, especialidad, fechaDeseada.Value));
	}
}

public readonly record struct ContactoEmail2025(
	string Valor
) {
	public static Result<ContactoEmail2025> Crear(string? input) {
		if (string.IsNullOrWhiteSpace(input))
			return new Result<ContactoEmail2025>.Error("El correo no puede estar vacío.");

		if (!Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			return new Result<ContactoEmail2025>.Error("Correo electrónico inválido.");

		return new Result<ContactoEmail2025>.Ok(new(input.Trim()));
	}
}
