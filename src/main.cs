class Program
{
    static void Main()
    {
        Console.Write("$ ");
        string? userInput = Console.ReadLine();
        Console.WriteLine($"{userInput}: command not found");
    }
}
