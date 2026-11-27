namespace Pecule.Imports.Parsing;

internal sealed record ParsedLine(int Number, DateOnly? Date, decimal? Amount, string Label, string? Error)
{
    public static ParsedLine Valid(int number, DateOnly date, decimal amount, string label) =>
        new(number, date, amount, label, null);

    public static ParsedLine Invalid(int number, string label, string error) =>
        new(number, null, null, label, error);
}
