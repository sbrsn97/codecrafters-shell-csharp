using System.Text;

public sealed class OutputTargets
{
    public TextWriter Stdout { get; }
    public TextWriter Stderr { get; }

    public OutputTargets(TextWriter stdout, TextWriter stderr)
    {
        Stdout = stdout;
        Stderr = stderr;
    }
}

public static class OutputWriterFactory
{
    public static OutputTargets Create(CommandLine cmd)
    {
        TextWriter stdout = CreateWriter(cmd.StdoutRedirectPath, Console.Out);
        TextWriter stderr = CreateWriter(cmd.StderrRedirectPath, Console.Error);

        return new OutputTargets(stdout, stderr);
    }

    private static TextWriter CreateWriter(string? path, TextWriter fallback)
    {
        if (string.IsNullOrWhiteSpace(path))
            return new NonClosingTextWriter(fallback);

        return new StreamWriter(path, append: false)
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