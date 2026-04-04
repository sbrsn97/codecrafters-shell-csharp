using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine($"Program was passed {args.Length} args (including program name).");
            return;
        }

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

            if (BuiltinCommands.Commands.TryGetValue(command, out var action))
            {
                action(userInput);
            }
            else
            {
                ExternalCommands.SearchForExecutables(userInput, true);
            }
        }
    }
}
