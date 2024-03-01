using System.Text;

namespace Minesweeper;

internal sealed class Board {
    private readonly Tile[,] _board;
    private readonly HashSet<Tile> _flaggedTiles;
    private readonly HashSet<Tile> _minedTiles;
    private readonly int? _seed;
    private bool _firstCheck = true;

    public Board(int row, int col, int mines, int? seed = null) {
        _seed = seed;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(row, nameof(row));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(col, nameof(col));
        if (row * col <= 9) {
            throw new ArgumentException($"{nameof(row)} and {nameof(col)} cannot multiply to be less than 9.");
        }
        RowAmount = row;
        ColumnAmount = col;
        if (mines <= 0 || mines >= RowAmount * ColumnAmount - 8) {
            throw new ArgumentOutOfRangeException(nameof(mines),
                $"{nameof(mines)} must be in between 1 and ({row * col - 8}) exclusive. Valued supplied: {mines}");
        }
        MineAmount = mines;
        _board = new Tile[RowAmount, ColumnAmount];
        for (int r = 0; r < RowAmount; r++) {
            for (int c = 0; c < ColumnAmount; c++) {
                _board[r, c] = new Tile(r, c);
            }
        }
        _flaggedTiles = new HashSet<Tile>(MineAmount);
        _minedTiles = new HashSet<Tile>(MineAmount);
    }

    public bool FlaggedAllMines => _flaggedTiles.SetEquals(_minedTiles);

    public Tile this[int i, int j] => _board[i, j];

    public int RowAmount { get; }

    public int ColumnAmount { get; }
    public int MineAmount { get; }

    public int FlagAmount => _flaggedTiles.Count;

    private void SetAllMines(int row, int col) {
        Random rand = _seed.HasValue ? new Random(_seed.Value) : new Random();
        while (_minedTiles.Count < MineAmount) {
        new_tile:
            int randRow = rand.Next(RowAmount);
            int randColumn = rand.Next(ColumnAmount);
            for (int r = -1; r <= 1; r++) {
                if (!IsValidRow(r + randRow)) {
                    continue;
                }
                for (int c = -1; c <= 1; c++) {
                    if (!IsValidColumn(c + randColumn)) {
                        continue;
                    }
                    if (r + randRow == row && c + randColumn == col) {
                        goto new_tile;
                    }
                }
            }
            _minedTiles.Add(_board[randRow, randColumn]);
        }
        foreach (Tile tile in _minedTiles) {
            tile.IsMine = true;
            for (int i = -1; i <= 1; i++) {
                if (!IsValidRow(tile.Row + i)) {
                    continue;
                }
                for (int j = -1; j <= 1; j++) {
                    if (!IsValidColumn(tile.Column + j)) {
                        continue;
                    }
                    if (i == 0 && j == 0) {
                        continue;
                    }
                    _board[tile.Row + i, tile.Column + j].SurroundingMines++;
                }
            }
        }
    }

    public void FlagTile(int row, int col) {
        Tile tile = _board[row, col];
        if (tile.IsChecked) {
            return;
        }
        if (tile.IsFlagged) {
            if (!_flaggedTiles.Contains(tile)) {
                throw new InvalidOperationException(
                    $"Flagged tile (Tile{tile.Row}, {tile.Column}) not in flagged tile set.");
            }
            _flaggedTiles.Remove(tile);
            tile.IsFlagged = false;
        }
        else {
            if (!_flaggedTiles.Add(tile)) {
                throw new InvalidOperationException(
                    $"Unflagged tile (Tile{tile.Row}, {tile.Column}) already in flagged tile set.");
            }
            tile.IsFlagged = true;
        }
    }

    public bool IsValidRow(int row) {
        return row >= 0 && row < RowAmount;
    }

    public bool IsValidColumn(int col) {
        return col >= 0 && col < ColumnAmount;
    }

    private void CheckSurroundingTiles(int row, int col) {
        for (int r = -1; r <= 1; r++) {
            if (!IsValidRow(row + r)) {
                continue;
            }
            for (int c = -1; c <= 1; c++) {
                if (!IsValidColumn(col + c)) {
                    continue;
                }
                if (r == 0 && c == 0) {
                    continue;
                }
                CheckTile(row + r, col + c);
            }
        }
    }

