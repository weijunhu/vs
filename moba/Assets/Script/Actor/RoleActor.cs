using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class RoleActor : Actor
{
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
        Camera mCamera = transform.Find("camera").GetComponent<Camera>();
        mCamera.enabled = false;
    }
}
