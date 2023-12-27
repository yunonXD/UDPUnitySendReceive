using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDP_Client : MonoBehaviour{
    private const string serverIP = "192.168.0.54"; // 서버의 IP 주소
    private const int serverPort = 9020;        // 서버의 포트 번호
    private UdpClient udpClient;


    [SerializeField] Text ClientState;
    [SerializeField] Text ClientText;

    void Start(){
        udpClient = new UdpClient();
        init_key_table();
        ConnectToServer();
    }

    void Update(){
        // 사용자의 입력을 받아서 서버에 전송
        // if (Input.anyKey){ 
        //     SendKeyTable(Input.inputString.ToUpper()); // 대문자로 변환      
        // }

        if(Input.anyKeyDown){
            string pressedKeys = GetPressedKeys();
            SendKeyTable(pressedKeys);
        }
    }

    string GetPressedKeys(){
    StringBuilder pressedKeys = new StringBuilder();

    // F1부터 F12까지 검사
    for(KeyCode keyCode = KeyCode.F1; keyCode <= KeyCode.F12; keyCode++){
        if(Input.GetKey(keyCode)){
            pressedKeys.Append(keyCode.ToString());
        }
    }

    // Alt 키 검사
    if(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
        pressedKeys.Append("Alt");
    }

    // 나머지 키 검사
    foreach(char c in Input.inputString){
        if(c != '\0'){
            pressedKeys.Append(c.ToString().ToUpper());
        }
    }

    return pressedKeys.ToString();
}

    

    void OnDestroy(){
        udpClient.Close();
    }

    private void ConnectToServer(){
        try{
            udpClient.Connect(serverIP, serverPort);
            Debug.Log("Connected to server");
        }
        catch (Exception e){
            Debug.LogError($"Failed to connect to server: {e.Message}");
        }
    }

   private async void SendKeyTable(string keyName){
    try{
        // 키 테이블 가져오기
        if (KeyTables.keyTableDictionary.TryGetValue(keyName, out var keyTable)){

            // 데이터 초기화
            Array.Clear(keyTable.make_str, 0, keyTable.make_str.Length);

            // keyTable의 make_str을 바이트 배열로 변환하여 서버로 전송
            keyTable.make_str_len = make_key_string(keyTable.make_str, keyTable.make_val);
            byte[] data = keyTable.make_str;

            await udpClient.SendAsync(data, keyTable.make_str_len);     //이 부분이 문제였다! 
            //Debug.Log($"Sent make_str for key {keyName}: {ByteArrayToString(data)}");
            ClientText.text = $"Sent make_str for key {keyName}: {ByteArrayToString(data)}";
            keyTable.make_str_len=0;
        }
    }
    catch (Exception e){
        Debug.LogError($"Failed to send KeyTable data: {e.Message}");
    }
}


//keytable 의 make_str ,  break_str , 에 대한 길이(make_str_len 및 break_str_len)
//를 계산하고 설정
public void init_key_table()
{
    foreach (var keyTable in KeyTables.keyTableDictionary.Values)
    {
        keyTable.make_str_len = make_key_string(keyTable.make_str, keyTable.make_val);

        if (keyTable.make_str_len == 0)
        {
            throw new Exception("KeyTable initialization error: Length mismatch");
        }

        keyTable.break_str_len = make_key_string(keyTable.break_str, keyTable.break_val);
    }
}
    int make_key_string(byte[] dest, long input)
{
    byte[] temp = new byte[DeviceProxy.KEY_CORD_SIZE];
    int len = 0;

    BitConverter.GetBytes(input).CopyTo(temp, 0);
    Array.Clear(dest, 0, DeviceProxy.KEY_CORD_SIZE);

    for (int i = 0; i < DeviceProxy.KEY_CORD_SIZE; i++)
    {
        if (temp[i] == 0x00) break;

        dest[i] = temp[i];
        len++;
    }

    if (len == 0) return 0;

    int j = 0;
    for (int i = len; i > 0; i--)
    {
        dest[j] = temp[i - 1];
        j++;
    }

    return len;
}

    // 바이트 배열을 문자열로 변환하는 헬퍼 함수
    string ByteArrayToString(byte[] byteArray)
    {
        return Encoding.ASCII.GetString(byteArray);
    }
}
