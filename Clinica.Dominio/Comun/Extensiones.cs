using System;
using System.Collections.Generic;
using System.Text;

namespace Clinica.Dominio.Comun;

public static class Extensiones {
	public static int IndexOf<T>(this IReadOnlyList<T> source, Predicate<T> pred) {
		for (int i = 0; i < source.Count; i++)
			if (pred(source[i])) return i;
		return -1;
	}
}
