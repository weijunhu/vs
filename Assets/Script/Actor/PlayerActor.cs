using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class PlayerActor : Actor
{
    ETCJoystick joy;

    protected override void InitStateMachine()
    {
        mStateMachineDic[ActorStateType.Idle] = new SelfIdleState();
        mStateMachineDic[ActorStateType.Move] = new SelfMoveState();
    }

    protected override void InitCurState()
    {
        CurState = mStateMachineDic[ActorStateType.Idle];
        CurState.Enter(this);
    }

    protected override void InitStateTransLimit()
    {

    }

    void Start()
    {
        joy = GameObject.FindObjectOfType<ETCJoystick>();
        if (joy != null)
        {
            joy.onMoveStart.AddListener(StartMoveCallBack);
            joy.onMove.AddListener(MoveCallBack);
            joy.onMoveEnd.AddListener(EndMoveCallBack);
        }
        Camera mCamera = transform.Find("camera").GetComponent<Camera>();
        mCamera.enabled = true;
    }

    void StartMoveCallBack()
    {
        _UdpSendManager.SendStartMove();
    }

    void MoveCallBack(Vector2 tVec2)
    {
        //发送遥感角度
        if (tVec2.x != 0)
        {
            int angle = (int)(Mathf.Atan2(tVec2.y, tVec2.x) * 180 / 3.14f);
            if (Mathf.Abs(Angle - angle) > 5)
            {
                _UdpSendManager.SendChangeDir(angle);
            }
        }
        else
        {
            int angle = tVec2.y > 0 ? 90 : -90;
            if (Mathf.Abs(Angle - angle) > 5)
            {
                _UdpSendManager.SendChangeDir(angle);
            }
        }
    }

    void EndMoveCallBack()
    {
        _UdpSendManager.SendEndMove();
    }

    void OnDestroy()
    {
        if (joy != null)
        {
            joy.onMoveStart.RemoveListener(StartMoveCallBack);
            joy.onMove.RemoveListener(MoveCallBack);
            joy.onMoveEnd.RemoveListener(EndMoveCallBack);
        }
    }
}
