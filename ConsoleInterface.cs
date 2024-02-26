namespace Minesweeper;

public static class ConsoleInterface {
    #region Delegates

    public delegate void KeyHandler();

    #endregion

    public static readonly Dictionary<ConsoleKey, KeyHandler>
        keyHandlerTable = new Dictionary<ConsoleKey, KeyHandler>();

    static ConsoleInterface() {
        foreach (ConsoleKey key in Enum.GetValues<ConsoleKey>()) {
            keyHandlerTable.Add(key, () => { });
        }
    }

    public static (int x, int y) CursorPosition {
        get => Console.GetCursorPosition();
        set {
            (int x, int y) newPosition = value;
            if (newPosition.x < 0) {
                newPosition.x = 0;
            }
            else if (newPosition.x > Console.BufferWidth) {
                newPosition.x = Console.BufferWidth;
            }
            if (newPosition.y < 0) {
                newPosition.y = 0;
            }
            else if (newPosition.y > Console.BufferHeight) {
                newPosition.y = Console.BufferHeight;
            }
            Console.SetCursorPosition(newPosition.x, newPosition.y);
        }
    }

    public static void DoKeyInput(ConsoleKeyInfo info) {
        ConsoleKey key = info.Key;
        keyHandlerTable[key]();
    }

    public static void ClearLastLine() {
        (int, int) originalPosition = CursorPosition;
        CursorPosition = (0, CursorPosition.y - 1);
        Console.Write(new string(' ', Console.BufferWidth));
        CursorPosition = originalPosition;
    }

    public static void PrintConsoleElement(ConsoleElement element) {
        (int x, int y) originalCursor = CursorPosition;
        string[] stringList = element.Message.Split('\n').ToArray();
        for (int i = 0; i < stringList.Length; i++) {
            CursorPosition = (element.ElementX, element.ElementY + i);
            Console.Write(stringList[i]);
        }
        CursorPosition = originalCursor;
    }

    public static void PrintConsoleElement<T>(ConsoleElement<T> element) {
        (int x, int y) originalCursor = CursorPosition;
        string[] stringList = element.Message.Split('\n').ToArray();
        for (int i = 0; i < stringList.Length; i++) {
            CursorPosition = (element.ElementX, element.ElementY + i);
            Console.Write(stringList[i]);
        }
        CursorPosition = originalCursor;
    }

    public static void ClearConsoleElement(ConsoleElement element) {
        (int x, int y) originalCursor = CursorPosition;
        int[] lengthList = element.Message.Split('\n').Select(s => s.Length).ToArray();
        for (int i = 0; i < lengthList.Length; i++) {
            CursorPosition = (element.ElementX, element.ElementY + i);
            Console.Write(new string(' ', lengthList[i]));
        }
        CursorPosition = originalCursor;
    }

    public static void ClearConsoleElement<T>(ConsoleElement<T> element) {
        (int x, int y) originalCursor = CursorPosition;
        int[] lengthList = element.Message.Split('\n').Select(s => s.Length).ToArray();
        for (int i = 0; i < lengthList.Length; i++) {
            CursorPosition = (element.ElementX, element.ElementY + i);
            Console.Write(new string(' ', lengthList[i]));
        }
        CursorPosition = originalCursor;
    }

    public static ConsoleElement MoveConsoleElement(int x, int y, ConsoleElement element) {
        (int x, int y) originalCursor = CursorPosition;
        ClearConsoleElement(element);
        ConsoleElement newElement = new ConsoleElement(x, y, element.Message);
        PrintConsoleElement(newElement);
        CursorPosition = originalCursor;
        return newElement;
    }

    public static ConsoleElement<T> MoveConsoleElement<T>(int x, int y, ConsoleElement<T> element) {
        (int x, int y) originalCursor = CursorPosition;
        ClearConsoleElement(element);
        ConsoleElement<T> newElement = new ConsoleElement<T>(x, y, element.obj, element.stringHandler);
        PrintConsoleElement(newElement);
        CursorPosition = originalCursor;
        return newElement;
    }
}

public readonly struct ConsoleElement(int x, int y, string message) {
    public string Message { get; init; } = message;

    public int ElementX { get; init; } = x;

    public int ElementY { get; init; } = y;
}

public readonly struct ConsoleElement<T>(int x, int y, T obj, ConsoleElement<T>.StringDelegate method) {
    public delegate string StringDelegate(T obj);

    public readonly StringDelegate stringHandler = method;

    public readonly T obj = obj;

    public int ElementX { get; init; } = x;

    public int ElementY { get; init; } = y;

    public string Message => stringHandler(obj);
}