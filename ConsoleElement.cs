namespace Minesweeper;

internal readonly struct ConsoleElement(int x, int y, string message) : IElement {
    public string Message { get; } = message;
    public int ElementX { get; } = x;
    public int ElementY { get; } = y;
}

internal readonly struct ConsoleElement<T>(int x, int y, T obj, ConsoleElement<T>.StringDelegate method) : IElement {
    public delegate string StringDelegate(T obj);

    public int ElementX { get; } = x;
    public int ElementY { get; } = y;
    public string Message => method(obj);
}

internal interface IElement {
    public int ElementX { get; }
    public int ElementY { get; }
    public string Message { get; }
}