namespace Minesweeper;

internal static partial class Program {
    private static Minesweeper _minesweeper = null!;

    private static void TextPlay() {
        while (true) {
            go_back:
            Position = (0, 0);
            Console.Clear();
            ConsoleInterface.PrintConsoleElement(_minesweeper.consoleElement);
            Position = (0, _minesweeper.RowAmount + 1);
            Console.WriteLine($"Mines remaining (according to number of tiles flagged): {
                _minesweeper.MineAmount - _minesweeper.FlagAmount}");
            int row;
            int col;
            bool validInput = false;
            bool checkingNotFlagging = false;
            do {
                Console.Write("Checking or flagging a tile?: ");
                string? input = Console.ReadLine()?.ToLower();
                switch (input) {
                    case "checking":
                    case "check":
                    case "c":
                        validInput = true;
                        checkingNotFlagging = true;
                        break;
                    case "flagging":
                    case "flag":
                    case "f":
                        validInput = true;
                        checkingNotFlagging = false;
                        break;
                    default:
                        WriteStatusMessage("Invalid input. Use (check/flag).");
                        break;
                }
            } while (!validInput);
            Position = (0, Position.y + 1);
            ConsoleInterface.ClearLastLine();
            validInput = false;
            do {
                Console.Write($"Enter row to {(checkingNotFlagging ? "check" : "flag")} (input 'back' to go back): ");
                string? input = Console.ReadLine();
                bool parsed = int.TryParse(input, out row);
                if (!parsed) {
                    if (input == "back") {
                        goto go_back;
                    }
                    WriteStatusMessage("Invalid input. Enter a nonnegative integer.");
                    continue;
                }
                if (!_minesweeper.IsValidRow(row)) {
                    WriteStatusMessage($"Row input out of the range of the board. Input a number in between 0 and {
                        _minesweeper.RowAmount - 1}.");
                    continue;
                }
                validInput = true;
            } while (!validInput);
            Position = (0, Position.y + 1);
            ConsoleInterface.ClearLastLine();
            validInput = false;
            do {
                Console.Write($"Enter column to {(checkingNotFlagging ? "check" : "flag")
                } (input 'back' to go back): ");
                string? input = Console.ReadLine();
                bool parsed = int.TryParse(input, out col);
                if (!parsed) {
                    if (input == "back") {
                        goto go_back;
                    }
                    WriteStatusMessage("Invalid input. Enter a nonnegative integer.");
                    continue;
                }
                if (!_minesweeper.IsValidColumn(col)) {
                    WriteStatusMessage($"Column input out of the range of the board. Input a number in between 0 and {
                        _minesweeper.ColumnAmount - 1}.");
                    continue;
                }
                validInput = true;
            } while (!validInput);
            Position = (0, Position.y + 1);
            ConsoleInterface.ClearLastLine();
            if (_minesweeper.IsChecked(row, col)) {
                Console.WriteLine($"Cannot {(checkingNotFlagging ? "check" : "flag")
                } a tile which has already been checked. Press any key to retry.");
                Console.ReadKey();
                goto go_back;
            }
            if (checkingNotFlagging) {
                if (_minesweeper.IsFlagged(row, col)) {
                    Console.WriteLine(
                        $"Cannot check a tile which has been flagged. Unflag the tile if you wish to check it. Press any key to retry.");
                    Console.ReadKey();
                    goto go_back;
                }
                _minesweeper.CheckTile(row, col);
                if (!_minesweeper.lostGame) {
                    continue;
                }
                Console.Clear();
                ConsoleInterface.PrintConsoleElement(_minesweeper.consoleElement);
                Position = (0, _minesweeper.RowAmount + 2);
                Console.WriteLine("Oops! You hit a mine! You lost! Press any key to exit...");
                Console.ReadKey();
                return;
            }
            _minesweeper.FlagTile(row, col);
            if (!_minesweeper.wonGame) {
                continue;
            }
            Console.Clear();
            ConsoleInterface.PrintConsoleElement(_minesweeper.consoleElement);
            Position = (0, _minesweeper.RowAmount + 2);
            Console.WriteLine("You flagged all the mines! You win! Press any key to exit...");
            Console.ReadKey();
            return;
        }
    }

