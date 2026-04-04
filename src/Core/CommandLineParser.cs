using System.Text;

public static class CommandLineParser
{
    public static CommandLine? Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        List<string> tokens = Tokenize(input);

        if (tokens.Count == 0)
            return null;

        string command = tokens[0];
        List<string> arguments = tokens.Skip(1).ToList();

        return new CommandLine(input, command, arguments);
    }

    private static List<string> Tokenize(string input)
    {
        List<string> tokens = new();
        StringBuilder current = new();
        bool inQuotes = false;

        foreach (char c in input)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            tokens.Add(current.ToString());

        return tokens;
    }
}