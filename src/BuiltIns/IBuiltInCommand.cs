using CodeCrafters.Shell.Classes;

namespace CodeCrafters.Shell.BuiltIns;

internal interface IBuiltInCommand
{
    void Execute(Token[] args);
    string CommandName { get; }
}