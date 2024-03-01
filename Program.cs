namespace Minesweeper;

public static partial class Program {
    private static (int x, int y) Position {
        get => ConsoleInterface.CursorPosition;
        set => ConsoleInterface.CursorPosition = value;
    }

    public static void Main(string[] args) {
        if (args.Length == 1 && (args[0] == "--help" || args[0] == "--h")) {
            Console.WriteLine("""
                              Minesweeper help:
                                    .../Minesweeper : Runs the program, gets user inputs from the application itself
                                    .../Minesweeper --h(elp) : Outputs information about command line arguments
                                    .../Minesweeper [r] [c] [m] : Runs the program with the specified board inputs, gets color and interface inputs from application
                                        [r] : The amount of rows in the board, must be a positive integer
                                        [c] : The amount of columns in the board, must be a positive integer
                                        [m] : The amount of mines in the board, must be a positive integer less than [r] * [c] - 8
                                    .../Minesweeper [r] [c] [m] [color] [inter] : Immediately starts a game of Minesweeper with the specified inputs
                                        [r]     : The amount of rows in the board, must be a positive integer
                                        [c]     : The amount of columns in the board, must be a positive integer
                                        [m]     : The amount of mines in the board, must be less than [r] * [c] - 8
                                        [color] : Either "colored" or "monochrome"
                                        [inter] : Either "keyboard" or "text"
                              """);
            return;
        }
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
            Console.WriteLine(
                $"Minesweeper has experienced a fatal error. Error: {e.Message}\nMinesweeper will now exit following any key press...");
            Console.ReadKey();
            return;
        }
        Position = (0, 0);
        Console.Clear();
    }
}