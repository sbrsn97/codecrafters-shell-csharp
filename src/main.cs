class Program
{
    static void Main(string[] args)
    {
        ReadEvalPrintLoop();
    }

    static void ReadEvalPrintLoop()
    {
        while (true)
        {
            Console.Write("$ ");
            string? userInput = Console.ReadLine();

            if (userInput == null)
                break;

            CommandLine? parsed = CommandLineParser.Parse(userInput);
            if (parsed == null)
                continue;

            if (BuiltinCommands.Commands.TryGetValue(parsed.Command, out var action))
            {
                action(parsed);
            }
            else
            {
                ExternalCommands.SearchForExecutables(parsed, true);
            }
        }
    }
}