using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using higgs_message;
using ProtoBuf;
using System.IO;

namespace IocpServer
{
    public class SendMsgManager
    {
        internal static void SendLogin(AsyncUserToken asyncUserToken, string nickname, uint userid)
        {
            AckLogin nLogin = new AckLogin();
            nLogin.nickname = nickname;
            nLogin.userid = userid;
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<AckLogin>(stream, nLogin);
                Send(asyncUserToken, Protocol.AckLogin, stream.ToArray());
            }
        }

        static void Send(AsyncUserToken asyncUserToken,Protocol type,byte[] bytes)
        {
            Console.WriteLine("发送消息类型 = {0} asyncUserToken = {1}", type, asyncUserToken.ConnectSocket.RemoteEndPoint.ToString());
            GameMessage message = new GameMessage();
            message.type = BitConverter.GetBytes((byte)type);
            message.data = bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<GameMessage>(stream, message);
                asyncUserToken.Send(stream.ToArray());
            }
        }

        internal static void SendMatch(AsyncUserToken token, List<uint> playeridlist, uint m_temp_roomid)
        {
            NtfMatch match = new NtfMatch();
            for (int i = 0; i < playeridlist.Count; i++)
            {
                PlayerInfo info = new PlayerInfo();
                info.userid = playeridlist[i];
                info.nickname = "";
                match.playerinfoList.Add(info);
            }
            match.roomid = m_temp_roomid;
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<NtfMatch>(stream, match);
                Send(token, Protocol.NtfMatch, stream.ToArray());
            }
        }

        internal static void SendSelectHeroFinish(AsyncUserToken token, Dictionary<uint, uint> userhero_Dic)
        {
            NtfSelectHeroFinish selectHeroFinish = new NtfSelectHeroFinish();
            foreach (var item in userhero_Dic)
            {
                PlayerHeroInfo info = new PlayerHeroInfo();
                info.userid = item.Key;
                info.heroid = item.Value;
                selectHeroFinish.select_herolist.Add(info);
            }
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<NtfSelectHeroFinish>(stream, selectHeroFinish);
                Send(token, Protocol.NtfSelectHeroFinish, stream.ToArray());
            }
        }
    }
}
