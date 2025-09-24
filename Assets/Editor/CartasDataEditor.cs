using Resoulnance.Cartas;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cartas_Data))]
public class CartasDataEditor : Editor
{
    private Cartas_Data data;

    private void OnEnable()
    {
        data = (Cartas_Data)target;

        if (data.cartas != null && data.cartas.Count > 0)
        {
            data.cartas.Sort((a, b) => a.id.CompareTo(b.id));
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Cartas Registradas", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (data.cartas == null) return;

        for (int i = 0; i < data.cartas.Count; i++)
        {
            var carta = data.cartas[i];
            if (carta == null) continue;

            EditorGUILayout.BeginHorizontal("box");

            // Nome e ID lado a lado
            EditorGUILayout.LabelField($"ID: {carta.id}", GUILayout.Width(40));
            EditorGUILayout.LabelField($"Nome: {carta.NomeCarta}", GUILayout.ExpandWidth(true));
            //EditorGUILayout.LabelField($"Nome: {carta.NomeCarta}", GUILayout.Width(150));

            // Botão Editar
            if (GUILayout.Button("Editar", GUILayout.Width(50)))
            {
                CartasEditorWindow.ShowWindow(data, i);
            }

            if (GUILayout.Button("Remover", GUILayout.Width(60)))
            {
                Undo.RecordObject(data, "Remover");
                data.cartas.RemoveAt(i);
                EditorUtility.SetDirty(data);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Adicionar Nova Carta"))
        {
            Undo.RecordObject(data, "Adicionar Carta");
            var novaCarta = new Carta()
            {
                NomeCarta = "Nova Carta",
                id = data.cartas.Count > 0 ? data.cartas.Max(c => c.id) + 1 : 0 // garante id único
            };

            data.cartas.Add(novaCarta);

            // Reorganiza por ID crescente
            data.cartas.Sort((a, b) => a.id.CompareTo(b.id));

            EditorUtility.SetDirty(data);

            // Abre a janela de edição já selecionando a carta recém adicionada
            int indexNovaCarta = data.cartas.IndexOf(novaCarta);
            CartasEditorWindow.ShowWindow(data, indexNovaCarta);
        }
    }
}
