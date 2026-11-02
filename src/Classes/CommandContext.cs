namespace CodeCrafters.Shell.Classes;

public record CommandContext(TextReader StandardInput, TextWriter StandardOutput, TextWriter StandardError);