using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomTransform : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private int mLocalAngle;
    public int LocalAngle
    {
        set
        {
            mLocalAngle = value;
            Vector3 vec = Vector3.zero;
            vec.y = -mLocalAngle;
            transform.localEulerAngles = vec;
        }
        get
        {
            return mLocalAngle;
        }
    }

    public int Angle
    {
        set
        {
            int mAngle = value;
            Transform parent = transform.parent;
            if (parent != null)
            {
                CustomTransform trans = parent.GetComponent<CustomTransform>();
                mAngle -= trans.Angle;
            }
            LocalAngle = mAngle;
        }
        get
        {
            int mAngle = mLocalAngle;
            Transform parent = transform.parent;
            if (parent != null)
            {
                CustomTransform trans = parent.GetComponent<CustomTransform>();
                mAngle += trans.Angle;
            }
            return mAngle;
        }
    }

    [SerializeField]
    [HideInInspector]
    private CustomVector3 mLocalPosition;
    public CustomVector3 LocalPosition
    {
        set
        {
            mLocalPosition = value;
            //transform.localPosition = mLocalPosition.value;
            transform.DOLocalMove(mLocalPosition.value, LockStepConfig.mRenderFrameUpdateTime);
        }
        get
        {
            return mLocalPosition;
        }
    }

    public CustomVector3 Position
    {
        set
        {
            CustomVector3 mPosition = value;
            Transform parent = transform.parent;
            if (parent != null)
            {
                CustomTransform parentTrans = parent.GetComponent<CustomTransform>();
                CustomVector3 parentPosition = parentTrans.Position;
                mPosition.x -= parentPosition.x;
                mPosition.z -= parentPosition.z;
                FixedPointF cos = CustomMath.GetCos(-parentTrans.Angle);
                FixedPointF sin = CustomMath.GetSin(-parentTrans.Angle);
                CustomVector3 tempVec = parentTrans.Position;
                tempVec.x = mPosition.x * cos - mPosition.z * sin;
                tempVec.z = mPosition.z * cos + mPosition.x * sin;
                mPosition = tempVec;
            }
            LocalPosition = mPosition;
        }
        get
        {
            CustomVector3 mPosition = mLocalPosition;
            Transform parent = transform.parent;
            if (parent != null)
            {
                CustomTransform parentTrans = parent.GetComponent<CustomTransform>();
                FixedPointF cos = CustomMath.GetCos(parentTrans.Angle);
                FixedPointF sin = CustomMath.GetSin(parentTrans.Angle);
                mPosition.x = mLocalPosition.x * cos - mLocalPosition.z * sin;
                mPosition.z = mLocalPosition.z * cos + mLocalPosition.x * sin;
                CustomVector3 parentPosition = parentTrans.Position;
                mPosition.x += parentPosition.x;
                mPosition.z += parentPosition.z;
            }
            return mPosition;
        }
    }
}
