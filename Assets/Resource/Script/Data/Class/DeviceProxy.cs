public class DeviceProxy{
    public const int KEY_CORD_SIZE = 8;
    public const int MAX_LINE = 2024;

    private static int messageCount;
    private static byte[] scanCode = new byte[KEY_CORD_SIZE];
    private static uint bufferCount;
    private static byte[] buffer = new byte[MAX_LINE];
    private static int head, tail;

    public static int MessageCount{
        get { return messageCount; }
        set { messageCount = value; }
    }

    public static byte[] ScanCode{
        get { return scanCode; }
        set { scanCode = value; }
    }

    public static uint BufferCount{
        get { return bufferCount; }
        set { bufferCount = value; }
    }

    public static byte[] Buffer{
        get { return buffer; }
        set { buffer = value; }
    }

    public static int Head{
        get { return head; }
        set { head = value; }
    }

    public static int Tail{
        get { return tail; }
        set { tail = value; }
    }
}