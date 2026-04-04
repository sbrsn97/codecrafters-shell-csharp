public sealed class CompletionResult
{
    public string NewBuffer { get; init; } = string.Empty;
    public bool RingBell { get; init; }
    public bool ShowCandidates { get; init; }
    public List<string> Candidates { get; init; } = new();

    public static CompletionResult NoChange(string input) =>
        new() { NewBuffer = input };

    public static CompletionResult Bell(string input) =>
        new() { NewBuffer = input, RingBell = true };

    public static CompletionResult Replace(string input) =>
        new() { NewBuffer = input };

    public static CompletionResult ShowMatches(string input, List<string> matches) =>
        new() { NewBuffer = input, ShowCandidates = true, Candidates = matches };
}