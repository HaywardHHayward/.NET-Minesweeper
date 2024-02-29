using System.Diagnostics;

namespace Minesweeper;

internal sealed class Minesweeper {
    #region ColorMode enum

    public enum ColorMode {
        None,
        Monochrome,
        Colored
    }

    #endregion

    #region InterfaceMode enum

    public enum InterfaceMode {
        None,
        Text,
        Keyboard
    }

    #endregion

    private readonly Board _board;
    internal readonly ConsoleElement<Board> ConsoleElement;

    public readonly InterfaceMode GameInterfaceMode;
    public bool LostGame;
    public bool WonGame;

    public Minesweeper(int row, int col, int mines, ColorMode cMode, InterfaceMode iMode) {
        _board = new Board(row, col, mines);
        GameInterfaceMode = iMode;
        ConsoleElement = cMode switch {
            ColorMode.Monochrome => GameInterfaceMode switch {
                InterfaceMode.Text => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringMonochromeText()),
                InterfaceMode.Keyboard => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringMonochromeKeyboard()),
                InterfaceMode.None => throw new ArgumentException("Minesweeper cannot have an interface mode of None."),
                _ => throw new UnreachableException()
            },
            ColorMode.Colored => GameInterfaceMode switch {
                InterfaceMode.Text => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringColoredText()),
                InterfaceMode.Keyboard => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringColoredKeyboard()),
                InterfaceMode.None => throw new ArgumentException("Minesweeper cannot have an interface mode of None."),
                _ => throw new UnreachableException()
            },
            ColorMode.None => throw new ArgumentException("Minesweeper cannot have a color mode of None."),
            _ => throw new UnreachableException()
        };
    }

    public int RowAmount => _board.RowAmount;
    public int ColumnAmount => _board.ColumnAmount;
    public int MineAmount => _board.MineAmount;
    public int FlagAmount => _board.FlagAmount;

    public bool IsValidRow(int row) {
        return _board.IsValidRow(row);
    }

    public bool IsValidColumn(int col) {
        return _board.IsValidColumn(col);
    }

    public void CheckTile(int row, int col) {
        _board.CheckTile(row, col);
        if (_board[row, col].IsMine && !_board[row, col].IsFlagged) {
            LostGame = true;
        }
    }

    public void FlagTile(int row, int col) {
        _board.FlagTile(row, col);
        if (_board.FlaggedAllMines) {
            WonGame = true;
        }
    }

    public bool IsFlagged(int row, int col) {
        return _board[row, col].IsFlagged;
    }

    public bool IsChecked(int row, int col) {
        return _board[row, col].IsChecked;
    }
}