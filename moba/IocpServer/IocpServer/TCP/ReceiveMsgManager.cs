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
    public class ReceiveMsgManager
    {
        internal static void Receive(AsyncUserToken asyncUserToken, byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                GameMessage message = Serializer.Deserialize<GameMessage>(stream);
                MessageHandle(asyncUserToken, message.type[0], message.data);
            }
        }

        internal static void MessageHandle(AsyncUserToken asyncUserToken,byte tType, byte[] tData)
        {
            using (MemoryStream stream = new MemoryStream(tData))
            {
                Console.WriteLine("收到消息类型 = {0}", (Protocol)tType);
                switch ((Protocol)tType)
                {
                    case Protocol.ReqLogin:
                        ReqLogin login = Serializer.Deserialize<ReqLogin>(stream);
                        MessageHandle(asyncUserToken,login);
                        break;
                    case Protocol.ReqMatch:
                        ReqMatch match = Serializer.Deserialize<ReqMatch>(stream);
                        MessageHandle(asyncUserToken, match);
                        break;
                    case Protocol.ReqSelectHero:
                        ReqSelectHero selectHero = Serializer.Deserialize<ReqSelectHero>(stream);
                        MessageHandle(asyncUserToken, selectHero);
                        break;
                }                
            }
        }

        internal static void MessageHandle(AsyncUserToken asyncUserToken, ReqLogin login)
        {
            asyncUserToken.mServer.m_UserManager.AddUser(asyncUserToken);            
            SendMsgManager.SendLogin(asyncUserToken, login.account, asyncUserToken.mUserid);            
        }

        internal static void MessageHandle(AsyncUserToken asyncUserToken, ReqMatch match)
        {
            asyncUserToken.mServer.m_RoomManager.ReqMatch(match);
        }

        internal static void MessageHandle(AsyncUserToken asyncUserToken, ReqSelectHero selectHero)
        {
            asyncUserToken.mServer.m_RoomManager.ReqSelectHero(selectHero);
        }
    }
}
