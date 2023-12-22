using UnityEngine;

public class KeyTableManager : MonoBehaviour{

    private void Start(){
        InitKeyTable();
        
    }

    void InitKeyTable(){

        for (int i = 0; i < KeyTables.KEY_TABLE_SIZE; ++i){
            KeyTables.key_tables[i].make_str_len = MakeKeyString(KeyTables.key_tables[i].make_str, KeyTables.key_tables[i].make_val);
            if (KeyTables.key_tables[i].make_str_len == 0){
                Debug.LogError("There is no data in Table");
            }
            KeyTables.key_tables[i].break_str_len = MakeKeyString(KeyTables.key_tables[i].break_str, KeyTables.key_tables[i].break_val);
        }
    }

    public int MakeKeyString(byte[] dest, long input){
        // 인풋받은 값을 byte arry로 변경
        byte[] temp = new byte[sizeof(long)];
        int len = 0;
        //input & temp 메모리 초기화 
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

    public void TestInput() {
        // 가상의 키 입력을 나타내는 스캔 코드 배열
        byte[] virtualInput = new byte[DeviceProxy.KEY_CORD_SIZE] { 0x16, 0x30, 0x2E, 0x20, 0x12, 0x21, 0x22, 0x23 };

        DeviceProxy.ScanCode = virtualInput;

    }

    public void ButtonCheckTest(){
        Debug.Log(Check());
    }

    //입력되어진 ScanCode 와 키 테이블을 비교.
    //
    public static bool Check(){
        //키 테이블 크기만큼 반복검사 (각 키에 대해서)
        for (int j = 0; j < KeyTables.KEY_TABLE_SIZE; ++j){

            if (!ByteArrayEquals(KeyTables.key_tables[j].make_str, DeviceProxy.ScanCode,
                KeyTables.key_tables[j].make_str_len))
                {
                SetKeyEvent(j, true);

                DeviceProxy.MessageCount = 0;
                // using queue
                #if _QUEUE_
                System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);  
                return true;
                #else
                ReplaceBuffer();  
                return true;
                #endif
            }
            else if (!ByteArrayEquals(KeyTables.key_tables[j].break_str,
                DeviceProxy.ScanCode, KeyTables.key_tables[j].break_str_len)){

                DeviceProxy.MessageCount = 0;

                SetKeyEvent(j, false);

                #if _QUEUE_
                System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);  
                return true;
                #else
                ReplaceBuffer();  
                return true;
                #endif
            }
        }



        #if _QUEUE_
        if (DeviceProxy.MessageCount == DeviceProxy.KEY_CORD_SIZE - 1){

            Debug.Log("KEYCODE MISMATCH");

            byte[] tmpCode = new byte[DeviceProxy.KEY_CORD_SIZE];
            System.Array.Copy(DeviceProxy.ScanCode, 0, tmpCode, 0, DeviceProxy.KEY_CORD_SIZE);
            System.Array.Copy(tmpCode, 1, DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE - 1);

            DeviceProxy.MessageCount = DeviceProxy.KEY_CORD_SIZE - 2;

            return false;
        }
        #else
        if (DeviceProxy.MessageCount == DeviceProxy.KEY_CORD_SIZE){

            Debug.Log("KEYCODE MISMATCH");

            DeviceProxy.BufferCount = 0;
            DeviceProxy.MessageCount = 0;
            System.Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
            System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);

            return true;
        }
        #endif

        return false;
    }

    private static void SetKeyEvent(int KeyIdx ,bool isMake){
        RzPair<int, bool> newPair = new RzPair<int, bool>{
            Key = KeyIdx,
            Item = isMake
        };

        PzMap<int ,bool> keyEvent = new PzMap<int, bool>();
        keyEvent.Append(newPair);

        Debug.Log($"SetKeyEvent - KeyIndex: {KeyIdx}, IsMake: {isMake}");
        
    }


    //버퍼 재배치 (키 이벤트 처리기)
    //Buffer > 배열의 일부를 자르고 그 일부를 배열의 처음으로 복사
    //ScanCode 배열 초기화
    //Array.Copy (복사 대상 , 복시 시작 위치 , 복사 대상 배열(목적지) ,  대상(목적지)의 시작 위치 , 복사할 요소 개수 )
    private static void ReplaceBuffer(){
        System.Array.Copy(DeviceProxy.Buffer , DeviceProxy.MessageCount , DeviceProxy.Buffer , 0 , DeviceProxy.MAX_LINE - DeviceProxy.MessageCount);
        System.Array.Clear(DeviceProxy.ScanCode , 0 , DeviceProxy.KEY_CORD_SIZE);
    
        //형변환 BufferCount >uint // MessageCount ->int 이므로 uint 로 일치과정필요
        DeviceProxy.BufferCount = (uint)(DeviceProxy.BufferCount - DeviceProxy.MessageCount);

        DeviceProxy.MessageCount =0;
    }


    //ByteArray 비교함수. 같으면 true return
    private static bool ByteArrayEquals( byte[] a1 ,byte[] a2 ,int lenth){
        for( int i=0; i <lenth; i++){
            if( a1[i] != a2[i] ) return false;
        }
        return true;
    }
}


