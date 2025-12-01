
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace Clinica.WebAPI.RouteConstraint;

public class PacienteIdRouteConstraint : IRouteConstraint {
	public bool Match(
		HttpContext? httpContext,
		IRouter? route,
		string routeKey,
		RouteValueDictionary values,
		RouteDirection routeDirection) {
		if (!values.TryGetValue(routeKey, out var raw)) return false;

		if (int.TryParse(raw?.ToString(), out int intVal)) {
			// Opcional: si querés, podés guardar el VO en HttpContext.Items
			// httpContext.Items[routeKey] = new PacienteId(intVal);
			return true;
		}

		return false;
	}
}
