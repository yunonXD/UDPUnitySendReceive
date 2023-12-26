using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private const string serverIP = "127.0.0.1"; // 서버의 IP 주소
    private const int serverPort = 12345; // 서버의 포트 번호
    private UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
        ConnectToServer();
    }

    void Update()
    {
        // 사용자의 입력을 받아서 서버에 전송
        if (Input.anyKeyDown)
        {
            // 아무 키나 눌렀을 때 동작하도록 변경
            SendKeyTable(Input.inputString);
        }
    }

    void OnDestroy()
    {
        udpClient.Close();
    }

    private void ConnectToServer()
    {
        try
        {
            udpClient.Connect(serverIP, serverPort);
            Debug.Log("Connected to server");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to server: {e.Message}");
        }
    }

    private void SendKeyTable(string keyName)
    {
        try
        {
            KeyTable keyTable = FindKeyTable(keyName);

            if (keyTable != null)
            {
                byte[] data = Encoding.UTF8.GetBytes($"KeyTable: {keyTable.name}, {keyTable.make_val}, {keyTable.break_val}, {keyTable.scan_key}, {keyTable.os_vk_key}");
                udpClient.Send(data, data.Length);
                Debug.Log($"Sent KeyTable data for key {keyName}");
            }
            else
            {
                Debug.LogWarning($"KeyTable for key {keyName} not found");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send KeyTable data: {e.Message}");
        }
    }

    private KeyTable FindKeyTable(string keyName)
    {
        return Array.Find(KeyTables.key_tables, keyTable => keyTable.name.Equals(keyName));
    }
}
