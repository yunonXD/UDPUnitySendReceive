using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDP_ServerV2 : MonoBehaviour{

    public Text serverStatusText;
    public Text receivedMessageText;

    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;
    [SerializeField]private int PortNum =5555;

    void Start(){
        udpServer = new UdpClient(PortNum);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        serverStatusText.text = "Server Status: " + PortNum;
    }


    //단순한 스트링 데이터 8바이트 길이로 받는 리시버
    void Update(){
    if (udpServer.Available > 0){

        byte[] receivedData = udpServer.Receive(ref remoteEndPoint);

        // receivedData와 패딩된 8바이트 데이터를 비교
        if (receivedData.Length == 8){

            string receivedString = Encoding.UTF8.GetString(receivedData);
            Debug.Log($"Received from {remoteEndPoint}: {receivedString} ");
            
            // 받은 메시지를 UI에 표시
            receivedMessageText.text = $"Received from {remoteEndPoint}: {receivedString}   {receivedString.Length}";
        }
        else{
            Debug.Log($"Invalid data received from {remoteEndPoint}");
        }
    }
}


    void OnApplicationQuit(){
        udpServer.Close();
    }
}
