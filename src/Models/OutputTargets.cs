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