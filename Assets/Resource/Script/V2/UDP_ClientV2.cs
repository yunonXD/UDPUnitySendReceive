// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using UnityEngine;
// using UnityEngine.UI;

// public class UDP_ClientV2 : MonoBehaviour{

//     public Text ClientStatusText;
//     public Text Client_TossMessage;

//     [SerializeField]private UdpClient udpClient;
//     private IPEndPoint serverEndPoint;

//     [SerializeField] private string m_TryConnectIP ="192.168.0.1"; //54
//     [SerializeField]private int port =5555;
//     public InputField TextIP;

//     void Start(){
//         try{
//             udpClient = new UdpClient();
//             serverEndPoint = new IPEndPoint(IPAddress.Parse(m_TryConnectIP), port);
//             UpdateClientStatus("Client started");
//         }
//         catch(Exception ex){
//             Debug.LogError("Error during client update: " + ex.Message);
//         }
//     }

//     void Update(){
//         if (Input.anyKeyDown){
//             string inputString = Input.inputString;

//             if (!string.IsNullOrEmpty(inputString)){


                    


//                 //byte[] data = Encoding.UTF8.GetBytes(inputString.PadRight(8)); // 8바이트로 패딩

//             // 역으로 리버스
//             //ReverseByteArray(data);

//             udpClient.Send(data, data.Length, serverEndPoint);
//             Client_TossMessage.text = $"Sent to server: {inputString}";
//             }
//         }
//     }

// // 바이트 배열을 역으로 리버스하는 함수
// void ReverseByteArray(byte[] array)
// {
//     int i = 0;
//     int j = array.Length - 1;

//     while (i < j)
//     {
//         // Swap array[i] and array[j]
//         byte temp = array[i];
//         array[i] = array[j];
//         array[j] = temp;

//         // Move indices towards the center
//         i++;
//         j--;
//     }
// }



//     void SendInputToServer(string userInput){
//         try{
//             byte[] data = Encoding.UTF8.GetBytes(userInput);
//             udpClient.Send(data, data.Length, serverEndPoint);
//             Client_TossMessage.text = Encoding.UTF8.GetString(data);
//         }
//         catch (Exception ex){
//             Debug.LogError("Error sending data to server: " + ex.Message);
//         }
//     }

//       public void InPut_IP(){
//         string P_ip = TextIP.text;
//         m_TryConnectIP = P_ip;
//     }

//     void OnApplicationQuit(){
//         udpClient.Close();
//     }

//     private void UpdateClientStatus(string status){
//         ClientStatusText.text = "Client Status: " + status;
//     }
// }
