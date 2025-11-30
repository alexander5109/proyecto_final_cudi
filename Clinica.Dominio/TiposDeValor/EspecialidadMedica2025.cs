using Clinica.Dominio.Comun;

namespace Clinica.Dominio.TiposDeValor;

// Enum simple para DTOs y mapeo automático
public enum EspecialidadCodigo2025 : byte {
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

public sealed record EspecialidadMedica2025(EspecialidadCodigo2025 CodigoInternoValor, string Titulo, int DuracionConsultaMinutos) : IComoTexto {
	public string ATexto() => $"{Titulo} (Duración de consulta: {DuracionConsultaMinutos} min)";

	// Especialidades predefinidas
	public static readonly EspecialidadMedica2025 ClinicoGeneral = new(EspecialidadCodigo2025.ClinicoGeneral, "Clínico General", 30);
	public static readonly EspecialidadMedica2025 Cardiologo = new(EspecialidadCodigo2025.Cardiologo, "Cardiólogo", 40);
	public static readonly EspecialidadMedica2025 Oftalmologo = new(EspecialidadCodigo2025.Oftalmologo, "Oftalmólogo", 20);
	public static readonly EspecialidadMedica2025 Otorrinolaringologo = new(EspecialidadCodigo2025.Otorrinolaringologo, "Otorrinolaringólogo", 25);
	public static readonly EspecialidadMedica2025 Psiquiatra = new(EspecialidadCodigo2025.Psiquiatra, "Psiquiatra", 50);
	public static readonly EspecialidadMedica2025 Psicologo = new(EspecialidadCodigo2025.Psicologo, "Psicólogo", 50);
	public static readonly EspecialidadMedica2025 Cirujano = new(EspecialidadCodigo2025.Cirujano, "Cirujano", 60);
	public static readonly EspecialidadMedica2025 Kinesiologo = new(EspecialidadCodigo2025.Kinesiologo, "Kinesiólogo", 30);
	public static readonly EspecialidadMedica2025 Nutricionista = new(EspecialidadCodigo2025.Nutricionista, "Nutricionista", 30);
	public static readonly EspecialidadMedica2025 Gastroenterologo = new(EspecialidadCodigo2025.Gastroenterologo, "Gastroenterólogo", 40);
	public static readonly EspecialidadMedica2025 Osteopata = new(EspecialidadCodigo2025.Osteopata, "Osteópata", 30);
	public static readonly EspecialidadMedica2025 Proctologo = new(EspecialidadCodigo2025.Proctologo, "Proctólogo", 30);
	public static readonly EspecialidadMedica2025 Pediatra = new(EspecialidadCodigo2025.Pediatra, "Pediatra", 25);
	public static readonly EspecialidadMedica2025 Ginecologo = new(EspecialidadCodigo2025.Ginecologo, "Ginecólogo", 35);
	public static readonly EspecialidadMedica2025 Traumatologo = new(EspecialidadCodigo2025.Traumatologo, "Traumatólogo", 40);
	public static readonly EspecialidadMedica2025 Neurologo = new(EspecialidadCodigo2025.Neurologo, "Neurólogo", 45);
	public static readonly EspecialidadMedica2025 Dermatologo = new(EspecialidadCodigo2025.Dermatologo, "Dermatólogo", 20);

	// Lista de todas
	public static readonly IReadOnlyList<EspecialidadMedica2025> Todas = new[]{
		ClinicoGeneral, Cardiologo, Oftalmologo, Otorrinolaringologo, Psiquiatra, Psicologo, Cirujano,
		Kinesiologo, Nutricionista, Gastroenterologo, Osteopata, Proctologo, Pediatra, Ginecologo,
		Traumatologo, Neurologo, Dermatologo
	};

	// Lookup seguro por código interno
	public static Result<EspecialidadMedica2025> CrearPorCodigoInterno(EspecialidadCodigo2025 codigo) {
		var esp = Todas.FirstOrDefault(e => e.CodigoInternoValor == codigo);
		return esp is not null
			? new Result<EspecialidadMedica2025>.Ok(esp)
			: new Result<EspecialidadMedica2025>.Error($"No existe la especialidad con CodigoInternoValor = {codigo}");
	}

	// Para facilitar conversion desde DTOs que usen byte
	//public static Result<EspecialidadMedica2025> CrearPorCodigoInterno(byte? codigo) {
	//	if (codigo is null)
	//		return new Result<EspecialidadMedica2025>.Error("El CodigoInternoValor no puede ser nulo.");

	//	if (!Enum.IsDefined(typeof(EspecialidadCodigo2025), codigo.Value))
	//		return new Result<EspecialidadMedica2025>.Error($"Valor inválido de EspecialidadCodigo2025: {codigo.Value}");

	//	return CrearPorCodigoInterno((EspecialidadCodigo2025)codigo.Value);
	//}
}
