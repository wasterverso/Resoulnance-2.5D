#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(SeparatorComponent))]
[CanEditMultipleObjects]
public class SeparatorComponentEditor : Editor
{
    // Vari�vel para controlar se o foldout est� aberto
    private bool showFields = false;

    public override void OnInspectorGUI()
    {
        SeparatorComponent header = (SeparatorComponent)target;

        // Estilo centralizado e negrito
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        // Define ret�ngulo para o cabe�alho
        Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 20);
        EditorGUI.DrawRect(rect, header.headerColor);

        // Divide o ret�ngulo para o label e o bot�o "Edit"
        Rect labelRect = new Rect(rect.x, rect.y, rect.width - 50, rect.height);
        Rect buttonRect = new Rect(rect.width - 50, rect.y, 50, rect.height);

        // Desenha o label
        EditorGUI.LabelField(labelRect, $"----- {header.headerName} -----", style);

        // Bot�o "Edit" � direita
        if (GUI.Button(buttonRect, "Edit"))
        {
            showFields = !showFields;
        }

        // Campos expandidos
        if (showFields)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headerName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headerColor"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif