namespace Minesweeper;

internal static partial class Program {
    private static (int x, int y) Position {
        get => ConsoleInterface.CursorPosition;
        set => ConsoleInterface.CursorPosition = value;
    }

    public static void Main(string[] args) {
        Console.Clear();
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
        Position = (0, 0);
        Console.Clear();
    }
}