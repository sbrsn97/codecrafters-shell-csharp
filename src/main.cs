class Program
{
    readonly string[] commands = {"echo"};
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
            userInput = userInput.Substring(command.Length+1);

            if(splittedInput.Count() > 0)


            switch(command)
            {
                case "exit":
                    Environment.Exit(0);
                    break;
                case "echo":
                    Console.WriteLine($"{userInput}");
                    break;
                default:
                    Console.WriteLine($"{command}: command not found");
                    break;
            }
        }
    }
}
