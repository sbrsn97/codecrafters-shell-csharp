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
                            Execute(candidate, cmd);
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
                    bool isExecutable =
                        mode.HasFlag(UnixFileMode.UserExecute) ||
                        mode.HasFlag(UnixFileMode.GroupExecute) ||
                        mode.HasFlag(UnixFileMode.OtherExecute);

                    if (isExecutable)
                    {
                        if (execute)
                            Execute(fullPath, cmd);
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

    private static void Execute(string executablePath, CommandLine cmd)
    {
        ProcessStartInfo start = new ProcessStartInfo
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string ext = Path.GetExtension(executablePath).ToLowerInvariant();

            if (ext == ".bat" || ext == ".cmd")
            {
                start.FileName = "cmd.exe";
                start.ArgumentList.Add("/C");
                start.ArgumentList.Add(executablePath);

                foreach (string arg in cmd.Arguments)
                    start.ArgumentList.Add(arg);
            }
            else
            {
                start.FileName = executablePath;

                foreach (string arg in cmd.Arguments)
                    start.ArgumentList.Add(arg);
            }
        }
        else
        {
            start.FileName = cmd.Command;

            foreach (string arg in cmd.Arguments)
                start.ArgumentList.Add(arg);
        }

        using Process proc = Process.Start(start)!;

        string stdout = proc.StandardOutput.ReadToEnd();
        string stderr = proc.StandardError.ReadToEnd();

        proc.WaitForExit();

        if (!string.IsNullOrEmpty(stdout))
            Console.Write(stdout);

        if (!string.IsNullOrEmpty(stderr))
            Console.Write(stderr);
    }
}