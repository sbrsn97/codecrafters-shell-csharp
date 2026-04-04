public sealed class CompletionState
{
    public string LastBuffer { get; set; } = string.Empty;
    public bool TabPressedOnce { get; set; }
}