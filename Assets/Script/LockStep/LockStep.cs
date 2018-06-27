using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using higgs_message;

public class LockStep : ScriptBase
{
    private float mLogicTempTime = 0;
    void Update()
    {
        mLogicTempTime += Time.deltaTime;
        if (mLogicTempTime > LockStepConfig.mRenderFrameUpdateTime)
        {
            for (int i = 0; i < mFastForwardSpeed; i++)
            {
                GameTurn();
                mLogicTempTime = 0;
            }
        }
    }

    private int mFastForwardSpeed = 1;
    public void SetFaseForward(int tValue)
    {
        mFastForwardSpeed = tValue;
    }

    private int GameFrameInTurn = 0;
    void GameTurn()
    {
        if (GameFrameInTurn == 0)
        {
            List<GameMessage> list = null;
            if (GameManager.Instance.LockFrameTurn(ref list))
            {
                if (list != null)
                    _UdpReciveManager.MsgHandle(list);
                GameFrameInTurn++;
            }
        }
        else
        {
            GameManager.Instance.UpdateEvent();

            if (GameFrameInTurn == LockStepConfig.mRenderFrameCount)
                GameFrameInTurn = 0;
            else
                GameFrameInTurn++;
        }
    }
}
