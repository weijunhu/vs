using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class ColliderEditor : Editor
{
    bool showBound = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginVertical();

        CustomCollider col = target as CustomCollider;
        //顶点坐标
        if (col.Bound != null)
        {
            showBound = EditorGUILayout.Foldout(showBound, "Bound");
            if (showBound)
            {
                GUILayoutOption bound_Option = GUILayout.Width(30);
                GUILayoutOption bound_xyzOption = GUILayout.Width(10);
                GUILayoutOption bound_xyz_valueOption = GUILayout.Width(70);
                for (int i = 0; i < col.Bound.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("p" + (i + 1), bound_Option);
                    GUILayout.Label("x", bound_xyzOption);
                    GUILayout.Label(col.Bound[i].x.value.ToString(), bound_xyz_valueOption);
                    GUILayout.Label("y", bound_xyzOption);
                    GUILayout.Label(col.Bound[i].y.value.ToString(), bound_xyz_valueOption);
                    GUILayout.Label("z", bound_xyzOption);
                    GUILayout.Label(col.Bound[i].z.value.ToString(), bound_xyz_valueOption);
                    GUILayout.EndHorizontal();
                }
            }
        }

        GUILayout.EndVertical();
    }

    static Color select_color = Color.green;
    static Color no_select_color = new Color(select_color.r/2, select_color.g / 2, select_color.b / 2, select_color.a);
    public virtual void OnSceneGUI()
    {
        CustomCollider collider = target as CustomCollider;
        collider.DrawGizmos(select_color);
    }

    [DrawGizmo(GizmoType.NonSelected)]
    static void DrawGizmos_NoSelected(Transform transform, GizmoType gizmoType)
    {
        CustomCollider collider = transform.GetComponent<CustomCollider>();
        if (collider == null)
            return;
        collider.DrawGizmos(no_select_color);
        Handles.Label(transform.position, transform.gameObject.name);
    }
}
