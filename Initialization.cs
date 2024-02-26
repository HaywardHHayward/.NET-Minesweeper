namespace Minesweeper;

internal static partial class Program {
    private static readonly HashSet<string> colorSet = ["colored", "color", "c", "coloured", "colour"];

    private static readonly HashSet<string> monochromeSet = ["monochrome", "mono", "m", "bw"];

    private static readonly HashSet<string> keyboardSet = [
        "keyboard-based", "keyboard based", "keyboard-mode", "keyboard mode", "keyboard", "key-based", "key based",
        "key-mode", "key mode", "key", "k-based", "k based", "k-mode", "k mode", "k"
    ];

    private static readonly HashSet<string> textSet =
        ["text-based", "text based", "text-mode", "text mode", "text", "t-based", "t based", "t-mode", "t mode", "t"];

    private static void InitializeWithArgs(string[] args) {
        int[] boardInputs = new int[3];
        Minesweeper.ColorMode cMode;
        Minesweeper.InterfaceMode iMode;
        if (args.Length >= 3) {
            for (int i = 0; i < 3; i++) {
                bool result = int.TryParse(args[i], out boardInputs[i]);
                if (!result) {
                    throw new ArgumentException(
                        $"Invalid argument(s), the first three arguments must be positive integers. Inputted arguments: {
                            args[..2]}");
                }
            }
            switch (args.Length) {
                case 5: {
                    HashSet<string> keyboardOneWord =
                        (from word in keyboardSet where word.Split([' ', '-']).Length == 1 select word).ToHashSet();
                    HashSet<string> textOneWord =
                        (from word in textSet where word.Split([' ', '-']).Length == 1 select word).ToHashSet();
                    if (colorSet.Contains(args[3])) {
                        cMode = Minesweeper.ColorMode.Colored;
                    }
                    else if (monochromeSet.Contains(args[3])) {
                        cMode = Minesweeper.ColorMode.Monochrome;
                    }
                    else {
                        throw new ArgumentException($"Invalid argument(s). Failed to initialize ColorMode. Input: {
                            args[3]}");
                    }
                    if (keyboardOneWord.Contains(args[4])) {
                        iMode = Minesweeper.InterfaceMode.Keyboard;
                    }
                    else if (textOneWord.Contains(args[4])) {
                        iMode = Minesweeper.InterfaceMode.Text;
                    }
                    else {
                        throw new ArgumentException($"Invalid argument(s). Failed to initialize InterfaceMode. Input: {
                            args[4]}");
                    }
                    break;
                }
                case 3:
                    (cMode, iMode) = InterfaceInitialization();
                    break;
                default:
                    throw new ArgumentException(
                        $"Invalid number of arguments. Minesweeper supports 0, 3, or 5 arguments. Number of arguments inputted: {
                            args.Length}");
            }
        }
        else {
            throw new ArgumentException(
                $"Invalid number of arguments. Minesweeper supports 0, 3, or 5 arguments. Number of arguments inputted: {
                    args.Length}");
        }
        Initialization(boardInputs, cMode, iMode);
    }

    private static void Initialization(int[]? boardInputs = null, Minesweeper.ColorMode? cMode = null,
                                       Minesweeper.InterfaceMode? iMode = null) {
        if (boardInputs != null) {
            if (boardInputs.Length != 3) {
                throw new ArgumentOutOfRangeException(nameof(boardInputs),
                                                      $"Board initialization must have three total members. Current count: {
                                                          boardInputs.Length}");
            }
            if (cMode != null && iMode != null) {
                _minesweeper = new Minesweeper(boardInputs[0], boardInputs[1], boardInputs[2], cMode.Value,
                                               iMode.Value);
                return;
            }
            if ((cMode != null && iMode == null) || (cMode == null && iMode != null)) {
                if (cMode != null) {
                    throw new ArgumentNullException(nameof(iMode), "Both mode values must be provided.");
                }
                throw new ArgumentNullException(nameof(cMode), "Both mode values must be provided.");
            }
            (cMode, iMode) = InterfaceInitialization();
            _minesweeper = new Minesweeper(boardInputs[0], boardInputs[1], boardInputs[2], cMode.Value, iMode.Value);
            return;
        }
        (int row, int col, int mines) = BoardInitialization();
        if (cMode != null || iMode != null) {
            throw new ArgumentNullException(nameof(boardInputs));
        }
        (cMode, iMode) = InterfaceInitialization();
        _minesweeper = new Minesweeper(row, col, mines, cMode.Value, iMode.Value);
    }

