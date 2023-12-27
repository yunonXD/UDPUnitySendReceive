using UnityEngine;
using System.Collections.Generic;
using System;

public class KeyTableManager : MonoBehaviour{

    // 새로운 맵 및 키 이벤트 리스트 선언
    static private PzMap<int, bool> key_events = new PzMap<int, bool>();
    private const int KEY_TABLE_SIZE = 113;  //키 테이블 사이즈 (추가시 수정 요망)
    private Queue<byte> messageQueue = new Queue<byte>();   // 데이터 넣기위한 큐


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
        while (GetSize() > 0){
            byte scanCode =0;
            if (GetByte(ref scanCode)){
                messageQueue.Enqueue(scanCode);
            }
        }
        ProcessQueue();
    }

    // 큐에 있는 메시지 처리    
    void ProcessQueue(){
        while (messageQueue.Count > 0){
            messageQueue.TryDequeue(out byte scanCode);
            DeviceProxy.ScanCode[DeviceProxy.MessageCount] = scanCode;
            if (!Check())
                DeviceProxy.MessageCount++;
        }
    }

    // 버퍼를 갱신하는 함수
    static public void replace_buff(){
        Array.Copy(DeviceProxy.Buffer, DeviceProxy.MessageCount, DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE - DeviceProxy.MessageCount);
        Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        DeviceProxy.BufferCount -= DeviceProxy.MessageCount;
        DeviceProxy.MessageCount = 0;
    }

   // 특정 키, 코드 패턴과 입력된 데이터를 비교하여 해당되는 키 이벤트를 처리하는 함수
    public static bool Check(){
    for (int j = 0; j < KEY_TABLE_SIZE; ++j){
        if (CompareArrays(KeyTables.keyTableDictionary[KeyTables.FindKeyStr(j.ToString())].make_str, DeviceProxy.ScanCode,
             KeyTables.keyTableDictionary[KeyTables.FindKeyStr(j.ToString())].make_str_len)){

            set_key_event(j, true);
            DeviceProxy.MessageCount = 0;

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
        else if (CompareArrays(KeyTables.keyTableDictionary[KeyTables.FindKeyStr(j.ToString())].break_str, DeviceProxy.ScanCode, KeyTables.keyTableDictionary[KeyTables.FindKeyStr(j.ToString())].break_str_len)){
            DeviceProxy.MessageCount = 0;
            set_key_event(j, false);

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
    if (DeviceProxy.MessageCount == DeviceProxy.KEY_CORD_SIZE){
        Debug.Log("KEYCODE MISMATCH");

        DeviceProxy.BufferCount = DeviceProxy.MessageCount = 0;
        Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
        Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);

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


    public static void Clear(){

        DeviceProxy.Head = DeviceProxy.Tail = 0;
        Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
        Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        DeviceProxy.MessageCount = 0;
    }


    // 키테이블 초기화
    public static void init_key_table(){
    foreach (var keyTable in KeyTables.keyTableDictionary.Values){
        keyTable.make_str_len = make_key_string(keyTable.make_str, keyTable.make_val);

        if (keyTable.make_str_len == 0)
            throw new Exception("KeyTable initialization error: Length mismatch");

        keyTable.break_str_len = make_key_string(keyTable.break_str, keyTable.break_val);
        }
    }

    // 키테이블 초기화 (키의 문자열 및 길이 설정)
    public static int  make_key_string(byte[] dest, long input){

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

        return (DeviceProxy.Head - DeviceProxy.Tail + DeviceProxy.MAX_LINE) % DeviceProxy.MAX_LINE;
    }

    //buff 에 byte 추가 (여유 공간이 있다면)
    private bool PutByte(byte b){

        if (GetSize() == (DeviceProxy.MAX_LINE - 1))
            return false;

        DeviceProxy.Buffer[DeviceProxy.Head++] = b;
        DeviceProxy.Head %= DeviceProxy.MAX_LINE;

        return true;
    }

    //buff 에 byte 가져오기 (비어있지 않다면)
    private bool GetByte(ref byte pb){

        if (GetSize() == 0)
            return false;

        pb = DeviceProxy.Buffer[DeviceProxy.Tail++];
        DeviceProxy.Tail %= DeviceProxy.MAX_LINE;

        return true;
    }
}