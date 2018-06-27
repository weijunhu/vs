using System.Collections.Generic;
using System.Net;

namespace IocpServer
{
    public class UserManager
    {
        private Server mServer;
        private Dictionary<uint, AsyncUserToken> mUserDic;
        private uint m_temp_userid = 1;

        public UserManager(Server tServer)
        {
            mServer = tServer;
            mUserDic = new Dictionary<uint, AsyncUserToken>();
        }

        public uint AddUser(AsyncUserToken tUserToken)
        {
            uint id = m_temp_userid;
            mUserDic[id] = tUserToken;

            tUserToken.mUserid = id;
            EndPoint point = tUserToken.ConnectSocket.RemoteEndPoint;
            tUserToken.mRemoteIp = point.ToString().Split(':')[0];
            System.Console.WriteLine("id = {0}, ip = {1}", id, tUserToken.mRemoteIp);

            m_temp_userid++;
            return id;
        }

        public AsyncUserToken GetUser(uint tId)
        {
            if (mUserDic.ContainsKey(tId))
                return mUserDic[tId];
            else
                return null;
        }

        public void RemoveUser(uint tId)
        {
            if (mUserDic.ContainsKey(tId))
                mUserDic.Remove(tId);
        }

        public bool ContainUser(uint tId)
        {
            return mUserDic.ContainsKey(tId);
        }
    }
}
