class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");
            string input = Console.ReadLine() ?? string.Empty;
            if (input == "exit")
            {
                Environment.Exit(0);
            }
            else if (input[..5] == "echo ")
            {
                Console.WriteLine(input[5..]);
            }
            else 
            {
                Console.WriteLine($"{input}: command not found");
            }
        }
    }
}
