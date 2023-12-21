using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDPServer : MonoBehaviour{
    public Text serverStatusText;
    public Text receivedMessageText;

    [SerializeField] private int m_Port =5555;

    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;

    private void Start(){
        udpServer = new UdpClient(m_Port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        UpdateServerStatus("Server started on port :" + m_Port);
    }

    private void Update(){
        try{
            if (udpServer.Available > 0){
                byte[] data = udpServer.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                //Debug.Log("Received: " + message);

                // GUI에 수신한 메시지 표시
                UpdateReceivedMessage(message);

                // 이곳에서 수신받은 정보를 처리
            }
        }
        catch (Exception e){    //예외처리 >> 대부분의 문제상황에서 이쪽으로 점프
            Debug.LogError("Error receiving data: " + e.Message);
        }
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