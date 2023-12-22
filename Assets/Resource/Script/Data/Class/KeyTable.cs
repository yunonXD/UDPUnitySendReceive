using System.Runtime.InteropServices;
using UnityEngine;

public class KeyTable{
    public string name = new string(' ', 20);
    public long make_val;
    public long break_val;
    public byte scan_key;
    public byte os_vk_key;      //no using this time dummy data

    public byte[] make_str = new byte[8];
    public int make_str_len;
    public byte[] break_str = new byte[8];
    public int break_str_len;

    public KeyTable(){
        System.Array.Clear(name.ToCharArray(), 0, name.Length);
        make_val = 0;
        break_val = 0;
        scan_key = 0;
        os_vk_key = 0;
    }

    public KeyTable(string aName, long aMakeVal, long aBreakVal, byte aScanKey, byte aOSVirtualKey = 0){
        System.Array.Clear(name.ToCharArray(), 0, name.Length);
        if (!string.IsNullOrEmpty(aName)){
            int length = Mathf.Min(aName.Length, name.Length);
            System.Array.Copy(aName.ToCharArray(), name.ToCharArray(), length);
        }

        make_val = aMakeVal;
        break_val = aBreakVal;
        scan_key = aScanKey;
        os_vk_key = aOSVirtualKey;
    }
}

public static class KeyTables{
    public const int KEY_TABLE_SIZE = 35;

    public static KeyTable[] key_tables = new KeyTable[KEY_TABLE_SIZE];

    static KeyTables(){
        InitializeKeyTables();
    }

    static void InitializeKeyTables(){
        key_tables = new KeyTable[]{
            new KeyTable("A", 0x1C, 0xF01C, Scan_Code.SCAN_A, (byte)'A'),
            new KeyTable("B", 0x32, 0xF032, Scan_Code.SCAN_B, (byte)'B'),
            new KeyTable("C", 0x21, 0xF021, Scan_Code.SCAN_C, (byte)'C'),
            new KeyTable("D", 0x23, 0xF023, Scan_Code.SCAN_D, (byte)'D'),
            new KeyTable("E", 0x24, 0xF024, Scan_Code.SCAN_E, (byte)'E'),
            new KeyTable("F", 0x2B, 0xF02B, Scan_Code.SCAN_F, (byte)'F'),
            new KeyTable("G", 0x34, 0xF034, Scan_Code.SCAN_G, (byte)'G'),
            new KeyTable("H", 0x33, 0xF033, Scan_Code.SCAN_H, (byte)'H'),
            new KeyTable("I", 0x43, 0xF043, Scan_Code.SCAN_I, (byte)'I'),
            new KeyTable("J", 0x3B, 0xF03B, Scan_Code.SCAN_J, (byte)'J'),
            new KeyTable("K", 0x42, 0xF042, Scan_Code.SCAN_K, (byte)'K'),
            new KeyTable("L", 0x4B, 0xF04B, Scan_Code.SCAN_L, (byte)'L'),
            new KeyTable("M", 0x3A, 0xF03A, Scan_Code.SCAN_M, (byte)'M'),
            new KeyTable("N", 0x31, 0xF031, Scan_Code.SCAN_N, (byte)'N'),
            new KeyTable("O", 0x44, 0xF044, Scan_Code.SCAN_O, (byte)'O'),
            new KeyTable("P", 0x4D, 0xF04D, Scan_Code.SCAN_P, (byte)'P'),
            new KeyTable("Q", 0x15, 0xF015, Scan_Code.SCAN_Q, (byte)'Q'),
            new KeyTable("R", 0x2D, 0xF02D, Scan_Code.SCAN_R, (byte)'R'),
            new KeyTable("S", 0x1B, 0xF01B, Scan_Code.SCAN_S, (byte)'S'),
            new KeyTable("T", 0x2C, 0xF02C, Scan_Code.SCAN_T, (byte)'T'),
            new KeyTable("U", 0x3C, 0xF03C, Scan_Code.SCAN_U, (byte)'U'),
            new KeyTable("V", 0x2A, 0xF02A, Scan_Code.SCAN_V, (byte)'V'),
            new KeyTable("W", 0x1D, 0xF01D, Scan_Code.SCAN_W, (byte)'W'),
            new KeyTable("X", 0x22, 0xF022, Scan_Code.SCAN_X, (byte)'X'),
            new KeyTable("Y", 0x35, 0xF035, Scan_Code.SCAN_Y, (byte)'Y'),
            new KeyTable("Z", 0x1A, 0xF01A, Scan_Code.SCAN_Z, (byte)'Z'),
            new KeyTable("0", 0x45, 0xF045, Scan_Code.SCAN_0, (byte)'0'),
            new KeyTable("1", 0x16, 0xF016, Scan_Code.SCAN_1, (byte)'1'),
            new KeyTable("2", 0x1E, 0xF01E, Scan_Code.SCAN_2, (byte)'2'),
            new KeyTable("3", 0x26, 0xF026, Scan_Code.SCAN_3, (byte)'3'),
            new KeyTable("4", 0x25, 0xF025, Scan_Code.SCAN_4, (byte)'4'),
            new KeyTable("5", 0x2E, 0xF02E, Scan_Code.SCAN_5, (byte)'5'),
            new KeyTable("6", 0x36, 0xF036, Scan_Code.SCAN_6, (byte)'6'),
            new KeyTable("7", 0x3D, 0xF03D, Scan_Code.SCAN_7, (byte)'7'),
            new KeyTable("8", 0x3E, 0xF03E, Scan_Code.SCAN_8, (byte)'8'),
            new KeyTable("9", 0x46, 0xF046, Scan_Code.SCAN_9, (byte)'9')
        };
    }


        //스캔 코드에 해당하는 키의 이름을 반환하는 함수
    public static string FindKeyStr(int scanCode){
        for (int i = 0; i < KEY_TABLE_SIZE; ++i){
            if (key_tables[i].scan_key == scanCode){
                return new string(key_tables[i].name); // char[]를 string으로 변환하여 반환
            }       
        }
        return null; // 스캔 코드에 해당하는 키가 없을 경우 null 반환
    }
}
