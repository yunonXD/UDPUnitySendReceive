using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDP_ServerV2 : MonoBehaviour
{
    public Text serverStatusText;
    public Text receivedMessageText;

    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;
    [SerializeField] private int PortNum = 5555;

    const int KEYCODE_SIZE = 8; // Assuming KEYCODE_SIZE is 8

    void Start()
    {
        udpServer = new UdpClient(PortNum);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        serverStatusText.text = "Server Status: " + PortNum;
    }

    void Update()
    {
        if (udpServer.Available > 0)
        {
            byte[] receivedData = udpServer.Receive(ref remoteEndPoint);

            // Check if the received data has a valid length
            if (receivedData.Length == KEYCODE_SIZE)
            {
                byte[] reversedData = new byte[KEYCODE_SIZE];
                int len = MakeKeyString(reversedData, BitConverter.ToInt64(receivedData, 0));

                // len이 KEYCODE_SIZE와 같은지 확인
                if (len == KEYCODE_SIZE)
                {
                    string receivedString = Encoding.UTF8.GetString(reversedData);
                    Debug.Log($"Received from {remoteEndPoint}: {receivedString}");

                    // 받은 메시지를 UI에 표시
                    receivedMessageText.text = $"Received from {remoteEndPoint}: {receivedString}   {receivedString.Length}";
                }
                else
                {
                    Debug.Log($"Invalid data length received from {remoteEndPoint}");
                }
            }
            else
            {
                Debug.Log($"Invalid data length received from {remoteEndPoint}");
            }
        }
    }

    // Unity에서 사용하는 함수
    int MakeKeyString(byte[] dest, long input)
    {
        // make input value to byte array
        byte[] temp = BitConverter.GetBytes(input);
        int len = 0;

        Array.Clear(dest, 0, KEYCODE_SIZE);

        for (int i = 0; i < KEYCODE_SIZE; i++)
        {
            if (temp[i] == 0x00)
                break;

            dest[i] = temp[i];
            len++;
        }

        if (len == 0)
            return 0;

        // reverse destination buffer
        Array.Reverse(dest, 0, len);

        return len;
    }

    void OnApplicationQuit()
    {
        udpServer.Close();
    }
}
