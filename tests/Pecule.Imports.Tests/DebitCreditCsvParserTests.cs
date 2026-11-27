using System.Globalization;

using Pecule.Imports.Parsing;

namespace Pecule.Imports.Tests;

public sealed class DebitCreditCsvParserTests
{
    private static IReadOnlyList<ParsedLine> Parse(string line) =>
       new DebitCreditCsvParser().Parse(new StringReader("Date;Libellé;Débit;Crédit\n" + line));

    [Fact]
    public void RecognizesItsHeader()
    {
        Assert.True(new DebitCreditCsvParser().CanRead("Date;Libellé;Débit;Crédit"));
    }

    [Fact]
    public void DoesNotRecognizeTheSignedAmountHeader()
    {
        Assert.False(new DebitCreditCsvParser().CanRead("Date;Libellé;Montant"));
    }

    [Theory]
    [InlineData("15/09/2026;CB MARCHAND;6,40;", "-6.40")]
    [InlineData("15/09/2026;VIR REMBOURSEMENT;;25,00", "25.00")]
    public void ReadsDebitAsNegativeAndCreditAsPositive(string line, string expected)
    {
        var parsed = Assert.Single(Parse(line));

        Assert.Null(parsed.Error);
        Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), parsed.Amount);
    }

    [Theory]
    [InlineData("15/09/2026;CB MARCHAND;6,40;6,40")]
    [InlineData("15/09/2026;CB MARCHAND;;")]
    public void ReportsALineWithBothOrNoAmountColumns(string line)
    {
        var parsed = Assert.Single(Parse(line));

        Assert.NotNull(parsed.Error);
        Assert.Null(parsed.Amount);
    }
}
