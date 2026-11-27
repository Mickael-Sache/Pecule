using System.Text;

using Pecule.Imports.Parsing;

namespace Pecule.Imports.Tests;

public sealed class StatementTextTests
{
    private const string Header = "Date;Libellé;Montant";
    private static async Task<string> ReadAsync(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        return await StatementText.ReadAsync(stream, CancellationToken.None);
    }

    [Fact]
    public async Task DecodesUtf8Content()
    {
        var text = await ReadAsync(Encoding.UTF8.GetBytes(Header));

        Assert.Equal(Header, text);
    }

    [Fact]
    public async Task DecodesLatin1Content()
    {
        // In Latin-1, 'é' is the byte 0xE9, invalid in UTF-8: fallback must be activated
        var text = await ReadAsync(Encoding.Latin1.GetBytes(Header));

        Assert.Equal(Header, text);
    }

    [Fact]
    public async Task RemovesTheUtf8ByteOrderMark()
    {
        byte[] bytes = [.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(Header)];

        var text = await ReadAsync(bytes);

        Assert.Equal(Header, text);
    }
}
