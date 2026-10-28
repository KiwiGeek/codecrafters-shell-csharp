internal interface IBuiltInCommand
{
    void Execute(string[] args);
    string CommandName { get; }

}