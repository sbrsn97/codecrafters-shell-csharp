public static class CommandResolver
{
    public static List<string> GetAllCommandNames()
    {
        return BuiltinCommands.Commands.Keys
            .Concat(ExternalCommands.GetExecutableNamesFromPath())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    public static List<string> GetMatches(string prefix)
    {
        return GetAllCommandNames()
            .Where(x => x.StartsWith(prefix, StringComparison.Ordinal))
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    public static bool TryResolveExternalCommand(string commandName, out string? executablePath)
    {
        executablePath = ExternalCommands.FindExecutablePath(commandName);
        return executablePath is not null;
    }
}