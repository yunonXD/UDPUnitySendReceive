using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

public class Server_KeyManager : MonoBehaviour{

    static private PzMap<int, bool> key_events = new PzMap<int, bool>();
    private const int KEY_TABLE_SIZE = 113;  // 키 테이블 사이즈 (추가시 수정 요망)
    public static Queue<byte> messageQueue = new Queue<byte>();   // 데이터 넣기위한 큐

    // 대략적인 상태 UI 에 띄우기
    public Text serverStatusText;
    public Text stateText;
    public Text receivedMessageText;

    [SerializeField] private int m_Port = 9020;
    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;

    void Start(){
        Clear();
        initKeyTable();
        UpdateServerStatus("Server started on port: " + m_Port);
    }

    void Update(){
        try{
            CheckVerify();
        }
        catch (Exception e){
            Debug.LogError("Error receiving data: " + e.Message);
        }
        SetDevData();
    }

    private void UpdateServerStatus(string status){
        udpServer = new UdpClient(m_Port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, m_Port);
        serverStatusText.text = "Server Status: " + status;
    }

    private void UpdateReceivedMessage(string message){
        receivedMessageText.text = "Received Message: " + message;
    }

    private void OnDestroy(){
        if (udpServer != null)
            udpServer.Close();
    }

    private async void CheckVerify(){
        if (udpServer.Available > 0){
            UdpReceiveResult result = await udpServer.ReceiveAsync();
            byte[] data = result.Buffer;
            int dataLength = result.Buffer.Length;
            DeviceProxy.ScanCode = data;

            Debug.Log(BitConverter.ToString(data));
        }
        else
        {
            stateText.text = "State: ...";
        }
    }

    // 키 입력 데이터 처리
    public static void SetDevData(){
        // using queue
        while (GetSize() > 0){
            byte scanCode = 0;
            if (GetByte(ref scanCode)){
                messageQueue.Enqueue(scanCode);
            }
        }
        ProcessQueue();
    }

    // 큐에 있는 메시지 처리    
    public static void ProcessQueue(){
        while (messageQueue.Count > 0){
            messageQueue.TryDequeue(out byte scanCode);
            DeviceProxy.ScanCode[DeviceProxy.MessageCount] = scanCode;
            if (!Check())
                DeviceProxy.MessageCount++;
        }
    }

    // 버퍼를 갱신하는 함수
    static public void ReplaceBuff(){
        Array.Copy(DeviceProxy.Buffer, DeviceProxy.MessageCount, DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE - DeviceProxy.MessageCount);
        Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        DeviceProxy.BufferCount -= DeviceProxy.MessageCount;
        DeviceProxy.MessageCount = 0;
    }

    // 특정 키, 코드 패턴과 입력된 데이터를 비교하여 해당되는 키 이벤트를 처리하는 함수
    public static bool Check(){
        for (int j = 0; j < KEY_TABLE_SIZE; ++j){
            string keyName = KeyTables.keyTableDictionary.ElementAt(j).Key;

            if (CompareArrays(KeyTables.keyTableDictionary[keyName].make_str, DeviceProxy.ScanCode,
                KeyTables.keyTableDictionary[keyName].make_str_len)){
                SetKeyEvent(j, true);
                DeviceProxy.MessageCount = 0;
                Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
            }
            else if (CompareArrays(KeyTables.keyTableDictionary[keyName].break_str, DeviceProxy.ScanCode, KeyTables.keyTableDictionary[keyName].break_str_len)){
                DeviceProxy.MessageCount = 0;
                SetKeyEvent(j, false);
                Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
                return true;
            }
        }

        Debug.Log("using queue. m_nMsgCount == DeviceProxy.KEY_CORD_SIZE - ? part(line number 104)");

        if (DeviceProxy.MessageCount == DeviceProxy.KEY_CORD_SIZE - 1){
            Debug.Log("KEYCODE MISMATCH");

            byte[] tmp_code = new byte[DeviceProxy.KEY_CORD_SIZE];
            Array.Copy(DeviceProxy.ScanCode, tmp_code, DeviceProxy.KEY_CORD_SIZE);
            Array.Copy(tmp_code, 1, DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE - 1);
            DeviceProxy.ScanCode[DeviceProxy.KEY_CORD_SIZE - 1] = 0;

            DeviceProxy.MessageCount = DeviceProxy.KEY_CORD_SIZE - 2;

            return false;
        }

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
    public static void initKeyTable(){
        foreach (var keyTable in KeyTables.keyTableDictionary.Values){
            keyTable.make_str_len = MakeKeyString(keyTable.make_str, keyTable.make_val);

            if (keyTable.make_str_len == 0)
                throw new Exception("KeyTable initialization error: Length mismatch");

            keyTable.break_str_len = MakeKeyString(keyTable.break_str, keyTable.break_val);
        }
    }

    // 키테이블 초기화 (키의 문자열 및 길이 설정)
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


    static void SetKeyEvent(int key_idx, bool is_make){
        key_events.Append(new RzPair<int, bool>(key_idx, is_make));
    }

    // buff 크기 반환
    public static int GetSize(){
        return (DeviceProxy.Head - DeviceProxy.Tail + DeviceProxy.MAX_LINE) % DeviceProxy.MAX_LINE;
    }

    // buff 에 byte 추가 (여유 공간이 있다면)
    private bool PutByte(byte b){
        if (GetSize() == (DeviceProxy.MAX_LINE - 1))
            return false;

        DeviceProxy.Buffer[DeviceProxy.Head++] = b;
        DeviceProxy.Head %= DeviceProxy.MAX_LINE;

        return true;
    }

    // buff 에 byte 가져오기 (비어있지 않다면)
    public static bool GetByte(ref byte pb){
        if (GetSize() == 0)
            return false;

        pb = DeviceProxy.Buffer[DeviceProxy.Tail++];
        DeviceProxy.Tail %= DeviceProxy.MAX_LINE;

        return true;
    }
}
