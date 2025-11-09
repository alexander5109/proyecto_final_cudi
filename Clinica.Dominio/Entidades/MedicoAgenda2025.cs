using Clinica.Dominio.Comun;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Clinica.Dominio.Tipos;

public readonly record struct MedicoAgenda2025(
	IReadOnlyList<MedicoDisponibilidadEnDia2025> DisponibilidadEnDia
) {
	public static Result<MedicoAgenda2025> Crear(IEnumerable<MedicoDisponibilidadEnDia2025> disponibilidades) {
		var list = disponibilidades.ToList();
		if (list.Count == 0)
			return new Result<MedicoAgenda2025>.Error("Debe especificar al menos una disponibilidad.");
		return new Result<MedicoAgenda2025>.Ok(new(list));
	}


	public bool EstaDisponibleEn(DateTime fechaYHora, TimeSpan duracion) {
		var dia = new MedicoDiaDeLaSemana2025(fechaYHora.DayOfWeek);
		var hora = TimeOnly.FromDateTime(fechaYHora);

		// Buscar si el médico trabaja ese día
		var diaAgenda = DisponibilidadEnDia
			.FirstOrDefault(d => d.DiaSemana.Value == dia.Value);

		if (diaAgenda.FranjasHorarias is null || diaAgenda.FranjasHorarias.Count == 0)
			return false;

		// Verificar si alguna franja cubre el rango solicitado
		foreach (var franja in diaAgenda.FranjasHorarias) {
			if (hora >= franja.Desde && hora.Add(duracion) <= franja.Hasta)
				return true;
		}

		return false;
	}







	// -------------------------------
	// 🔽 Serialización
	// -------------------------------
	public string ToJson(bool indented = false) {
		var options = new JsonSerializerOptions {
			WriteIndented = indented,
			Converters = { new JsonStringEnumConverter() }
		};
		return JsonSerializer.Serialize(this, options);
	}

	// -------------------------------
	// 🔼 Deserialización
	// -------------------------------
	public static Result<MedicoAgenda2025> FromJson(string json) {
		try {
			var options = new JsonSerializerOptions {
				Converters = { new JsonStringEnumConverter() }
			};

			var temp = JsonSerializer.Deserialize<AgendaMedico2025DTO>(json, options);
			if (temp is null)
				return new Result<MedicoAgenda2025>.Error("JSON vacío o inválido.");

			// Reconstruimos dominio validando
			var disponibilidades = new List<MedicoDisponibilidadEnDia2025>();
			foreach (var d in temp.Disponibilidades) {
				Result<MedicoDiaDeLaSemana2025> dia = MedicoDiaDeLaSemana2025.Crear(d.Dia);
				if (dia is Result<MedicoDiaDeLaSemana2025>.Error errDia)
					return new Result<MedicoAgenda2025>.Error(errDia.Mensaje);

				var franjas = new List<MedicoFranjaHoraria2025>();
				foreach (var f in d.Franjas) {
					var fr = MedicoFranjaHoraria2025.Crear(TimeOnly.Parse(f.Desde), TimeOnly.Parse(f.Hasta));
					if (fr is Result<MedicoFranjaHoraria2025>.Error errFr)
						return new Result<MedicoAgenda2025>.Error(errFr.Mensaje);
					franjas.Add(((Result<MedicoFranjaHoraria2025>.Ok)fr).Value);
				}

				disponibilidades.Add(new MedicoDisponibilidadEnDia2025(((Result<MedicoDiaDeLaSemana2025>.Ok)dia).Value, franjas));
			}

			return Crear(disponibilidades);
		} catch (Exception ex) {
			return new Result<MedicoAgenda2025>.Error($"Error al deserializar: {ex.Message}");
		}
	}

	// DTO interno solo para (de)serialización
	private record class AgendaMedico2025DTO {
		public List<DisponibilidadEnDiaDTO> Disponibilidades { get; set; } = new();
	}

	private record class DisponibilidadEnDiaDTO {
		public string Dia { get; set; } = string.Empty;
		public List<FranjaHorariaDTO> Franjas { get; set; } = new();
	}

	private record class FranjaHorariaDTO {
		public string Desde { get; set; } = string.Empty;
		public string Hasta { get; set; } = string.Empty;
	}
}
