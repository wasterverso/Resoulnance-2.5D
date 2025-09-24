#if(UNITY_EDITOR)

using UnityEditor;
using UnityEngine;
using System.IO;

public static class CustomScriptCreator
{
    [MenuItem("Assets/Create/Meus Scripts Personalizados/Predicted State", false, 0)]
    public static void CreatePredictedState()
    {
        // Caminho relativo do template
        string templatePath = "Assets/Plugins/Meu Script Personalzado/PredictedStateTemplate.cs.txt";

        // Nome inicial sugerido
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewPredictedState.cs");
    }
}

#endif