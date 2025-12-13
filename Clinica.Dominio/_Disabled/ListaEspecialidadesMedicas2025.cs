//using Clinica.Dominio.FunctionalToolkit;
//using Clinica.Dominio.TiposDeEntidad;
//using System.Text;

//namespace Clinica.Dominio.TiposDeEntidad;

//public sealed record ListaEspecialidadesMedicas2025(
//	IReadOnlyList<Especialidad2025> Valores
//) : IComoTexto {
//	public string ATextoDiaYHoras() {
//		if (Valores is null || Valores.Count == 0)
//			return "No hay especialidades asignadas.";

//        StringBuilder sb = new();
//		sb.AppendLine("Listado de especialidades:");

//		foreach (Especialidad2025 esp in Valores)
//			sb.AppendLine($"  • {esp.ATextoDiaYHoras()}");

//		return sb.ToString();
//	}
//}

