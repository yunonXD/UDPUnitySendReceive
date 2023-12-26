using UnityEngine;
using System;

public class KeyTableManager : MonoBehaviour{

    //m_nMsgCount 현재 처리중인 키 이벤트의 갯수
    static private int m_nMsgCount; 

    //m_ScanCode 현재 처리중인 키 이벤트 스캔코드를 저장하는 배열
    static private byte[] m_ScanCode = new byte[DeviceProxy.KEY_CORD_SIZE];

    //buff 키 이벤트 저장할 공간
    static private byte[] buff = new byte[DeviceProxy.MAX_LINE];
    static private int buff_count;
    private int m_iHead, m_iTail;       //큐를 사용해서 저장, 가져오므로 해드테일 선언

    // 새로운 맵 및 키 이벤트 리스트 선언
    static private PzMap<int, bool> key_events = new PzMap<int, bool>();
    private const int KEY_TABLE_SIZE = 35;  //키 테이블 사이즈 (추가시 수정 요망)



    void Start(){
        Clear();
        init_key_table();
    }

    void Update(){
        set_dev_data();
    }

    // 키 입력 데이터 처리
    void set_dev_data(){
        // using queue
        #if _QUEUE_
        while (GetSize() > 0){
            GetByte(ref m_ScanCode[m_nMsgCount]);
            if (!check())
                m_nMsgCount++;
        }
        #else
        m_nMsgCount = 1;

        while (buff_count > 0){
            if (buff_count < m_nMsgCount){
                buff_count = m_nMsgCount = 0;
                Array.Clear(buff, 0, DeviceProxy.MAX_LINE);
                Array.Clear(m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return;
            }

            Array.Copy(buff, m_ScanCode, m_nMsgCount);
            Check();
            m_nMsgCount++;
        }
        #endif
    }

    // 버퍼를 갱신하는 함수
    static public void replace_buff(){
        Array.Copy(buff, m_nMsgCount, buff, 0, DeviceProxy.MAX_LINE - m_nMsgCount);
        Array.Clear(m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        buff_count -= m_nMsgCount;
        m_nMsgCount = 0;
    }

    // 특정 키, 코드 패턴과 입력된 데이터를 비교하여 해당되는 키 이벤트를 처리하는 함수
    public static bool Check(){
        for (int j = 0; j < KEY_TABLE_SIZE; ++j){
            if (CompareArrays(KeyTables.key_tables[j].make_str, m_ScanCode, KeyTables.key_tables[j].make_str_len)){
                set_key_event(j, true);
                m_nMsgCount = 0;

                // 큐 없으면 따로 작성해둔 버퍼로 작동함.
                #if _QUEUE_
                Debug.Log("using queue Clear");
                Array.Clear(m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
                #else
                Debug.Log("using replace_buff");
                replace_buff();
                return true;
                #endif
            }
            else if (CompareArrays(KeyTables.key_tables[j].break_str, m_ScanCode, KeyTables.key_tables[j].break_str_len)){
                m_nMsgCount = 0;
                set_key_event(j, false);

                // 큐 없으면 따로 작성해둔 버퍼로 작동함.
                #if _QUEUE_
                Debug.Log("using queue Clear");
                Array.Clear(m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
                #else
                Debug.Log("using replace_buff");
                replace_buff();
                return true;
                #endif
            }   
        }

        // 큐 사용하는데 없으면 false 리턴
        #if _QUEUE_
        Debug.Log("using queue. m_nMsgCount == DeviceProxy.KEY_CORD_SIZE - ? part(line number 104)");
        
        if (m_nMsgCount == DeviceProxy.KEY_CORD_SIZE - 1){
            Debug.Log("KEYCODE MISMATCH");

            byte[] tmp_code = new byte[DeviceProxy.KEY_CORD_SIZE];
            Array.Copy(m_ScanCode, tmp_code, DeviceProxy.KEY_CORD_SIZE);
            Array.Copy(tmp_code, 1, m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE - 1);
            m_ScanCode[DeviceProxy.KEY_CORD_SIZE - 1] = 0;

            m_nMsgCount = DeviceProxy.KEY_CORD_SIZE - 2;

            return false;
        }
        #else
        if (m_nMsgCount == DeviceProxy.KEY_CORD_SIZE){
            Debug.Log("KEYCODE MISMATCH");

            buff_count = m_nMsgCount = 0;
            Array.Clear(buff, 0, DeviceProxy.MAX_LINE);
            Array.Clear(m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);

            return true;
        }
        #endif

        return false;
    }

    // 배열 비교 함수   arr1 과 arr2 의 길이가 같다면 true
    static public bool CompareArrays(byte[] arr1, byte[] arr2, int length){

        for (int i = 0; i < length; i++){
            if (arr1[i] != arr2[i])
                return false;
        }
        return true;
    }

    void Clear(){

        m_iHead = m_iTail = 0;
        Array.Clear(buff, 0, DeviceProxy.MAX_LINE);
        Array.Clear(m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        m_nMsgCount = 0;
    }


    // 키테이블 초기화
    void init_key_table(){

        for (int i = 0; i < KEY_TABLE_SIZE; ++i){

            KeyTables.key_tables[i].make_str_len = make_key_string(KeyTables.key_tables[i].make_str, KeyTables.key_tables[i].make_val);

            if (KeyTables.key_tables[i].make_str_len == 0)
                throw new Exception("KeyTable initialization error: Length mismatch");      //테이블 길이가 0? 무조건 에러

            KeyTables.key_tables[i].break_str_len = make_key_string(KeyTables.key_tables[i].break_str, KeyTables.key_tables[i].break_val);
        }
    }

    // 키테이블 초기화 (키의 문자열 및 길이 설정)
    int make_key_string(byte[] dest, long input){

        byte[] temp = new byte[DeviceProxy.KEY_CORD_SIZE];
        int len = 0;

        BitConverter.GetBytes(input).CopyTo(temp, 0);
        Array.Clear(dest, 0, DeviceProxy.KEY_CORD_SIZE);

        for (int i = 0; i < DeviceProxy.KEY_CORD_SIZE; i++){

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

    // 키 이벤트 설정 함수
    static void set_key_event(int key_idx, bool is_make){
        key_events.Append(new RzPair<int, bool>(key_idx, is_make));
    }

    //buff 크기 반환
    private int GetSize(){

        return (m_iHead - m_iTail + DeviceProxy.MAX_LINE) % DeviceProxy.MAX_LINE;
    }

    //buff 에 byte 추가 (여유 공간이 있다면)
    private bool PutByte(byte b){

        if (GetSize() == (DeviceProxy.MAX_LINE - 1))
            return false;

        buff[m_iHead++] = b;
        m_iHead %= DeviceProxy.MAX_LINE;

        return true;
    }

    //buff 에 byte 가져오기 (비어있지 않다면)
    private bool GetByte(ref byte pb){

        if (GetSize() == 0)
            return false;

        pb = buff[m_iTail++];
        m_iTail %= DeviceProxy.MAX_LINE;

        return true;
    }
}