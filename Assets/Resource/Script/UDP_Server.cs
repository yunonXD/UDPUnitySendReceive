using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.EventSystems;

public class UDPServer : MonoBehaviour{

    [SerializeField] private TMP_InputField inputfieldTest;
    [SerializeField] private TextMeshProUGUI serverStatusText;
    [SerializeField] private TextMeshProUGUI StateText;
    [SerializeField] private TextMeshProUGUI receivedMessageText;
    [SerializeField] private int m_Port =9020;
    [SerializeField] private bool isHangulMode =false;      //한글유무

    [SerializeField] private EventSystem eventSystem;


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

            if(isHangulMode){
                //한글 입력을 위한 IME 활성화
                Input.imeCompositionMode = IMECompositionMode.On;
                Debug.Log("Korean IME ON");
            }
            else{
                Input.imeCompositionMode = IMECompositionMode.Off;
                Debug.Log("Korean IME OFF");
            }

            ProcessReceivedData();
        }
    }

private Queue<byte[]> cumulativeDataQueue = new Queue<byte[]>(); // 모든 데이터를 누적하는 큐

private void ProcessReceivedData() {
    while (receivedDataQueue.Count > 0) {
        byte[] newData = receivedDataQueue.Dequeue(); // 새로운 데이터를 큐에서 가져옴

        // 바이트 데이터에서 바이트가 들어있는 부분을 카운트하고 출력
        string partialMessage = Input.compositionString;
        foreach (byte b in newData) {
            if (b != 0x00) {
                int asciiValue = b;

                if (isHangulMode && KeyTables.KeyTableForLong.ContainsKey(asciiValue)) {
                    // 한글 입력 상태에서 키를 눌렀을 때 한글로 변환하여 부분 메시지에 추가
                    KeyTable keyTable = KeyTables.KeyTableForLong[asciiValue];
                    partialMessage += keyTable.KRname;
                } else {
                    KeyTable keyTable = KeyTables.KeyTableForLong[asciiValue];
                    partialMessage += keyTable.name;
                }
            } else {
                // 0x00은 데이터의 끝을 나타내므로, 반복문 종료
                break;
            }
        }

        // 출력 업데이트
        receivedMessageText.text = "Received Message: " + partialMessage;
        inputfieldTest.text += partialMessage;

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

    //버퍼와 스캔코드, 메시지 카운트를 모두 초기화.
    public static void Clear(){
        DeviceProxy.Head = DeviceProxy.Tail = 0;
        Array.Clear(DeviceProxy.Buffer, 0, DeviceProxy.MAX_LINE);
        Array.Clear(DeviceProxy.ScanCode, 0, DeviceProxy.KEY_CORD_SIZE);
        DeviceProxy.MessageCount = 0;
    }

}