public static class CompletionEngine
{
    public static CompletionResult Complete(string input, CompletionState state)
    {
        if (string.IsNullOrEmpty(input) || input.EndsWith(' ') || input.Contains(' '))
        {
            state.TabPressedOnce = false;
            state.LastBuffer = input;
            return CompletionResult.NoChange(input);
        }

        var matches = CommandResolver.GetMatches(input);

        if (matches.Count == 0)
        {
            state.TabPressedOnce = false;
            state.LastBuffer = input;
            return CompletionResult.Bell(input);
        }

        if (matches.Count == 1)
        {
            state.TabPressedOnce = false;
            state.LastBuffer = matches[0] + " ";
            return CompletionResult.Replace(matches[0] + " ");
        }

        bool sameInputAsBefore = state.LastBuffer == input;

        if (!state.TabPressedOnce || !sameInputAsBefore)
        {
            state.TabPressedOnce = true;
            state.LastBuffer = input;
            return CompletionResult.Bell(input);
        }

        state.TabPressedOnce = false;
        state.LastBuffer = input;
        return CompletionResult.ShowMatches(input, matches);
    }
}