using System.IO;
using System.Text;

public static class OutputWriterFactory
{
    public static TextWriter CreateOutputWriter(CommandLine cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.StdoutRedirectPath))
            return new NonClosingTextWriter(Console.Out);

        return new StreamWriter(cmd.StdoutRedirectPath, append: false)
        {
            AutoFlush = true
        };
    }
}

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