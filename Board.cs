using System.Text;

namespace Minesweeper;

internal sealed class Board {
    private readonly Tile[,] _board;
    private readonly HashSet<Tile> _flaggedTiles;
    private readonly HashSet<Tile> _minedTiles;
    private readonly int? _seed;
    private readonly HashSet<Tile> _uncheckedTiles;
    private bool _firstCheck = true;
    public bool FoundAllMines => _minedTiles.SetEquals(_uncheckedTiles);
    public int RowAmount { get; }
    public int ColumnAmount { get; }
    public int MineAmount { get; }
    public int FlagAmount => _flaggedTiles.Count;

    public Board(int row, int col, int mines, int? seed = null) {
        _seed = seed;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(row, nameof(row));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(col, nameof(col));
        if (row * col <= 1) {
            throw new ArgumentException("Cannot have a one by one board.");
        }
        RowAmount = row;
        ColumnAmount = col;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(mines, nameof(mines));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(mines, row * col, nameof(mines));
        MineAmount = mines;
        _board = new Tile[RowAmount, ColumnAmount];
        _uncheckedTiles = new HashSet<Tile>(row * col);
        for (int r = 0; r < RowAmount; r++) {
            for (int c = 0; c < ColumnAmount; c++) {
                Tile tile = new(r, c);
                _board[r, c] = tile;
                _uncheckedTiles.Add(tile);
            }
        }
        _flaggedTiles = new HashSet<Tile>(MineAmount);
        _minedTiles = new HashSet<Tile>(MineAmount);
    }

    public Tile this[int i, int j] => _board[i, j];

    private void SetAllMines(int row, int col) {
        Random rand = _seed.HasValue ? new Random(_seed.Value) : new Random();
        while (_minedTiles.Count < MineAmount) {
            int randRow = rand.Next(RowAmount);
            int randColumn = rand.Next(ColumnAmount);
            bool validMine = true;
            if (MineAmount < RowAmount * ColumnAmount - 8) {
                foreach (Tile tile in SurroundingTiles(randRow, randColumn)) {
                    if (tile.Row + randRow == row && tile.Column + randColumn == col) {
                        validMine = false;
                    }
                }
            }
            else {
                if (randRow == row && randColumn == col) {
                    continue;
                }
            }
            if (!validMine) {
                continue;
            }
            _minedTiles.Add(_board[randRow, randColumn]);
        }
        foreach (Tile mine in _minedTiles) {
            mine.IsMine = true;
            foreach (Tile tile in SurroundingTiles(mine.Row, mine.Column)) {
                tile.SurroundingMines++;
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
                throw new
                    InvalidOperationException($"Flagged tile (Tile{tile.Row}, {tile.Column}) not in flagged tile set.");
            }
            _flaggedTiles.Remove(tile);
            tile.IsFlagged = false;
        }
        else {
            if (!_flaggedTiles.Add(tile)) {
                throw new
                    InvalidOperationException($"Unflagged tile (Tile{tile.Row}, {tile.Column}) already in flagged tile set.");
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

    public void CheckTile(int row, int col) {
        if (_firstCheck) {
            SetAllMines(row, col);
            _firstCheck = false;
        }
        Tile tile = _board[row, col];
        if (tile.IsChecked || tile.IsFlagged) {
            return;
        }
        if (tile is { IsMine: false, SurroundingMines: 0 }) {
            tile.IsChecked = true;
            if (!_uncheckedTiles.Remove(tile)) {
                throw new InvalidOperationException($"Tile {tile.Row}, {tile.Column} not in unchecked tile set.");
            }
            foreach (Tile tile1 in SurroundingTiles(row, col)) {
                CheckTile(tile1.Row, tile1.Column);
            }
        }
        else {
            if (!_uncheckedTiles.Remove(tile)) {
                throw new InvalidOperationException($"Tile {tile.Row}, {tile.Column} not in unchecked tile set.");
            }
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

    private IEnumerable<Tile> SurroundingTiles(int row, int col) {
        List<Tile> surroundingTiles = new(8);
        for (int r = -1; r <= 1; r++) {
            if (!IsValidRow(r + row)) {
                continue;
            }
            for (int c = -1; c <= 1; c++) {
                if (!IsValidColumn(c + col)) {
                    continue;
                }
                if (r == 0 && c == 0) {
                    continue;
                }
                surroundingTiles.Add(_board[row + r, col + c]);
            }
        }
        return surroundingTiles.AsEnumerable();
    }
}