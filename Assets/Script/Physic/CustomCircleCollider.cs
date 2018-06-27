using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomCircleCollider : CustomCollider
{
    /// <summary>
    /// 不要修改分母，防止计算过程中整数溢出
    /// </summary>
    [SerializeField]
    [HideInInspector]
    public FixedPointF mRadius;

    public override ColliderType Type
    {
        get
        {
            return ColliderType.Circle;
        }
    }

    public override List<CustomVector3> Bound
    {
        get
        {
            return null;
        }
    }
    public override void DrawGizmos(Color tColor)
    {
#if UNITY_EDITOR
        FixedPointF radius = mRadius;
        Vector3 pos = mTrans.Position.value;
        Gizmos.color = tColor;
        Handles.color = tColor;
        Handles.CircleHandleCap(0, pos, Quaternion.Euler(90, 0, 0), radius.value, EventType.Repaint);
#endif
    }
#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        mRadius = new FixedPointF(1);
    }
#endif
}
