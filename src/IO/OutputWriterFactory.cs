using System.Text;
public static class OutputWriterFactory
{
    public static OutputTargets Create(CommandLine cmd)
    {
        TextWriter stdout = CreateWriter(
            cmd.StdoutRedirectPath,
            Console.Out,
            cmd.StdoutAppend);

        TextWriter stderr = CreateWriter(
            cmd.StderrRedirectPath,
            Console.Error,
            cmd.StderrAppend);

        return new OutputTargets(stdout, stderr);
    }

    private static TextWriter CreateWriter(string? path, TextWriter fallback, bool append)
    {
        if (string.IsNullOrWhiteSpace(path))
            return new NonClosingTextWriter(fallback);

        return new StreamWriter(path, append)
        {
            AutoFlush = true
        };
    }
}