using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDPClient : MonoBehaviour{
    public Text ClientStatusText;
    public Text Client_TossMessage;


    [SerializeField] private string m_TryConnectIP ="192.168.0.54";
    [SerializeField] private int m_Port =5555;
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;

    private int m_Count =0;

    private void Start(){
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(m_TryConnectIP), m_Port);
        m_Count =0;
        UpdateClientStatus("Client started");
    }

    private void Update(){
        SendMessage();
        CleanScrean();
    }

    private void SendInputToServer(string message){
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndPoint);
        Client_TossMessage.text = "Client sent Mess : " + message;
    }

    private void SandInputScanCode(byte scancode){
        byte[] data = new byte[] {scancode};
        udpClient.Send(data, data.Length, serverEndPoint);
        Client_TossMessage.text = "scancode : " + scancode;
        //여기서 ScanCode 에 대한 전송값 전달 

    }

    private void UpdateClientStatus(string status){
        ClientStatusText.text = "Client Status: " + status;
    }

private void SendMessage(){

    if (Input.anyKeyDown){
        m_Count++;
        string pressedKey = Input.inputString;
        if (!string.IsNullOrEmpty(pressedKey)){
            char firstChar = pressedKey[0];
            SandInputScanCode((byte)firstChar);
        }
    }
}
    private void CleanScrean(){
        if(Input.GetKeyDown(KeyCode.F1)){
            m_Count =0;
            Client_TossMessage.text = "Client sent Mess :";
        }
    }

    private void OnDestroy(){
        if (udpClient != null)
            udpClient.Close();
    }
}