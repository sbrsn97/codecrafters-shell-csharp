using System;
using System.Collections.Generic;
using System.Linq;

public static class CompletionEngine
{
    public static CompletionResult Complete(string input, CompletionState state)
    {
        if (string.IsNullOrEmpty(input) || input.EndsWith(' ') || input.Contains(' '))
        {
            state.Reset();
            return CompletionResult.NoChange(input);
        }

        List<string> matches = CommandResolver.GetMatches(input);

        if (matches.Count == 0)
        {
            state.Reset();
            return CompletionResult.Bell(input);
        }

        if (matches.Count == 1)
        {
            state.Reset();
            string only = matches[0];

            if (input == only)
                return CompletionResult.Replace(only + " ");

            return CompletionResult.Replace(only + " ");
        }

        string lcp = GetLongestCommonPrefix(matches);

        if (lcp.Length > input.Length)
        {
            state.Reset();
            return CompletionResult.Replace(lcp);
        }

        bool sameInputAsBefore = state.AwaitingSecondTab && state.LastInput == input;

        if (!sameInputAsBefore)
        {
            state.LastInput = input;
            state.AwaitingSecondTab = true;
            return CompletionResult.Bell(input);
        }

        state.Reset();
        return CompletionResult.ShowMatches(input, matches);
    }

    private static string GetLongestCommonPrefix(List<string> values)
    {
        if (values.Count == 0)
            return string.Empty;

        string prefix = values[0];

        for (int i = 1; i < values.Count; i++)
        {
            prefix = GetCommonPrefix(prefix, values[i]);

            if (prefix.Length == 0)
                break;
        }

        return prefix;
    }

    private static string GetCommonPrefix(string a, string b)
    {
        int len = Math.Min(a.Length, b.Length);
        int i = 0;

        while (i < len && a[i] == b[i])
            i++;

        return a[..i];
    }
}