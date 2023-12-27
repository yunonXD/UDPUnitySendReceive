using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UDPServer : MonoBehaviour{
    public Text serverStatusText;
    public Text StateText;
    public Text receivedMessageText;

    [SerializeField] private int m_Port =9020;

    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;

    private void Start(){
        KeyTableManager.init_key_table();
        KeyTableManager.Clear();

        udpServer = new UdpClient(m_Port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        UpdateServerStatus("Server started on port :" + m_Port);
    }

    private void Update(){
        try{

            if (udpServer.Available > 0){

                byte[] data = udpServer.Receive(ref remoteEndPoint);    //클라이언트에게 받아온 데이터
                DeviceProxy.ScanCode = data;
                Debug.Log(DeviceProxy.ScanCode);

                bool keyIntegrity = KeyTableManager.Check();

                StateText.text = keyIntegrity ? "Key Table Integrity passed gid gud :) "  : " KeyCode Table Integrity Failed :( ";
                receivedMessageText.text = "Scancode: " + BitConverter.ToString(DeviceProxy.ScanCode);
    
            }
        }
        catch (Exception e){

            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    private void ReceiveSTRdata(){
        try{
            
            


        }
        catch(Exception e){
            Debug.LogError($"Failed to check table with input make_str : {e.Message}" );
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
