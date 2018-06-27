using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorStateType
{
    Idle,
    Move,
}

public abstract class ActorGameState
{
    public abstract ActorStateType StateType { get; }
    public abstract void Enter(params object[] param);
    public abstract void Update();
    public abstract void Exit();
}
