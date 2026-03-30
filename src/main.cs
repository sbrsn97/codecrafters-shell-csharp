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

            switch(userInput)
            {
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine($"{userInput}: command not found");
                    break;
            }
        }
    }
}
