class Program
{
    static void Main()
    {
        Console.Write("$ ");
        string? input = Console.ReadLine();
        Console.WriteLine($"{input}: command not found");
    }
}
