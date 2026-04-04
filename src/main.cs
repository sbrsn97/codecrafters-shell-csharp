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
                Console.WriteLine(ex.Message);
                continue;
            }

            if (parsed == null)
                continue;

            using TextWriter output = OutputWriterFactory.CreateOutputWriter(parsed);

            if (BuiltinCommands.Commands.TryGetValue(parsed.Command, out var action))
            {
                action(parsed, output);
            }
            else
            {
                bool found = ExternalCommands.SearchForExecutables(parsed, true, output);

                if (!found && OperatingSystem.IsWindows() && parsed.Command == "cat")
                {
                    BuiltinCommands.RunCat(parsed, output);
                }
            }
        }
    }
}