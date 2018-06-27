using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalSystem : ScriptBase
{
    private List<CustomCollider> mColliderList = new List<CustomCollider>();
    private bool mIsOpen = false;

    public void Open()
    {
        mIsOpen = true;
        Clear();
        RegistSceneCollider(SceneConfig.mWallParentPath, null);
    }

    /// <summary>
    /// 注册场景中的碰撞器到物理系统
    /// </summary>
    void RegistSceneCollider(string tPath, Transform tTrans)
    {
        Transform parent = null;
        if (!string.IsNullOrEmpty(tPath))
            parent = GameObject.Find(tPath).transform;
        else
            parent = tTrans;

        if (parent == null)
            return;

        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = parent.GetChild(i);
            CustomCollider collider = child.GetComponent<CustomCollider>();
            if (collider != null)
            {
                collider.mIsobstacle = true;
                if (child.gameObject.activeInHierarchy)
                    collider.Regist();
            }
            if (child.childCount > 0)
                RegistSceneCollider(null, child);
        }
    }

    public void Close()
    {
        mIsOpen = false;
        Clear();
    }

    public void Regist(CustomCollider tCollider)
    {
        if (!mIsOpen)
            return;
        if (!mColliderList.Contains(tCollider))
        {
            mColliderList.Add(tCollider);
        }
    }

    public void UnRegist(CustomCollider tCollider)
    {
        if (!mIsOpen)
            return;
        if (mColliderList.Contains(tCollider))
        {
            mColliderList.Remove(tCollider);
        }
    }

    public void UpdateCollider()
    {
        if (!mIsOpen)
            return;
        for (int i = 0; i < mColliderList.Count; i++)
            mColliderList[i].InitOffsetPosition();

        for (int i = 0; i < mColliderList.Count - 1; i++)
        {
            for (int j = i + 1; j < mColliderList.Count; j++)
            {
                Check(mColliderList[i], mColliderList[j]);
            }
        }

        for (int i = 0; i < mColliderList.Count; i++)
            mColliderList[i].UpdatePosition();
    }

    void Check(CustomCollider tCollider1, CustomCollider tCollider2)
    {
        ColliderType type1 = tCollider1.Type;
        ColliderType type2 = tCollider2.Type;
        bool result = false;
        switch (type1)
        {
            case ColliderType.Circle:
                switch (type2)
                {
                    case ColliderType.Circle:
                        result = CheckCollider.CheckCircleAndCircle(tCollider1 as CustomCircleCollider, tCollider2 as CustomCircleCollider);
                        break;
                    case ColliderType.Polygon:
                    case ColliderType.Rectangle:
                        result = CheckCollider.CheckPolygonAndCircle(tCollider2 as CustomPolygonCollider, tCollider1 as CustomCircleCollider);
                        break;
                }
                break;
            case ColliderType.Polygon:
            case ColliderType.Rectangle:
                switch (type2)
                {
                    case ColliderType.Circle:
                        result = CheckCollider.CheckPolygonAndCircle(tCollider1 as CustomPolygonCollider, tCollider2 as CustomCircleCollider);
                        break;
                    case ColliderType.Polygon:
                    case ColliderType.Rectangle:
                        result = CheckCollider.CheckPolygonAndPolygon(tCollider1 as CustomPolygonCollider, tCollider2 as CustomPolygonCollider);
                        break;
                }
                break;
        }
    }

    private void Clear()
    {
        mColliderList.Clear();
    }


}
