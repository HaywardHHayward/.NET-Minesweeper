using System.Drawing;
using Pastel;

namespace Minesweeper;

internal sealed class Tile : IEquatable<Tile> {
    private readonly int _column;
    private readonly int _row;

    // A byte representing the properties of a tile, the three least significant bits represent
    // whether the tile is a mine, is checked, or has been flagged from least to most significant
    // respectively. Then the four most significant bits represent how many mines are surrounding
    // the tile with a straight conversion, i.e. 0b0000 is zero, 0b0001 is one, so on and so forth.
    private byte _tileProperties;

    public int Row {
        get => _row;
        init {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Row));
            _row = value;
        }
    }

    public int Column {
        get => _column;
        init {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(Column));
            _column = value;
        }
    }

    public bool IsMine {
        get => (_tileProperties & Masks.MineMask) == Masks.MineMask;
        internal set {
            if (value) {
                _tileProperties |= Masks.MineMask;
            }
            else {
                _tileProperties &= Masks.MineInverse;
            }
        }
    }

    public bool IsChecked {
        get => (_tileProperties & Masks.CheckMask) == Masks.CheckMask;
        internal set {
            if (value) {
                _tileProperties |= Masks.CheckMask;
            }
            else {
                _tileProperties &= Masks.CheckInverse;
            }
        }
    }

    public bool IsFlagged {
        get => (_tileProperties & Masks.FlagMask) == Masks.FlagMask;
        internal set {
            if (value) {
                _tileProperties |= Masks.FlagMask;
            }
            else {
                _tileProperties &= Masks.FlagInverse;
            }
        }
    }

    public byte SurroundingMines {
        get => Convert.ToByte((_tileProperties & Masks.SurroundingMask) >> Masks.SurroundingOffset);
        internal set {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, 9, nameof(SurroundingMines));
            _tileProperties &= Masks.SurroundingInverse;
            _tileProperties |= Convert.ToByte(value << Masks.SurroundingOffset);
        }
    }

    public Tile(int row, int col) {
        Row = row;
        Column = col;
    }

    public bool Equals(Tile? other) {
        return other != null &&
               _column == other._column &&
               _row == other._row &&
               _tileProperties == other._tileProperties;
    }

    public string ToStringMonochrome() {
        if (IsFlagged) {
            return "P";
        }
        if (!IsChecked) {
            return "O";
        }
        if (IsMine) {
            return "X";
        }
        return SurroundingMines == 0 ? " " : SurroundingMines.ToString();
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) {
            return false;
        }
        if (ReferenceEquals(this, obj)) {
            return true;
        }
        return obj is Tile tile && Equals(tile);
    }

    public override int GetHashCode() {
        return HashCode.Combine(_column, _row);
    }

    public string ToStringColored() {
        if (IsFlagged) {
            return "P".Pastel(Color.Red).PastelBg(Color.Silver);
        }
        if (!IsChecked) {
            return " ".PastelBg(Color.Silver);
        }
        if (IsMine) {
            return "X".Pastel(Color.Black).PastelBg(Color.Gray);
        }
        return SurroundingMines switch {
            0 => " ".PastelBg(Color.Gray),
            1 => "1".Pastel(Color.Blue).PastelBg(Color.Gray),
            2 => "2".Pastel(Color.DarkGreen).PastelBg(Color.Gray),
            3 => "3".Pastel(Color.DarkRed).PastelBg(Color.Gray),
            4 => "4".Pastel(Color.Indigo).PastelBg(Color.Gray),
            5 => "5".Pastel(Color.Maroon).PastelBg(Color.Gray),
            6 => "6".Pastel(Color.DarkCyan).PastelBg(Color.Gray),
            7 => "7".Pastel(Color.SaddleBrown).PastelBg(Color.Gray),
            8 => "8".Pastel(Color.LightGray).PastelBg(Color.Gray),
            _ => throw new ArgumentOutOfRangeException(nameof(SurroundingMines),
                                                       $"{nameof(Tile)}.{nameof(SurroundingMines)} invariant has been violated. Impossible value has been achieved. Value of {nameof(SurroundingMines)}: {SurroundingMines}")
        };
    }

    public static bool operator ==(Tile? left, Tile? right) {
        return Equals(left, right);
    }

    public static bool operator !=(Tile? left, Tile? right) {
        return !Equals(left, right);
    }

    private static class Masks {
        public const byte MineMask = 0b1 << 0;
        public const byte MineInverse = 0xFF ^ MineMask;
        public const byte CheckMask = 0b1 << 1;
        public const byte CheckInverse = 0xFF ^ CheckMask;
        public const byte FlagMask = 0b1 << 2;
        public const byte FlagInverse = 0xFF ^ FlagMask;
        public const byte SurroundingMask = 0b1111 << SurroundingOffset;
        public const byte SurroundingInverse = 0xFF ^ SurroundingMask;
        public const byte SurroundingOffset = 4;
    }
}