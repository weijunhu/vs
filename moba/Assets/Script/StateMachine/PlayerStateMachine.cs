using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfIdleState : ActorGameState
{
    private Actor mActor;

    public override ActorStateType StateType
    {
        get
        {
            return ActorStateType.Idle;
        }
    }

    public override void Enter(params object[] param)
    {
        mActor = param[0] as Actor;
        if (mActor != null && mActor.ActorObj != null)
        {
            Animation animation = mActor.ActorObj.GetComponent<Animation>();
            if (animation != null)
            {
                animation.wrapMode = WrapMode.Loop;
                animation.Play("idle");
            }
        }
    }

    public override void Exit()
    {
        mActor = null;
    }

    public override void Update()
    {

    }
}

public class SelfMoveState : ActorGameState
{
    private Actor mActor;

    public override ActorStateType StateType
    {
        get
        {
            return ActorStateType.Move;
        }
    }

    public override void Enter(params object[] param)
    {
        mActor = param[0] as Actor;
        if (mActor != null && mActor.ActorObj != null)
        {
            Animation animation = mActor.ActorObj.GetComponent<Animation>();
            if (animation != null)
            {
                animation.wrapMode = WrapMode.Loop;
                animation.Play("run");
            }
            mActor.IsMove = true;
        }
    }

    public override void Exit()
    {
        mActor.IsMove = false;
        mActor = null;
    }

    public override void Update()
    {
        if (mActor != null && mActor.ActorObj != null)
        {
            mActor.Move();
        }
    }
}
