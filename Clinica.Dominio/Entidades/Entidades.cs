using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Clinica.Dominio.FunctionalProgramingTools;
using static Clinica.Dominio.Entidades.Entidades;

namespace Clinica.Dominio.Entidades;

public static class HorarioExtensions {


	private static readonly TimeOnly InicioMañana = new(8, 0);
	private static readonly TimeOnly FinMañana = new(13, 0);

	private static readonly TimeOnly InicioTarde = new(13, 0);
	private static readonly TimeOnly FinTarde = new(18, 0);

	public static bool CoincideConPreferencia(
		this HorarioMedico2025 horario,
		TardeOMañana pref) {
		if (pref.Tarde)
			return horario.Desde.Valor >= InicioTarde &&
				   horario.Hasta.Valor <= FinTarde;

		return horario.Desde.Valor >= InicioMañana &&
			   horario.Hasta.Valor <= FinMañana;
	}
}

public static partial class Entidades {




	public static Dictionary<string, DayOfWeek> DiasDeLaSemanaToEnum = new(StringComparer.OrdinalIgnoreCase) {
		["domingo"] = DayOfWeek.Sunday,
		["lunes"] = DayOfWeek.Monday,
		["martes"] = DayOfWeek.Tuesday,
		["miercoles"] = DayOfWeek.Wednesday,
		["miércoles"] = DayOfWeek.Wednesday,
		["jueves"] = DayOfWeek.Thursday,
		["viernes"] = DayOfWeek.Friday,
		["sabado"] = DayOfWeek.Saturday,
		["sábado"] = DayOfWeek.Saturday,
		// Inglés
		["sunday"] = DayOfWeek.Sunday,
		["monday"] = DayOfWeek.Monday,
		["tuesday"] = DayOfWeek.Tuesday,
		["wednesday"] = DayOfWeek.Wednesday,
		["thursday"] = DayOfWeek.Thursday,
		["friday"] = DayOfWeek.Friday,
		["saturday"] = DayOfWeek.Saturday
	};
	public static string AEspañol(this DayOfWeek dia) => dia switch {
		DayOfWeek.Monday => "Lunes",
		DayOfWeek.Tuesday => "Martes",
		DayOfWeek.Wednesday => "Miércoles",
		DayOfWeek.Thursday => "Jueves",
		DayOfWeek.Friday => "Viernes",
		DayOfWeek.Saturday => "Sábado",
		DayOfWeek.Sunday => "Domingo",
		_ => throw new ArgumentOutOfRangeException(nameof(dia), dia, null)
	};
	public static string AString(this FechaDeNacimiento2025 fecha) => fecha.Valor.ToString("dd/MM/yyyy");
	public static string AString(this FechaIngreso2025 fecha) => fecha.Valor.ToString("dd/MM/yyyy");
	public static string AString(this ProvinciaArgentina2025 provincia) => provincia.Nombre;
	public static string AString(this DiaSemana2025 diaSemana) => CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetDayName(diaSemana.Valor);
	public static string AString(this Turno2025 turno) {
		var medicoTexto = turno.MedicoAsignado is null ? $"Especialidad {turno.Especialidad.Titulo}" : turno.MedicoAsignado.ToString();
		return $"Turno de {turno.Paciente.NombreCompleto} con {medicoTexto} el {turno.FechaYHora:g}";
	}

	public static string AString(this HorarioHora2025 hora) => hora.Valor.ToString("HH:mm");
	public static string AString(this ListaHorarioMedicos2025 horarios) => string.Join(", ", horarios.Valores.Select(h => h.ToString()));

	public static string AString(this MedicoSueldoMinimo2025 sueldo) => sueldo.Valor.ToString("C", CultureInfo.CurrentCulture);



	public static bool SeSolapaCon(this HorarioMedico2025 uno, HorarioMedico2025 otro)
		=> uno.DiaSemana == otro.DiaSemana
		&& uno.Desde.Valor < otro.Hasta.Valor
		&& otro.Desde.Valor < uno.Hasta.Valor;

	public static bool TienenDisponibilidad(this ListaHorarioMedicos2025 listaHorarios, DateTime fechaYHora, TimeSpan duracion) {
		if (listaHorarios.Valores is null || listaHorarios.Valores.Count == 0)
			return false;

		var diaSemana = DiaSemana2025.Crear(fechaYHora.DayOfWeek).GetOrRaise();
		var desde = new HorarioHora2025(TimeOnly.FromDateTime(fechaYHora));
		var hasta = new HorarioHora2025(desde.Valor.Add(duracion));

		// Hay disponibilidad si existe al menos un horario que cubra ese rango
		return listaHorarios.Valores.Any(h =>
			h.DiaSemana == diaSemana &&
			h.Desde.Valor <= desde.Valor &&
			h.Hasta.Valor >= hasta.Valor
		);
	}



	public enum TurnoEstado2025 {
		Programado,
		Reprogramado,
		Cancelado,
		Atendido
	}

	public record Turno2025(
		Medico2025 MedicoAsignado,
		Paciente2025 Paciente,
		EspecialidadMedica2025 Especialidad,
		DateTime FechaHoraDesde,
		DateTime FechaHoraHasta,
		TurnoEstado2025 Estado
	) {

		public static Result<Turno2025> Programar(
			SolicitudDeTurno solicitud,
			DisponibilidadEspecialidad2025 disp
		) {
			// --- Validación de los Result ----
			//if (solicitudResult is Result<SolicitudDeTurno>.Error solError)
			//	return new Result<Turno2025>.Error($"Error en solicitud: {solError.Mensaje}");

			//if (dispResult is Result<DisponibilidadEspecialidad2025>.Error dispError)
			//	return new Result<Turno2025>.Error($"Error en disponibilidad: {dispError.Mensaje}");

			//var solicitud = ((Result<SolicitudDeTurno>.Ok)solicitudResult).Valor;
			//var disp = ((Result<DisponibilidadEspecialidad2025>.Ok)dispResult).Valor;

			// --- Coherencias de dominio ---
			if (solicitud.Paciente is null)
				return new Result<Turno2025>.Error("La solicitud no tiene paciente.");

			if (solicitud.Especialidad.CodigoInterno != disp.Especialidad.CodigoInterno)
				return new Result<Turno2025>.Error("La disponibilidad no corresponde a la especialidad solicitada.");

			// El turno NO tiene por qué ser validado contra DateTime.Now.
			// Asumimos que la disponibilidad ya fue generada correctamente por el dominio.

			// --- Construcción del Turno ---
			var turno = new Turno2025(
				MedicoAsignado: disp.Medico,
				Paciente: solicitud.Paciente,
				Especialidad: disp.Especialidad,
				FechaHoraDesde: disp.FechaHoraDesde,
				FechaHoraHasta: disp.FechaHoraHasta,
				Estado: TurnoEstado2025.Programado
			);

			return new Result<Turno2025>.Ok(turno);
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


	public readonly record struct DisponibilidadEspecialidad2025(
		EspecialidadMedica2025 Especialidad,
		Medico2025 Medico,
		DateTime FechaHoraDesde,
		DateTime FechaHoraHasta
	);


	public record Medico2025(
		NombreCompleto2025 NombreCompleto,
		ListaEspecialidadesMedicas2025 Especialidades,
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
			Result<ListaEspecialidadesMedicas2025> especialidadResult,
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

}