using System.Globalization;

namespace Pecule.Imports.Parsing;

internal static class FrenchValues
{
    private static readonly CultureInfo French = CultureInfo.GetCultureInfo("fr-FR");

    public static bool TryParseDate(string value, out DateOnly date) =>
        DateOnly.TryParseExact(value.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

    public static bool TryParseAmount(string value, out decimal amount)
    {
        // « 1 234,56 » : ordinary space, insecable or thin depends on each bank
        var compact = value.Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        return decimal.TryParse(compact, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, French, out amount);
    }
}
