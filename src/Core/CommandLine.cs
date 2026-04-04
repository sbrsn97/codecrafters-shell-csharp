public sealed class CommandLine
{
    public string RawInput { get; }
    public string Command { get; }
    public List<string> Arguments { get; }

    public CommandLine(string rawInput, string command, List<string> arguments)
    {
        RawInput = rawInput;
        Command = command;
        Arguments = arguments;
    }
}