// Hayden Reckward (c) 2024

namespace Minesweeper;

public class Tile(byte row, byte column) {
    private TileState _state = new();
    public byte Row { get; } = row;
    public byte Column { get; } = column;
    public byte SurroundingMines => _state.SurroundingMines;
    public bool IsMine => _state.Mine;
    public bool IsChecked => _state.Checked;
    public bool IsFlagged => _state.Flagged;

    public void Increment() {
        if (_state.SurroundingMines < 8) {
            _state.SurroundingMines++;
        }
    }

    public void BecomeMined() {
        if (!_state.Checked) {
            _state.Mine = true;
        }
    }

    public void Check() {
        if (!_state.Flagged) {
            _state.Checked = true;
        }
    }

    public void ToggleFlag() {
        if (!_state.Checked) {
            _state.Flagged = !_state.Flagged;
        }
    }

    private struct TileState() {
        private byte _internalValue = 0b0000_0000;

        public byte SurroundingMines {
            get => (byte)(_internalValue & 0xF);
            set => _internalValue = (byte)((_internalValue & 0xF0) | (value & 0x0F));
        }

        public bool Mine {
            get => (_internalValue & (1 << 5)) == 1 << 5;
            set => _internalValue = (byte)((_internalValue & 0b1110_1111) | (value ? 1 << 5 : 0));
        }

        public bool Checked {
            get => (_internalValue & (1 << 6)) == 1 << 6;
            set => _internalValue = (byte)((_internalValue & 0b1101_1111) | (value ? 1 << 6 : 0));
        }

        public bool Flagged {
            get => (_internalValue & (1 << 7)) == 1 << 7;
            set => _internalValue = (byte)((_internalValue & 0b1011_1111) | (value ? 1 << 7 : 0));
        }
    }
}