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
        try{
            if (udpServer.Available > 0){
                byte[] data = udpServer.Receive(ref remoteEndPoint);

                DeviceProxy.ScanCode = data;
                
                bool integrityCheckResult = KeyTableManager.Check();    //무결성검사 여부 확인

                if (integrityCheckResult){
                    StateText.text = "Key Table integrity check passed.";
                    receivedMessageText.text = "Scancode: " + BitConverter.ToString(DeviceProxy.ScanCode);
                }
                else{
                    receivedMessageText.text = "Key Table integrity check failed!";
                }

            }
        }
        catch (Exception e){    //예외처리 >> 대부분의 문제상황에서 이쪽으로 점프
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    public void ScanCodeTester(){
        bool integrityCheckResult = KeyTableManager.Check(); // 무결성 검사 여부 확인
        if (integrityCheckResult){
            StateText.text = "Key Table integrity check passed.";
            receivedMessageText.text = "Scancode: " + BitConverter.ToString(DeviceProxy.ScanCode);
        }
        else{
            receivedMessageText.text = "Key Table integrity check failed!";
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