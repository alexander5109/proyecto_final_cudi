using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinica.Dominio.Tests.Escenarios;

namespace Clinica.Dominio.Tests;

internal class Program {
	static void Main(string[] args) {
		var test1 = new DisponibilidadEscenariosTests();
		test1.Escenario_Asignar_turnos_por_orden_de_solicitud();

	}
}
