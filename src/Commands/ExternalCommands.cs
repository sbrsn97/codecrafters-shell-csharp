using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

public static class ExternalCommands
{
    public static bool SearchForExecutables(CommandLine cmd, bool execute, TextWriter output)
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
                            Execute(candidate, cmd, output);
                        else
                            PrintTypeFound(programName, candidate, output);

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
                    bool isExecutable =
                        mode.HasFlag(UnixFileMode.UserExecute) ||
                        mode.HasFlag(UnixFileMode.GroupExecute) ||
                        mode.HasFlag(UnixFileMode.OtherExecute);

                    if (isExecutable)
                    {
                        if (execute)
                            Execute(fullPath, cmd, output);
                        else
                            PrintTypeFound(programName, fullPath, output);

                        found = true;
                    }
                }
            }

            if (found)
                break;
        }

        if (!found)
        {
            output.WriteLine(execute
                ? $"{programName}: command not found"
                : $"{programName}: not found");
        }

        return found;
    }

    private static void PrintTypeFound(string command, string path, TextWriter output)
    {
        output.WriteLine($"{command} is {path}");
    }

    private static void Execute(string executablePath, CommandLine cmd, TextWriter output)
    {
        var start = new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = false
        };

        foreach (string arg in cmd.Arguments)
        {
            start.ArgumentList.Add(arg);
        }

        using Process proc = Process.Start(start)!;
        string stdout = proc.StandardOutput.ReadToEnd();
        output.Write(stdout);
        proc.WaitForExit();
    }
}