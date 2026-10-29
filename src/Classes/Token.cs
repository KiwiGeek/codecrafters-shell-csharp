namespace CodeCrafters.Shell.Classes;

public class Token(string value)
{
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));
    public override string ToString() => Value!;
    public static implicit operator string?(Token token) => token.Value;
}