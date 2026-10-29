namespace CodeCrafters.Shell.BuiltIns;

internal interface IBuiltInCommand
{
    void Execute(string[] args);
    string CommandName { get; }

}