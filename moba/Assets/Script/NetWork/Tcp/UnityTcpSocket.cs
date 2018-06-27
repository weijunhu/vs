using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class UnityTcpSocket : ScriptBase
{
    private Socket socket = null;
    private int sendTimeOut = 5000;
    private int receiveTimeOut = 5000;
    private ManualResetEvent manual = new ManualResetEvent(false);

    private const int bufferSize = 1024 * 1024;

    // 发送消息数组
    private bool sending = false;
    private int sendOffset = 0;
    private byte[] sendBytes = new byte[bufferSize];

    private byte[] receiveBuffer = new byte[bufferSize];
    // 接收消息数组
    private byte[] receiveBytes = new byte[1024 * 1024];
    private int receiveOffset = 0;

    public Action<Protocol> ReceiveAction;
    
    void Update()
    {
        byte[] bytes = OneMsg();
        if (bytes != null)
            _TcpReciveManager.Receive(bytes);
    }

    public bool ConnectToServer()
    {
        if (socket != null && socket.Connected)
            return true;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.SendTimeout = sendTimeOut;
        socket.ReceiveTimeout = receiveTimeOut;
        socket.BeginConnect(IPAddress.Parse("127.0.0.1"), 7000, ConnectCallBack, null);
        //socket.BeginConnect(IPAddress.Parse("39.108.179.167"), 7000, ConnectCallBack, null);
        // 将事件状态设置为非终止状态，配合WaitOne，导致线程阻止。
        manual.Reset();
        if (manual.WaitOne(5000))
        {
            if (socket.Connected)
            {
                socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(Receive), null);
                return true;
            }
            else
                return false;
        }
        return false;
    }

    void ConnectCallBack(IAsyncResult result)
    {
        manual.Set();
    }

    byte[] OneMsg()
    {
        lock (receiveBytes)
        {
            if (receiveOffset >= 4)
            {
                byte[] head = new byte[4];
                Array.Copy(receiveBytes, 0, head, 0, 4);
                int length = BitConverter.ToInt32(head, 0);
                if (length <= receiveOffset)
                {
                    byte[] content = new byte[length - 4];
                    Array.Copy(receiveBytes, 4, content, 0, content.Length);

                    for (int i = 0; i < receiveOffset - length; i++)
                    {
                        receiveBytes[i] = receiveBytes[i + length];
                    }
                    receiveOffset -= length;
                    return content;
                }
                else
                    return null;
            }
            return null;
        }
    }

    void Receive(IAsyncResult ar)
    {
        int receiveSize = socket.EndReceive(ar);
        if (receiveSize > 0)
        {
            lock (receiveBytes)
            {
                Array.Copy(receiveBuffer, 0, receiveBytes, receiveOffset, receiveSize);
                receiveOffset += receiveSize;            
                socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(Receive), socket);
            }
        }
        else
        {
            ShutDown();
        }
    }

    public void Send(byte[] bytes)
    {
        if (socket != null && socket.Connected)
        {
            int length = bytes.Length + 4;
            byte[] lengthbyte = BitConverter.GetBytes(length);
            lock (sendBytes)
            {
                Array.Copy(lengthbyte, 0, sendBytes, sendOffset, lengthbyte.Length);
                sendOffset += lengthbyte.Length;
                Array.Copy(bytes, 0, sendBytes, sendOffset, bytes.Length);
                sendOffset += bytes.Length;
                if (!sending)
                {
                    sending = true;
                    socket.BeginSend(sendBytes, 0, sendOffset, SocketFlags.None, SendAsync, null);
                }
            }
        }
    }

    void SendAsync(IAsyncResult result)
    {
        int sendsize = socket.EndSend(result);
        lock (sendBytes)
        {
            for (int i = 0; i < sendOffset - sendsize; i++)
            {
                sendBytes[i] = sendBytes[i + sendsize];
            }
            sendOffset -= sendsize;
            if (sendOffset > 0)
                socket.BeginSend(sendBytes, 0, sendOffset, SocketFlags.None, SendAsync, null);
            else
                sending = false;
        }
    }

    public void ShutDown()
    {
        if (socket != null && socket.Connected)
        {
            Debug.LogError("关闭连接");
            try { socket.Shutdown(SocketShutdown.Both); }
            catch (Exception e) { e.ToString(); }
            finally { socket.Close(); socket = null; }
        }
    }
}
