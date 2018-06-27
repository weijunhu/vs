using System;
using System.IO;
using System.Net.Sockets;
using ProtoBuf;
using higgs_message;

namespace IocpServer
{
    public class AsyncUserToken
    {
        #region 网络相关
        public SocketAsyncEventArgs receiveArgs;
        private int sendTimeOut = 5000;
        private int receiveTimeOut = 5000;

        // 发送消息数组
        private bool sending = false;
        private byte[] sendBytes = new byte[1024 * 1024];
        private int sendOffset = 0;

        // 接收消息数组
        private byte[] receiveBytes = new byte[1024 * 1024];
        private int receiveOffset = 0;

        private Socket socket = null;

        #endregion 网络相关

        #region 玩家数据相关
        public Server mServer { private set; get; }
        public uint mUserid = 0;
        public string mRemoteIp = "";
        #endregion

        /// <summary>
        /// 创建发送和接收套接字并指定UserToken
        /// </summary>
        public AsyncUserToken(Server tServer)
        {
            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.UserToken = this;
            mServer = tServer;
        }

        /// <summary>
        /// 设置Socket并绑定到发送和接收异步套接字
        /// </summary>
        public Socket ConnectSocket
        {
            get { return socket; }
            set
            {
                socket = value;
                if (value != null)
                {
                    socket.SendTimeout = sendTimeOut;
                    socket.ReceiveTimeout = receiveTimeOut;
                    receiveArgs.AcceptSocket = socket;
                }
            }
        }

        public void Receive()
        {
            lock (receiveBytes)
            {
                Array.Copy(receiveArgs.Buffer, receiveArgs.Offset, receiveBytes, receiveOffset, receiveArgs.BytesTransferred);
                receiveOffset += receiveArgs.BytesTransferred;
            }

            MessageHandle();
        }

        byte[] GetOneMsg()
        {
            lock (receiveBytes)
            {
                if (receiveOffset >= 4)
                {
                    //获得长度
                    byte[] bytes = new byte[4];
                    Array.Copy(receiveBytes, 0, bytes, 0, 4);
                    int length = BitConverter.ToInt32(bytes, 0);
                    //返回一条消息
                    if (receiveOffset >= length)
                    {
                        bytes = new byte[length - 4];
                        Array.Copy(receiveBytes, 4, bytes, 0, bytes.Length);
                        //将剩下的字节流前移
                        for (int i = 0; i < receiveOffset - length; i++)
                        {
                            receiveBytes[i] = receiveBytes[i + length];
                        }
                        receiveOffset -= length;

                        return bytes;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        void MessageHandle()
        {
            while (true)
            {
                byte[] bytes = GetOneMsg();
                if (bytes != null)
                    ReceiveMsgManager.Receive(this, bytes);
                else
                    break;
            }
        }

        public void Send(byte[] newbytes)
        {
            uint length = (uint)(newbytes.Length + 4);
            byte[] lengthByte = BitConverter.GetBytes(length);

            lock (sendBytes)
            {
                Array.Copy(lengthByte, 0, sendBytes, sendOffset, lengthByte.Length);
                sendOffset += lengthByte.Length;
                Array.Copy(newbytes, 0, sendBytes, sendOffset, newbytes.Length);
                sendOffset += newbytes.Length;
                if (!sending)
                {
                    sending = true;
                    socket.BeginSend(sendBytes, 0, sendOffset, SocketFlags.None, SendAsync, null);
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
    }
}
