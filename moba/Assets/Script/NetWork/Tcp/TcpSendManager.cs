using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using ProtoBuf;
using higgs_message;
using System.Net;

public class TcpSendManager : ScriptBase
{
    public void SendLogin(string name, string password)
    {
        ReqLogin login = new ReqLogin();
        login.account = name;
        login.password = password;
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, login);
            Send(Protocol.ReqLogin, stream.ToArray());
        }
    }

    public void SendMatch(uint fight_type)
    {
        ReqMatch match = new ReqMatch();
        match.userid = NetData.Instance.mUserData.Userid;
        match.fight_type = fight_type;
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, match);
            Send(Protocol.ReqMatch, stream.ToArray());
        }
    }

    public void SendSelectHero(uint heroid)
    {
        ReqSelectHero match = new ReqSelectHero();
        match.userid = NetData.Instance.mUserData.Userid;
        match.heroid = heroid;
        match.roomid = NetData.Instance.mFightData.RoomId;
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, match);
            Send(Protocol.ReqSelectHero, stream.ToArray());
        }
    }

    void Send(Protocol type,byte[] bytes)
    {
        GameMessage message = new GameMessage();
        message.type = BitConverter.GetBytes((byte)type);
        using (MemoryStream stream = new MemoryStream())
        {
            message.data = bytes;
            Serializer.Serialize(stream, message);
            _UnityTcpSocket.Send(stream.ToArray());
        }
    }
}