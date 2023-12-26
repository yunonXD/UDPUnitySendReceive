using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDPServer : MonoBehaviour{
    public Text serverStatusText;
    public Text StateText;
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
    try
    {
        if (udpServer.Available > 0)
        {
            byte[] data = udpServer.Receive(ref remoteEndPoint); // 클라이언트로부터 받아온 데이터

            Debug.Log($"Received {data.Length} bytes.");

            // Ensure the received message is not empty
            if (data.Length > 0)
            {
                // Check the received message
                if (CheckMessage(data))
                {
                    receivedMessageText.text = $"Received Key: {Encoding.UTF8.GetString(data)}\nMessage check passed!";
                }
                else
                {
                    receivedMessageText.text = $"Received Key: {Encoding.UTF8.GetString(data)}\nMessage check failed!";
                }
            }
        }
    }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }


    private bool CheckMessage(byte[] messageBytes)
{
    // Assuming KEYCODE_SIZE is 8, modify this based on the actual size
    const int KEYCODE_SIZE = 8;

    // Check for message length
    if (messageBytes.Length != KEYCODE_SIZE)
    {
        Debug.LogError("Invalid message length!");
        return false;
    }

    string receivedMessage = Encoding.UTF8.GetString(messageBytes);
    string expectedMessage = "Error_there is no 8b string.";

    if (receivedMessage == expectedMessage)
        return true;
    else
    {
        Debug.LogError("Message content mismatch!");
        return false;
    }
}


    public void ScanCodeTester(){       //무결성 검사

        if (KeyTableManager.Check()){

            StateText.text = "Key Table integrity check passed.";
            receivedMessageText.text = "Scancode: " + BitConverter.ToString(DeviceProxy.ScanCode);
        }
        else{
            StateText.text = "Key Table integrity check failed!";
        }
    }

    private void UpdateServerStatus(string status){
        serverStatusText.text = "Server Status: " + status;
    }

    private void OnDestroy(){
        if (udpServer != null)
            udpServer.Close();
    }
}