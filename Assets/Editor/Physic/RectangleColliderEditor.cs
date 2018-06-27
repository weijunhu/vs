using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomRectangleCollider))]
public class RectangleColliderEditor : ColliderEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CustomRectangleCollider col = target as CustomRectangleCollider;

        GUILayoutOption width_height_Option = GUILayout.Width(50);
        GUILayoutOption content_Option = GUILayout.Width(80);
        GUILayoutOption suffix_Option = GUILayout.Width(50);

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Width", width_height_Option);
        float value = 0;
        if (float.TryParse(GUILayout.TextField(col.Width.ToString(), content_Option), out value))
            col.Width = new FixedPointF(value);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Height", width_height_Option);
        if (float.TryParse(GUILayout.TextField(col.Height.ToString(), content_Option), out value))
            col.Height = new FixedPointF(value);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
