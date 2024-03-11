namespace Minesweeper;

public static partial class Program {
    private static readonly HashSet<string> ColorSet = ["colored", "color", "c", "coloured", "colour"];

    private static readonly HashSet<string> MonochromeSet = ["monochrome", "mono", "m", "bw"];

    private static readonly HashSet<string> KeyboardSet = [
        "keyboard-based", "keyboard based", "keyboard-mode", "keyboard mode", "keyboard", "key-based", "key based",
        "key-mode", "key mode", "key", "k-based", "k based", "k-mode", "k mode", "k"
    ];

    private static readonly HashSet<string> TextSet = [
        "text-based", "text based", "text-mode", "text mode", "text", "t-based", "t based", "t-mode", "t mode", "t"
    ];

    private static void InitializeWithArgs(string[] args) {
        int[] boardInputs = new int[3];
        Minesweeper.ColorMode cMode;
        Minesweeper.InterfaceMode iMode;
        if (args.Length >= 3) {
            for (int i = 0; i < 3; i++) {
                bool result = int.TryParse(args[i], out boardInputs[i]);
                if (!result) {
                    throw new
                        ArgumentException($"Invalid argument(s), the first three arguments must be positive integers. Inputted arguments: {string.Join(", ", args)}");
                }
            }
            switch (args.Length) {
                case 5: {
                    HashSet<string> keyboardOneWord =
                        (from word in KeyboardSet where word.Split([' ', '-']).Length == 1 select word).ToHashSet();
                    HashSet<string> textOneWord =
                        (from word in TextSet where word.Split([' ', '-']).Length == 1 select word).ToHashSet();
                    if (ColorSet.Contains(args[3].ToLower())) {
                        cMode = Minesweeper.ColorMode.Colored;
                    }
                    else if (MonochromeSet.Contains(args[3].ToLower())) {
                        cMode = Minesweeper.ColorMode.Monochrome;
                    }
                    else {
                        throw new
                            ArgumentException($"Invalid argument(s). Failed to initialize ColorMode. Input: {args[3]}");
                    }
                    if (keyboardOneWord.Contains(args[4].ToLower())) {
                        iMode = Minesweeper.InterfaceMode.Keyboard;
                    }
                    else if (textOneWord.Contains(args[4].ToLower())) {
                        iMode = Minesweeper.InterfaceMode.Text;
                    }
                    else {
                        throw new
                            ArgumentException($"Invalid argument(s). Failed to initialize InterfaceMode. Input: {args[4]}");
                    }
                    break;
                }
                case 3:
                    (cMode, iMode) = InterfaceInitialization();
                    break;
                default:
                    throw new
                        ArgumentException($"Invalid number of arguments. Minesweeper supports 0, 3, or 5 arguments. Number of arguments inputted: {args.Length}");
            }
        }
        else {
            throw new
                ArgumentException($"Invalid number of arguments. Minesweeper supports 0, 3, or 5 arguments. Number of arguments inputted: {args.Length}");
        }
        Initialization(boardInputs, cMode, iMode);
    }

    private static void Initialization(int[]? boardInputs = null, Minesweeper.ColorMode? cMode = null,
        Minesweeper.InterfaceMode? iMode = null) {
        if (boardInputs != null) {
            if (boardInputs.Length != 3) {
                throw new ArgumentOutOfRangeException(nameof(boardInputs),
                                                      $"Board initialization must have three total members. Current count: {boardInputs.Length}");
            }
            if (cMode != null && iMode != null) {
                s_minesweeper = new Minesweeper(boardInputs[0], boardInputs[1], boardInputs[2], cMode.Value,
                                                iMode.Value);
                return;
            }
            if ((cMode != null && iMode == null) || (cMode == null && iMode != null)) {
                throw new ArgumentNullException(cMode != null ? nameof(iMode) : nameof(cMode),
                                                "Both mode values must be provided.");
            }
            (cMode, iMode) = InterfaceInitialization();
            s_minesweeper = new Minesweeper(boardInputs[0], boardInputs[1], boardInputs[2], cMode.Value, iMode.Value);
            return;
        }
        (int row, int col, int mines) = BoardInitialization();
        if (cMode != null || iMode != null) {
            throw new ArgumentNullException(nameof(boardInputs));
        }
        (cMode, iMode) = InterfaceInitialization();
        s_minesweeper = new Minesweeper(row, col, mines, cMode.Value, iMode.Value);
    }

