using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UDP_ServerV2 : MonoBehaviour{


    public Text serverStatusText;
    public Text receivedMessageText;

    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;
    [SerializeField]private int PortNum =5555;

    void Start(){
        udpServer = new UdpClient(PortNum);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        serverStatusText.text = "Server Status: " + PortNum;
    }

    void Update(){
        if (udpServer.Available > 0){
            byte[] receivedData = udpServer.Receive(ref remoteEndPoint);
            string clientInput = Encoding.UTF8.GetString(receivedData);


            string[] validInputs = { "up", "down", "left", "right" };

            bool isValidInput = Array.Exists(validInputs, input => input.Equals(clientInput));

            if (isValidInput)
                receivedMessageText.text = "Received valid input from client: " + clientInput;

            else
                receivedMessageText.text = "Received invalid input from client: " + clientInput;
        }
    }
    void OnApplicationQuit(){
        udpServer.Close();
    }
}
