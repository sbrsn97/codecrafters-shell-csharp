using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class BuiltinCommands
{
    public static readonly Dictionary<string, Action<CommandLine, TextWriter>> Commands =
        new()
        {
            ["echo"] = (cmd, output) => output.WriteLine(string.Join(' ', cmd.Arguments)),
            ["pwd"]  = (cmd, output) => output.WriteLine(Directory.GetCurrentDirectory()),
            ["exit"] = (cmd, output) => Environment.Exit(0),
            ["type"] = (cmd, output) => PrintCommandType(cmd, output),
            ["cd"]   = (cmd, output) => ChangeDirectory(cmd, output),
            ["ls"]   = (cmd, output) => ListDirectory(cmd, output)
        };

    private static void PrintCommandType(CommandLine cmd, TextWriter output)
    {
        if (cmd.Arguments.Count == 0)
        {
            output.WriteLine("type: missing argument");
            return;
        }

        string target = cmd.Arguments[0];

        if (Commands.ContainsKey(target))
        {
            output.WriteLine($"{target} is a shell builtin");
        }
        else
        {
            ExternalCommands.SearchForExecutables(
                new CommandLine(target, target, new List<string>(), null),
                false,
                output);
        }
    }

    private static void ChangeDirectory(CommandLine cmd, TextWriter output)
    {
        if (cmd.Arguments.Count == 0)
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Directory.SetCurrentDirectory(homePath);
            return;
        }

        if (cmd.Arguments.Count > 1)
        {
            output.WriteLine("cd: too many arguments");
            return;
        }

        string path = ExpandHomePath(cmd.Arguments[0]);

        try
        {
            Directory.SetCurrentDirectory(path);
        }
        catch (DirectoryNotFoundException)
        {
            output.WriteLine($"cd: {cmd.Arguments[0]}: No such file or directory");
        }
        catch (UnauthorizedAccessException)
        {
            output.WriteLine($"cd: {cmd.Arguments[0]}: Permission denied");
        }
        catch (IOException)
        {
            output.WriteLine($"cd: {cmd.Arguments[0]}: Not a directory");
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

    public static void RunCat(CommandLine cmd, TextWriter output)
    {
        CatFile(cmd, output);
    }

    private static void CatFile(CommandLine cmd, TextWriter output)
    {
        if (cmd.Arguments.Count == 0)
        {
            output.WriteLine("cat: missing file operand");
            return;
        }

        foreach (string file in cmd.Arguments)
        {
            if (!File.Exists(file))
            {
                output.WriteLine($"cat: {file}: No such file or directory");
                continue;
            }

            output.Write(File.ReadAllText(file));
        }
    }

    private static void ListDirectory(CommandLine cmd, TextWriter output)
    {
        string? targetPath = null;

        foreach (string arg in cmd.Arguments)
        {
            if (arg == "-1")
                continue;

            if (targetPath == null)
            {
                targetPath = ExpandHomePath(arg);
                continue;
            }

            output.WriteLine("ls: too many arguments");
            return;
        }

        string path = targetPath ?? Directory.GetCurrentDirectory();

        try
        {
            if (File.Exists(path))
            {
                output.WriteLine(Path.GetFileName(path));
                return;
            }

            if (!Directory.Exists(path))
            {
                output.WriteLine($"ls: cannot access '{(targetPath ?? path)}': No such file or directory");
                return;
            }

            var entries = Directory
                .GetFileSystemEntries(path)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

            foreach (var entry in entries)
            {
                output.WriteLine(entry);
            }
        }
        catch (UnauthorizedAccessException)
        {
            output.WriteLine($"ls: cannot open directory '{path}': Permission denied");
        }
        catch (IOException)
        {
            output.WriteLine($"ls: cannot access '{path}'");
        }
    }
}