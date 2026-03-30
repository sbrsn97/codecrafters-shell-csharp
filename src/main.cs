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
                    PrintCommandType(command, userInput);
                    break;
                default:
                    Console.WriteLine($"{command}: command not found");
                    break;
            }
        }

        static void PrintCommandType(string command, string input)
        {
            string[] defined = ["exit", "echo", "type"];

            if(defined.Any(x => x == command))
                Console.WriteLine($"{command} is a shell builtin");
            else
                Console.WriteLine($"{command}: command not found");
        }
    }
}
