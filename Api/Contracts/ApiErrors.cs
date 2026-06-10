using CIF.Domain.Common;

namespace CIF.Api.Contracts;

public sealed record ValidationErrorResponse(string Error, IReadOnlyCollection<Error> Details, string? TraceId)
{
    public static ValidationErrorResponse FromErrors(IReadOnlyCollection<Error> errors, string? traceId) =>
        new("ValidationFailed", errors, traceId);
}
