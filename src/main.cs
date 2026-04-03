using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        ReadEvalPrintLoop(); 
    }

    static void ReadEvalPrintLoop()
    {
        while(true)
        {
            Console.Write("$ ");
            string? userInput = Console.ReadLine();
            if(userInput == null)
                break;
                
            string[] splitInput = userInput.Split(new[] { ' ' }, 2);
            string command = splitInput[0];
            string fullCommand = userInput;

            if(splitInput.Length > 1)
                userInput = splitInput[1];

            switch(command)
            {
                case "echo":
                    Console.WriteLine($"{userInput}");
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                case "type":
                    PrintCommandType(userInput);
                    break;
                default:
                    if (!SearchForExecutables(fullCommand, true))
                    {
                        Console.WriteLine($"{command}: command not found");
                    }
                    break;
            }
        }

        static void PrintCommandType(string input)
        {
            string[] defined = ["exit", "echo", "type"];

            if(defined.Any(x => x == input))
                Console.WriteLine($"{input} is a shell builtin");
            else
                SearchForExecutables(input, false);
        }

        static bool SearchForExecutables(string input, bool execute)
        {
            bool found = false;
            string? path = Environment.GetEnvironmentVariable("PATH");
            string[] directories = path!.Split(Path.PathSeparator);
            string[] splitInput = input.Split(new[] { ' '}, 2);
            
            string programName = splitInput[0];

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
                                Execute(input);
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
                                Execute(input);
                            else
                                PrintTypeFound(programName, fullPath);
                            found = true;
                        }
                    }
                }

                if (found) break;
            }

            if (!found)
            {
                if (execute)
                    Console.WriteLine($"{programName}: command not found");
                else
                    Console.WriteLine($"{programName}: not found"); 
            }

            return found;
        }

        static void PrintTypeFound(string command, string path) 
        {
            Console.WriteLine($"{command} is {path}");
        }


        static void Execute(string fullCommandLine)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "/bin/sh";
            start.Arguments = $"-c \"{fullCommandLine}\"";
            start.RedirectStandardOutput = true;
            start.UseShellExecute = false;

            int exitCode;

            using (Process proc = Process.Start(start)!)
            {
                string output = proc.StandardOutput.ReadToEnd();
                Console.Write(output);
                proc.WaitForExit();

                exitCode = proc.ExitCode;
            }
        }
    }
}
