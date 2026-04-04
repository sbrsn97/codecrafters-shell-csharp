using System.Text;

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
            string? userInput = ReadInputWithTabCompletion();

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

    static string? ReadInputWithTabCompletion()
    {
        var buffer = new StringBuilder();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return buffer.ToString();
            }

            if (keyInfo.Key == ConsoleKey.Tab)
            {
                string current = buffer.ToString();

                string? completed = TryAutocompleteBuiltin(current);
                if (completed is not null && completed != current)
                {
                    ClearCurrentConsoleInput(buffer);
                    buffer.Clear();
                    buffer.Append(completed);
                    Console.Write(buffer.ToString());
                }

                continue;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer.Remove(buffer.Length - 1, 1);
                    Console.Write("\b \b");
                }

                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                buffer.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        }
    }

    static string? TryAutocompleteBuiltin(string input)
    {
        var matches = BuiltinCommands.Commands.Keys
            .Where(x => x.StartsWith(input, StringComparison.Ordinal))
            .OrderBy(x => x)
            .ToList();

        if (matches.Count == 1)
            return matches[0] + " ";

        return input;
    }
    static void ClearCurrentConsoleInput(StringBuilder buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            Console.Write("\b \b");
        }
    }
}