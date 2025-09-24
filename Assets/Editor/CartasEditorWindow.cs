using PurrNet.Prediction;
using Resoulnance.Cartas;
using UnityEditor;
using UnityEngine;

public class CartasEditorWindow : EditorWindow
{
    private Cartas_Data cartasData;
    private int selectedIndex = -1;
    private Vector2 scroll;

    public static void ShowWindow(Cartas_Data data, int index = -1)
    {
        var window = GetWindow<CartasEditorWindow>("Editor de Cartas");
        window.cartasData = data;
        window.selectedIndex = index;
    }

    private void OnGUI()
    {
        GUILayout.Label("Editor de Cartas", EditorStyles.boldLabel);

        cartasData = (Cartas_Data)EditorGUILayout.ObjectField("Cartas Data", cartasData, typeof(Cartas_Data), false);

        if (cartasData == null || cartasData.cartas.Count == 0)
        {
            EditorGUILayout.HelpBox("Arraste o ScriptableObject Cartas_Data e certifique-se de que haja cartas.", MessageType.Info);
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, cartasData.cartas.Count - 1);
        var carta = cartasData.cartas[selectedIndex];

        GUILayout.Label($"Editando Carta {selectedIndex + 1}/{cartasData.cartas.Count}", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        // ---------------- Nome / Tipo ----------------
        carta.NomeCarta = EditorGUILayout.TextField("Nome", carta.NomeCarta);
        carta.id = EditorGUILayout.IntField("ID", carta.id);
        carta.cartaAtiva = (CartaAtivaPassiva)EditorGUILayout.EnumPopup("Tipo", carta.cartaAtiva);

        // ---------------- Permissões ----------------
        GUILayout.Space(10);
        GUILayout.Label("Permissões", EditorStyles.boldLabel);
        carta.podeComprar = EditorGUILayout.Toggle("Pode Comprar", carta.podeComprar);
        carta.podeColocarDeck = EditorGUILayout.Toggle("Pode Colocar no Deck", carta.podeColocarDeck);

        // ---------------- Sprites ----------------
        GUILayout.Space(10);
        GUILayout.Label("Sprites", EditorStyles.boldLabel);
        carta.splashSprite = (Sprite)EditorGUILayout.ObjectField("Splash", carta.splashSprite, typeof(Sprite), false);

        // ---------------- Infos ----------------
        GUILayout.Space(10);
        GUILayout.Label("Infos", EditorStyles.boldLabel);
        carta.tempoRecarga = EditorGUILayout.FloatField("Recarga", carta.tempoRecarga);
        carta.acao = EditorGUILayout.TextField("Ação", carta.acao);

        // ---------------- EfeitosCarta ----------------
        GUILayout.Space(10);
        GUILayout.Label("Efeitos da Carta", EditorStyles.boldLabel);
        if (carta.efeitosCarta == null) carta.efeitosCarta = new EfeitosCarta();
        carta.efeitosCarta.Alvo = (TipoAlvoFinal)EditorGUILayout.EnumPopup("Alvo", carta.efeitosCarta.Alvo);
        carta.efeitosCarta.dano = EditorGUILayout.FloatField("Dano", carta.efeitosCarta.dano);
        carta.efeitosCarta.danoReal = EditorGUILayout.FloatField("Dano Real", carta.efeitosCarta.danoReal);
        carta.efeitosCarta.vida = EditorGUILayout.FloatField("Vida", carta.efeitosCarta.vida);
        carta.efeitosCarta.escudo = EditorGUILayout.FloatField("Escudo", carta.efeitosCarta.escudo);
        carta.efeitosCarta.velocidade = EditorGUILayout.FloatField("Velocidade", carta.efeitosCarta.velocidade);
        carta.efeitosCarta.controle = EditorGUILayout.FloatField("Controle", carta.efeitosCarta.controle);
        carta.efeitosCarta.imunidade = EditorGUILayout.FloatField("Imunidade", carta.efeitosCarta.imunidade);

        // ---------------- Atributos ----------------
        GUILayout.Space(10);
        GUILayout.Label("Atributos", EditorStyles.boldLabel);
        carta.tipo = (Tipagem)EditorGUILayout.EnumPopup("Tipo", carta.tipo);
        carta.valorAtributo1 = EditorGUILayout.FloatField("Valor 1", carta.valorAtributo1);
        carta.valorAtributo2 = EditorGUILayout.FloatField("Valor 2", carta.valorAtributo2);

        // ---------------- Controle de Instância ----------------
        GUILayout.Space(10);
        GUILayout.Label("Controle de Instância", EditorStyles.boldLabel);
        carta.arrastar = EditorGUILayout.Toggle("Arrastar", carta.arrastar);
        carta.in_Mapa = EditorGUILayout.Toggle("No Mapa", carta.in_Mapa);
        carta.In_Player = EditorGUILayout.Toggle("No Player", carta.In_Player);

        // ---------------- Procurar no Range ----------------
        GUILayout.Space(10);
        GUILayout.Label("Procurar no Range", EditorStyles.boldLabel);
        carta.alvoNoRange = EditorGUILayout.Toggle("Alvo no Range", carta.alvoNoRange);
        carta.scalaDoCircle = EditorGUILayout.FloatField("Escala do Círculo", carta.scalaDoCircle);
        carta.rangeProcurar = EditorGUILayout.FloatField("Range Procurar", carta.rangeProcurar);

        // ---------------- Prefabs ----------------
        GUILayout.Space(10);
        GUILayout.Label("Prefabs", EditorStyles.boldLabel);
        carta.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", carta.prefab, typeof(GameObject), false);
        carta.hitPrefab = (GameObject)EditorGUILayout.ObjectField("Hit Prefab", carta.hitPrefab, typeof(GameObject), false);

        EditorGUILayout.EndScrollView();

        // Navegação
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("← Carta Anterior"))
        {
            selectedIndex = (selectedIndex - 1 + cartasData.cartas.Count) % cartasData.cartas.Count;
            scroll = Vector2.zero;
        }
        if (GUILayout.Button("Próxima Carta →"))
        {
            selectedIndex = (selectedIndex + 1) % cartasData.cartas.Count;
            scroll = Vector2.zero;
        }
        EditorGUILayout.EndHorizontal();

        // Marca asset como modificado
        EditorUtility.SetDirty(cartasData);
    }
}
