using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UDP_ClientV2 : MonoBehaviour{

    public Text ClientStatusText;
    public Text Client_TossMessage;

    [SerializeField]private UdpClient udpClient;
    private IPEndPoint serverEndPoint;

    [SerializeField] private string m_TryConnectIP ="192.168.0.1"; //54
    [SerializeField]private int port =5555;
    public TMP_InputField TextIP;

    void Start(){
        try{
            udpClient = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(m_TryConnectIP), port);
            UpdateClientStatus("Client started");
        }
        catch(Exception ex){
            Debug.LogError("Error during client update: " + ex.Message);
        }
    }

     void Update(){

        try{
            if (Input.GetKeyDown(KeyCode.UpArrow)){
                SendInputToServer("up");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)){
                SendInputToServer("down");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)){
                SendInputToServer("left");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)){
                SendInputToServer("right");
            }
        }
        catch(Exception ex){
            Debug.LogError("Error during client update: " + ex.Message);
        }
    }


    void SendInputToServer(string userInput){
        try{
            byte[] data = Encoding.UTF8.GetBytes(userInput);
            udpClient.Send(data, data.Length, serverEndPoint);
            Client_TossMessage.text = Encoding.UTF8.GetString(data);
        }
        catch (Exception ex){
            Debug.LogError("Error sending data to server: " + ex.Message);
        }
    }

      public void InPut_IP(){
        string P_ip = TextIP.text;
        m_TryConnectIP = P_ip;
    }

    void OnApplicationQuit(){
        udpClient.Close();
    }

    private void UpdateClientStatus(string status){
        ClientStatusText.text = "Client Status: " + status;
    }
}
