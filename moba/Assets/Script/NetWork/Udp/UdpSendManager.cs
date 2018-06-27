using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using higgs_message;
using System.IO;
using ProtoBuf;
using System;
using System.Net;

public class UdpSendManager : ScriptBase
{
    public void SendReqGameStart()
    {
        ReqGameStart gameStart = new ReqGameStart();
        gameStart.userid = NetData.Instance.mUserData.Userid;
        gameStart.roomid = NetData.Instance.mFightData.RoomId;
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, gameStart);
            Send(Protocol.ReqGameStart, stream.ToArray());
        }
    }

    public void SendStartMove()
    {
        ReqStartMove mReqStartMove = new ReqStartMove();
        mReqStartMove.userid = NetData.Instance.mUserData.Userid;
        mReqStartMove.roomid = NetData.Instance.mFightData.RoomId;
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, mReqStartMove);
            Send(Protocol.ReqStartMove, stream.ToArray());
        }
    }

    public void SendChangeDir(int tAngle)
    {
        ReqChangeDir mReqChangeDir = new ReqChangeDir();
        mReqChangeDir.userid = NetData.Instance.mUserData.Userid;
        mReqChangeDir.roomid = NetData.Instance.mFightData.RoomId;
        mReqChangeDir.angle = tAngle;
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, mReqChangeDir);
            Send(Protocol.ReqChangeDir, stream.ToArray());
        }
    }

    public void SendEndMove()
    {
        ReqEndMove mReqEndMove = new ReqEndMove();
        mReqEndMove.userid = NetData.Instance.mUserData.Userid;
        mReqEndMove.roomid = NetData.Instance.mFightData.RoomId;
        using (MemoryStream stream = new MemoryStream())
        {
            Serializer.Serialize(stream, mReqEndMove);
            Send(Protocol.ReqEndMove, stream.ToArray());
        }
    }

    void Send(Protocol type, byte[] bytes)
    {
        GameMessage message = new GameMessage();
        message.type = BitConverter.GetBytes((byte)type);
        using (MemoryStream stream = new MemoryStream())
        {
            message.data = bytes;
            Serializer.Serialize(stream, message);
            _UnityUdpSocket.Send(stream.ToArray());
        }
    }
}
