using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomPolygonCollider))]
public class PolygonColliderEditor : ColliderEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CustomPolygonCollider col = target as CustomPolygonCollider;
        col.mEdit = GUILayout.Toggle(col.mEdit, "Edit");
    }
    public override void OnSceneGUI()
    {
        base.OnSceneGUI();
        CustomPolygonCollider col = target as CustomPolygonCollider;
        if (!col.mEdit)
            return;

        Camera camera = Camera.current;
        //获取鼠标的位置
        Vector2 mousePos = Event.current.mousePosition;
        //scene视图中原点是左上角，但相机的原点是左下角，所以要转换下y值
        mousePos.y = camera.pixelHeight - mousePos.y;

        int index = -1;
        float length = 0;
        float percentage = 0;
        for (int i = 0; i < col.Bound.Count; i++)
        {
            Vector3 startViewPos = camera.WorldToScreenPoint(col.LocalToWorld(col.Bound[i]).value);
            Vector3 endViewPos = camera.WorldToScreenPoint(col.LocalToWorld(col.Bound[(i + 1) % col.Bound.Count]).value);
            //平行于X轴
            if (LineAndXParallel(startViewPos, endViewPos))
            {
                //平行于Y轴
                if (LineAndYParallel(startViewPos, endViewPos))
                {
                    continue;
                }
                else
                {
                    float len = Mathf.Abs(mousePos.y - startViewPos.y);
                    float percent = (mousePos.x - startViewPos.x) / (endViewPos.x - startViewPos.x);
                    bool first = index == -1;
                    bool replace = len < length;
                    index = first ? i : replace ? i : index;
                    length = first ? len : replace ? len : length;
                    percentage = first ? percent : replace ? percent : percentage;
                }
            }
            //平行于Y轴
            else if (LineAndYParallel(startViewPos, endViewPos))
            {
                float len = Mathf.Abs(mousePos.x - startViewPos.x);
                float percent = (mousePos.y - startViewPos.y) / (endViewPos.y - startViewPos.y);
                bool first = index == -1;
                bool replace = len < length;
                index = first ? i : replace ? i : index;
                length = first ? len : replace ? len : length;
                percentage = first ? percent : replace ? percent : percentage;

            }
            else
            {
                float k = (endViewPos.y - startViewPos.y) / (endViewPos.x - startViewPos.x);//y = kx+b;kx - y + b = 0
                float b = startViewPos.y - k * startViewPos.x;
                float len = Mathf.Abs((k * mousePos.x - mousePos.y + b)) / Mathf.Sqrt(1 + Mathf.Pow(k, 2));
                float percent = (mousePos.y - startViewPos.y) / (endViewPos.y - startViewPos.y);
                bool first = index == -1;
                bool replace = len < length;
                index = first ? i : replace ? i : index;
                length = first ? len : replace ? len : length;
                percentage = first ? percent : replace ? percent : percentage;
            }
        }

        //点击右键
        bool mRightMouseDown = Event.current.button == 1 && Event.current.type == EventType.MouseDown;
        //按下shift
        bool mShiftDown = Event.current.shift;
        //按下alt
        bool mAlt = Event.current.alt;

        if (index != -1 && percentage > 0)
        {
            Vector3 start = col.LocalToWorld(col.Bound[index]).value;
            Vector3 end = col.LocalToWorld(col.Bound[(index + 1) % col.Bound.Count]).value;
            Vector3 pos = start + (end - start) * percentage;
            //添加节点
            if (mShiftDown && !mAlt)
            {
                Handles.color = Color.green;
                Handles.CircleHandleCap(0, pos, Quaternion.Euler(90, 0, 0), 0.5f, EventType.Repaint);
                if (mRightMouseDown)
                {
                    CustomVector3 mCustomPos = CustomVector3.GetCustomVector3ByVector3(pos);
                    mCustomPos = col.WorldToLocal(mCustomPos);
                    if (index + 1 < col.Bound.Count)
                        col.Bound.Insert((index + 1), mCustomPos);
                    else
                        col.Bound.Add(mCustomPos);
                }
            }
        }

        //移除节点
        if (!mShiftDown && mAlt)
        {
            index = -1;
            length = 0;
            for (int i = 0; i < col.Bound.Count; i++)
            {
                bool first = index == -1;
                Vector3 viewPos = camera.WorldToScreenPoint(col.LocalToWorld(col.Bound[i]).value);
                float len = Vector3.Distance(viewPos, mousePos);
                bool replace = len < length;
                index = first ? i : replace ? i : index;
                length = first ? len : replace ? len : length;
            }
            if (index != -1)
            {
                Handles.color = Color.red;
                Vector3 pos = col.LocalToWorld(col.Bound[index]).value;
                Handles.CircleHandleCap(0, pos, Quaternion.Euler(90, 0, 0), 0.5f, EventType.Repaint);
                if (mRightMouseDown)
                {
                    if (col.Bound.Count > 3)
                        col.Bound.RemoveAt(index);
                    else
                        Debug.Log("多变形顶点数目不能小于3个");
                }
            }
        }

        //节点拖拽并且限制节点
        for (int i = 0; i < col.Bound.Count; i++)
        {
            CustomVector3 customPos = CustomVector3.GetCustomVector3ByVector3(Handles.PositionHandle(col.LocalToWorld(col.Bound[i]).value, Quaternion.identity));
            customPos = col.WorldToLocal(customPos);
            customPos.y = new FixedPointF(0);
            col.Bound[i] = customPos;
        }
    }

    //判断两点连线是否平行于X轴
    static bool LineAndXParallel(Vector3 tStartViewPos, Vector3 tEndViewPos)
    {
        if (Mathf.Abs(tStartViewPos.y - tEndViewPos.y) < 0.01f)
            return true;
        else
            return false;
    }
    //判断两点连线是否平行于Y轴
    static bool LineAndYParallel(Vector3 tStartViewPos, Vector3 tEndViewPos)
    {
        if (Mathf.Abs(tStartViewPos.x - tEndViewPos.x) < 0.01f)
            return true;
        else
            return false;
    }
}