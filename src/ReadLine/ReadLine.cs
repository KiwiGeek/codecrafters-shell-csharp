namespace CodeCrafters.Shell.ReadLine;

public static class ReadLine
{
    public static ReadContext Context { get; } = new();

    public static string? Read(string prompt = "", string @default = "") => Read(Context, prompt, @default);
    public static string? ReadPassword(string prompt = "") => ReadPassword(Context, prompt);

    extension(ReadContext context)
    {
        [PublicAPI] public string? Read(string prompt = "", string @default = "")
        {
            KeyHandler keyHandler = new KeyHandler(prompt, context, false);
            string? text = GetText(context.Console, keyHandler);

            if (string.IsNullOrWhiteSpace(text))
            {
                if (!string.IsNullOrWhiteSpace(@default))
                    text = @default;
            }
            else
            {
                if (context.HistoryEnabled)
                    context.History.Add(text);
            }

            return text;
        }
    }
    extension(ReadContext context)
    {
        [PublicAPI] public string? ReadPassword(string prompt = "")
        {
            KeyHandler keyHandler = new (prompt, context, true);
            return GetText(context.Console, keyHandler);
        }
    }
    private static string? GetText(IConsole console, KeyHandler keyHandler)
    {
        ConsoleKeyInfo keyInfo = console.ReadKey();
        while (keyInfo.Key != ConsoleKey.Enter)
        {
            keyHandler.Handle(keyInfo);

            if (keyHandler.Text is null)
                break;

            keyInfo = console.ReadKey();
        }

        console.WriteLine();
        return keyHandler.Text;
    }
}