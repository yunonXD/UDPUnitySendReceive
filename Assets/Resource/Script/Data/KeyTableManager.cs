using UnityEngine;

public class KeyTableManager : MonoBehaviour{

    void Start(){
        InitKeyTable();
        find_key_str(0x02);
    }

    void InitKeyTable(){

        for (int i = 0; i < KeyTables.KEY_TABLE_SIZE; ++i){
            KeyTables.key_tables[i].make_str_len = MakeKeyString(KeyTables.key_tables[i].make_str, KeyTables.key_tables[i].make_val);
            if (KeyTables.key_tables[i].make_str_len == 0)
            {
                Debug.Log("There is no data in Table");
            }
            KeyTables.key_tables[i].break_str_len = MakeKeyString(KeyTables.key_tables[i].break_str, KeyTables.key_tables[i].break_val);
            new string(KeyTables.key_tables[i].name);
            Debug.Log(KeyTables.key_tables[i].name);
            Debug.Log(KeyTables.key_tables[i].make_val);
            Debug.Log(KeyTables.key_tables[i].break_val);
            Debug.Log(KeyTables.key_tables[i].scan_key);
        }

    }

    //스캔 코드에 해당하는 키의 이름을 반환하는 함수
    string find_key_str(int scanCode){
        for (int i = 0; i < KeyTables.KEY_TABLE_SIZE; ++i){
            if (KeyTables.key_tables[i].scan_key == scanCode){
                
                return new string(KeyTables.key_tables[i].name); // char[]를 string으로 변환하여 반환
            }       
        }
        return null; // 스캔 코드에 해당하는 키가 없을 경우 null 반환
    }



    int MakeKeyString(byte[] dest, long input){
        // make input value to byte array
        byte[] temp = new byte[sizeof(long)];
        int len = 0;

        System.Buffer.BlockCopy(System.BitConverter.GetBytes(input), 0, temp, 0, sizeof(long));
        System.Array.Clear(dest, 0, dest.Length);

        for (int i = 0; i < temp.Length; i++){
            if (temp[i] == 0x00) break;

            dest[i] = temp[i];
            len++;
        }

        if (len == 0) return 0;

        int j = 0;
        for (int i = len; i > 0; i--){
            dest[j] = temp[i - 1];
            j++;
        }
        return len;
    }
}