    public void CheckTile(int row, int col) {
        if (_firstCheck) {
            SetAllMines(row, col);
            _firstCheck = false;
        }
        Tile tile = _board[row, col];
        if (tile.IsChecked) {
            return;
        }
        if (tile.IsFlagged) {
            return;
        }
        if (tile is { IsMine: false, SurroundingMines: 0 }) {
            tile.IsChecked = true;
            CheckSurroundingTiles(row, col);
        }
        else {
            tile.IsChecked = true;
        }
    }

    public string ToStringMonochromeText() {
        int rowLength = (RowAmount - 1).ToString().Length;
        // Subtracting by two because when only the last digit is longer that the previous digits, we can just use
        // the previous digits length since only the last character would benefit from the expanded padding.
        // We can do this by subtracting two from the ColumnAmount, since it'll skip over the last value.
        int colLength = int.Max(ColumnAmount - 2, 0).ToString().Length;
        const int boardPadding = 1;
        StringBuilder output = new();
        for (int r = -1; r < RowAmount; r++) {
            if (r == -1) {
                output.Append(' ', rowLength + boardPadding);
            }
            if (r != -1) {
                output.Append(r.ToString().PadLeft(rowLength).PadRight(rowLength + boardPadding));
            }
            for (int c = 0; c < ColumnAmount; c++) {
                output.Append(r == -1
                    ? c.ToString().PadRight(colLength + boardPadding)
                    : _board[r, c].ToStringMonochrome().PadRight(colLength + boardPadding));
            }
            if (r != RowAmount - 1) {
                output.AppendLine();
            }
        }
        return output.ToString();
    }

    public string ToStringColoredText() {
        int rowLength = (RowAmount - 1).ToString().Length;
        // Subtracting by two because when only the last digit is longer that the previous digits, we can just use
        // the previous digits length since only the last character would benefit from the expanded padding.
        // We can do this by subtracting two from the ColumnAmount, since it'll skip over the last value.
        int colLength = int.Max(ColumnAmount - 2, 0).ToString().Length;
        const int boardPadding = 1;
        StringBuilder output = new();
        for (int r = -1; r < RowAmount; r++) {
            if (r != -1) {
                output.Append(r.ToString().PadLeft(rowLength).PadRight(rowLength + boardPadding));
            }
            if (r == -1) {
                output.Append(' ', rowLength + boardPadding);
            }
            for (int c = 0; c < ColumnAmount; c++) {
                if (r == -1) {
                    output.Append(c.ToString().PadRight(colLength + boardPadding));
                }
                else
                    // Pastel adds extra characters to the string, which we need to account for when formatting,
                    // which is why we add Tile.ToStringColored().Length - Tile.ToString().Length to the 
                    // padding width, to maintain consistency with Board.ToString().
                {
                    output.Append(_board[r, c]
                                  .ToStringColored()
                                  .PadRight(colLength +
                                            boardPadding +
                                            (_board[r, c].ToStringColored().Length -
                                             _board[r, c].ToStringMonochrome().Length)));
                }
            }
            if (r != RowAmount - 1) {
                output.AppendLine();
            }
        }
        return output.ToString();
    }

    public string ToStringMonochromeKeyboard() {
        StringBuilder output = new();
        for (int r = 0; r < RowAmount; r++) {
            for (int c = 0; c < ColumnAmount; c++) {
                output.Append(_board[r, c].ToStringMonochrome() + " ");
            }
            if (r != RowAmount - 1) {
                output.AppendLine();
            }
        }
        return output.ToString();
    }

    public string ToStringColoredKeyboard() {
        StringBuilder output = new();
        for (int r = 0; r < RowAmount; r++) {
            for (int c = 0; c < ColumnAmount; c++) {
                output.Append(_board[r, c].ToStringColored() + " ");
            }
            if (r != RowAmount - 1) {
                output.AppendLine();
            }
        }
        return output.ToString();
    }
}