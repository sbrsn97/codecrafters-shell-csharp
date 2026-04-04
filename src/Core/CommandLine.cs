public sealed class CommandLine
{
    public string RawInput { get; }
    public string Command { get; }
    public List<string> Arguments { get; }

    public string? StdoutRedirectPath { get; }
    public bool StdoutAppend { get; }

    public string? StderrRedirectPath { get; }
    public bool StderrAppend { get; }

    public CommandLine(
        string rawInput,
        string command,
        List<string> arguments,
        string? stdoutRedirectPath,
        bool stdoutAppend,
        string? stderrRedirectPath,
        bool stderrAppend)
    {
        RawInput = rawInput;
        Command = command;
        Arguments = arguments;
        StdoutRedirectPath = stdoutRedirectPath;
        StdoutAppend = stdoutAppend;
        StderrRedirectPath = stderrRedirectPath;
        StderrAppend = stderrAppend;
    }
}