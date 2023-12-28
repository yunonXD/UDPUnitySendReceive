using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyTable{
    public string name = new string(' ', 20);
    public long make_val;
    public long break_val;
    public byte scan_key;
    public byte os_vk_key;

    public byte[] make_str = new byte[8];       //주어진 테이블의 키를 생성
    public int make_str_len;
    public byte[] break_str = new byte[8];      //주어진 테이블의 키를 끊기위한 
    public int break_str_len;

    public KeyTable(){
        Array.Clear(name.ToCharArray(), 0, name.Length);
        make_val = 0;
        break_val = 0;
        scan_key = 0;
        os_vk_key = 0;
    }

    //(키의 이름 , 눌렀을때 의 값 , 땠을때의 값 , 스캔코드 , 가상키)
    //또한 눌렀을때의 문자열 길이, 땠을때의 문자열 길이 저장
    public KeyTable(string aName, long aMakeVal, long aBreakVal, byte aScanKey, byte aOSVirtualKey = 0){

        name = aName;
        make_val = aMakeVal;
        break_val = aBreakVal;
        scan_key = aScanKey;
        os_vk_key = aOSVirtualKey;
    }
}

public static class KeyTables{
    public static readonly Dictionary<string, KeyTable> keyTableDictionary = new Dictionary<string, KeyTable>();
    public static readonly Dictionary<long ,KeyTable> KeyTableForLong = new Dictionary<long, KeyTable>();
    public const int KEY_TABLE_SIZE = 113;

    static KeyTables(){
        InitializeKeyTables();
    }

