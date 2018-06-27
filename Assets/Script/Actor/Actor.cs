using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public abstract class Actor : ScriptBase
{
    protected ActorGameState CurState { set; get; }
    /// <summary>
    /// 所有的状态
    /// </summary>
    protected Dictionary<ActorStateType, ActorGameState> mStateMachineDic = new Dictionary<ActorStateType, ActorGameState>();
    /// <summary>
    /// 限制状态跳转字典
    /// </summary>
    protected Dictionary<ActorStateType, List<ActorStateType>> mTransShieldDic = new Dictionary<ActorStateType, List<ActorStateType>>();

    /// <summary>
    /// 初始化状态机
    /// </summary>
    protected abstract void InitStateMachine();
    /// <summary>
    /// 初始化当前状态
    /// </summary>
    protected abstract void InitCurState();
    /// <summary>
    /// 状态机跳转限制
    /// </summary>
    protected abstract void InitStateTransLimit();

    public void TransState(ActorStateType tStateType, params object[] param)
    {
        if (CurState == null)
            return;
        if (tStateType == CurState.StateType)
            return;
        else
        {
            ActorGameState mState = null;
            if (mStateMachineDic.TryGetValue(tStateType, out mState))
            {
                List<ActorStateType> shieldList = null;
                if (mTransShieldDic.TryGetValue(CurState.StateType, out shieldList))
                    if (shieldList.Contains(tStateType))
                        return;
                CurState.Exit();
                CurState = mState;
                CurState.Enter(this, param);
            }
        }
    }

    public uint Id { set; get; }

    /// <summary>
    /// 模型
    /// </summary>
    private GameObject mActorObj;
    public GameObject ActorObj
    {
        get
        {
            if (mActorObj == null)
                mActorObj = transform.FindChild("rotate/actor").gameObject;
            return mActorObj;
        }
    }

    private Transform mRotateTransform;
    public Transform RotateTransform
    {
        get
        {
            if (mRotateTransform == null)
                mRotateTransform = transform.FindChild("rotate").transform;
            return mRotateTransform;
        }
    }

    void Awake()
    {
        InitStateMachine();
        InitCurState();
        InitStateTransLimit();
    }

    public void UpdateEvent()
    {
        if (CurState != null)
            CurState.Update();
    }

    #region 位置相关
    public bool IsMove { get; set; }

    /// <summary>
    /// 速度：规定分母必须为100
    /// </summary>
    public FixedPointF Speed { set; get; }

    public CustomVector3 Position
    {
        get
        {
            CustomTransform mTrans = GetComponent<CustomTransform>();
            return mTrans.Position;
        }
        set
        {
            CustomTransform mTrans = GetComponent<CustomTransform>();
            mTrans.Position = value;
        }
    }

    public int Angle
    {
        get
        {
            CustomTransform mTrans = RotateTransform.GetComponent<CustomTransform>();
            return mTrans.Angle;
        }
        set
        {
            CustomTransform mTrans = RotateTransform.GetComponent<CustomTransform>();
            mTrans.Angle = value;
        }
    }

    public virtual void Move()
    {
        CustomTransform mTrans = GetComponent<CustomTransform>();
        CustomTransform mRotateTrans = RotateTransform.GetComponent<CustomTransform>();
        CustomVector3 temp;

        FixedPointF x = CustomMath.GetCos(mRotateTrans.Angle);
        FixedPointF z = CustomMath.GetSin(mRotateTrans.Angle);

        temp.x = x * Speed * LockStepConfig.mRenderFrameRate;
        temp.y = new FixedPointF(0);
        temp.z = z * Speed * LockStepConfig.mRenderFrameRate;
        
        mTrans.LocalPosition += temp;
    }
    #endregion 位置相关
}
