using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDPServer : MonoBehaviour
{
    public Text serverStatusText;
    public Text receivedMessageText;

    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;

    private void Start()
    {
        udpServer = new UdpClient(5555);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        UpdateServerStatus("Server started on port 5555");
    }

    private void Update()
    {
        try
        {
            if (udpServer.Available > 0)
            {
                byte[] data = udpServer.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log("Received: " + message);

                // GUI�� ������ �޽��� ǥ��
                UpdateReceivedMessage(message);

                // ���⿡�� ���� �Է� ����(message)�� ó���ϸ� �˴ϴ�.
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    private void UpdateServerStatus(string status)
    {
        serverStatusText.text = "Server Status: " + status;
    }

    private void UpdateReceivedMessage(string message)
    {
        receivedMessageText.text = "Received Message: " + message;
    }

    private void OnDestroy()
    {
        if (udpServer != null)
            udpServer.Close();
    }
}