    static void InitializeKeyTables(){

        var keyTables = new KeyTable[]{
            
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
            new KeyTable("9", 0x46, 0xF046, Scan_Code.SCAN_9, (byte)'9'),
            new KeyTable("`", 0x0E, 0xF00E, Scan_Code.SCAN_APOSTROPHE, 222),
            new KeyTable("-", 0x4E, 0xF04E, Scan_Code.SCAN_MINUS, (byte)'-'),
            new KeyTable("=", 0x55, 0xF055, Scan_Code.SCAN_EQUALS, 187),
	        new KeyTable("\\", 0x5D, 0xF05D, Scan_Code.SCAN_BACKSLASH, 220),
            new KeyTable("BKSP", 0x66, 0xF066, Scan_Code.SCAN_BACK, VirsualKeyCode.VK_BACK),
	        new KeyTable("SPACE", 0x29, 0xF029, Scan_Code.SCAN_SPACE, VirsualKeyCode.VK_SPACE),
	        new KeyTable("TAB", 0x0D, 0xF00D, Scan_Code.SCAN_TAB, VirsualKeyCode.VK_TAB),
	        new KeyTable("CAPS", 0x58, 0xF058, Scan_Code.SCAN_CAPITAL, VirsualKeyCode.VK_CAPITAL),
	        new KeyTable("L SHIFT", 0x12, 0xF012, Scan_Code.SCAN_LSHIFT, VirsualKeyCode.VK_LSHIFT),
	        new KeyTable("L CONTROL", 0x14, 0xF014, Scan_Code.SCAN_LCONTROL, VirsualKeyCode.VK_LCONTROL),
	        new KeyTable("L GUI", 0xE01F, 0xE0F01F, Scan_Code.SCAN_LWIN, VirsualKeyCode.VK_LWIN),
	        new KeyTable("L ALT", 0x11, 0xF011, Scan_Code.SCAN_LMENU, VirsualKeyCode.VK_LMENU),
	        new KeyTable("R SHIFT", 0x59, 0xF059, Scan_Code.SCAN_RSHIFT, VirsualKeyCode.VK_RSHIFT),
	        new KeyTable("R CONTROL", 0xE014, 0xE0F014, Scan_Code.SCAN_RCONTROL, VirsualKeyCode.VK_RCONTROL),
	        new KeyTable("R GUI", 0xE027, 0xE0F027, Scan_Code.SCAN_RWIN, VirsualKeyCode.VK_RWIN),
	        new KeyTable("R ALT", 0xE011, 0xE0F011, Scan_Code.SCAN_RMENU, VirsualKeyCode.VK_HANGEUL),
	        new KeyTable("APPS", 0xE02F, 0xE0F02F, Scan_Code.SCAN_APPS, VirsualKeyCode.VK_APPS),
	        new KeyTable("ENTER", 0x5A, 0xF05A, Scan_Code.SCAN_RETURN, VirsualKeyCode.VK_RETURN),
	        new KeyTable("ESC", 0x76, 0xF076, Scan_Code.SCAN_ESCAPE, VirsualKeyCode.VK_ESCAPE),
	        new KeyTable("F1", 0x05, 0xF005, Scan_Code.SCAN_F1, VirsualKeyCode.VK_F1),
	        new KeyTable("F2", 0x06, 0xF006, Scan_Code.SCAN_F2, VirsualKeyCode.VK_F2),
	        new KeyTable("F3", 0x04, 0xF004, Scan_Code.SCAN_F3, VirsualKeyCode.VK_F3),
	        new KeyTable("F4", 0x0C, 0xF00C, Scan_Code.SCAN_F4, VirsualKeyCode.VK_F4),
	        new KeyTable("F5", 0x03, 0xF003, Scan_Code.SCAN_F5, VirsualKeyCode.VK_F5),
	        new KeyTable("F6", 0x0B, 0xF00B, Scan_Code.SCAN_F6, VirsualKeyCode.VK_F6),
	        new KeyTable("F7", 0x83, 0xF083, Scan_Code.SCAN_F7, VirsualKeyCode.VK_F7),
	        new KeyTable("F8", 0x0A, 0xF00A, Scan_Code.SCAN_F8, VirsualKeyCode.VK_F8),
	        new KeyTable("F9", 0x01, 0xF001, Scan_Code.SCAN_F9, VirsualKeyCode.VK_F9),
	        new KeyTable("F10", 0x09, 0xF009, Scan_Code.SCAN_F10, VirsualKeyCode.VK_F10),
	        new KeyTable("F11", 0x78, 0xF078, Scan_Code.SCAN_F11, VirsualKeyCode.VK_F11),
	        new KeyTable("F12", 0x07, 0xF007, Scan_Code.SCAN_F12, VirsualKeyCode.VK_F12),
	        //new KeyTable("PRINT", 0xE012, 0xE0F012, Scan_Code.SCAN_PRINT),
	        new KeyTable("PRINT", 0xE012, 0xE0F012, Scan_Code.SCAN_SYSRQ, VirsualKeyCode.VK_SNAPSHOT),
	        new KeyTable("SCROLL", 0x7E, 0xF07E, Scan_Code.SCAN_SCROLL, VirsualKeyCode.VK_SCROLL),
	        new KeyTable("PAUSE", 0x77, 0xF077, Scan_Code.SCAN_PAUSE, VirsualKeyCode.VK_PAUSE),
	        new KeyTable("[", 0x54, 0xF054, Scan_Code.SCAN_LBRACKET, 219),
	        new KeyTable("INSERT", 0xE070, 0xE0F070, Scan_Code.SCAN_INSERT, VirsualKeyCode.VK_INSERT),
	        new KeyTable("HOME", 0xE06C, 0xE0F06C, Scan_Code.SCAN_HOME, VirsualKeyCode.VK_HOME),
	        new KeyTable("PAGEUP", 0xE07D, 0xE0F07D, Scan_Code.SCAN_PRIOR, VirsualKeyCode.VK_PRIOR),
	        new KeyTable("DELETE", 0xE071, 0xE0F071, Scan_Code.SCAN_DELETE, VirsualKeyCode.VK_DELETE),
	        new KeyTable("END", 0xE069, 0xE0F069, Scan_Code.SCAN_END, VirsualKeyCode.VK_END),
	        new KeyTable("PAGEDOWN", 0xE07A, 0xE0F07A, Scan_Code.SCAN_NEXT, VirsualKeyCode.VK_NEXT),
	        new KeyTable("U ARROW", 0xE075, 0xE0F075, Scan_Code.SCAN_UP, VirsualKeyCode.VK_UP),
	        new KeyTable("L ARROW", 0xE06B, 0xE0F06B, Scan_Code.SCAN_LEFT, VirsualKeyCode.VK_LEFT),
	        new KeyTable("D ARROW", 0xE072, 0xE0F072, Scan_Code.SCAN_DOWN, VirsualKeyCode.VK_DOWN),
	        new KeyTable("R ARROW", 0xE074, 0xE0F074, Scan_Code.SCAN_RIGHT, VirsualKeyCode.VK_RIGHT),
	        new KeyTable("NUMLOCK", 0x77, 0xF077, Scan_Code.SCAN_NUMLOCK, VirsualKeyCode.VK_NUMLOCK),
	        new KeyTable("KP /", 0xE04A, 0xE0F04A, Scan_Code.SCAN_DIVIDE, VirsualKeyCode.VK_DIVIDE),
	        new KeyTable("KP *", 0x7C, 0xF07C, Scan_Code.SCAN_MULTIPLY, VirsualKeyCode.VK_MULTIPLY),
	        new KeyTable("KP -", 0x7B, 0xF07B, Scan_Code.SCAN_SUBTRACT, VirsualKeyCode.VK_SUBTRACT),
	        new KeyTable("KP +", 0x79, 0xF079, Scan_Code.SCAN_ADD, VirsualKeyCode.VK_ADD),
	        new KeyTable("KP ENTER", 0xE05A, 0xE0F05A, Scan_Code.SCAN_NUMPADENTER, VirsualKeyCode.VK_RETURN),
	        new KeyTable("KP .", 0x71, 0xF071, Scan_Code.SCAN_DECIMAL, VirsualKeyCode.VK_DECIMAL),
	        new KeyTable("KP 0", 0x70, 0xF070, Scan_Code.SCAN_NUMPAD0, VirsualKeyCode.VK_NUMPAD0),
	        new KeyTable("KP 1", 0x69, 0xF069, Scan_Code.SCAN_NUMPAD1, VirsualKeyCode.VK_NUMPAD1),
	        new KeyTable("KP 2", 0x72, 0xF072, Scan_Code.SCAN_NUMPAD2, VirsualKeyCode.VK_NUMPAD2),
	        new KeyTable("KP 3", 0x7A, 0xF07A, Scan_Code.SCAN_NUMPAD3, VirsualKeyCode.VK_NUMPAD3),
	        new KeyTable("KP 4", 0x6B, 0xF06B, Scan_Code.SCAN_NUMPAD4, VirsualKeyCode.VK_NUMPAD4),
	        new KeyTable("KP 5", 0x73, 0xF073, Scan_Code.SCAN_NUMPAD5, VirsualKeyCode.VK_NUMPAD5),
	        new KeyTable("KP 6", 0x74, 0xF074, Scan_Code.SCAN_NUMPAD6, VirsualKeyCode.VK_NUMPAD6),
	        new KeyTable("KP 7", 0x6C, 0xF06C, Scan_Code.SCAN_NUMPAD7, VirsualKeyCode.VK_NUMPAD7),
	        new KeyTable("KP 8", 0x75, 0xF075, Scan_Code.SCAN_NUMPAD8, VirsualKeyCode.VK_NUMPAD8),
	        new KeyTable("KP 9", 0x7D, 0xF07D, Scan_Code.SCAN_NUMPAD9, VirsualKeyCode.VK_NUMPAD9),
	        new KeyTable("]", 0x5B, 0xF05B, Scan_Code.SCAN_RBRACKET, 221),
	        new KeyTable(";", 0x4C, 0xF04C, Scan_Code.SCAN_SEMICOLON, 186),
	        new KeyTable("'", 0x52, 0xF052, Scan_Code.SCAN_APOSTROPHE, 222),
	        new KeyTable(",", 0x41, 0xF041, Scan_Code.SCAN_COMMA, 188),
	        new KeyTable(".", 0x49, 0xF049, Scan_Code.SCAN_PERIOD, 190),
	        new KeyTable("/", 0x4A, 0xF04A, Scan_Code.SCAN_SLASH, 191),
	        //new KeyTable("enemy_align", 0xA6, 0xF0A6, SCAN_ENEMY_ALIGN ),   //detect later by kwonoh 207.02.06
	        new KeyTable("enemy_align", 0x60, 0xF060, Scan_Code.SCAN_ENEMY_ALIGN, VirsualKeyCode.VK_HOME),
	        new KeyTable("cbr", 0xA7, 0xF0A7, Scan_Code.SCAN_CBR),
	        new KeyTable("enemy", 0xA8, 0xF0A8, Scan_Code.SCAN_ENEMY),
	        new KeyTable("nextw", 0xAF, 0xF0AF, Scan_Code.SCAN_NEXTW),
            //
	        new KeyTable("4wayup", 0xC5, 0xF0C5, Scan_Code.SCAN_4WAYUP, VirsualKeyCode.VK_UP),
	        new KeyTable("4waydown", 0xC6, 0xF0C6, Scan_Code.SCAN_4WAYDOWN, VirsualKeyCode.VK_DOWN),
	        new KeyTable("4wayleft", 0xC7, 0xF0C7, Scan_Code.SCAN_4WAYLEFT, VirsualKeyCode.VK_LEFT),
	        new KeyTable("4wayright", 0xC8, 0xF0C8, Scan_Code.SCAN_4WAYRIGHT, VirsualKeyCode.VK_RIGHT),
	        new KeyTable("change", 0x90, 0xF090, Scan_Code.SCAN_CHANGE, VirsualKeyCode.VK_HANGUL)
        };

        foreach (var keyTable in keyTables){
            keyTableDictionary[keyTable.name] = keyTable;
        }

        foreach(var keyTable in keyTables){
            KeyTableForLong[keyTable.make_val] = keyTable;
        }
    }

    public static string FindKeyStr(string name){
        if (keyTableDictionary.TryGetValue(name, out var keyTable)){
            return keyTable.name;
        }

        return null;
    }

     public static int GetKeyIndex(KeyTable keyTable){
        foreach (var kvp in keyTableDictionary){
            if (kvp.Value == keyTable){
                // 해당 KeyTable의 이름을 int로 변환하여 반환
                if (int.TryParse(kvp.Key, out int keyIndex))    return keyIndex;
                else    return -1;
            }
        }

        return -1; // 해당하는 KeyTable을 찾지 못한 경우
    }


}