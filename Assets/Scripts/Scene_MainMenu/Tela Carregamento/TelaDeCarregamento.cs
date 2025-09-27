using Resoulnance.AvatarCustomization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TelaDeCarregamento : MonoBehaviour
{
    [Serializable]
    class PlayerObj
    {
        [Header("Id Player")]
        public string idPlayer;

        [Header("Objs")]
        public GameObject obj;
        public Text nick;
        public AtribuirDados_AvatarCustom avatarCustom;
        public Image perfil;
        public Image moldura;
    }

    public static TelaDeCarregamento Instance;

    [Header("Carregamento")]
    [SerializeField] GameObject carregamento_Painel;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;

    [Header("Carregamento Avatar")]
    [SerializeField] GameObject avatarCarregamentoPainel;
    [SerializeField] PlayerObj[] bluePlayers = new PlayerObj[4];
    [SerializeField] PlayerObj[] redPlayers = new PlayerObj[4];

    [Header("Carregamento Match")]
    [SerializeField] GameObject matchCarregamentoPainel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region (Carregamento Normal)
    public void CarregarCena(string _sceneName, bool fadeOutAutomatico)
    {
        StartCoroutine(CarregarCenaAsync(_sceneName, fadeOutAutomatico));
    }

    private IEnumerator CarregarCenaAsync(string sceneName, bool fadeOutAutomatico)
    {
        // Ativa o painel
        canvasGroup.alpha = 0f;
        carregamento_Painel.SetActive(true);

        // FADE IN
        yield return StartCoroutine(Fade(0f, 1f, false));

        // Começa o carregamento assíncrono
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // segura até fade out

        // Espera carregar quase tudo
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Ativa a cena
        asyncLoad.allowSceneActivation = true;

        // Espera um frame pra garantir que a cena trocou
        yield return null;

        if (!fadeOutAutomatico) yield break;

        yield return StartCoroutine(Fade(1f, 0f, false));

        // Esconde o painel
        carregamento_Painel.SetActive(false);
    }

    public void FazerFadeOut()
    {
        StartCoroutine(Fade(1f, 0f, true));
    }

    private IEnumerator Fade(float start, float end, bool fadeOutManual)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        canvasGroup.alpha = end;

        if (end == 0f && fadeOutManual)
        {
            carregamento_Painel.SetActive(false);
        }
    }
    #endregion

    #region (Carregamento Avatar)
    public void IniciarCarregamentoAvatar()
    {
        foreach (var player in bluePlayers)
        {
            player.obj.SetActive(false);
        }

        foreach (var player in redPlayers)
        {
            player.obj.SetActive(false);
        }

        avatarCarregamentoPainel.SetActive(true);

        ListTeamController listTeamController = ListTeamController.Instance;
        List<Jogador> jogadores = listTeamController.JogadoresConfig;

        int blueIndex = 0;
        int redIndex = 0;

        foreach (var jogador in jogadores)
        {
            if (jogador.team == Team.Blue) // Time Azul
            {
                // Garante que não vamos estourar o limite do array
                if (blueIndex < bluePlayers.Length)
                {
                    PreencherSlot(bluePlayers[blueIndex], jogador);
                    blueIndex++;
                }
            }
            else if (jogador.team == Team.Red)// Time Vermelho
            {
                if (redIndex < redPlayers.Length)
                {
                    PreencherSlot(redPlayers[redIndex], jogador);
                    redIndex++;
                }
            }
        }
    }

    private void PreencherSlot(PlayerObj slot, Jogador jogador)
    {
        slot.idPlayer = jogador.authId;
        slot.nick.text = jogador.nickname;
        //slot.avatarCustom.AtribuirDados(jogador.avatarCustom);
        slot.avatarCustom.MostrarAvatar(true);

        slot.obj.SetActive(true);
    }

    public void DesativarAvatarPainel()
    {
        avatarCarregamentoPainel.SetActive(false);
    }
    #endregion

    public void CarregamentoAchouPartida(bool carregar)
    {
        matchCarregamentoPainel.SetActive(carregar);
    }
}
