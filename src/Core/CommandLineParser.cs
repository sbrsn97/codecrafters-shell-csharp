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

        return new CommandLine(input, tokens[0], tokens.Skip(1).ToList());
    }

    private static List<string> Tokenize(string input)
    {
        List<string> tokens = new();
        StringBuilder current = new();

        bool inDoubleQuotes = false;
        bool inSingleQuotes = false;

        foreach (char c in input)
        {
            if (c == '"' && !inSingleQuotes)
            {
                inDoubleQuotes = !inDoubleQuotes;
                continue;
            }

            if (c == '\'' && !inDoubleQuotes)
            {
                inSingleQuotes = !inSingleQuotes;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inDoubleQuotes && !inSingleQuotes)
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