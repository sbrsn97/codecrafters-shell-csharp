public sealed class CommandLine
{
    public string RawInput { get; }
    public string Command { get; }
    public List<string> Arguments { get; }
    public string? StdoutRedirectPath { get; }
    public string? StderrRedirectPath { get; }

    public CommandLine(
        string rawInput,
        string command,
        List<string> arguments,
        string? stdoutRedirectPath,
        string? stderrRedirectPath)
    {
        RawInput = rawInput;
        Command = command;
        Arguments = arguments;
        StdoutRedirectPath = stdoutRedirectPath;
        StderrRedirectPath = stderrRedirectPath;
    }
}