using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;

namespace Clinica.Dominio.TiposDeEnum;

public sealed record Especialidad2025(
	EspecialidadEnum Codigo,
	string Titulo,
	int DuracionConsultaMinutos
) : IComoTexto {
	public string ATexto() => $"{Titulo} (Consulta: {DuracionConsultaMinutos} mins)";

	// Especialidades predefinidas
	public static readonly Especialidad2025 ClinicoGeneral = new(EspecialidadEnum.ClinicoGeneral, "Clínico General", 30);
	public static readonly Especialidad2025 Cardiologo = new(EspecialidadEnum.Cardiologo, "Cardiólogo", 40);
	public static readonly Especialidad2025 Oftalmologo = new(EspecialidadEnum.Oftalmologo, "Oftalmólogo", 20);
	public static readonly Especialidad2025 Otorrinolaringologo = new(EspecialidadEnum.Otorrinolaringologo, "Otorrinolaringólogo", 25);
	public static readonly Especialidad2025 Psiquiatra = new(EspecialidadEnum.Psiquiatra, "Psiquiatra", 50);
	public static readonly Especialidad2025 Psicologo = new(EspecialidadEnum.Psicologo, "Psicólogo", 50);
	public static readonly Especialidad2025 Cirujano = new(EspecialidadEnum.Cirujano, "Cirujano", 60);
	public static readonly Especialidad2025 Kinesiologo = new(EspecialidadEnum.Kinesiologo, "Kinesiólogo", 30);
	public static readonly Especialidad2025 Nutricionista = new(EspecialidadEnum.Nutricionista, "Nutricionista", 30);
	public static readonly Especialidad2025 Gastroenterologo = new(EspecialidadEnum.Gastroenterologo, "Gastroenterólogo", 40);
	public static readonly Especialidad2025 Osteopata = new(EspecialidadEnum.Osteopata, "Osteópata", 30);
	public static readonly Especialidad2025 Proctologo = new(EspecialidadEnum.Proctologo, "Proctólogo", 30);
	public static readonly Especialidad2025 Pediatra = new(EspecialidadEnum.Pediatra, "Pediatra", 25);
	public static readonly Especialidad2025 Ginecologo = new(EspecialidadEnum.Ginecologo, "Ginecólogo", 35);
	public static readonly Especialidad2025 Traumatologo = new(EspecialidadEnum.Traumatologo, "Traumatólogo", 40);
	public static readonly Especialidad2025 Neurologo = new(EspecialidadEnum.Neurologo, "Neurólogo", 45);
	public static readonly Especialidad2025 Dermatologo = new(EspecialidadEnum.Dermatologo, "Dermatólogo", 20);

	// Lista de todas
	public static readonly IReadOnlyList<Especialidad2025> Todas = [
		ClinicoGeneral, Cardiologo, Oftalmologo, Otorrinolaringologo, Psiquiatra, Psicologo, Cirujano,
		Kinesiologo, Nutricionista, Gastroenterologo, Osteopata, Proctologo, Pediatra, Ginecologo,
		Traumatologo, Neurologo, Dermatologo
	];

	// Asumimos o crasheamos
	public static Especialidad2025 Representar(EspecialidadEnum codigo) => Todas.FirstOrDefault(e => e.Codigo == codigo)!;


	public static Result<Especialidad2025> CrearResult(EspecialidadEnum? codigo) {
		if (codigo == null) {
			return new Result<Especialidad2025>.Error($"No se pudo validar la especialidad con codigo nulo");
		}
		Especialidad2025? esp = Todas.FirstOrDefault(e => e.Codigo == codigo);
		return esp is not null
			? new Result<Especialidad2025>.Ok(esp)
			: new Result<Especialidad2025>.Error($"No existe la especialidad con Codigo = {codigo}");
	}

	//public static Especialidad2025? Crear(EspecialidadEnum codigo) => Todas.FirstOrDefault(e => e.Codigo == codigo);

	// Para facilitar conversion desde DTOs que usen byte
	//public static Result<Especialidad2025> CrearResult(byte? codigo) {
	//	if (codigo is null)
	//		return new Result<Especialidad2025>.Error("El Codigo no puede ser nulo.");

	//	if (!Enum.IsDefined(typeof(EspecialidadEnum), codigo.Value))
	//		return new Result<Especialidad2025>.Error($"Valor inválido de EspecialidadEnum: {codigo.Value}");

	//	return CrearResult((EspecialidadEnum)codigo.Value);
	//}
}
