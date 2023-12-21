using UnityEngine;
//오버플로우 주의!! 테스트중!!!


namespace TABLE{
//start if table

public struct KeyTableBase{
    public char[] name;
    public long make_val;
    public long break_val;
    public byte[] make_str;
    public int make_str_len;
    public byte[] break_str;
    public int break_str_len;

    public KeyTableBase(string aName, long aMakeVal, long aBreakVal){
        name = new char[20];
        for (int i = 0; i < Mathf.Min(aName.Length, 20); i++){
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



public class KeyTable
{
    public KeyTableBase[] key_tables;

    public const int KEY_TABLE_SIZE = 13;

    public KeyTable(){
        key_tables = new KeyTableBase[KEY_TABLE_SIZE];

        key_tables[0] = new KeyTableBase("A", 0x1C, 0xF01C);
        key_tables[1] = new KeyTableBase("B", 0x32, 0xF032);
        key_tables[2] = new KeyTableBase("C", 0x21, 0xF021);
        key_tables[3] = new KeyTableBase("D", 0x23, 0xF023);
        key_tables[4] = new KeyTableBase("E", 0x24, 0xF024);
        key_tables[5] = new KeyTableBase("F", 0x2B, 0xF02B);
        key_tables[6] = new KeyTableBase("G", 0x34, 0xF034);
        key_tables[7] = new KeyTableBase("H", 0x33, 0xF033);
        key_tables[8] = new KeyTableBase("I", 0x43, 0xF043);
        key_tables[9] = new KeyTableBase("J", 0x3B, 0xF03B);
        key_tables[10] = new KeyTableBase("K", 0x3A, 0xF042);
        key_tables[11] = new KeyTableBase("L", 0x4B, 0xF04B);
        key_tables[12] = new KeyTableBase("M", 0x3A, 0xF03A);
    }
}

//end of table
}