using System.Collections.Generic;
using higgs_message;

namespace IocpServer
{
    public class RoomManager
    {
        private Server mServer;
        private Dictionary<RoomType, Queue<uint>> m_MatchMap;
        private Dictionary<uint, Room> m_RoomDic;
        private uint m_temp_roomid = 1;
        private RoomPool m_RoomPool;

        public RoomManager(int tRoomCount, Server tServer)
        {
            mServer = tServer;
            m_MatchMap = new Dictionary<RoomType, Queue<uint>>();
            m_MatchMap[RoomType.OneVOne] = new Queue<uint>();
            m_MatchMap[RoomType.TwoVTwo] = new Queue<uint>();
            m_MatchMap[RoomType.ThreeVThree] = new Queue<uint>();
            m_RoomDic = new Dictionary<uint, Room>();

            m_RoomPool = new RoomPool(tRoomCount);
            for (int i = 0; i < tRoomCount; i++)
            {
                Room mRoom = new Room(mServer);
                m_RoomPool.Push(mRoom);
            }
        }

        public void ReqMatch(ReqMatch reqMatch)
        {
            uint userid = reqMatch.userid;
            uint roomType = reqMatch.fight_type;
            if (!CheckDataaVailability(userid))
                return;

            System.Console.WriteLine("ReqMatch userid = {0}, roomType = {1}", userid, roomType);
            RoomType type = (RoomType)roomType;
            uint count = (uint)type;
            if (!m_MatchMap[type].Contains(userid))
            {
                m_MatchMap[type].Enqueue(userid);
                uint playercount = count * 2;
                if (m_MatchMap[type].Count >= playercount)
                {
                    List<uint> playeridlist = new List<uint>();
                    for (int i = 0; i < playercount; i++)
                        playeridlist.Add(m_MatchMap[type].Dequeue());

                    Room room = m_RoomPool.Pop();
                    m_RoomDic[m_temp_roomid] = room;
                    room.Init(playeridlist, m_temp_roomid);
                    m_temp_roomid++;
                }
            }
        }

        public void ReqSelectHero(ReqSelectHero selectHero)
        {
            uint userid = selectHero.userid;
            uint roomid = selectHero.roomid;
            uint heroid = selectHero.heroid;
            if (!CheckDataaVailability(userid, roomid))
                return;

            System.Console.WriteLine("ReqSelectHero userid = {0}, roomid = {1}, heroid = {2}", userid, roomid, heroid);
            Room room = m_RoomDic[roomid];
            room.SetHero(userid, heroid);
        }

        public Room GetRoom(uint tRoomid)
        {
            Room room;
            if (m_RoomDic.TryGetValue(tRoomid, out room))
                return room;
            return null;
        }

        bool CheckDataaVailability(uint tUserid)
        {
            if (!mServer.m_UserManager.ContainUser(tUserid))
            {
                System.Console.WriteLine("无效用户id = {0}", tUserid);
                return false;
            }
            return true;
        }
        bool CheckDataaVailability(uint tUserid, uint tRoomid)
        {
            if (!mServer.m_UserManager.ContainUser(tUserid))
            {
                System.Console.WriteLine("无效用户id = {0}", tUserid);
                return false;
            }
            if (!m_RoomDic.ContainsKey(tRoomid))
            {
                System.Console.WriteLine("无效房间id = {0}", tRoomid);
                return false;
            }
            return true;
        }
    }
}
