using System.Net;

namespace Clinica.Shared.ApiDtos;

public record ApiErrorDto(
	string Title,
	HttpStatusCode Status
	//string? Detail = null
);
//public sealed record ProblemDetailsDto(
//	string? Type,
//	string? Title,
//	int? Status,
//	string? TraceId
//);

//public sealed record ApiErrorEnvelopeDto(
//	ApiErrorDto Error
//);