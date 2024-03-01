namespace Minesweeper;

public static partial class Program {
    private static readonly (int, int) StartPosition = (0, 0);

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
                                  .../Minesweeper --keywords : Outputs all the keywords used for the different color/interface modes.
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
        if (args is ["--keywords"]) {
            Console.WriteLine($"""
                               Minesweeper keywords:
                                   Colored mode: {string.Join(", ", ColorSet)}
                                   Monochrome mode: {string.Join(", ", MonochromeSet)}
                                   Keyboard mode (both in-game and command line argument): {string.Join(", ", from word
                                           in KeyboardSet where word.Split([' ', '-']).Length == 1 select word)}
                                   Keyboard mode (only in-game): {string.Join(", ", from word
                                           in KeyboardSet where word.Split([' ', '-']).Length > 1 select word)}
                                   Text mode (both in-game and command line argument): {string.Join(", ", from word
                                           in TextSet where word.Split([' ', '-']).Length == 1 select word)}
                                   Text mode (only in-game): {string.Join(", ", from word
                                           in KeyboardSet where word.Split([' ', '-']).Length > 1 select word)}
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
            if (s_minesweeper.GameInterfaceMode == Minesweeper.InterfaceMode.Text) {
                TextPlay();
            }
            else {
                KeyboardPlay();
            }
        }
        catch (Exception e) {
            Console.Clear();
            Position = StartPosition;
            Console.WriteLine(
                $"Minesweeper has experienced a fatal error. Error: {e.Message}\nMinesweeper will now exit following any key press...");
            Console.ReadKey();
        }
        Position = StartPosition;
        Console.Clear();
    }
}