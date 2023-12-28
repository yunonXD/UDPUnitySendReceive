public class DeviceProxy{
    public const int KEY_CORD_SIZE = 8;
    public const int MAX_LINE = 2024;

    //m_nMsgCount 현재 처리중인 키 이벤트의 갯수
    private static int messageCount;    //m_nMsgCount

    //m_ScanCode 현재 처리중인 키 이벤트 스캔코드를 저장하는 배열
    private static byte[] scanCode = new byte[KEY_CORD_SIZE];
    private static int bufferCount;

    private static PzMap<int, bool> Key_events = new PzMap<int, bool>();

    //buff 키 이벤트 저장할 공간
    private static byte[] buffer = new byte[MAX_LINE];
    private static int head, tail;

    public static PzMap<int, bool> KeyEvent {
        get { return Key_events; }
        set { Key_events = value; }
    }

    public static int MessageCount{
        get { return messageCount; }
        set { messageCount = value; }
    }

    public static byte[] ScanCode{
        get { return scanCode; }
        set { scanCode = value; }
    }

    public static int BufferCount{
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