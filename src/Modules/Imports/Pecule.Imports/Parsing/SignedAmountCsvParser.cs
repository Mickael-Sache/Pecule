using CsvHelper;

using Pecule.Imports.Contracts;

namespace Pecule.Imports.Parsing;

internal sealed class SignedAmountCsvParser : IStatementParser
{
    private static readonly CsvReaderOptions Options = new()
    {
        Separator = ';',
        MissingField = CsvMissingFieldHandling.ReturnEmpty,
    };

    public StatementFormat Format => StatementFormat.SignedAmountCsv;

    public bool CanRead(string firstLine) =>
        firstLine.Trim().Equals("Date;Libellé;Montant", StringComparison.OrdinalIgnoreCase);

    public IReadOnlyList<ParsedLine> Parse(TextReader reader)
    {
        using var csv = CsvReader.Create(reader, Options);

        var lines = new List<ParsedLine>();
        while (csv.Read())
        {
            var number = lines.Count + 1;
            var rawDate = csv.GetString(0);
            var label = csv.GetString(1).Trim();
            var rawAmount = csv.GetString(2);

            // - Case: Date is empty or ivalid
            if (!FrenchValues.TryParseDate(rawDate, out var date))
            {
                lines.Add(ParsedLine.Invalid(number, label, $"la date « {rawDate} » n'existe pas ou n'est pas au format JJ/MM/AAAA."));
            }
            // - Case: Amount is not a number or ivalid
            else if (!FrenchValues.TryParseAmount(rawAmount, out var amount))
            {
                lines.Add(ParsedLine.Invalid(number, label, $"le montant « {rawAmount} » n'est pas un nombre valide."));
            }
            // - Case: Valid line
            else
            {
                lines.Add(ParsedLine.Valid(number, date, amount, label));
            }
        }

        return lines;
    }
}
