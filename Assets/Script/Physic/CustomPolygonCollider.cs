using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomPolygonCollider : CustomCollider
{
    //是否处于编辑碰撞器状态
    [SerializeField]
    [HideInInspector]
    public bool mEdit = false;
    
    [SerializeField]
    [HideInInspector]
    protected List<CustomVector3> mBounds;

    public override List<CustomVector3> Bound
    {
        get
        {
            return mBounds;
        }
    }

    public override ColliderType Type
    {
        get
        {
            return ColliderType.Polygon;
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
        mBounds = new List<CustomVector3>();
        // 默认添加四个点
        CustomVector3 pos = new CustomVector3();
        pos.x = new FixedPointF(-500, 1000);
        pos.y = new FixedPointF(0, 1000);
        pos.z = new FixedPointF(-500, 1000);
        mBounds.Add(pos);
        pos = new CustomVector3();
        pos.x = new FixedPointF(500, 1000);
        pos.y = new FixedPointF(0, 1000);
        pos.z = new FixedPointF(-500, 1000);
        mBounds.Add(pos);
        pos = new CustomVector3();
        pos.x = new FixedPointF(500, 1000);
        pos.y = new FixedPointF(0, 1000);
        pos.z = new FixedPointF(500, 1000);
        mBounds.Add(pos);
        pos = new CustomVector3();
        pos.x = new FixedPointF(-500, 1000);
        pos.y = new FixedPointF(0, 1000);
        pos.z = new FixedPointF(500, 1000);
        mBounds.Add(pos);
    }
#endif
}
