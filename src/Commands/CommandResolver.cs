public static class CommandResolver
{
    public static List<string> GetMatches(string prefix)
    {
        return BuiltinCommands.Commands.Keys
            .Concat(ExternalCommands.GetExecutableNamesFromPath())
            .Distinct(StringComparer.Ordinal)
            .Where(x => x.StartsWith(prefix, StringComparison.Ordinal))
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }
}