    private static (int, int, int) BoardInitialization() {
    input_size:
        bool valid = false;
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
            valid = true;
        } while (!valid);
        Position = (0, Position.y + 1);
        ConsoleInterface.ClearLastLine();
        valid = false;
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
            valid = true;
        } while (!valid);
        if (row * col <= 1) {
            Console.Clear();
            Console.WriteLine("Cannot have a one by one board. Try again.");
            goto input_size;
        }
        Position = (0, Position.y + 1);
        ConsoleInterface.ClearLastLine();
        valid = false;
        int mines;
        do {
            Console.Write("Input desired number of mines: ");
            bool parsed = int.TryParse(Console.ReadLine(), out mines);
            if (!parsed) {
                WriteStatusMessage();
                continue;
            }
            if (mines <= 0 || mines >= row * col) {
                WriteStatusMessage($"Number of mines must be greater than zero and less than {row * col}. Try again.");
                continue;
            }
            valid = true;
        } while (!valid);
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
            if (ColorSet.Contains(colorInput)) {
                color = Minesweeper.ColorMode.Colored;
                validColor = true;
            }
            else if (MonochromeSet.Contains(colorInput)) {
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
            if (KeyboardSet.Contains(interfInput)) {
                interf = Minesweeper.InterfaceMode.Keyboard;
                validInterf = true;
            }
            else if (TextSet.Contains(interfInput)) {
                interf = Minesweeper.InterfaceMode.Text;
                validInterf = true;
            }
            else {
                WriteStatusMessage("Invalid input. Please input your desired interface mode.");
            }
        } while (!validInterf);
        return (color, interf);
    }

    private static void EnsureWindowSize() {
        ConsoleElement expandMessage = new(0, 0, "");
        if (s_minesweeper.GameInterfaceMode == Minesweeper.InterfaceMode.Text) {
            bool SmallWidth() {
                return Console.WindowWidth <
                       int.Max(80, s_minesweeper.ColumnAmount * (s_minesweeper.ColumnAmount - 2).ToString().Length + 1);
            }

            bool SmallHeight() {
                return Console.WindowHeight < s_minesweeper.RowAmount + 10;
            }

            while (SmallWidth() || SmallHeight()) {
                ConsoleInterface.ClearConsoleElement(expandMessage);
                expandMessage =
                    new ConsoleElement(0, 0,
                                       $"Please expand your screen{(SmallWidth() && SmallHeight() ? " both horizontally and vertical" : SmallWidth() ? " horizontal" : " vertical")}ly.");
                ConsoleInterface.PrintConsoleElement(expandMessage);
                Thread.Sleep(10);
            }
        }
        else {
            bool SmallWidth() {
                return Console.WindowWidth < int.Max(80, s_minesweeper.ColumnAmount * 2);
            }

            bool SmallHeight() {
                return Console.WindowHeight < s_minesweeper.RowAmount + 2;
            }

            while (SmallWidth() || SmallHeight()) {
                ConsoleInterface.ClearConsoleElement(expandMessage);
                expandMessage =
                    new ConsoleElement(0, 0,
                                       $"Please expand your screen{(SmallWidth() && SmallHeight() ? " both horizontally and vertical" : SmallWidth() ? " horizontal" : " vertical")}ly.");
                ConsoleInterface.PrintConsoleElement(expandMessage);
                Thread.Sleep(10);
            }
        }
    }
}