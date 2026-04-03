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
                    Console.WriteLine($"{command}: command not found");
                    break;
            }
        }

        static void PrintCommandType(string input)
        {
            string[] defined = ["exit", "echo", "type"];

            if(defined.Any(x => x == input))
                Console.WriteLine($"{input} is a shell builtin");
            else
                SearchForExecutables(input);
        }

        static void SearchForExecutables(string input)
        {
            bool found = false;
            string args = "";
            string? path = Environment.GetEnvironmentVariable("PATH");
            string[] directories = path!.Split(Path.PathSeparator);
            string[] splitInput = input.Split(new[] { ' '}, 2);
            input = splitInput[0];

            if(splitInput.Length > 1)
                args = splitInput[1];

            foreach (string directory in directories)
            {
                string fullPath = Path.Combine(directory, input);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string[] extensions = { ".exe", ".bat", ".cmd", ".com" };
                    foreach (var ext in extensions)
                    {
                        string candidate = fullPath + ext;
                        if (File.Exists(candidate))
                        {
                            Execute(candidate, args);
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
                            Execute(fullPath, args);
                            found = true;
                        }
                    }
                }

                if (found) break;
            }

            if (!found)
                Console.WriteLine($"{input}: not found");
        }

        static void Execute(string exePath, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = args; 
            start.FileName = exePath;

            int exitCode;

            using (Process proc = Process.Start(start)!)
            {
                proc.WaitForExit();

                exitCode = proc.ExitCode;
            }
        }
    }
}
