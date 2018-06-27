using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomCircleCollider))]
public class CircleColliderEditor : ColliderEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CustomCircleCollider col = target as CustomCircleCollider;

        GUILayoutOption option1 = GUILayout.Width(50);
        GUILayoutOption option2 = GUILayout.Width(80);
        
        //半径
        GUILayout.BeginHorizontal();
        GUILayout.Label("Radius", option1);
        float value = 0;
        if (float.TryParse(GUILayout.TextField(col.mRadius.value.ToString(), option2), out value))
            col.mRadius = new FixedPointF(value);
        GUILayout.EndHorizontal();
    }
}
