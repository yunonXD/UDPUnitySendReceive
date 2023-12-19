using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDPClient : MonoBehaviour
{
    public Text clientStatusText;

    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;

    private void Start()
    {
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);

        UpdateClientStatus("Client started");
    }

    private void Update()
    {
        // 예제로 입력 정보를 보내는 코드
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendInputToServer("Space Key Pressed");
        }
    }

    private void SendInputToServer(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndPoint);
        Debug.Log("Sent: " + message);
    }

    private void UpdateClientStatus(string status)
    {
        clientStatusText.text = "Client Status: " + status;
    }

    private void OnDestroy()
    {
        if (udpClient != null)
            udpClient.Close();
    }
}
