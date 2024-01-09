using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
//192.168.0.54
public class UDP_Client : MonoBehaviour{
    private const string serverIP = "192.168.0.54"; // 서버의 IP 주소
    private const int serverPort = 9020;        // 서버의 포트 번호
    private UdpClient udpClient;
    private string LocalpressedKey =null;                       //입력키 저장 (키 누름 땜 인식을 위함)
    private StringBuilder pressedKeys = new StringBuilder();
    Queue<string> stringQueue = new Queue<string>();


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

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode))){
            if (Input.GetKeyDown(keyCode)){
                LocalpressedKey = GetPressedKeys();
                stringQueue.Enqueue(LocalpressedKey);
                SendKeyTable(LocalpressedKey,Input.anyKey);
            }
            else if(Input.GetKeyUp(keyCode)){
                LocalpressedKey = stringQueue.Dequeue();
                SendKeyTable(LocalpressedKey,Input.anyKey);
            }  
        }  
    }

    string GetPressedKeys(){
        StringBuilder pressedKeys = new StringBuilder();

        void AddKeyIfPressed(KeyCode keyCode, string keyName){
            if (Input.GetKey(keyCode)){
                pressedKeys.Append(keyName);
            }
        }

        for (KeyCode keyCode = KeyCode.F1; keyCode <= KeyCode.F12; keyCode++){
            if (Input.GetKey(keyCode)) pressedKeys.Append(keyCode.ToString());
        }

        AddKeyIfPressed(KeyCode.Backspace, "BKSP");
        AddKeyIfPressed(KeyCode.Space, "SPACE");
        AddKeyIfPressed(KeyCode.Return, "ENTER");
        AddKeyIfPressed(KeyCode.Tab, "TAB");
        AddKeyIfPressed(KeyCode.CapsLock, "CAPS");
        AddKeyIfPressed(KeyCode.Print, "PRINT");
        AddKeyIfPressed(KeyCode.ScrollLock, "SCROLL");
        AddKeyIfPressed(KeyCode.Pause, "PAUSE");
        AddKeyIfPressed(KeyCode.Insert, "INSERT");
        AddKeyIfPressed(KeyCode.Home, "HOME");
        AddKeyIfPressed(KeyCode.PageUp, "PAGEUP");
        AddKeyIfPressed(KeyCode.Delete, "DELETE");
        AddKeyIfPressed(KeyCode.End, "END");
        AddKeyIfPressed(KeyCode.PageDown, "PAGEDOWN");
        AddKeyIfPressed(KeyCode.Numlock, "NUMLOCK");
    
        AddKeyIfPressed(KeyCode.KeypadDivide, "KP /");
        AddKeyIfPressed(KeyCode.KeypadMultiply, "KP *");
        AddKeyIfPressed(KeyCode.KeypadMinus, "KP -");
        AddKeyIfPressed(KeyCode.KeypadPlus, "KP +");
        AddKeyIfPressed(KeyCode.KeypadEnter, "KP ENTER");
        AddKeyIfPressed(KeyCode.KeypadPeriod, "KP .");
        for (int i = 0; i <= 9; i++){
            AddKeyIfPressed(KeyCode.Keypad0 + i, $"KP {i}");
        }

        AddKeyIfPressed(KeyCode.RightShift, "R SHIFT");
        AddKeyIfPressed(KeyCode.LeftShift, "L SHIFT");

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)){
            pressedKeys.Append("change");
        }

        AddKeyIfPressed(KeyCode.UpArrow, "U ARROW");
        AddKeyIfPressed(KeyCode.DownArrow, "D ARROW");
        AddKeyIfPressed(KeyCode.LeftArrow, "L ARROW");
        AddKeyIfPressed(KeyCode.RightArrow, "R ARROW");

        foreach (char c in Input.inputString){
            if (c != '\0' && c != '\b' && !char.IsWhiteSpace(c))
                pressedKeys.Append(c.ToString().ToUpper());   
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

      private async void SendKeyTable(string keyName, bool keyDown){
        try{
            if (KeyTables.keyTableDictionary.TryGetValue(keyName, out var keyTable)){
                // 데이터 초기화
                Array.Clear(keyTable.make_str, 0, keyTable.make_str.Length);
                // make_str 또는 break_str을 바이트 배열로 변환하여 서버로 전송
                int dataLen = keyDown ? make_key_string(keyTable.make_str, keyTable.make_val) : make_key_string(keyTable.break_str, keyTable.break_val);
                byte[] data = keyDown ? keyTable.make_str : keyTable.break_str;

                await udpClient.SendAsync(data, dataLen);

                ClientText.text = $"Sent {(keyDown ? "make_str" : "break_str")} for key {keyName}";
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

}