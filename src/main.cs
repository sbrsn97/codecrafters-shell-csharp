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
            Console.WriteLine($"{userInput}: command not found");
        }
    }
}
