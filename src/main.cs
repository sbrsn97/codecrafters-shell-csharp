using System.Text;
using System.Linq;

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
        const string prompt = "$ ";
        var buffer = new StringBuilder();

        Console.Out.Write(prompt);
        Console.Out.Flush();

        int promptTop = Console.CursorTop;

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.Out.WriteLine();
                Console.Out.Flush();
                return buffer.ToString();
            }

            if (keyInfo.Key == ConsoleKey.Tab)
            {
                string current = buffer.ToString();
                string completed = TryAutocompleteBuiltin(current);

                if (completed != current)
                {
                    buffer.Clear();
                    buffer.Append(completed);
                    RedrawInput(prompt, buffer.ToString(), promptTop);
                }
                else
                {
                    Console.Out.Write('\a');
                    Console.Out.Flush();
                }

                continue;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer.Remove(buffer.Length - 1, 1);
                    Console.Out.Write("\b \b");
                    Console.Out.Flush();
                }

                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                buffer.Append(keyInfo.KeyChar);
                Console.Out.Write(keyInfo.KeyChar);
                Console.Out.Flush();
            }
        }
    }

    static void RedrawInput(string prompt, string text, int promptTop)
    {
        Console.SetCursorPosition(0, promptTop);

        string fullLine = prompt + text;
        Console.Out.Write(fullLine);

        int remaining = Console.BufferWidth - fullLine.Length;
        if (remaining > 0)
        {
            Console.Out.Write(new string(' ', remaining));
        }

        Console.SetCursorPosition(fullLine.Length, promptTop);
        Console.Out.Flush();
    }

    static string TryAutocompleteBuiltin(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (input.EndsWith(' '))
            return input;

        if (input.Contains(' '))
            return input;

        var matches = BuiltinCommands.Commands.Keys
            .Where(x => x.StartsWith(input, StringComparison.Ordinal))
            .OrderBy(x => x)
            .ToList();

        if (matches.Count != 1)
            return input;

        string match = matches[0];

        if (input == match)
            return match + " ";

        return match + " ";
    }
}