using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UDPClient : MonoBehaviour{

#region TextField
    public Text ClientStatusText;
    public Text Client_TossMessage;
    public InputField TextIP;
#endregion

#region ClientField
    [SerializeField] private string m_TryConnectIP ="192.168.0.1"; //54
    [SerializeField] private int m_Port =5555;
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;
#endregion


    private void Start(){
        try{

            udpClient = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(m_TryConnectIP), m_Port);
            UpdateClientStatus("Client started");

        }
        catch (Exception ex){

            Debug.LogError("Error initializing udpClient: " + ex.Message);
        }
    }

    private void Update(){
        SendMessage();
        CleanScrean();
    }

    public void InPut_IP(){
        string P_ip = TextIP.text;
        m_TryConnectIP = P_ip;
    }

    private void UpdateClientStatus(string status){
        ClientStatusText.text = "Client Status: " + status;
    }


//    private void SandInputScanCode(byte[] virtualInput){
    
//         if (udpClient != null){
//             udpClient.Send(virtualInput, virtualInput.Length, serverEndPoint);
//             Client_TossMessage.text = "Virtual Key Table sent to server: " + GetKeyName(virtualInput);
//         }
//         else
//             Client_TossMessage.text = "Error: udpClient is not initialized.";
//     }



    private void SendMessage()
{
    if (Input.anyKeyDown)
    {
        // Get the first key pressed
        string pressedKey = Input.inputString;
        if (!string.IsNullOrEmpty(pressedKey))
        {
            // Assuming KEYCODE_SIZE is 8, modify this based on the actual size
            const int KEYCODE_SIZE = 8;

            // Ensure the message is 8 bytes long
            if (pressedKey.Length < KEYCODE_SIZE)
            {
                // Pad the message with ASCII 0 to make it 8 bytes
                pressedKey = pressedKey.PadRight(KEYCODE_SIZE, '\0');
            }
            else if (pressedKey.Length > KEYCODE_SIZE)
            {
                Debug.LogError("Pressed key must be exactly 8 characters long. Truncating to 8 characters.");
                // Truncate the message to 8 characters
                pressedKey = pressedKey.Substring(0, KEYCODE_SIZE);
            }

            // Convert the key to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(pressedKey);

            // Send the message to the server
            udpClient.Send(messageBytes, messageBytes.Length, serverEndPoint);

            // Update UI
            Client_TossMessage.text = "Virtual Key Table sent to server: " + Encoding.UTF8.GetString(messageBytes);
        }
    }
}



    private byte[] GetScanCode(char key){
        // 키 테이블에서 해당 키의 스캔 코드를 찾아서 반환

        foreach (KeyTable keyTable in KeyTables.key_tables){

            if (keyTable.name.Length > 0 && (keyTable.name[0] == key ||
                     char.ToLower(keyTable.name[0]) == char.ToLower(key))){
                Debug.Log("Scan Code found: " + keyTable.scan_key);
                return BitConverter.GetBytes(keyTable.scan_key);
            }
            else{
                Debug.LogError("해당 키가 적용되는 스캔코드가 없어요");
            }
        }
        return null;
    }

    private string GetKeyName(byte[] scanCode){
        //스캔코드로부터 이름 찾아내서 리턴
        int scanCodeInt = BitConverter.ToInt32(scanCode , 0);
        string keyName = KeyTables.FindKeyStr(scanCodeInt);
        return keyName != null? keyName : "Wut the hack";
    }

    private void CleanScrean(){

        if(Input.GetKeyDown(KeyCode.F1)){
            Client_TossMessage.text = "Client sent Mess :";
        }
    }

    private void OnDestroy(){

        if (udpClient != null)
            udpClient.Close();
    }
}