using higgs_message;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IocpServer
{
    public class UdpReceiveMsgManager
    {
        public static void Receive(byte[] bytes, Server tServer, IPEndPoint tSenderPoint)
        {
            byte[] content = new byte[bytes.Length - 4];
            Array.Copy(bytes, 4, content, 0, content.Length);
            using (MemoryStream stream = new MemoryStream(content))
            {
                GameMessage message = Serializer.Deserialize<GameMessage>(stream);
                MessageHandle(message.type[0], message.data, bytes, tServer, tSenderPoint);
            }
        }

        internal static void MessageHandle(byte tType, byte[] tData, byte[] bytes, Server tServer, IPEndPoint tSenderPoint)
        {
            uint userid = 0;
            uint roomid = 0;
            Protocol mType = (Protocol)tType;
            using (MemoryStream stream = new MemoryStream(tData))
            {
                switch (mType)
                {
                    case Protocol.ReqGameStart:
                        ReqGameStart reqGameStart = Serializer.Deserialize<ReqGameStart>(stream);
                        userid = reqGameStart.userid;
                        roomid = reqGameStart.roomid;
                        break;
                    case Protocol.ReqStartMove:
                        ReqStartMove reqStartMove = Serializer.Deserialize<ReqStartMove>(stream);
                        userid = reqStartMove.userid;
                        roomid = reqStartMove.roomid;
                        break;
                    case Protocol.ReqChangeDir:
                        ReqChangeDir reqChangeDir = Serializer.Deserialize<ReqChangeDir>(stream);
                        userid = reqChangeDir.userid;
                        roomid = reqChangeDir.roomid;
                        break;
                    case Protocol.ReqEndMove:
                        ReqEndMove reqEndMove = Serializer.Deserialize<ReqEndMove>(stream);
                        userid = reqEndMove.userid;
                        roomid = reqEndMove.roomid;
                        break;
                }
            }
            if (userid != 0 && roomid != 0)
            {
                Room room = tServer.m_RoomManager.GetRoom(roomid);
                if (mType == Protocol.ReqGameStart)
                {
                    room.HeroReady(userid, tSenderPoint);
                }
                else
                {
                    if (room != null)
                        room.Receive(bytes, userid, tSenderPoint);
                }
            }
        }
    }
}
