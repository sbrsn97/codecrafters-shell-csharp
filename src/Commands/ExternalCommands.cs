using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

public static class ExternalCommands
{
    public static bool SearchForExecutables(CommandLine cmd, bool execute, TextWriter stdout, TextWriter stderr)
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
                            Execute(cmd, stdout, stderr);
                        else
                            PrintTypeFound(programName, candidate, stdout);

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
                            Execute(cmd, stdout, stderr);
                        else
                            PrintTypeFound(programName, fullPath, stdout);

                        found = true;
                    }
                }
            }

            if (found)
                break;
        }

        if (!found)
        {
            stderr.WriteLine(execute
                ? $"{programName}: command not found"
                : $"{programName}: not found");
        }

        return found;
    }

    private static void PrintTypeFound(string command, string path, TextWriter stdout)
    {
        stdout.WriteLine($"{command} is {path}");
    }

    private static void Execute(CommandLine cmd, TextWriter stdout, TextWriter stderr)
    {
        var start = new ProcessStartInfo
        {
            FileName = cmd.Command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (string arg in cmd.Arguments)
        {
            start.ArgumentList.Add(arg);
        }

        using Process proc = Process.Start(start)!;

        string stdOutText = proc.StandardOutput.ReadToEnd();
        string stdErrText = proc.StandardError.ReadToEnd();

        proc.WaitForExit();

        stdout.Write(stdOutText);
        stderr.Write(stdErrText);
    }
        public static IEnumerable<string> GetExecutableNamesFromPath()
    {
        string? path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(path))
            yield break;

        var seen = new HashSet<string>(StringComparer.Ordinal);
        string[] directories = path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        foreach (string directory in directories)
        {
            if (!Directory.Exists(directory))
                continue;

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(directory);
            }
            catch
            {
                continue;
            }

            foreach (string file in files)
            {
                string? candidate = GetExecutableCommandName(file);
                if (candidate is null)
                    continue;

                if (seen.Add(candidate))
                    yield return candidate;
            }
        }
    }

    private static string? GetExecutableCommandName(string fullPath)
    {
        if (!File.Exists(fullPath))
            return null;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string ext = Path.GetExtension(fullPath);

            if (!string.Equals(ext, ".exe", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(ext, ".bat", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(ext, ".cmd", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(ext, ".com", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return Path.GetFileNameWithoutExtension(fullPath);
        }
        else
        {
            UnixFileMode mode;
            try
            {
                mode = File.GetUnixFileMode(fullPath);
            }
            catch
            {
                return null;
            }

            bool isExecutable =
                mode.HasFlag(UnixFileMode.UserExecute) ||
                mode.HasFlag(UnixFileMode.GroupExecute) ||
                mode.HasFlag(UnixFileMode.OtherExecute);

            if (!isExecutable)
                return null;

            return Path.GetFileName(fullPath);
        }
    }
}