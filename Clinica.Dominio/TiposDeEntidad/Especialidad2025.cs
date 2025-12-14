using Clinica.Dominio.FunctionalToolkit;
using Clinica.Dominio.IInterfaces;
using Clinica.Dominio.TiposDeEnum;

namespace Clinica.Dominio.TiposDeEntidad;

public sealed record Especialidad2025(
	EspecialidadEnumCodigo Codigo,
	string Titulo,
	int DuracionConsultaMinutos
) : IComoTexto {
	public string ATexto() => $"{Titulo} (Consulta: {DuracionConsultaMinutos} mins)";

	// Especialidades predefinidas
	public static readonly Especialidad2025 ClinicoGeneral = new(EspecialidadEnumCodigo.ClinicoGeneral, "Clínico General", 30);
	public static readonly Especialidad2025 Cardiologo = new(EspecialidadEnumCodigo.Cardiologo, "Cardiólogo", 40);
	public static readonly Especialidad2025 Oftalmologo = new(EspecialidadEnumCodigo.Oftalmologo, "Oftalmólogo", 20);
	public static readonly Especialidad2025 Otorrinolaringologo = new(EspecialidadEnumCodigo.Otorrinolaringologo, "Otorrinolaringólogo", 25);
	public static readonly Especialidad2025 Psiquiatra = new(EspecialidadEnumCodigo.Psiquiatra, "Psiquiatra", 50);
	public static readonly Especialidad2025 Psicologo = new(EspecialidadEnumCodigo.Psicologo, "Psicólogo", 50);
	public static readonly Especialidad2025 Cirujano = new(EspecialidadEnumCodigo.Cirujano, "Cirujano", 60);
	public static readonly Especialidad2025 Kinesiologo = new(EspecialidadEnumCodigo.Kinesiologo, "Kinesiólogo", 30);
	public static readonly Especialidad2025 Nutricionista = new(EspecialidadEnumCodigo.Nutricionista, "Nutricionista", 30);
	public static readonly Especialidad2025 Gastroenterologo = new(EspecialidadEnumCodigo.Gastroenterologo, "Gastroenterólogo", 40);
	public static readonly Especialidad2025 Osteopata = new(EspecialidadEnumCodigo.Osteopata, "Osteópata", 30);
	public static readonly Especialidad2025 Proctologo = new(EspecialidadEnumCodigo.Proctologo, "Proctólogo", 30);
	public static readonly Especialidad2025 Pediatra = new(EspecialidadEnumCodigo.Pediatra, "Pediatra", 25);
	public static readonly Especialidad2025 Ginecologo = new(EspecialidadEnumCodigo.Ginecologo, "Ginecólogo", 35);
	public static readonly Especialidad2025 Traumatologo = new(EspecialidadEnumCodigo.Traumatologo, "Traumatólogo", 40);
	public static readonly Especialidad2025 Neurologo = new(EspecialidadEnumCodigo.Neurologo, "Neurólogo", 45);
	public static readonly Especialidad2025 Dermatologo = new(EspecialidadEnumCodigo.Dermatologo, "Dermatólogo", 20);

	// Lista de todas
	public static readonly IReadOnlyList<Especialidad2025> Todas = [
		ClinicoGeneral, Cardiologo, Oftalmologo, Otorrinolaringologo, Psiquiatra, Psicologo, Cirujano,
		Kinesiologo, Nutricionista, Gastroenterologo, Osteopata, Proctologo, Pediatra, Ginecologo,
		Traumatologo, Neurologo, Dermatologo
	];

	// Asumimos o crasheamos
	public static Especialidad2025 Representar(EspecialidadEnumCodigo codigo) => Todas.FirstOrDefault(e => e.Codigo == codigo)!;


	public static Result<Especialidad2025> CrearResult(EspecialidadEnumCodigo codigo) {
		Especialidad2025? esp = Todas.FirstOrDefault(e => e.Codigo == codigo);
		return esp is not null
			? new Result<Especialidad2025>.Ok(esp)
			: new Result<Especialidad2025>.Error($"No existe la especialidad con Codigo = {codigo}");
	}

	//public static Especialidad2025? Crear(EspecialidadEnumCodigo codigo) => Todas.FirstOrDefault(e => e.Codigo == codigo);

	// Para facilitar conversion desde DTOs que usen byte
	//public static Result<Especialidad2025> CrearResult(byte? codigo) {
	//	if (codigo is null)
	//		return new Result<Especialidad2025>.Error("El Codigo no puede ser nulo.");

	//	if (!Enum.IsDefined(typeof(EspecialidadEnumCodigo), codigo.Value))
	//		return new Result<Especialidad2025>.Error($"Valor inválido de EspecialidadEnumCodigo: {codigo.Value}");

	//	return CrearResult((EspecialidadEnumCodigo)codigo.Value);
	//}
}
