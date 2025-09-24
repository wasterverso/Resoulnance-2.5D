#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(SeparatorComponent))]
[CanEditMultipleObjects]
public class SeparatorComponentEditor : Editor
{
    // Variável para controlar se o foldout está aberto
    private bool showFields = false;

    public override void OnInspectorGUI()
    {
        SeparatorComponent header = (SeparatorComponent)target;

        // Estilo centralizado e negrito
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        // Define retângulo para o cabeçalho
        Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 20);
        EditorGUI.DrawRect(rect, header.headerColor);

        // Divide o retângulo para o label e o botão "Edit"
        Rect labelRect = new Rect(rect.x, rect.y, rect.width - 50, rect.height);
        Rect buttonRect = new Rect(rect.width - 50, rect.y, 50, rect.height);

        // Desenha o label
        EditorGUI.LabelField(labelRect, $"----- {header.headerName} -----", style);

        // Botão "Edit" à direita
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