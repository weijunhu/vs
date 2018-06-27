using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using ProtoBuf;
using higgs_message;

public class TcpReciveManager : ScriptBase
{
    public void Receive(byte[] bytes)
    {
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            GameMessage message = Serializer.Deserialize<GameMessage>(stream);
            Protocol protocol = (Protocol)message.type[0];
            switch (protocol)
            {
                case Protocol.AckLogin:
                    using (MemoryStream stream2 = new MemoryStream(message.data))
                    {
                        AckLogin login = Serializer.Deserialize<AckLogin>(stream2);
                        Debug.Log("userid = " + login.userid);
                        Debug.Log("nickname = " + login.nickname);
                        NetData.Instance.mUserData.Userid = login.userid;
                        NetData.Instance.mUserData.Nickname = login.nickname;
                        _ViewManager.ClearView();
                        _ViewManager.LoadView("prefab/ui/matchview_prefab");
                        break;
                    }
                case Protocol.NtfMatch:
                    using (MemoryStream stream2 = new MemoryStream(message.data))
                    {
                        NtfMatch match = Serializer.Deserialize<NtfMatch>(stream2);
                        Debug.Log("match.playerinfoList.Count = " + match.playerinfoList.Count);
                        NetData.Instance.mFightData.PlayerInfoList = match.playerinfoList;
                        NetData.Instance.mFightData.RoomId = match.roomid;
                        _TcpSendManager.SendSelectHero(1);
                        break;
                    }
                case Protocol.NtfSelectHeroFinish:
                    using (MemoryStream stream2 = new MemoryStream(message.data))
                    {
                        NtfSelectHeroFinish selectHeroFinish = Serializer.Deserialize<NtfSelectHeroFinish>(stream2);
                        Debug.Log("selectHeroFinish.select_herolist.Count = " + selectHeroFinish.select_herolist.Count);
                        NetData.Instance.mFightData.PlayerHeroInfoList = selectHeroFinish.select_herolist;
                        LoadingManager.LoadSceneAsync(SceneConfig.Fight);
                        break;
                    }
            }
            if (_UnityTcpSocket.ReceiveAction != null)
                _UnityTcpSocket.ReceiveAction(protocol);
        }
    }
}