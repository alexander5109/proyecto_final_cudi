using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace Clinica.WebAPI.RouteConstraint;

public class MedicoIdRouteConstraint : IRouteConstraint {
	public bool Match(
		HttpContext? httpContext,
		IRouter? route,
		string routeKey,
		RouteValueDictionary values,
		RouteDirection routeDirection) {
		if (!values.TryGetValue(routeKey, out var raw)) return false;

		if (int.TryParse(raw?.ToString(), out int intVal)) {
			// httpContext.Items[routeKey] = new MedicoId(intVal);
			return true;
		}

		return false;
	}
}
