using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetData
{
    private static NetData mInstance;
    public static NetData Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new NetData();
            return mInstance;
        }
    }

    public UserData mUserData = new UserData();
    public FightData mFightData = new FightData();
}

public class UserData
{
    public uint Userid { get; set; }
    public string Nickname { get; set; }
}

public class FightData
{
    public uint RoomId { get; set; }
    public List<higgs_message.PlayerInfo> PlayerInfoList { get; set; }
    public List<higgs_message.PlayerHeroInfo> PlayerHeroInfoList { get; set; }
}
