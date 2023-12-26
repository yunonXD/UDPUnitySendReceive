using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UDPClient : MonoBehaviour{

#region TextField
    public Text ClientStatusText;
    public Text Client_TossMessage;
    public TMP_InputField TextIP;
#endregion

#region ClientField
    [SerializeField] private string m_TryConnectIP ="192.168.0.1"; //54
    [SerializeField] private int m_Port =5555;
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;
#endregion


    private void Start(){
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(m_TryConnectIP), m_Port);
        UpdateClientStatus("Client started");
    }

    private void Update(){
        SendMessage();
        CleanScrean();
    }

    public void InPut_IP(){
        string P_ip = TextIP.text;
        m_TryConnectIP = P_ip;
    }

    private void SendInputToServer(byte[] virtualInput){
    udpClient.Send(virtualInput, virtualInput.Length, serverEndPoint);
    Client_TossMessage.text = "Client sent Message: " + BitConverter.ToString(virtualInput);
    }


    private void UpdateClientStatus(string status){
        ClientStatusText.text = "Client Status: " + status;
    }


   private void SandInputScanCode(byte[] virtualInput){
        udpClient.Send(virtualInput, virtualInput.Length, serverEndPoint);
        Client_TossMessage.text = "Virtual Key Table sent to server." + BitConverter.ToString(virtualInput);
    }

    //서버에게 전송할 가상 키테이블 생성
    private byte[] GenerateVirtualKey(char inputkey){

        // 입력한 문자에 해당하는 상수명
        string constantName = "SCAN_" + inputkey.ToString().ToUpper();
        Debug.Log(constantName);

        // 상수명에 해당하는 상수가 정의되어 있는지 확인
        if (Enum.IsDefined(typeof(Scan_Code), constantName)){

            // 상수명에 해당하는 상수 값을 가져오기
            int scanCodeValue = (int)Enum.Parse(typeof(Scan_Code), constantName);

            return new byte[DeviceProxy.KEY_CORD_SIZE] { (byte)scanCodeValue, 0, 0, 0, 0, 0, 0, 0 };
        }
        else{
            // 상수가 정의되어 있지 않으면 기본값 반환 또는 예외 처리
            return new byte[DeviceProxy.KEY_CORD_SIZE] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }
    }


    private void SendMessage(){

        if (Input.anyKeyDown){

            string pressedKey = Input.inputString;

            if (!string.IsNullOrEmpty(pressedKey)){
                char firstChar = pressedKey[0];
                byte[] virtualInput = GenerateVirtualKey(firstChar);
                SandInputScanCode(virtualInput);
            }
        }
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