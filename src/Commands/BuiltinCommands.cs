using System;
using System.Collections.Generic;
using System.IO;

public static class BuiltinCommands
{
    public static readonly Dictionary<string, Action<CommandLine>> Commands =
        new ()
        {
            ["echo"] = cmd => Console.WriteLine(cmd),
            ["exit"] = cmd => Environment.Exit(0),
            ["pwd"]  = cmd => Console.WriteLine(Directory.GetCurrentDirectory()),
            ["type"] = cmd => PrintCommandType(cmd!),
            ["cd"]   = cmd => ChangeDirectory(cmd),
            ["cat"]  = cmd => CatFile(cmd),
            ["ls"] = cmd => ListDirectory(cmd),
        };

    private static void PrintCommandType(CommandLine cmd)
    {
        if (cmd.Arguments.Count == 0)
        {
            Console.WriteLine("type: missing argument");
            return;
        }

        string target = cmd.Arguments[0];

        if (Commands.ContainsKey(target))
            Console.WriteLine($"{target} is a shell builtin");
        else
            ExternalCommands.SearchForExecutables(
                new CommandLine(target, target, new List<string>()),
                false);
    }

    private static void ChangeDirectory(CommandLine cmd)
    {
        if (cmd.Arguments.Count == 0)
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Directory.SetCurrentDirectory(homePath);
            return;
        }

        if (cmd.Arguments.Count > 1)
        {
            Console.WriteLine("cd: too many arguments");
            return;
        }

        string path = ExpandHomePath(cmd.Arguments[0]);

        try
        {
            Directory.SetCurrentDirectory(path);
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"cd: {cmd.Arguments[0]}: No such file or directory");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"cd: {cmd.Arguments[0]}: Permission denied");
        }
        catch (IOException)
        {
            Console.WriteLine($"cd: {cmd.Arguments[0]}: Not a directory");
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

    private static void CatFile(CommandLine cmd)
    {
        if (cmd.Arguments.Count == 0)
        {
            Console.WriteLine("cat: missing file operand");
            return;
        }

        foreach (string file in cmd.Arguments)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"cat: {file}: No such file or directory");
                continue;
            }

            Console.Write(File.ReadAllText(file));
        }
    }

    private static void ListDirectory(CommandLine cmd)
    {
        if (cmd.Arguments.Count > 1)
        {
            Console.WriteLine("ls: too many arguments");
            return;
        }

        string path = cmd.Arguments.Count == 0
            ? Directory.GetCurrentDirectory()
            : ExpandHomePath(cmd.Arguments[0]);

        try
        {
            if (File.Exists(path))
            {
                Console.WriteLine(Path.GetFileName(path));
                return;
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"ls: cannot access '{cmd.Arguments[0]}': No such file or directory");
                return;
            }

            var entries = Directory
                .GetFileSystemEntries(path)
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"ls: cannot open directory '{path}': Permission denied");
        }
        catch (IOException)
        {
            Console.WriteLine($"ls: cannot access '{path}'");
        }
    }
}
