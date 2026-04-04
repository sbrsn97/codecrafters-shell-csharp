using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class BuiltinCommands
{
    public static readonly Dictionary<string, Action<CommandLine, TextWriter, TextWriter>> Commands =
        new()
        {
            ["echo"] = (cmd, stdout, stderr) => stdout.WriteLine(string.Join(' ', cmd.Arguments)),
            ["pwd"]  = (cmd, stdout, stderr) => stdout.WriteLine(Directory.GetCurrentDirectory()),
            ["exit"] = (cmd, stdout, stderr) => Environment.Exit(0),
            ["type"] = (cmd, stdout, stderr) => PrintCommandType(cmd, stdout, stderr),
            ["cd"]   = (cmd, stdout, stderr) => ChangeDirectory(cmd, stdout, stderr),
            ["ls"]   = (cmd, stdout, stderr) => ListDirectory(cmd, stdout, stderr)
        };

    private static void PrintCommandType(CommandLine cmd, TextWriter stdout, TextWriter stderr)
    {
        if (cmd.Arguments.Count == 0)
        {
            stderr.WriteLine("type: missing argument");
            return;
        }

        string target = cmd.Arguments[0];

        if (Commands.ContainsKey(target))
        {
            stdout.WriteLine($"{target} is a shell builtin");
        }
        else
        {
            ExternalCommands.SearchForExecutables(
                new CommandLine(target, target, new List<string>(), null, null),
                false,
                stdout,
                stderr);
        }
    }

    private static void ChangeDirectory(CommandLine cmd, TextWriter stdout, TextWriter stderr)
    {
        if (cmd.Arguments.Count == 0)
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Directory.SetCurrentDirectory(homePath);
            return;
        }

        if (cmd.Arguments.Count > 1)
        {
            stderr.WriteLine("cd: too many arguments");
            return;
        }

        string path = ExpandHomePath(cmd.Arguments[0]);

        try
        {
            Directory.SetCurrentDirectory(path);
        }
        catch (DirectoryNotFoundException)
        {
            stderr.WriteLine($"cd: {cmd.Arguments[0]}: No such file or directory");
        }
        catch (UnauthorizedAccessException)
        {
            stderr.WriteLine($"cd: {cmd.Arguments[0]}: Permission denied");
        }
        catch (IOException)
        {
            stderr.WriteLine($"cd: {cmd.Arguments[0]}: Not a directory");
        }
    }

    private static string ExpandHomePath(string path)
    {
        if (path == "~")
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (path.StartsWith("~/") || path.StartsWith("~\\"))
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string relativePart = path[2..];
            return Path.Combine(homePath, relativePart);
        }

        return path;
    }

    public static void RunCat(CommandLine cmd, TextWriter stdout, TextWriter stderr)
    {
        CatFile(cmd, stdout, stderr);
    }

    private static void CatFile(CommandLine cmd, TextWriter stdout, TextWriter stderr)
    {
        if (cmd.Arguments.Count == 0)
        {
            stderr.WriteLine("cat: missing file operand");
            return;
        }

        foreach (string file in cmd.Arguments)
        {
            if (!File.Exists(file))
            {
                stderr.WriteLine($"cat: {file}: No such file or directory");
                continue;
            }

            stdout.Write(File.ReadAllText(file));
        }
    }

    private static void ListDirectory(CommandLine cmd, TextWriter stdout, TextWriter stderr)
    {
        if (cmd.Arguments.Count > 1)
        {
            stderr.WriteLine("ls: too many arguments");
            return;
        }

        string path = cmd.Arguments.Count == 0
            ? Directory.GetCurrentDirectory()
            : ExpandHomePath(cmd.Arguments[0]);

        try
        {
            if (File.Exists(path))
            {
                stdout.WriteLine(Path.GetFileName(path));
                return;
            }

            if (!Directory.Exists(path))
            {
                stderr.WriteLine($"ls: cannot access '{cmd.Arguments[0]}': No such file or directory");
                return;
            }

            var entries = Directory
                .GetFileSystemEntries(path)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

            foreach (var entry in entries)
            {
                stdout.WriteLine(entry);
            }
        }
        catch (UnauthorizedAccessException)
        {
            stderr.WriteLine($"ls: cannot open directory '{path}': Permission denied");
        }
        catch (IOException)
        {
            stderr.WriteLine($"ls: cannot access '{path}'");
        }
    }
}