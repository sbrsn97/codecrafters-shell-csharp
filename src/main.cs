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
                
            string[] splittedInput = userInput.Split(' ');
            string command = splittedInput[0];
            userInput = userInput.Substring(command.Length).Trim();

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
                //Console.WriteLine($"{input}: not found");
                SearchForExecutables(input);
        }

        static void SearchForExecutables(string input)
        {
            string? path = Environment.GetEnvironmentVariable("PATH");
            string[] directories = path!.Split(Path.PathSeparator);
            bool found = false;

            foreach(string directory in directories)
            {
                string fullPath = Path.Combine(directory, input);

                if(!File.Exists(fullPath))
                {
                    continue;
                }

                UnixFileMode mode = File.GetUnixFileMode(fullPath);

                bool isExecutable = mode.HasFlag(UnixFileMode.GroupExecute) ||
                                    mode.HasFlag(UnixFileMode.OtherExecute) ||
                                    mode.HasFlag(UnixFileMode.UserExecute);

                if(isExecutable)
                {
                    Console.WriteLine($"{input} is {fullPath}");
                    found = true;
                }
            }
            
            if(!found)
                Console.WriteLine($"{input}: not found");
        }
    }
}
