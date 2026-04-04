using System.Text;

public sealed class NonClosingTextWriter : TextWriter
{
    private readonly TextWriter _inner;

    public NonClosingTextWriter(TextWriter inner)
    {
        _inner = inner;
    }

    public override Encoding Encoding => _inner.Encoding;

    public override void Write(char value) => _inner.Write(value);
    public override void Write(string? value) => _inner.Write(value);
    public override void WriteLine(string? value) => _inner.WriteLine(value);
    public override void Flush() => _inner.Flush();

    protected override void Dispose(bool disposing)
    {
        Flush();
    }
}