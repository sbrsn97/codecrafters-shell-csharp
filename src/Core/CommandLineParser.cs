using System.Text;

public static class CommandLineParser
{
    public static CommandLine? Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        List<Token> tokens = Lex(input);
        if (tokens.Count == 0)
            return null;

        return ParseTokens(input, tokens);
    }

    private static List<Token> Lex(string input)
    {
        List<Token> tokens = new();
        StringBuilder current = new();

        bool inSingleQuotes = false;
        bool inDoubleQuotes = false;
        bool tokenStarted = false;

        void FlushWord()
        {
            if (!tokenStarted)
                return;

            tokens.Add(new Token(TokenType.Word, current.ToString()));
            current.Clear();
            tokenStarted = false;
        }

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inSingleQuotes)
            {
                if (c == '\'')
                    inSingleQuotes = false;
                else
                    current.Append(c);

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
                    if (i + 1 < input.Length)
                    {
                        char next = input[i + 1];

                        if (next == '"' || next == '\\')
                        {
                            current.Append(next);
                            i++;
                        }
                        else
                        {
                            current.Append('\\');
                        }
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
                FlushWord();
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

            if (c == '\\')
            {
                if (i + 1 < input.Length)
                {
                    current.Append(input[i + 1]);
                    i++;
                }
                else
                {
                    current.Append('\\');
                }

                tokenStarted = true;
                continue;
            }

            if (c == '1' && i + 2 < input.Length && input[i + 1] == '>' && input[i + 2] == '>')
            {
                FlushWord();
                tokens.Add(new Token(TokenType.AppendFdStdout, "1>>"));
                i += 2;
                continue;
            }

            if (c == '>' && i + 1 < input.Length && input[i + 1] == '>')
            {
                FlushWord();
                tokens.Add(new Token(TokenType.AppendStdout, ">>"));
                i++;
                continue;
            }

            if (c == '1' && i + 1 < input.Length && input[i + 1] == '>')
            {
                FlushWord();
                tokens.Add(new Token(TokenType.RedirectFdStdout, "1>"));
                i++;
                continue;
            }

            if (c == '2' && i + 1 < input.Length && input[i + 1] == '>')
            {
                FlushWord();
                tokens.Add(new Token(TokenType.RedirectStderr, "2>"));
                i++;
                continue;
            }

            if (c == '>')
            {
                FlushWord();
                tokens.Add(new Token(TokenType.RedirectStdout, ">"));
                continue;
            }

            current.Append(c);
            tokenStarted = true;
        }

        if (inSingleQuotes || inDoubleQuotes)
            throw new InvalidOperationException("Unterminated quote");

        FlushWord();
        return tokens;
    }

    private static CommandLine ParseTokens(string rawInput, List<Token> tokens)
    {
        List<string> words = new();
        string? stdoutRedirectPath = null;
        bool stdoutAppend = false;
        string? stderrRedirectPath = null;

        for (int i = 0; i < tokens.Count; i++)
        {
            Token token = tokens[i];

            if (token.Type == TokenType.Word)
            {
                words.Add(token.Value);
                continue;
            }

            if (token.Type == TokenType.RedirectStdout ||
                token.Type == TokenType.RedirectFdStdout)
            {
                if (i + 1 >= tokens.Count || tokens[i + 1].Type != TokenType.Word)
                    throw new InvalidOperationException("Missing redirection target");

                stdoutRedirectPath = tokens[i + 1].Value;
                stdoutAppend = false;
                i++;
                continue;
            }

            if (token.Type == TokenType.AppendStdout ||
                token.Type == TokenType.AppendFdStdout)
            {
                if (i + 1 >= tokens.Count || tokens[i + 1].Type != TokenType.Word)
                    throw new InvalidOperationException("Missing redirection target");

                stdoutRedirectPath = tokens[i + 1].Value;
                stdoutAppend = true;
                i++;
                continue;
            }

            if (token.Type == TokenType.RedirectStderr)
            {
                if (i + 1 >= tokens.Count || tokens[i + 1].Type != TokenType.Word)
                    throw new InvalidOperationException("Missing redirection target");

                stderrRedirectPath = tokens[i + 1].Value;
                i++;
                continue;
            }
        }

        if (words.Count == 0)
            return null!;

        string command = words[0];
        List<string> arguments = words.Skip(1).ToList();

        return new CommandLine(
            rawInput,
            command,
            arguments,
            stdoutRedirectPath,
            stdoutAppend,
            stderrRedirectPath);
    }
}