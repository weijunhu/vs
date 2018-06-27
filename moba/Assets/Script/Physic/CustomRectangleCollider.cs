using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomRectangleCollider : CustomPolygonCollider
{
    /// <summary>
    /// 宽
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private FixedPointF mWidth;
    public FixedPointF Width
    {
        set
        {
            mWidth = value;
            FixedPointF halfwidth = value / 2;
            CustomVector3 vec3 = mBounds[0];
            vec3.x = -halfwidth;
            mBounds[0] = vec3;
            vec3 = mBounds[1];
            vec3.x = halfwidth;
            mBounds[1] = vec3;
            vec3 = mBounds[2];
            vec3.x = halfwidth;
            mBounds[2] = vec3;
            vec3 = mBounds[3];
            vec3.x = -halfwidth;
            mBounds[3] = vec3;
        }
        get
        {
            return mWidth;
        }
    }

    /// <summary>
    /// 高
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private FixedPointF mHeight;
    public FixedPointF Height
    {
        set
        {
            mHeight = value;
            FixedPointF halfheight = value / 2;
            CustomVector3 vec3 = mBounds[0];
            vec3.z = -halfheight;
            mBounds[0] = vec3;
            vec3 = mBounds[1];
            vec3.z = -halfheight;
            mBounds[1] = vec3;
            vec3 = mBounds[2];
            vec3.z = halfheight;
            mBounds[2] = vec3;
            vec3 = mBounds[3];
            vec3.z = halfheight;
            mBounds[3] = vec3;
        }
        get
        {
            return mHeight;
        }
    }

    public override List<CustomVector3> Bound
    {
        get
        {
            return new List<CustomVector3>(mBounds);
        }
    }

    public override ColliderType Type
    {
        get
        {
            return ColliderType.Rectangle;
        }
    }
    public override void DrawGizmos(Color tColor)
    {
#if UNITY_EDITOR
        List<CustomVector3> worldBound = LocalToWorldBound;
        Handles.color = tColor;
        for (int i = 0; i < worldBound.Count; i++)
            Handles.DrawLine(worldBound[i].value, worldBound[(i + 1) % worldBound.Count].value);
#endif
    }
#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        Width = new FixedPointF(1);
        Height = new FixedPointF(1);
    }
#endif
}
