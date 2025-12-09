using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

// Enum simple para DTOs y mapeo automático
public enum EspecialidadCodigo : byte {
	ClinicoGeneral = 1,
	Cardiologo = 2,
	Oftalmologo = 3,
	Otorrinolaringologo = 4,
	Psiquiatra = 5,
	Psicologo = 6,
	Cirujano = 7,
	Kinesiologo = 8,
	Nutricionista = 9,
	Gastroenterologo = 10,
	Osteopata = 11,
	Proctologo = 12,
	Pediatra = 13,
	Ginecologo = 14,
	Traumatologo = 15,
	Neurologo = 16,
	Dermatologo = 17
}

//public static class EnumExtentions {
//	public string ToDomain(this EspecialidadCodigo code) {

//		return Especialidad2025.

//	}


//}


public sealed record Especialidad2025(
	EspecialidadCodigo Codigo,
	string Titulo,
	int DuracionConsultaMinutos
) : IComoTexto {
	public string ATexto() => $"{Titulo} (Consulta: {DuracionConsultaMinutos} mins)";

	// Especialidades predefinidas
	public static readonly Especialidad2025 ClinicoGeneral = new(EspecialidadCodigo.ClinicoGeneral, "Clínico General", 30);
	public static readonly Especialidad2025 Cardiologo = new(EspecialidadCodigo.Cardiologo, "Cardiólogo", 40);
	public static readonly Especialidad2025 Oftalmologo = new(EspecialidadCodigo.Oftalmologo, "Oftalmólogo", 20);
	public static readonly Especialidad2025 Otorrinolaringologo = new(EspecialidadCodigo.Otorrinolaringologo, "Otorrinolaringólogo", 25);
	public static readonly Especialidad2025 Psiquiatra = new(EspecialidadCodigo.Psiquiatra, "Psiquiatra", 50);
	public static readonly Especialidad2025 Psicologo = new(EspecialidadCodigo.Psicologo, "Psicólogo", 50);
	public static readonly Especialidad2025 Cirujano = new(EspecialidadCodigo.Cirujano, "Cirujano", 60);
	public static readonly Especialidad2025 Kinesiologo = new(EspecialidadCodigo.Kinesiologo, "Kinesiólogo", 30);
	public static readonly Especialidad2025 Nutricionista = new(EspecialidadCodigo.Nutricionista, "Nutricionista", 30);
	public static readonly Especialidad2025 Gastroenterologo = new(EspecialidadCodigo.Gastroenterologo, "Gastroenterólogo", 40);
	public static readonly Especialidad2025 Osteopata = new(EspecialidadCodigo.Osteopata, "Osteópata", 30);
	public static readonly Especialidad2025 Proctologo = new(EspecialidadCodigo.Proctologo, "Proctólogo", 30);
	public static readonly Especialidad2025 Pediatra = new(EspecialidadCodigo.Pediatra, "Pediatra", 25);
	public static readonly Especialidad2025 Ginecologo = new(EspecialidadCodigo.Ginecologo, "Ginecólogo", 35);
	public static readonly Especialidad2025 Traumatologo = new(EspecialidadCodigo.Traumatologo, "Traumatólogo", 40);
	public static readonly Especialidad2025 Neurologo = new(EspecialidadCodigo.Neurologo, "Neurólogo", 45);
	public static readonly Especialidad2025 Dermatologo = new(EspecialidadCodigo.Dermatologo, "Dermatólogo", 20);

	// Lista de todas
	public static readonly IReadOnlyList<Especialidad2025> Todas = [
		ClinicoGeneral, Cardiologo, Oftalmologo, Otorrinolaringologo, Psiquiatra, Psicologo, Cirujano,
		Kinesiologo, Nutricionista, Gastroenterologo, Osteopata, Proctologo, Pediatra, Ginecologo,
		Traumatologo, Neurologo, Dermatologo
	];

	// Asumimos o crasheamos
	public static Especialidad2025 Representar(EspecialidadCodigo codigo) => Todas.FirstOrDefault(e => e.Codigo == codigo)!;


	public static Result<Especialidad2025> CrearResult(EspecialidadCodigo codigo) {
		Especialidad2025? esp = Todas.FirstOrDefault(e => e.Codigo == codigo);
		return esp is not null
			? new Result<Especialidad2025>.Ok(esp)
			: new Result<Especialidad2025>.Error($"No existe la especialidad con Codigo = {codigo}");
	}

	// Para facilitar conversion desde DTOs que usen byte
	//public static Result<Especialidad2025> CrearResult(byte? codigo) {
	//	if (codigo is null)
	//		return new Result<Especialidad2025>.Error("El Codigo no puede ser nulo.");

	//	if (!Enum.IsDefined(typeof(EspecialidadCodigo), codigo.Value))
	//		return new Result<Especialidad2025>.Error($"Valor inválido de EspecialidadCodigo: {codigo.Value}");

	//	return CrearResult((EspecialidadCodigo)codigo.Value);
	//}
}
