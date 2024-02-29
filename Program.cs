namespace Minesweeper;

public static partial class Program {
    private static (int x, int y) Position {
        get => ConsoleInterface.CursorPosition;
        set => ConsoleInterface.CursorPosition = value;
    }

    public static void Main(string[] args) {
        Console.Clear();
        try {
            if (args.Length != 0) {
                InitializeWithArgs(args);
            }
            else {
                Initialization();
            }
            Console.Clear();
            EnsureWindowSize();
            Console.Clear();
            if (_minesweeper.GameInterfaceMode == Minesweeper.InterfaceMode.Text) {
                TextPlay();
            }
            else {
                KeyboardPlay();
            }
        }
        catch (Exception e) {
            Console.Clear();
            Console.WriteLine($"Minesweeper has experienced a fatal error. Error: {e.Message}\nMinesweeper will now exit following any key press...");
            Console.ReadKey();
            return;
        }
        Position = (0, 0);
        Console.Clear();
    }
}