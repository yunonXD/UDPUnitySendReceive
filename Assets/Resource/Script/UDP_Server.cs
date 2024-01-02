using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public class UDPServer : MonoBehaviour{
    [SerializeField] private TextMeshProUGUI serverStatusText;
    [SerializeField] private TextMeshProUGUI StateText;
    [SerializeField] private TextMeshProUGUI receivedMessageText;
    [SerializeField] private int m_Port =9020;


    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;
    private Queue<byte[]> receivedDataQueue = new Queue<byte[]>(); // 수신된 데이터를 큐에 저장

    private void Start(){
        KeyTableManagerServer.init_key_table();
        KeyTableManagerServer.Clear();

        udpServer = new UdpClient(m_Port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        UpdateServerStatus("Server started on port :" + m_Port);
    }

    private void Update(){

        try{
            CheckVerify();
        }
        catch (Exception e){

            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    private async void CheckVerify(){

        if (udpServer.Available > 0){

            UdpReceiveResult result = await udpServer.ReceiveAsync();

            byte[] data = result.Buffer;            // 수신된 데이터 버퍼
            int dataLength = result.Buffer.Length;  // 수신된 데이터의 길이를 얻음

            receivedDataQueue.Enqueue(data);

            ProcessReceivedData();
        }
    }

private Queue<byte[]> cumulativeDataQueue = new Queue<byte[]>(); // 모든 데이터를 누적하는 큐

private void ProcessReceivedData(){
    while (receivedDataQueue.Count > 0){
        byte[] newData = receivedDataQueue.Dequeue(); // 새로운 데이터를 큐에서 가져옴

        // 큐에 있던 데이터와 합쳐서 새로운 큐를 만듦
        Queue<byte[]> combinedQueue = new Queue<byte[]>(cumulativeDataQueue);
        combinedQueue.Enqueue(newData);

        // 바이트 데이터에서 바이트가 들어있는 부분을 카운트하고 출력
        string accumulatedMessage = "";
        foreach (byte[] data in combinedQueue) {
            int byteCount = 0;
            for (int i = 0; i < data.Length; i++) {
                if (data[i] != 0x00) {
                    byteCount++;
                } else {
                    break;
                }
            }

            if (byteCount >= 1) {
                // 바이트 배열의 값을 아스키 코드로 변환하여 누적된 메시지에 추가
                for (int i = 0; i < byteCount; i++) {
                    int asciiValue = data[i];
                    KeyTable keyTable = KeyTables.KeyTableForLong[asciiValue];
                    accumulatedMessage += keyTable.name;
                }
            } else {
                Debug.LogWarning("Received Data is not sufficient to convert to Decimal.");
            }
        }

        // 출력 업데이트
        receivedMessageText.text = "Received Message: " + accumulatedMessage;

        // 누적된 큐를 갱신
        cumulativeDataQueue = new Queue<byte[]>(combinedQueue);
    }
}




    public void _CheckQueueData(){
        foreach(byte[] data in cumulativeDataQueue){
            int byteCount = 0;
            for (int i = 0; i < data.Length; i++) {
                if (data[i] != 0x00) {
                    byteCount++;
                }
                else 
                    break;
                
            }

            if (byteCount >= 1) {
                // 바이트 배열의 각 값을 아스키 코드로 변환하여 출력
                for (int i = 0; i < byteCount; i++) {
                    int asciiValue = data[i];
                    Debug.Log(asciiValue);
                }   
            }
            else 
                Debug.LogWarning("Received Data is not sufficient to convert to Decimal.");
            }
    }



    public void _CheckQueueLenth(){

        Debug.Log(cumulativeDataQueue.Count);

    }



    private void UpdateServerStatus(string status){
        serverStatusText.text = "Server Status: " + status;
    }

    private void UpdateReceivedMessage(string message){
        receivedMessageText.text = "Received Message: " + message;
    }

    private void OnDestroy(){
        if (udpServer != null)
            udpServer.Close();
    }
}


public class KeyTableManagerServer{
    
    public KeyTableManagerServer(){
        Clear();
        init_key_table();
    }

    //장치로부터 수신한 데이터를 처리
    //하나는 데이터를 큐에서 추출하고, 다른 하나는 직접 버퍼에서 처리
    public static void set_dev_data(){

        //#if _QUEUE_
        while (GetSize() > 0){
            GetByte(ref DeviceProxy.ScanCode[DeviceProxy.MessageCount]);
            if (!Check())
                DeviceProxy.MessageCount++;
        }
        //#else
        //DeviceProxy.MessageCount = 1;

        // while (DeviceProxy.BufferCount > 0){
        //     if (DeviceProxy.BufferCount < DeviceProxy.MessageCount)
        //     {
        //         DeviceProxy.BufferCount = DeviceProxy.MessageCount = 0;
        //         Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
        //         Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        //         return;
        //     }

        //     Array.Copy(DeviceProxy.Buffer, DeviceProxy.ScanCode, DeviceProxy.MessageCount);
        //     Check();
        //     DeviceProxy.MessageCount++;
        // }
        //#endif
    }


    //버퍼의 내용을 재배열. 
    //처리한 메시지를 제거하고, 나머지 메시지를 버퍼의 앞으로 이동시켜버림.
    public static void replace_buff(){
        Array.Copy(DeviceProxy.Buffer, DeviceProxy.MessageCount, DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE - DeviceProxy.MessageCount);
        Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);

        DeviceProxy.BufferCount -= DeviceProxy.MessageCount;

        DeviceProxy.MessageCount = 0;
    }


    //키 이벤트를 확인. 입력된 스캔코드가 키 이벤트와 일치하면 해당 이벤트를 설정하고,
    //일치하지 않으면 에러 메시지를 로그에 출력
    public static bool Check(){
        for (int j = 0; j < KeyTables.KEY_TABLE_SIZE; ++j){
            string keyName = KeyTables.keyTableDictionary.ElementAt(j).Key;

            if (Equals(KeyTables.keyTableDictionary[keyName].make_str, DeviceProxy.ScanCode)){
                set_key_event(KeyTables.keyTableDictionary[keyName], true);
                DeviceProxy.MessageCount = 0;
                //#if _QUEUE_
                Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
                //#else
                //replace_buff();
                //return true;
                //#endif
            }
            else if (Equals(KeyTables.keyTableDictionary[keyName].break_str, DeviceProxy.ScanCode)){
                DeviceProxy.MessageCount = 0;
                set_key_event(KeyTables.keyTableDictionary[keyName], false);

                //#if _QUEUE_
                Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
                //#else
                //replace_buff();
                //return true;
                //#endif
            }
        }


    //#if _QUEUE_
        if (DeviceProxy.MessageCount == DeviceProxy.KEY_CORD_SIZE - 1){
            Debug.Log("KEYCODE MISMATCH\n");

            byte[] tmp_code = new byte[DeviceProxy.KEY_CORD_SIZE];
            Array.Copy(DeviceProxy.ScanCode, tmp_code, DeviceProxy.KEY_CORD_SIZE);
            Array.Copy(tmp_code, 1, DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE - 1);
            DeviceProxy.ScanCode[DeviceProxy.KEY_CORD_SIZE - 1] = 0;

            DeviceProxy.MessageCount = DeviceProxy.KEY_CORD_SIZE - 2;

            return false;
        }
        else
            return false;
    // #else
    //     if (DeviceProxy.MessageCount == DeviceProxy.KEY_CORD_SIZE){
    //         Debug.Log("KEYCODE MISMATCH\n");

    //         DeviceProxy.BufferCount = DeviceProxy.MessageCount = 0;
    //         Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
    //         Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);

    //     return true;
    //     }
    // #endif

        //return false;
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
    //특정 키의 이벤트가 발생하면 이 함수를 호출하여 이벤트를 KeyEvent에 저장
    private static void set_key_event(KeyTable keyTable, bool is_make){
       DeviceProxy.KeyEvent.Append(new RzPair<int, bool>(KeyTables.GetKeyIndex(keyTable), is_make));
    }


    //버퍼와 스캔코드, 메시지 카운트를 모두 초기화.
    public static void Clear(){
        DeviceProxy.Head = DeviceProxy.Tail = 0;
        Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
        Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        DeviceProxy.MessageCount = 0;
    }


    // 현재 버퍼에 저장된 데이터의 크기를 반환
    public static int GetSize(){
        return (DeviceProxy.Head - DeviceProxy.Tail + DeviceProxy.MAX_LINE) % DeviceProxy.MAX_LINE;
    }


    //버퍼에 바이트를 추가
    public static bool PutByte(byte b){
        if (GetSize() == (DeviceProxy.MAX_LINE - 1))
            return false;

        DeviceProxy.Buffer[DeviceProxy.Head++] = b;
        DeviceProxy.Head %= DeviceProxy.MAX_LINE;

        return true;
    }


    //버퍼에서 바이트를 추출
    public static bool GetByte(ref byte pb){
        if (GetSize() == 0)
            return false;

        pb = DeviceProxy.Buffer[DeviceProxy.Tail++];
        DeviceProxy.Tail %= DeviceProxy.MAX_LINE;

        return true;
    }
}