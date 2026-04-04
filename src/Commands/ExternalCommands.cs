using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

public static class ExternalCommands
{
    public static bool SearchForExecutables(CommandLine cmd, bool execute)
    {
        bool found = false;
        string? path = Environment.GetEnvironmentVariable("PATH");
        string[] directories = path!.Split(Path.PathSeparator);

        string programName = cmd.Command;

        foreach (string directory in directories)
        {
            string fullPath = Path.Combine(directory, programName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string[] extensions = { ".exe", ".bat", ".cmd", ".com" };

                foreach (var ext in extensions)
                {
                    string candidate = fullPath + ext;
                    if (File.Exists(candidate))
                    {
                        if (execute)
                            Execute(cmd);
                        else
                            PrintTypeFound(programName, candidate);

                        found = true;
                        break;
                    }
                }
            }
            else
            {
                if (File.Exists(fullPath))
                {
                    var mode = File.GetUnixFileMode(fullPath);
                    bool isExecutable = mode.HasFlag(UnixFileMode.UserExecute) ||
                                        mode.HasFlag(UnixFileMode.GroupExecute) ||
                                        mode.HasFlag(UnixFileMode.OtherExecute);

                    if (isExecutable)
                    {
                        if (execute)
                            Execute(cmd);
                        else
                            PrintTypeFound(programName, fullPath);

                        found = true;
                    }
                }
            }

            if (found)
                break;
        }

        if (!found)
        {
            Console.WriteLine(execute
                ? $"{programName}: command not found"
                : $"{programName}: not found");
        }

        return found;
    }

    private static void PrintTypeFound(string command, string path)
    {
        Console.WriteLine($"{command} is {path}");
    }

    private static void Execute(CommandLine cmd)
    {
        ProcessStartInfo start = new ProcessStartInfo();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            start.FileName = "cmd.exe";
            start.Arguments = $"/C {cmd.RawInput}";
        }
        else
        {
            start.FileName = "/bin/sh";
            start.Arguments = $"-c \"{cmd.RawInput.Replace("\"", "\\\"")}\"";
        }

        start.RedirectStandardOutput = true;
        start.UseShellExecute = false;

        using Process proc = Process.Start(start)!;
        string output = proc.StandardOutput.ReadToEnd();
        Console.Write(output);
        proc.WaitForExit();
    }
}