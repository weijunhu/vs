using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IocpServer
{
    public class Server
    {
        //连接数量
        private int m_numConnections;
        //缓存
        private BufferManager m_bufferManager;

        private Socket listenSocket;
        private SocketAsyncEventArgsPool m_readWritePool;
        private int m_totalBytesRead;
        private int m_numConnectedSockets;
        private Semaphore m_maxNumberAcceptedClients;

        public UdpServer m_RoomServer;

        public UserManager m_UserManager;
        public RoomManager m_RoomManager;

        /// <summary>
        ///  构造  
        /// </summary>
        /// <param name="numConnections">连接数</param>
        /// <param name="receiveBufferSize"></param>
        /// <param name="tRoomCount"></param>
        public Server(int numConnections, int receiveBufferSize, int tRoomCount)
        {
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;
            m_numConnections = numConnections;
            //初始化缓存池
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections, receiveBufferSize);
            //异步套接字池
            m_readWritePool = new SocketAsyncEventArgsPool(numConnections);
            //创建一个可授权numConnections个许可证的信号量，且初始值为numConnections
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);

            // 预先分配SocketAsyncEventArgs对象的池
            for (int i = 0; i < m_numConnections; i++)
            {
                // 创建发送和接收套接字并指定UserToken（SocketAsyncEventArgs）
                AsyncUserToken userToken = new AsyncUserToken(this);
                userToken.receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_bufferManager.SetBuffer(userToken.receiveArgs);
                m_readWritePool.Push(userToken);
            }
            //初始化玩家管理器
            m_UserManager = new UserManager(this);
            //初始化房间管理器
            m_RoomManager = new RoomManager(tRoomCount, this);
        }

        public void Start(IPEndPoint localEndPoint, int tPort)
        {
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            listenSocket.Listen(m_numConnections);
            StartAccept(null);
            Console.WriteLine("开启Tcp....");
            m_RoomServer = new UdpServer(tPort, this);
            Console.WriteLine("开启Ucp....");
        }

        /// <summary>
        /// 开始监听连接
        /// </summary>
        /// <param name="acceptEventArg"></param>
        void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                acceptEventArg.AcceptSocket = null;
            }
            //申请一个许可证(等待...)
            m_maxNumberAcceptedClients.WaitOne();
            //接入一个客户端
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// 客户端连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }


        void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref m_numConnectedSockets);
            Console.WriteLine("Client connection accepted. There are {0} clients connected to the server",
                m_numConnectedSockets);

            lock (m_readWritePool)
            {
                AsyncUserToken userToken = m_readWritePool.Pop();
                userToken.ConnectSocket = e.AcceptSocket;
                Console.WriteLine("接入socket = {0}", userToken.ConnectSocket.RemoteEndPoint.ToString());
                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.receiveArgs);
                if (!willRaiseEvent)
                {
                    ProcessReceive(userToken.receiveArgs);
                }
            }
            StartAccept(e);
        }

        /// <summary>
        /// This method is called whenever a receive or send operation is completed on a socket 
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);
                Console.WriteLine("The server has read a total of {0} bytes", m_totalBytesRead);

                AsyncUserToken userToken = e.UserToken as AsyncUserToken;
                userToken.Receive();

                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;

            uint userid = userToken.mUserid;
            lock (m_UserManager)
            {
                m_UserManager.RemoveUser(userid);
            }

            if (userToken.ConnectSocket != null)
            {
                try { userToken.ConnectSocket.Shutdown(SocketShutdown.Both); }
                catch (Exception ee) { Console.WriteLine(ee.ToString()); }
                finally
                {
                    try
                    {
                        userToken.ConnectSocket.Close();
                        userToken.ConnectSocket = null;
                        Interlocked.Decrement(ref m_numConnectedSockets);
                        Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", m_numConnectedSockets);
                        m_maxNumberAcceptedClients.Release();
                        lock (m_readWritePool)
                        {
                            m_readWritePool.Push(userToken);
                        }
                    }
                    catch (Exception eee)
                    {
                        Console.WriteLine("断开异常");
                        Console.WriteLine(eee.ToString());
                    }
                }
            }
        }
    }
}
