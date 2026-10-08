class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");
            string? input = Console.ReadLine();
            if (input == "exit")
            {
                Environment.Exit(0);
            }
            Console.WriteLine($"{input}: command not found");
        }
    }
}
