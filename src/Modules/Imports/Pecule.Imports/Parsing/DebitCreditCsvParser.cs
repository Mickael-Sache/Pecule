using CsvHelper;

using Pecule.Imports.Contracts;

namespace Pecule.Imports.Parsing;

internal sealed class DebitCreditCsvParser : IStatementParser
{
    private static readonly CsvReaderOptions Options = new()
    {
        Separator = ';',
        MissingField = CsvMissingFieldHandling.ReturnEmpty,
    };

    public StatementFormat Format => StatementFormat.DebitCreditCsv;

    public bool CanRead(string firstLine) =>
        firstLine.Trim().Equals("Date;Libellé;Débit;Crédit", StringComparison.OrdinalIgnoreCase);

    public IReadOnlyList<ParsedLine> Parse(TextReader reader)
    {
        using var csv = CsvReader.Create(reader, Options);

        var lines = new List<ParsedLine>();
        while (csv.Read())
        {
            var number = lines.Count + 1;
            var rawDate = csv.GetString(0);
            var label = csv.GetString(1).Trim();
            var rawDebitAmount = csv.GetString(2);
            var rawCreditAmount = csv.GetString(3);

            var hasDebit = !string.IsNullOrWhiteSpace(rawDebitAmount);
            var hasCredit = !string.IsNullOrWhiteSpace(rawCreditAmount);

            var rawAmount = hasDebit ? rawDebitAmount : rawCreditAmount;

            // - Case: Date is empty or invalid
            if (!FrenchValues.TryParseDate(rawDate, out var date))
            {
                lines.Add(ParsedLine.Invalid(number, label, $"la date « {rawDate} » n'existe pas ou n'est pas au format JJ/MM/AAAA."));
            }
            // - Case: Both Debit and Credit has values
            else if (hasDebit == hasCredit)
            {
                lines.Add(ParsedLine.Invalid(number, label, "une seule des colonnes Débit et Crédit doit être remplie."));
            }
            // - Case: Parsed amount is not a valid number
            else if (!FrenchValues.TryParseAmount(rawAmount, out var amount))
            {
                lines.Add(ParsedLine.Invalid(number, label, $"le montant « {rawAmount} » n'est pas un nombre valide."));
            }
            // - Case: Valid line, applying (-) for debit
            else
            {
                lines.Add(ParsedLine.Valid(number, date, hasDebit ? -amount : amount, label));
            }
        }

        return lines;
    }
}
