using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using higgs_message;
using System.IO;
using ProtoBuf;

public class UdpReciveManager: ScriptBase
{
    public void Receive(byte[] bytes)
    {
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                uint frameindex = reader.ReadUInt32();
                List<GameMessage> list = new List<GameMessage>();
                while (true)
                {
                    try
                    {
                        int length = reader.ReadInt32();
                        byte[] temp_bytes = reader.ReadBytes(length - 4);
                        using (MemoryStream stream2 = new MemoryStream(temp_bytes))
                        {
                            GameMessage message = Serializer.Deserialize<GameMessage>(stream2);
                            list.Add(message);
                        }
                    }
                    catch
                    {
                        GameManager.Instance.AddOneFrame(frameindex, list);
                        break;
                    }
                }
            }
        }
    }

    public void MsgHandle(List<GameMessage> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameMessage message = list[i];
            Protocol protocol = (Protocol)message.type[0];
            switch (protocol)
            {
                case Protocol.ReqStartMove:
                    using (MemoryStream stream = new MemoryStream(message.data))
                    {
                        ReqStartMove mReqStartMove = Serializer.Deserialize<ReqStartMove>(stream);
                        Process(mReqStartMove);
                        break;
                    }
                case Protocol.ReqChangeDir:
                    using (MemoryStream stream = new MemoryStream(message.data))
                    {
                        ReqChangeDir mReqChangeDir = Serializer.Deserialize<ReqChangeDir>(stream);
                        Process(mReqChangeDir);
                        break;
                    }
                case Protocol.ReqEndMove:
                    using (MemoryStream stream = new MemoryStream(message.data))
                    {
                        ReqEndMove mReqEndMove = Serializer.Deserialize<ReqEndMove>(stream);
                        Process(mReqEndMove);
                        break;
                    }
            }
        }
    }

    void Process(ReqStartMove tReqStartMove)
    {
        uint userid = tReqStartMove.userid;
        Actor actor = GameManager.Instance.GetActor(userid);
        if (actor != null)
        {
            actor.TransState(ActorStateType.Move);
        }
    }

    void Process(ReqChangeDir tReqChangeDir)
    {
        uint userid = tReqChangeDir.userid;
        Actor actor = GameManager.Instance.GetActor(userid);
        if (actor != null)
        {
            actor.Angle = tReqChangeDir.angle;
        }
    }

    void Process(ReqEndMove tReqEndMove)
    {
        uint userid = tReqEndMove.userid;
        Actor actor = GameManager.Instance.GetActor(userid);
        if (actor != null)
        {
            actor.TransState(ActorStateType.Idle);
        }
    }
}
