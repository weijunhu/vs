using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Text;

public class UnityUdpSocket : ScriptBase
{
    public static Encoding mEncoding = Encoding.UTF8;
    // 定义节点
    private IPEndPoint ipEndPoint = null;
    // 定义UDP发送和接收
    private UdpClient mUdpClient = null;
    // 定义端口
    private const int mPort = 7001;
    
    Queue<byte[]> sendMsgInfoQueue = new Queue<byte[]>();
    bool sending = false;

    bool isOpen = false;

    bool mInGame = false;

    object lockobj = new object();


    void Awake()
    {
        //ipEndPoint = new IPEndPoint(IPAddress.Parse("39.108.179.167"), mPort);
        ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), mPort);
        mUdpClient = new UdpClient(0);
    }

    public void Open()
    {
        isOpen = true;
        mInGame = false;
        // 实例化        
        mUdpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    public void Connect()
    {
        StartCoroutine("ReqGameStartEnum");
    }

    IEnumerator ReqGameStartEnum()
    {
        while (!mInGame)
        {
            _UdpSendManager.SendReqGameStart();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Close()
    {
        isOpen = false;
        sendMsgInfoQueue.Clear();
    }

    // 接收回调函数
    private void ReceiveCallback(IAsyncResult iar)
    {
        if (!isOpen)
            return;
        mInGame = true;
        IPEndPoint point = null;
        byte[] receiveBytes = mUdpClient.EndReceive(iar, ref point);
        _UdpReciveManager.Receive(receiveBytes);
        mUdpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    public void Send(byte[] bytes)
    {
        if (!isOpen)
            return;

        int length = bytes.Length + 4;
        byte[] lengthbyte = BitConverter.GetBytes(length);
        byte[] sendBytes = new byte[length];
        Array.Copy(lengthbyte, 0, sendBytes, 0, lengthbyte.Length);
        Array.Copy(bytes, 0, sendBytes, lengthbyte.Length, bytes.Length);

        lock(lockobj)
        {
            if (!sending)
            {
                sending = true;
                mUdpClient.BeginSend(sendBytes, sendBytes.Length, ipEndPoint, new AsyncCallback(SendCallback), null);
            }
            else
            {
                sendMsgInfoQueue.Enqueue(sendBytes);
            }
        }
    }

    private void SendCallback(IAsyncResult iar)
    {
        if (!isOpen)
            return;
        lock (lockobj)
        {
            if (sendMsgInfoQueue.Count > 0)
            {
                byte[] bytes = sendMsgInfoQueue.Dequeue();
                mUdpClient.BeginSend(bytes, bytes.Length, ipEndPoint, new AsyncCallback(SendCallback), mUdpClient);
            }
            else
            {
                sending = false;
            }
        }
    }
}
