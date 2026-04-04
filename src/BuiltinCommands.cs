using System;
using System.Collections.Generic;
using System.IO;

public static class BuiltinCommands
{
    public static readonly Dictionary<string, Action<string?>> Commands =
        new Dictionary<string, Action<string?>>()
        {
            ["echo"] = arg => Console.WriteLine(arg),
            ["exit"] = arg => Environment.Exit(0),
            ["pwd"]  = arg => Console.WriteLine(Directory.GetCurrentDirectory()),
            ["type"] = arg => PrintCommandType(arg!),
            ["cd"]   = arg => ChangeDirectory(arg!)
        };

    private static void PrintCommandType(string input)
    {
        if (Commands.ContainsKey(input))
            Console.WriteLine($"{input} is a shell builtin");
        else
            ExternalCommands.SearchForExecutables(input, false);
    }

    private static void ChangeDirectory(string path)
    {
        try
        {
            if(Directory.Exists(path))
                Directory.SetCurrentDirectory(path);
            else
                Console.WriteLine($"cd: {path}: No such file or directory");
        }
        catch (System.Exception)
        {
            Console.WriteLine($"Unhandled exception. System.IO.DirectoryNotFoundException: Could not find a part of the path '{path}'.");
        }
    }
}
