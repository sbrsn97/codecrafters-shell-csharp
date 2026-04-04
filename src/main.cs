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

            CommandLine? parsed;
            try
            {
                parsed = CommandLineParser.Parse(userInput);
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine(ex.Message);
                continue;
            }

            if (parsed == null)
                continue;

            OutputTargets targets = OutputWriterFactory.Create(parsed);

            using TextWriter stdout = targets.Stdout;
            using TextWriter stderr = targets.Stderr;

            if (BuiltinCommands.Commands.TryGetValue(parsed.Command, out var action))
            {
                action(parsed, stdout, stderr);
            }
            else
            {
                bool found = ExternalCommands.SearchForExecutables(parsed, true, stdout, stderr);

                if (!found && OperatingSystem.IsWindows() && parsed.Command == "cat")
                {
                    BuiltinCommands.RunCat(parsed, stdout, stderr);
                }
            }
        }
    }
}