internal class Echo : IBuiltInCommand
{
    public string CommandName => "echo";

    public void Execute(string[] args)
    {
        Console.WriteLine(string.Join(" ", args[0..]));
    }

}