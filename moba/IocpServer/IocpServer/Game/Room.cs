using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace IocpServer
{
    public class Room
    {
        private byte[] mbytes = new byte[1024 * 1024];
        private Timer timer = null;

        /// <summary>
        /// 玩家id列表
        /// </summary>
        private List<uint> m_useridList;
        /// <summary>
        /// key为userid，value为heroid
        /// </summary>
        private Dictionary<uint, uint> userhero_Dic;
        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<byte[]> messageQueue;
        /// <summary>
        /// 玩家的连接信息
        /// </summary>
        private Dictionary<uint, IPEndPoint> mConnectDic;

        private bool start = false;
        private uint frameIndex = 1;
        private uint mRoomid = 0;

        private Server mServer;

        public Room(Server tServer)
        {
            mServer = tServer;
            m_useridList = new List<uint>();
            userhero_Dic = new Dictionary<uint, uint>();
            messageQueue = new Queue<byte[]>();
            mConnectDic = new Dictionary<uint, IPEndPoint>();
        }

        /// <summary>
        /// 初始化房间
        /// </summary>
        /// <param name="useridlist"></param>
        /// <param name="tRoomid"></param>
        public void Init(List<uint> useridlist, uint tRoomid)
        {
            m_useridList = useridlist;
            mRoomid = tRoomid;

            userhero_Dic.Clear();
            messageQueue.Clear();
            mConnectDic.Clear();
            start = false;
            frameIndex = 1;
            Console.WriteLine("匹配成功，创建房间");
            for (int i = 0; i < m_useridList.Count; i++)
            {
                userhero_Dic[m_useridList[i]] = 0;
                mConnectDic[m_useridList[i]] = null;

                AsyncUserToken token = mServer.m_UserManager.GetUser(m_useridList[i]);
                if (token != null)
                    SendMsgManager.SendMatch(token, m_useridList, mRoomid);
            }
        }

        /// <summary>
        /// 设置英雄
        /// </summary>
        /// <param name="tUserid"></param>
        /// <param name="tHeroid"></param>
        public void SetHero(uint tUserid, uint tHeroid)
        {
            if (!m_useridList.Contains(tUserid))
                return;
            userhero_Dic[tUserid] = tHeroid;
            bool all_set_finish = true;
            foreach (var item in userhero_Dic)
            {
                if (item.Value == 0)
                {
                    all_set_finish = false;
                    break;
                }
            }

            if (all_set_finish)
            {
                for (int i = 0; i < m_useridList.Count; i++)
                {
                    uint temp_userid = m_useridList[i];
                    AsyncUserToken token = mServer.m_UserManager.GetUser(temp_userid);
                    if (token != null)
                        SendMsgManager.SendSelectHeroFinish(token, userhero_Dic);
                }
            }
        }

        /// <summary>
        /// 玩家准备完毕，等待进入游戏
        /// </summary>
        /// <param name="tUserid"></param>
        public void HeroReady(uint tUserid, IPEndPoint tSenderPoint)
        {
            if (start)
                return;
            if (!m_useridList.Contains(tUserid))
                return;
            mConnectDic[tUserid] = tSenderPoint;

            Console.WriteLine("玩家{0}准备,IPEndPoint = {1}", tUserid, tSenderPoint.ToString());

            foreach (var item in mConnectDic)
            {
                if (item.Value == null)
                {
                    return;
                }
            }

            Console.WriteLine("所有玩家准备完毕,开始游戏");
            GameStart();
        }

        /// <summary>
        /// 玩家准备完毕，游戏开始
        /// </summary>
        void GameStart()
        {
            start = true;
            timer = new Timer(TimerFunc, null, 0, 100);
        }

        public void Receive(byte[] bytes, uint tUserid, IPEndPoint tSenderPoint)
        {
            if (!start)
                return;
            lock (messageQueue)
                messageQueue.Enqueue(bytes);
        }

        void TimerFunc(object obj)
        {
            if (!start)
                return;
            lock (messageQueue)
            {
                uint frame = frameIndex;
                byte[] framebytes = BitConverter.GetBytes(frame);
                Array.Copy(framebytes, 0, mbytes, 0, framebytes.Length);
                int index = framebytes.Length;
                while (messageQueue.Count > 0)
                {
                    byte[] tempbytes = messageQueue.Dequeue();
                    Array.Copy(tempbytes, 0, mbytes, index, tempbytes.Length);
                    index += tempbytes.Length;
                }
                foreach (var item in mConnectDic)
                {
                    IPEndPoint point = item.Value;
                    byte[] sendbytes = new byte[index];
                    Array.Copy(mbytes, 0, sendbytes, 0, index);
                    MsgInfo msg = new MsgInfo(point, sendbytes, sendbytes.Length);
                    mServer.m_RoomServer.SendMessage(msg);
                    mServer.m_RoomServer.SendMessage(msg);
                    mServer.m_RoomServer.SendMessage(msg);
                }
                //Console.WriteLine("发送第{0}帧", frameIndex);
                frameIndex++;
            }
        }
    }
}