    private static (int, int, int) BoardInitialization() {
        input_size:
        bool validRow = false;
        int row;
        do {
            Console.Write("Input desired number of rows: ");
            bool parsed = int.TryParse(Console.ReadLine(), out row);
            if (!parsed) {
                WriteStatusMessage();
                continue;
            }
            if (row <= 0) {
                WriteStatusMessage("Number of rows must be greater than zero. Try again.");
                continue;
            }
            validRow = true;
        } while (!validRow);
        Position = (0, Position.y + 1);
        ConsoleInterface.ClearLastLine();
        bool validCol = false;
        int col;
        do {
            Console.Write("Input desired number of columns: ");
            bool parsed = int.TryParse(Console.ReadLine(), out col);
            if (!parsed) {
                WriteStatusMessage();
                continue;
            }
            if (col <= 0) {
                WriteStatusMessage("Number of columns must be greater than zero. Try again.");
                continue;
            }
            validCol = true;
        } while (!validCol);
        if (row * col <= 9) {
            Console.Clear();
            Console.WriteLine(
                "Number of rows and the number of columns must multiply to be greater than 9. Try again.");
            goto input_size;
        }
        Position = (0, Position.y + 1);
        ConsoleInterface.ClearLastLine();
        bool validMines = false;
        int mines;
        do {
            Console.Write("Input desired number of mines: ");
            bool parsed = int.TryParse(Console.ReadLine(), out mines);
            if (!parsed) {
                WriteStatusMessage();
                continue;
            }
            if (mines <= 0 || mines >= row * col - 8) {
                WriteStatusMessage($"Number of mines must be in between 1 and {row * col - 8}. Try again.");
                continue;
            }
            validMines = true;
        } while (!validMines);
        Position = (0, Position.y + 1);
        ConsoleInterface.ClearLastLine();
        return (row, col, mines);
    }

    private static void WriteStatusMessage(string message = "Invalid input. Try again.") {
        Position = (0, Position.y + 1);
        ConsoleInterface.ClearLastLine();
        Position = (0, Position.y - 1);
        Console.WriteLine(message);
        Position = (0, Position.y - 1);
        ConsoleInterface.ClearLastLine();
        Position = (0, Position.y - 1);
    }

    private static (Minesweeper.ColorMode, Minesweeper.InterfaceMode) InterfaceInitialization() {
        bool validColor = false;
        Minesweeper.ColorMode color = Minesweeper.ColorMode.None;
        do {
            Console.Write("Input desired color mode (colored/monochrome): ");
            string colorInput = Console.ReadLine()?.ToLower() ?? string.Empty;
            if (colorSet.Contains(colorInput)) {
                color = Minesweeper.ColorMode.Colored;
                validColor = true;
            }
            else if (monochromeSet.Contains(colorInput)) {
                color = Minesweeper.ColorMode.Monochrome;
                validColor = true;
            }
            else {
                WriteStatusMessage("Invalid input. Please input your desired color mode.");
            }
        } while (!validColor);
        Position = (0, Position.y + 1);
        ConsoleInterface.ClearLastLine();
        Minesweeper.InterfaceMode interf = Minesweeper.InterfaceMode.None;
        bool validInterf = false;
        do {
            Console.Write("Input desired interface mode (text-based/keyboard-based): ");
            string interfInput = Console.ReadLine()?.ToLower() ?? string.Empty;
            if (keyboardSet.Contains(interfInput)) {
                interf = Minesweeper.InterfaceMode.Keyboard;
                validInterf = true;
            }
            else if (textSet.Contains(interfInput)) {
                interf = Minesweeper.InterfaceMode.Text;
                validInterf = true;
            }
            else {
                WriteStatusMessage("Invalid input. Please input your desired interface mode.");
            }
        } while (!validInterf);
        return (color, interf);
    }
}