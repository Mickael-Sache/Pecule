using Pecule.Imports.Contracts;

namespace Pecule.Imports.Parsing;

internal interface IStatementParser
{
    StatementFormat Format { get; }
    bool CanRead(string firstLine);
    IReadOnlyList<ParsedLine> Parse(TextReader reader);
}
