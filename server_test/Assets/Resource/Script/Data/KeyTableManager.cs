using UnityEngine;

public class KeyTableManager : MonoBehaviour
{
    public struct KeyTableBase
    {
        public char[] name;
        public long make_val;
        public long break_val;
        public byte[] make_str;
        public int make_str_len;
        public byte[] break_str;
        public int break_str_len;

        public KeyTableBase(string aName, long aMakeVal, long aBreakVal)
        {
            name = new char[20];
            for (int i = 0; i < Mathf.Min(aName.Length, 20); i++)
            {
                name[i] = aName[i];
            }

            make_val = aMakeVal;
            break_val = aBreakVal;

            make_str = new byte[8];
            make_str_len = 0;
            break_str = new byte[8];
            break_str_len = 0;
        }
    }

    private KeyTableBase[] keyTables;
    private const int KEY_TABLE_SIZE = 13;

    void Start()
    {
        InitKeyTable();
    }

    void InitKeyTable()
    {
        keyTables = new KeyTableBase[KEY_TABLE_SIZE];

        keyTables[0] = new KeyTableBase("A", 0x1C, 0xF01C);
        keyTables[1] = new KeyTableBase("B", 0x32, 0xF032);
        keyTables[2] = new KeyTableBase("C", 0x21, 0xF021);
        keyTables[3] = new KeyTableBase("D", 0x23, 0xF023);
        keyTables[4] = new KeyTableBase("E", 0x24, 0xF024);
        keyTables[5] = new KeyTableBase("F", 0x2B, 0xF02B);
        keyTables[6] = new KeyTableBase("G", 0x34, 0xF034);
        keyTables[7] = new KeyTableBase("H", 0x33, 0xF033);
        keyTables[8] = new KeyTableBase("I", 0x43, 0xF043);
        keyTables[9] = new KeyTableBase("J", 0x3B, 0xF03B);
        keyTables[10] = new KeyTableBase("K", 0x3A, 0xF042);
        keyTables[11] = new KeyTableBase("L", 0x4B, 0xF04B);
        keyTables[12] = new KeyTableBase("M", 0x3A, 0xF03A);

        for (int i = 0; i < KEY_TABLE_SIZE; ++i)
        {
            keyTables[i].make_str_len = MakeKeyString(keyTables[i].make_str, keyTables[i].make_val);
            if (keyTables[i].make_str_len == 0)
            {
                //int a = 1;
            }
            keyTables[i].break_str_len = MakeKeyString(keyTables[i].break_str, keyTables[i].break_val);
        }

        //데이터 확인
        for(int i=0; i<KEY_TABLE_SIZE; ++i){
            Debug.Log(keyTables[i].name);
        }

    }

    int MakeKeyString(byte[] dest, long input)
    {
        // make input value to byte array
        byte[] temp = new byte[sizeof(long)];
        int len = 0;

        System.Buffer.BlockCopy(System.BitConverter.GetBytes(input), 0, temp, 0, sizeof(long));
        System.Array.Clear(dest, 0, dest.Length);

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == 0x00) break;

            dest[i] = temp[i];
            len++;
        }

        if (len == 0) return 0;

        // reverse destination buffer
        int j = 0;
        for (int i = len; i > 0; i--)
        {
            dest[j] = temp[i - 1];
            j++;
        }

        return len;
    }
}
