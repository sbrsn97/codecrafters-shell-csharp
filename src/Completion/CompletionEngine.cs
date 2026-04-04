public static class CompletionEngine
{
    public static CompletionResult Complete(string input, CompletionState state)
    {
        if (string.IsNullOrEmpty(input) || input.Contains(' ') || input.EndsWith(' '))
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
            string completed = matches[0] == input ? input + " " : matches[0] + " ";
            return CompletionResult.Replace(completed);
        }

        bool sameInput = state.AwaitingSecondTab && state.LastInput == input;

        if (!sameInput)
        {
            state.LastInput = input;
            state.AwaitingSecondTab = true;
            return CompletionResult.Bell(input);
        }

        state.AwaitingSecondTab = false;
        state.LastInput = input;
        return CompletionResult.ShowMatches(input, matches);
    }
}