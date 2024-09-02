using Minesweeper;

namespace MinesweeperTests;

public class TileTests {
    
    [Test]
    public void TileRow() {
        for (byte i = 0; i < byte.MaxValue; i++) {
            Tile tile = new(i, 0);
            Assert.That(tile.Row, Is.EqualTo(i));
        }
    }

    [Test]
    public void TileColumn() {
        for (byte i = 0; i < byte.MaxValue; i++) {
            Tile tile = new(0, i);
            Assert.That(tile.Column, Is.EqualTo(i));
        }
    }

    [Test]
    public void TileMined() {
        Tile uncheckedTile = new(0, 0);
        Tile checkedTile = new(0, 0);
        Assert.False(uncheckedTile.IsMine);
        Assert.False(checkedTile.IsMine);
        checkedTile.Check();
        uncheckedTile.BecomeMined();
        checkedTile.BecomeMined();
        Assert.True(uncheckedTile.IsMine);
        Assert.False(checkedTile.IsMine);
    }

    [Test]
    public void TileIncrement() {
        Tile tile = new(0, 0);
        for (byte i = 0; i < 9; i++) {
            Assert.That(tile.SurroundingMines, Is.EqualTo(i));
            tile.Increment();
        }
    }

    [Test]
    public void TileCheck() {
        Tile checkedTile = new(0, 0);
        Tile flaggedTile = new(0, 0);
        Assert.False(checkedTile.IsChecked);
        Assert.False(flaggedTile.IsChecked);
        flaggedTile.ToggleFlag();
        checkedTile.Check();
        flaggedTile.Check();
        Assert.True(checkedTile.IsChecked);
        Assert.False(flaggedTile.IsChecked);
    }

    [Test]
    public void TileFlag() {
        Tile flaggedTile = new(0, 0);
        Tile checkedTile = new(0, 0);
        Assert.False(flaggedTile.IsFlagged);
        Assert.False(checkedTile.IsFlagged);
        checkedTile.Check();
        flaggedTile.ToggleFlag();
        checkedTile.ToggleFlag();
        Assert.False(checkedTile.IsFlagged);
        Assert.True(flaggedTile.IsFlagged);
    }
}