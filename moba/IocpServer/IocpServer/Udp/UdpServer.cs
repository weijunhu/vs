using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IocpServer
{
    public class UdpServer
    {
        Server mServer = null;
        UdpClient udpClient = null;
        UdpClient sendClient = null;        

        public UdpServer(int tPort, Server tServer)
        {
            mServer = tServer;
            udpClient = new UdpClient(tPort);
            sendClient = new UdpClient(0);
            udpClient.BeginReceive(ReceiveAsync, null);            
        }

        void ReceiveAsync(IAsyncResult tResult)
        {
            IPEndPoint senderPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvData = udpClient.EndReceive(tResult, ref senderPoint);
            UdpReceiveMsgManager.Receive(recvData,mServer, senderPoint);
            udpClient.BeginReceive(ReceiveAsync, null);
        }

        Queue<MsgInfo> msgQueue = new Queue<MsgInfo>();
        bool isSending = false;
        object sendlock = new object();

        public void SendMessage(MsgInfo tMsg)
        {
            lock (sendlock)
            {
                if (!isSending)
                {
                    isSending = true;
                    sendClient.BeginSend(tMsg.sendbytes, tMsg.length, tMsg.point, SendAsync, null);
                }
                else
                    msgQueue.Enqueue(tMsg);
            }
        }

        void SendAsync(IAsyncResult tResult)
        {
            lock (sendlock)
            {                
                int sendcount = sendClient.EndSend(tResult);
                if (msgQueue.Count > 0)
                {
                    MsgInfo msg = msgQueue.Dequeue();
                    sendClient.BeginSend(msg.sendbytes, msg.length, msg.point, SendAsync, null);
                }
                else
                    isSending = false;
            }
        }
    }

    public struct MsgInfo
    {
        public int length;
        public IPEndPoint point;
        public byte[] sendbytes;

        public MsgInfo(IPEndPoint point, byte[] sendbytes, int length)
        {
            this.point = point;
            this.sendbytes = sendbytes;
            this.length = length;
        }
    }
}
