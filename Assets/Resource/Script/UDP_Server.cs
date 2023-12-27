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
            CheckVerify();
        }
        catch (Exception e){

            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    private async void CheckVerify(){

        if (udpServer.Available > 0){

            UdpReceiveResult result = await udpServer.ReceiveAsync();
            //byte[] data = udpServer.Receive(ref remoteEndPoint);
            //DeviceProxy.ScanCode = data;

            byte[] data = result.Buffer;            // 수신된 데이터 버퍼
            int dataLength = result.Buffer.Length;  // 수신된 데이터의 길이를 얻음
            Debug.Log(data.Length);
            DeviceProxy.ScanCode = data;

            //Debug.Log(BitConverter.ToString(data));
            //Debug.Log($"Data Length: {dataLength}");
            //Debug.Log(Encoding.ASCII.GetString(data));

            KeyTableManager.set_dev_data();

            bool keyIntegrity = KeyTableManager.Check();

            if (keyIntegrity) {
                StateText.text = "Key Table Integrity passed gid gud :) ";
                receivedMessageText.text = "Scancode: " + BitConverter.ToString(DeviceProxy.ScanCode);
                }
            else {
                StateText.text = "KeyCode Table Integrity Failed :( ";
                receivedMessageText.text = "Invalid Scancode received!";
                }
    
            }
        else
                StateText.text = "State" + " : ...";
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
