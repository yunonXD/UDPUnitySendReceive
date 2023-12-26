// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using UnityEngine;
// using UnityEngine.UI;

// public class UDPServer : MonoBehaviour{
//     public Text serverStatusText;
//     public Text StateText;
//     public Text receivedMessageText;

//     [SerializeField] private int m_Port =5555;

//     private UdpClient udpServer;
//     private IPEndPoint remoteEndPoint;

//     private void Start(){
//         udpServer = new UdpClient(m_Port);
//         remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

//         UpdateServerStatus("Server started on port :" + m_Port);
//     }

//     private void Update(){
//         try{

//             if (udpServer.Available > 0){

//                 byte[] data = udpServer.Receive(ref remoteEndPoint);    //클라이언트에게 받아온 데이터

//                 DeviceProxy.ScanCode = data;        //를 스캔코드에 쏘옥

//                 ScanCodeTester();
//             }
//         }
//         catch (Exception e){

//             Debug.LogError("Error receiving data: " + e.Message);
//         }
//     }

//     public void ScanCodeTester(){       //무결성 검사

//         if (KeyTableManager.Check()){

//             StateText.text = "Key Table integrity check passed.";
//             receivedMessageText.text = "Scancode: " + BitConverter.ToString(DeviceProxy.ScanCode);
//         }
//         else{
//             receivedMessageText.text = "Key Table integrity check failed!";
//         }
//     }

//     private void UpdateServerStatus(string status){
//         serverStatusText.text = "Server Status: " + status;
//     }

//     private void UpdateReceivedMessage(string message){
//         receivedMessageText.text = "Received Message: " + message;
//     }

//     private void OnDestroy(){
//         if (udpServer != null)
//             udpServer.Close();
//     }
// }