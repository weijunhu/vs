using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
    None,
    Rectangle,
    Circle,
    Polygon,
}

public abstract class CustomCollider : ScriptBase
{
    //初始化
    public CustomTransform mTrans;

    void Awake()
    {
        Regist();
    }

    void OnEnable()
    {
        Regist();
    }

    void OnDisable()
    {
        UnRegist();
    }

    void OnDestroy()
    {
        UnRegist();
    }

    public void Regist()
    {
        _PhysicalSystem.Regist(this);
    }

    public void UnRegist()
    {
        _PhysicalSystem.UnRegist(this);
    }

    public abstract ColliderType Type { get; }

    public abstract List<CustomVector3> Bound { get; }
    
    /// <summary>
    /// 本地坐标转世界坐标，复合变换
    /// </summary>
    public CustomVector3 LocalToWorld(CustomVector3 localPos)
    {
        CustomVector3 vec = localPos;
        FixedPointF cos = CustomMath.GetCos(mTrans.Angle);
        FixedPointF sin = CustomMath.GetSin(mTrans.Angle);
        CustomVector3 vec2 = localPos;
        vec2.x = vec.x * cos - vec.z * sin;
        vec2.z = vec.z * cos + vec.x * sin;
        CustomVector3 tOffset = mTrans.Position;
        vec2.x += tOffset.x;
        vec2.z += tOffset.z;
        return vec2;
    }

    /// <summary>
    /// 世界坐标转本地坐标，复合变换
    /// </summary>
    public CustomVector3 WorldToLocal(CustomVector3 worldPos)
    {
        CustomVector3 tOffset = mTrans.Position;
        worldPos.x -= tOffset.x;
        worldPos.z -= tOffset.z;
        FixedPointF cos = CustomMath.GetCos(-mTrans.Angle);
        FixedPointF sin = CustomMath.GetSin(-mTrans.Angle);
        CustomVector3 vec2 = mTrans.Position;
        vec2.x = worldPos.x * cos - worldPos.z * sin;
        vec2.z = worldPos.z * cos + worldPos.x * sin;
        return vec2;
    }

    /// <summary>
    /// 所有顶点的世界坐标
    /// 每个坐标sxyz的分母都为1000，因为地图边界小于（-1000，1000），所以分子小于范围（-1000000，1000000）
    /// </summary>
    public List<CustomVector3> LocalToWorldBound
    {
        get
        {
            if (Bound == null)
                return new List<CustomVector3>();
            List<CustomVector3> list = new List<CustomVector3>();
            for (int i = 0; i < Bound.Count; i++)
            {
                list.Add(LocalToWorld(Bound[i]));
            }
            return list;
        }
    }
    public bool mIsobstacle;

    public abstract void DrawGizmos(Color tColor);

    #region 碰撞推开偏移
    private CustomVector3 CatchOffsetPosition = new CustomVector3(0, 0, 0);
    public void InitOffsetPosition()
    {
        CatchOffsetPosition = new CustomVector3(0, 0, 0);
    }
    public void AddOffsetPosition(CustomVector3 tVec3)
    {
        CatchOffsetPosition += tVec3;
    }
    public void UpdatePosition()
    {
        mTrans.LocalPosition += CatchOffsetPosition;
    }
    #endregion

#if UNITY_EDITOR
    protected virtual void Reset()
    {
        mTrans = GetComponent<CustomTransform>();
    }
#endif
}