    private static void KeyboardPlay() {
        ConsoleElement tutorial = new ConsoleElement(0, _minesweeper.RowAmount + 2, "Press C to check and F to flag.");
        ConsoleElement won = new ConsoleElement(0, _minesweeper.RowAmount + 1,
                                                "You flagged all the mines! You won! Press any key to exit...");
        ConsoleElement lost = new ConsoleElement(0, _minesweeper.RowAmount + 1,
                                                 "Oops! You hit a mine! You lost! Press any key to exit...");
        bool playing = true;
        InitializeKeyHandler();
        ConsoleInterface.PrintConsoleElement(tutorial);
        while (playing) {
            ConsoleElement minesRemaining = new ConsoleElement(0, _minesweeper.RowAmount + 1,
                                                               $"Number of mines remaining (according to flag count): {
                                                                   _minesweeper.MineAmount - _minesweeper.FlagAmount}");
            ConsoleInterface.PrintConsoleElement(_minesweeper.consoleElement);
            ConsoleInterface.PrintConsoleElement(minesRemaining);
            ConsoleKeyInfo info = Console.ReadKey(true);
            ConsoleInterface.DoKeyInput(info);
            ConsoleInterface.ClearConsoleElement(_minesweeper.consoleElement);
            ConsoleInterface.ClearConsoleElement(minesRemaining);
        }
        ConsoleInterface.PrintConsoleElement(_minesweeper.consoleElement);
        ConsoleInterface.ClearConsoleElement(tutorial);
        ConsoleInterface.PrintConsoleElement(_minesweeper.wonGame ? won : lost);
        Console.ReadKey();
        return;

        void InitializeKeyHandler() {
            ConsoleInterface.keyHandlerTable[ConsoleKey.LeftArrow] = () => Position = (Position.x - 2, Position.y);
            ConsoleInterface.keyHandlerTable[ConsoleKey.A] = () => Position = (Position.x - 2, Position.y);
            ConsoleInterface.keyHandlerTable[ConsoleKey.RightArrow] = () =>
                Position = (int.Min(Position.x + 2, (_minesweeper.ColumnAmount - 1) * 2), Position.y);
            ConsoleInterface.keyHandlerTable[ConsoleKey.D] = () =>
                Position = (int.Min(Position.x + 2, (_minesweeper.ColumnAmount - 1) * 2), Position.y);
            ConsoleInterface.keyHandlerTable[ConsoleKey.UpArrow] = () => Position = (Position.x, Position.y - 1);
            ConsoleInterface.keyHandlerTable[ConsoleKey.W] = () => Position = (Position.x, Position.y - 1);
            ConsoleInterface.keyHandlerTable[ConsoleKey.DownArrow] = () =>
                Position = (Position.x, int.Min(Position.y + 1, _minesweeper.RowAmount - 1));
            ConsoleInterface.keyHandlerTable[ConsoleKey.D] = () =>
                Position = (Position.x, int.Min(Position.y + 1, _minesweeper.RowAmount - 1));
            ConsoleInterface.keyHandlerTable[ConsoleKey.C] = CheckTile;
            ConsoleInterface.keyHandlerTable[ConsoleKey.F] = FlagTile;
        }

        void CheckTile() {
            int row = Position.y;
            int col = Position.x / 2;
            _minesweeper.CheckTile(row, col);
            if (_minesweeper.lostGame) {
                playing = false;
            }
        }

        void FlagTile() {
            int row = Position.y;
            int col = Position.x / 2;
            _minesweeper.FlagTile(row, col);
            if (_minesweeper.wonGame) {
                playing = false;
            }
        }
    }
}