namespace CodeCrafters.Shell.BuiltIns;

internal interface IBuiltInCommand
{
    string CommandName { get; }
    void Execute(Token[] tokens);
}