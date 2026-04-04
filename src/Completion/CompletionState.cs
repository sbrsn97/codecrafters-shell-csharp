public sealed class CompletionState
{
    public string LastInput { get; set; } = string.Empty;
    public bool AwaitingSecondTab { get; set; }

    public void Reset()
    {
        LastInput = string.Empty;
        AwaitingSecondTab = false;
    }
}