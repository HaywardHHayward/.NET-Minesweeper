namespace Minesweeper;

internal static class ConsoleInterface {
    public delegate void KeyHandler();

    public static readonly Dictionary<ConsoleKey, KeyHandler> KeyHandlerTable = new();

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

    static ConsoleInterface() {
        foreach (ConsoleKey key in Enum.GetValues<ConsoleKey>()) {
            KeyHandlerTable.Add(key, () => { });
        }
    }

    public static void DoKeyInput(ConsoleKeyInfo info) {
        ConsoleKey key = info.Key;
        KeyHandlerTable[key]();
    }

    public static void ClearLastLine() {
        (int, int) originalPosition = CursorPosition;
        CursorPosition = (0, CursorPosition.y - 1);
        Console.Write(new string(' ', int.Max(Console.BufferWidth, Console.WindowWidth)));
        CursorPosition = originalPosition;
    }

    public static void PrintConsoleElement(IElement element) {
        (int x, int y) originalCursor = CursorPosition;
        string[] stringList = element.Message.Split('\n').ToArray();
        for (int i = 0; i < stringList.Length; i++) {
            CursorPosition = (element.ElementX, element.ElementY + i);
            Console.Write(stringList[i]);
        }
        CursorPosition = originalCursor;
    }

    public static void PrintConsoleElements(IEnumerable<IElement> elements) {
        foreach (IElement element in elements) {
            PrintConsoleElement(element);
        }
    }

    public static void ClearConsoleElement(IElement element) {
        (int x, int y) originalCursor = CursorPosition;
        int[] lengthList = element.Message.Split('\n').AsParallel().Select(s => s.Length).ToArray();
        for (int i = 0; i < lengthList.Length; i++) {
            CursorPosition = (element.ElementX, element.ElementY + i);
            Console.Write(new string(' ', lengthList[i]));
        }
        CursorPosition = originalCursor;
    }

    public static void ClearConsoleElements(IEnumerable<IElement> elements) {
        foreach (IElement element in elements) {
            ClearConsoleElement(element);
        }
    }
}