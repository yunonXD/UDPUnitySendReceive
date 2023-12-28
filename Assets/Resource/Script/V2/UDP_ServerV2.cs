using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct net_event_info
{
    public byte cmd;
    public int oid;
    public byte event_id;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public char[] data;

    public net_event_info(byte cmd, int oid, byte event_id, string data)
    {
        this.cmd = cmd;
        this.oid = oid;
        this.event_id = event_id;
        this.data = new char[256];
        Array.Copy(data.ToCharArray(), this.data, Math.Min(data.Length, 256));
    }
}

public class UDP_ServerV2 : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private Queue<byte[]> receivedDataQueue = new Queue<byte[]>(); // 수신된 데이터를 큐에 저장
    private readonly object lockObject = new object(); // 큐에 접근할 때 사용할 lock 오브젝트

    void Start()
    {
        int port = 9020;
        udpClient = new UdpClient(port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // 수신을 처리하는 코루틴 시작
        StartCoroutine(ReceiveDataCoroutine());
    }

    void Update()
    {
        // 메인 스레드에서 큐에 쌓인 데이터 처리
        lock (lockObject)
        {
            while (receivedDataQueue.Count > 0)
            {
                byte[] receivedData = receivedDataQueue.Dequeue();
                ProcessReceivedData(receivedData);
            }
        }
    }

    IEnumerator ReceiveDataCoroutine()
    {
        while (true)
        {
            try
            {
                UdpReceiveResult result = udpClient.ReceiveAsync().Result;
                byte[] receivedData = result.Buffer;

                // 수신된 데이터를 큐에 저장
                lock (lockObject)
                {
                    receivedDataQueue.Enqueue(receivedData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }

            yield return null;
        }
    }

    private void ProcessReceivedData(byte[] data)
    {
        int structSize = Marshal.SizeOf(typeof(net_event_info));
        int messageCount = data.Length / structSize;

        for (int i = 0; i < messageCount; i++)
        {
            // 수신된 데이터에서 net_event_info 구조체 추출
            byte[] messageData = new byte[structSize];
            Array.Copy(data, i * structSize, messageData, 0, structSize);

            // 바이트 배열을 net_event_info 구조체로 변환
            net_event_info eventInfo = ByteArrayToStructure<net_event_info>(messageData);

            // eventInfo를 사용하여 구조체의 필드에 접근 가능
            // 예: eventInfo.oid, eventInfo.event_id, eventInfo.cmd

            // 수신된 데이터를 디버그로 찍어줌
            Debug.Log($"Received Data - cmd: {eventInfo.cmd}, oid: {eventInfo.oid}, event_id: {eventInfo.event_id}, data: {new string(eventInfo.data)}");
        }
    }

    // 바이트 배열을 구조체로 변환
    private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        T structure;
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

        try
        {
            structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        }
        finally
        {
            handle.Free();
        }

        return structure;
    }
}
