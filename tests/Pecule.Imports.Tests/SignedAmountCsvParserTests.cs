using Pecule.Imports.Parsing;

namespace Pecule.Imports.Tests
{
    public sealed class SignedAmountCsvParserTests
    {
        private static IReadOnlyList<ParsedLine> Parse(string content) => new SignedAmountCsvParser().Parse(new StringReader(content));

        [Theory]
        [InlineData("Date;Libellé;Montant")]
        [InlineData("date;libellé;montant")]
        [InlineData("  Date;Libellé;Montant  ")]
        public void RecognizesItsHeader(string firstLine)
        {
            var parser = new SignedAmountCsvParser();

            Assert.True(parser.CanRead(firstLine));
        }

        [Fact]
        public void DoesNotRecognizeAnotherHeader()
        {
            var parser = new SignedAmountCsvParser();

            Assert.False(parser.CanRead("Date;Libellé;Débit;Crédit"));
        }

        [Fact]
        public void ReadsAValidLine()
        {
            // Arrange
            const string content = """
            Date;Libellé;Montant
            02/10/2026;  CB BOULANGERIE DU MARCHE ;-6,40
            """;

            // Act
            var lines = Parse(content);

            // Assert
            var line = Assert.Single(lines);
            Assert.Equal(1, line.Number);
            Assert.Equal(new DateOnly(2026, 10, 2), line.Date);
            Assert.Equal(-6.40m, line.Amount);
            Assert.Equal("CB BOULANGERIE DU MARCHE", line.Label);
            Assert.Null(line.Error);
        }

        [Fact]
        public void ReportInvalidDateAndKeepsReading()
        {
            // Arrange
            const string content = """
            Date;Libellé;Montant
            31/09/2026;  CB BOULANGERIE DU MARCHE ;-6,40
            02/10/2026; CB BOUCHER;-12,80
            """;

            // Act
            var lines = Parse(content);

            // Assert
            Assert.Equal(2, lines.Count);
            Assert.Contains("31/09/2026", lines[0].Error);
            Assert.Null(lines[1].Error);
            Assert.Equal(new DateOnly(2026, 10, 2), lines[1].Date);
            Assert.Equal(-12.80m, lines[1].Amount);
            Assert.Equal("CB BOUCHER", lines[1].Label);
        }

        [Fact]
        public void ReportIncompleteLineWithoutThrowing()
        {
            // Arrange
            const string content = """
            Date;Libellé;Montant
            02/10/2026;  CB BOULANGERIE DU MARCHE ;
            """;

            // Act
            var lines = Parse(content);

            // Assert
            var line = Assert.Single(lines);
            Assert.NotNull(line.Error);
            Assert.Null(line.Amount);
        }

        [Fact]
        public void CanReadQuotedLabelWithSemiColon()
        {
            // Arrange
            const string content = """
            Date;Libellé;Montant
            02/10/2026;"CB;BOULANGERIE DU MARCHE";-6,40
            """;

            // Act
            var lines = Parse(content);

            // Assert
            var line = Assert.Single(lines);
            Assert.Equal("CB;BOULANGERIE DU MARCHE", line.Label);
            Assert.Equal(-6.40m, line.Amount);
        }
    }
}
