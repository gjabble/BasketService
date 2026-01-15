namespace BasketService.API.Domain.Outcomes;

public sealed record ValidationFailed(IReadOnlyDictionary<string, string[]> Errors);