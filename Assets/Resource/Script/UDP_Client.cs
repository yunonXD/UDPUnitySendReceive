using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//192.168.0.54
public class UDP_Client : MonoBehaviour{
    private const string serverIP = "192.168.0.54"; // 서버의 IP 주소
    private const int serverPort = 9020;        // 서버의 포트 번호
    private UdpClient udpClient;
    private bool wasAnyKeyPreviouslyPressed = false;        //키 입력 여부 false=눌림 true=들림
    private string LocalpressedKey =null;                       //입력키 저장 (키 누름 땜 인식을 위함)
    private StringBuilder pressedKeys = new StringBuilder();

    [SerializeField] TextMeshProUGUI ClientState;
    [SerializeField] TextMeshProUGUI ClientText;
    [SerializeField] TMP_InputField InputField_Client;

    void Start(){
        udpClient = new UdpClient();
        init_key_table();
        ConnectToServer();
        LocalpressedKey=null;
        pressedKeys.Clear();
    }

    void Update(){
        
        if (Input.anyKey){
            // 아무 키가 눌린 상태에서만 실행되는 코드 작성
            if (!wasAnyKeyPreviouslyPressed){
                LocalpressedKey = GetPressedKeys();
                SendKeyTable(LocalpressedKey);
                //Debug.Log("input in: "+LocalpressedKey);
            }
            wasAnyKeyPreviouslyPressed = true;
        }
        else{
            // 아무 키가 놓예진 상태에서만 실행되는 코드 작성
            if (wasAnyKeyPreviouslyPressed){
                LocalpressedKey = GetPressedKeys();
                SendKeyTable(LocalpressedKey);
                //Debug.Log("input up: "+LocalpressedKey);
            }
            wasAnyKeyPreviouslyPressed = false;
            LocalpressedKey=null;
            pressedKeys.Clear();
        }
    }


    public void InputKeyClient(){
        if (Input.anyKey){
            // 아무 키가 눌린 상태에서만 실행되는 코드 작성
            if (!wasAnyKeyPreviouslyPressed){
                LocalpressedKey = GetPressedKeys();
                SendKeyTable(LocalpressedKey);
                //Debug.Log("input in: "+LocalpressedKey);
            }
            wasAnyKeyPreviouslyPressed = true;
        }
        else{
            // 아무 키가 놓예진 상태에서만 실행되는 코드 작성
            if (wasAnyKeyPreviouslyPressed){
                LocalpressedKey = GetPressedKeys();
                SendKeyTable(LocalpressedKey);
                //Debug.Log("input up: "+LocalpressedKey);
            }
            wasAnyKeyPreviouslyPressed = false;
            LocalpressedKey=null;
            pressedKeys.Clear();
        }
    }

    string GetPressedKeys(){

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
        }
        catch (Exception e){
            Debug.LogError($"Failed to connect to server: {e.Message}");
        }
    }

    private async void SendKeyTable(string keyName){

        try{
            // 키 테이블 가져오기
            if (KeyTables.keyTableDictionary.TryGetValue(keyName, out var keyTable)){

                if(!wasAnyKeyPreviouslyPressed){        //키 down 상태일때

                    // make_str 데이터 초기화
                    Array.Clear(keyTable.make_str, 0, keyTable.make_str.Length);
                    // keyTable의 make_str을 바이트 배열로 변환하여 서버로 전송
                    keyTable.make_str_len = make_key_string(keyTable.make_str, keyTable.make_val);
                    byte[] data_make_str = keyTable.make_str;

                    await udpClient.SendAsync(data_make_str, keyTable.make_str_len);

                    ClientText.text = $"Sent make_str for key {keyName}";
                    keyTable.make_str_len=0;
                }
                else if(wasAnyKeyPreviouslyPressed){    //키 up 상태일때

                    // break_str 데이터 초기화
                    Array.Clear(keyTable.break_str, 0, keyTable.break_str.Length);
                    // keyTable의 break_str을 바이트 배열로 변환하여 서버로 전송
                    keyTable.break_str_len = make_key_string(keyTable.break_str, keyTable.break_val);
                    byte[] data_break_str = keyTable.break_str;

                    await udpClient.SendAsync(data_break_str, keyTable.break_str_len);

                    ClientText.text = $"Sent break_str for key {keyName}";
                    keyTable.break_str_len=0;
                }
                else
                    Debug.LogError("key input errer");
            }                
        }
        catch (Exception e){
            Debug.LogError($"Failed to send KeyTable data: {e.Message}");   
        }
    }


//keytable 의 make_str ,  break_str , 에 대한 길이(make_str_len 및 break_str_len)
//를 계산하고 설정
    public void init_key_table(){

        foreach (var keyTable in KeyTables.keyTableDictionary.Values){

            keyTable.make_str_len = make_key_string(keyTable.make_str, keyTable.make_val);

            if (keyTable.make_str_len == 0){
                throw new Exception("KeyTable initialization error: Length mismatch");
            }
            keyTable.break_str_len = make_key_string(keyTable.break_str, keyTable.break_val);
        }
    }

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

    // 바이트 배열을 문자열로 변환하는 헬퍼 함수
    string ByteArrayToString(byte[] byteArray){
        return Encoding.ASCII.GetString(byteArray);
    }
}
