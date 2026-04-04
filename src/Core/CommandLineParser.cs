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

        bool inSingleQuotes = false;
        bool inDoubleQuotes = false;
        bool tokenStarted = false;

        void FlushToken()
        {
            if (!tokenStarted)
                return;

            tokens.Add(current.ToString());
            current.Clear();
            tokenStarted = false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inSingleQuotes)
            {
                if (c == '\'')
                {
                    inSingleQuotes = false;
                }
                else
                {
                    current.Append(c);
                }

                tokenStarted = true;
                continue;
            }

            if (inDoubleQuotes)
            {
                if (c == '"')
                {
                    inDoubleQuotes = false;
                    tokenStarted = true;
                    continue;
                }

                if (c == '\\')
                {
                    if (i + 1 >= input.Length)
                    {
                        current.Append('\\');
                        tokenStarted = true;
                        continue;
                    }

                    char next = input[i + 1];

                    if (next == '"' || next == '\\' || next == '$' || next == '`')
                    {
                        current.Append(next);
                        i++;
                    }
                    else
                    {
                        current.Append('\\');
                    }

                    tokenStarted = true;
                    continue;
                }

                current.Append(c);
                tokenStarted = true;
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                FlushToken();
                continue;
            }

            if (c == '\\')
            {
                if (i + 1 >= input.Length)
                {
                    current.Append('\\');
                    tokenStarted = true;
                    continue;
                }

                current.Append(input[i + 1]);
                i++;
                tokenStarted = true;
                continue;
            }

            if (c == '\'')
            {
                inSingleQuotes = true;
                tokenStarted = true;
                continue;
            }

            if (c == '"')
            {
                inDoubleQuotes = true;
                tokenStarted = true;
                continue;
            }

            current.Append(c);
            tokenStarted = true;
        }

        if (inSingleQuotes || inDoubleQuotes)
            throw new InvalidOperationException("Unterminated quote");

        FlushToken();
        return tokens;
    }
}