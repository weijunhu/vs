using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomTransform))]
public class CustomTransformEditor : Editor
{
    /// <summary>
    /// 更新子物体的位置和角度
    /// </summary>
    /// <param name="root"></param>
    void UpdateChild(Transform root)
    {
        CustomTransform ctrans = root.GetComponent<CustomTransform>();
        if (ctrans != null)
        {
            UpdateTrans(ctrans);
            Transform trans = root.transform;
            for (int i = 0; i < trans.childCount; i++)
            {
                UpdateChild(trans.GetChild(i));
            }
        }        
    }

    /// <summary>
    /// 更新子物体的位置和角度
    /// </summary>
    /// <param name="ctrans"></param>
    void UpdateTrans(CustomTransform ctrans)
    {
        Transform trans = ctrans.transform;
        //更新角度
        int angle = -(int)trans.transform.localEulerAngles.y;
        angle -= angle / 360 * 360;
        if (angle < -180)
            angle += 360;
        if (angle > 180)
            angle -= 360;
        if (ctrans.LocalAngle != angle)
            ctrans.LocalAngle = angle;
        //更新位置
        CustomVector3 vec = CustomVector3.GetCustomVector3ByVector3(trans.localPosition);
        if (ctrans.LocalPosition != vec)
            ctrans.LocalPosition = vec;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CustomTransform trans = target as CustomTransform;

        //编辑器状态下，拖拽更新
        if (!Application.isPlaying)
        {
            trans.LocalPosition = CustomVector3.GetCustomVector3ByVector3(trans.transform.localPosition);
            UpdateChild(trans.transform);
        }

        EditorGUILayout.BeginVertical();

        GUILayoutOption style1 = GUILayout.Width(10);
        GUILayoutOption style2 = GUILayout.Width(80);
        GUILayoutOption style3 = GUILayout.Width(100);

        CustomVector3 localPosition = trans.LocalPosition;
        EditorGUILayout.LabelField("LocalPostion");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("x", style1);
        EditorGUILayout.LabelField(localPosition.x.value.ToString(), style2);
        EditorGUILayout.LabelField("y", style1);
        EditorGUILayout.LabelField(localPosition.y.value.ToString(), style2);
        EditorGUILayout.LabelField("z", style1);
        EditorGUILayout.LabelField(localPosition.z.value.ToString(), style2);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("LocalAngle", style3);
        EditorGUILayout.LabelField(trans.LocalAngle.ToString(), style2);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Postion");

        EditorGUILayout.BeginHorizontal();
        CustomVector3 Position = trans.Position;
        EditorGUILayout.LabelField("x", style1);
        EditorGUILayout.LabelField(Position.x.value.ToString(), style2);
        EditorGUILayout.LabelField("y", style1);
        EditorGUILayout.LabelField(Position.y.value.ToString(), style2);
        EditorGUILayout.LabelField("z", style1);
        EditorGUILayout.LabelField(Position.z.value.ToString(), style2);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Angle", style3);
        EditorGUILayout.LabelField(trans.Angle.ToString(), style2);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}
