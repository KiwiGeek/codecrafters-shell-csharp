namespace CodeCrafters.Shell.ReadLine.Abstractions;

internal class Console2 : IConsole
{
    public void CursorAdvance(int count)
    {
        if (count > 0)
            Write(Ansi.Cursor.Right(count));
        else if (count < 0)
            Write(Ansi.Cursor.Left(-count));
    }

    public ConsoleKeyInfo ReadKey() => Console.ReadKey(true);
    public void Write(string value) => Console.Write(value);
    public void WriteLine(string value) => Console.WriteLine(value);
}