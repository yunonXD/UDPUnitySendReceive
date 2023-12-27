using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


public class OccDeviceM{
    private PzMap<int, bool> key_events = new PzMap<int, bool>();
    private Queue<byte> messageQueue = new Queue<byte>();

    public OccDeviceM(){
        Clear();
        init_key_table();
    }

    //장치로부터 수신한 데이터를 처리
    //하나는 데이터를 큐에서 추출하고, 다른 하나는 직접 버퍼에서 처리
    public void set_dev_data(){

        #if _QUEUE_
        while (GetSize() > 0){
            GetByte(ref DeviceProxy.scanCode[DeviceProxy.messageCount]);
            if (!check())
                DeviceProxy.messageCount++;
        }
        #else
        DeviceProxy.MessageCount = 1;

        while (DeviceProxy.BufferCount > 0){
            if (DeviceProxy.BufferCount < DeviceProxy.MessageCount)
            {
                DeviceProxy.BufferCount = DeviceProxy.MessageCount = 0;
                System.Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
                System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return;
            }

            System.Array.Copy(DeviceProxy.Buffer, DeviceProxy.ScanCode, DeviceProxy.MessageCount);
            check();
            DeviceProxy.MessageCount++;
        }
        #endif
    }


    //버퍼의 내용을 재배열. 
    //처리한 메시지를 제거하고, 나머지 메시지를 버퍼의 앞으로 이동시켜버림.
    public void replace_buff(){
        System.Array.Copy(DeviceProxy.Buffer, DeviceProxy.MessageCount, DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE - DeviceProxy.MessageCount);
        System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);

        DeviceProxy.BufferCount -= DeviceProxy.MessageCount;

        DeviceProxy.MessageCount = 0;
    }


    //키 이벤트를 확인. 입력된 스캔코드가 키 이벤트와 일치하면 해당 이벤트를 설정하고,
    //일치하지 않으면 에러 메시지를 로그에 출력
    public bool check(){
        for (int j = 0; j < KeyTables.KEY_TABLE_SIZE; ++j){
            string keyName = KeyTables.keyTableDictionary.ElementAt(j).Key;

            if (System.Array.Equals(KeyTables.keyTableDictionary[keyName].make_str, DeviceProxy.ScanCode)){
                set_key_event(KeyTables.keyTableDictionary[keyName], true);
                DeviceProxy.MessageCount = 0;
                #if _QUEUE_
                System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
                #else
                replace_buff();
                return true;
                #endif
            }
            else if (System.Array.Equals(KeyTables.keyTableDictionary[keyName].break_str, DeviceProxy.ScanCode)){
                DeviceProxy.MessageCount = 0;
                set_key_event(KeyTables.keyTableDictionary[keyName], false);

                #if _QUEUE_
                System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
                #else
                replace_buff();
                return true;
                #endif
            }
        }


    #if _QUEUE_
        if (m_nMsgCount == DeviceProxy.KEY_CORD_SIZE - 1){
            Debug.Log("KEYCODE MISMATCH\n");

            byte[] tmp_code = new byte[DeviceProxy.KEY_CORD_SIZE];
            System.Array.Copy(m_ScanCode, tmp_code, DeviceProxy.KEY_CORD_SIZE);
            System.Array.Copy(tmp_code, 1, m_ScanCode, 0, DeviceProxy.KEY_CORD_SIZE - 1);
            m_ScanCode[DeviceProxy.KEY_CORD_SIZE - 1] = 0;

            m_nMsgCount = DeviceProxy.KEY_CORD_SIZE - 2;

            return false;
        }
    #else
        if (DeviceProxy.MessageCount == DeviceProxy.KEY_CORD_SIZE){
            Debug.Log("KEYCODE MISMATCH\n");

            DeviceProxy.BufferCount = DeviceProxy.MessageCount = 0;
            System.Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
            System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);

        return true;
        }
    #endif

        return false;
    }


    // 키테이블 초기화
    public static void init_key_table(){
        foreach (var keyTable in KeyTables.keyTableDictionary.Values){
            keyTable.make_str_len = MakeKeyString(keyTable.make_str, keyTable.make_val);

            if (keyTable.make_str_len == 0)
                throw new Exception("KeyTable initialization error: Length mismatch");

            keyTable.break_str_len = MakeKeyString(keyTable.break_str, keyTable.break_val);
        }
    }

    //입력된 키 값을 바이트 배열로 변환
    //변환된 바이트 배열은 키의 make_str 또는 break_str에 저장
    public static int MakeKeyString(byte[] dest, long input){
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

    //키 이벤트를 설정.
    //특정 키의 이벤트가 발생하면 이 함수를 호출하여 이벤트를 key_events에 저장
    private void set_key_event(KeyTable keyTable, bool is_make){
        key_events.Append(new RzPair<int, bool>(KeyTables.GetKeyIndex(keyTable), is_make));
    }


    //버퍼와 스캔코드, 메시지 카운트를 모두 초기화.
    public void Clear(){
        DeviceProxy.Head = DeviceProxy.Tail = 0;
        System.Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
        System.Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        DeviceProxy.MessageCount = 0;
    }


    // 현재 버퍼에 저장된 데이터의 크기를 반환
    public int GetSize(){
        return (DeviceProxy.Head - DeviceProxy.Tail + DeviceProxy.MAX_LINE) % DeviceProxy.MAX_LINE;
    }


    //버퍼에 바이트를 추가
    public bool PutByte(byte b){
        if (GetSize() == (DeviceProxy.MAX_LINE - 1))
            return false;

        DeviceProxy.Buffer[DeviceProxy.Head++] = b;
        DeviceProxy.Head %= DeviceProxy.MAX_LINE;

        return true;
    }


    //버퍼에서 바이트를 추출
    public bool GetByte(ref byte pb){
        if (GetSize() == 0)
            return false;

        pb = DeviceProxy.Buffer[DeviceProxy.Tail++];
        DeviceProxy.Tail %= DeviceProxy.MAX_LINE;

        return true;
    }
}