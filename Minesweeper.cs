using System.Diagnostics;

namespace Minesweeper;

public class Minesweeper {
    #region ColorMode enum

    public enum ColorMode { None, Monochrome, Colored }

    #endregion

    #region InterfaceMode enum

    public enum InterfaceMode { None, Text, Keyboard }

    #endregion

    private readonly Board _board;
    internal readonly ConsoleElement<Board> consoleElement;

    public readonly InterfaceMode interfaceMode;
    public bool lostGame;
    public bool wonGame;

    public Minesweeper(int row, int col, int mines, ColorMode cMode, InterfaceMode iMode) {
        _board = new Board(row, col, mines);
        interfaceMode = iMode;
        consoleElement = cMode switch {
            ColorMode.Monochrome => interfaceMode switch {
                InterfaceMode.Text => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringMonochromeText()),
                InterfaceMode.Keyboard => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringMonochromeKeyboard()),
                InterfaceMode.None => throw new UnreachableException(),
                _ => throw new UnreachableException()
            },
            ColorMode.Colored => interfaceMode switch {
                InterfaceMode.Text => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringColoredText()),
                InterfaceMode.Keyboard => new ConsoleElement<Board>(0, 0, _board, s => s.ToStringColoredKeyboard()),
                InterfaceMode.None => throw new UnreachableException(),
                _ => throw new UnreachableException()
            },
            ColorMode.None => throw new UnreachableException(),
            _ => throw new UnreachableException()
        };
    }

    public int RowAmount => _board.RowAmount;
    public int ColumnAmount => _board.ColumnAmount;
    public int MineAmount => _board.MineAmount;
    public int FlagAmount => _board.FlagAmount;
    public bool IsValidRow(int row) => _board.IsValidRow(row);
    public bool IsValidColumn(int col) => _board.IsValidColumn(col);

    public void CheckTile(int row, int col) {
        _board.CheckTile(row, col);
        if (_board[row, col].IsMine) {
            lostGame = true;
        }
    }

    public void FlagTile(int row, int col) {
        _board.FlagTile(row, col);
        if (_board.FlaggedAllMines) {
            wonGame = true;
        }
    }

    public bool IsMine(int row, int col) {
        return _board[row, col].IsMine;
    }

    public bool IsFlagged(int row, int col) {
        return _board[row, col].IsFlagged;
    }

    public bool IsChecked(int row, int col) {
        return _board[row, col].IsChecked;
    }
}