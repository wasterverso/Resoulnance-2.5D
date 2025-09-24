using PurrNet;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;
using UnityEngine.UI;

public class Depositar_btn : MonoBehaviour
{
    [Header("Res Script")]
    [SerializeField] ListCristaisController listCristaisController;
    ArenaReferences arenaReferences;

    [Header("Controle")]
    public bool enviarCristais = false;
    public bool botaoAtivado = true;

    [Header("Refs Int")]
    [SerializeField] Image fundoCountdown;
    [SerializeField] Image frenteBtn;

    [Header("Sprites")]
    [SerializeField] Sprite botaoPadrao;
    [SerializeField] Sprite semCristais;
    [SerializeField] Image bloqueio_img;

    ulong meuId;

    private void Start()
    {
        arenaReferences = ArenaReferences.Instance;
        meuId = NetworkManager.main.localPlayer.id;
    }

    private void OnEnable()
    {
        fundoCountdown.fillAmount = 1f;
        frenteBtn.sprite = botaoPadrao;
        enviarCristais = false;
    }

    private void Update()
    {
        if (!botaoAtivado) return;

        if (enviarCristais)
        {
            var tempoPassado = arenaReferences.playerReferences.playerCristals.currentState.tempoGuardarCristal;
            float fraction = tempoPassado / 1;
            fundoCountdown.fillAmount = 1f - fraction; // Inverte para preencher de 0 a 1
        }
    }

    public void OnButtonDown()
    {
        bool temCristais = listCristaisController.TemCristaisNaMao(meuId);
        if (temCristais)
        {
            arenaReferences.playerReferences.playerCristals.ChamarGuardarCristais();
            fundoCountdown.fillAmount = 0f;
            frenteBtn.sprite = botaoPadrao;
            enviarCristais = true;
        }
        else
        {
            fundoCountdown.fillAmount = 1f;
            frenteBtn.sprite = semCristais;
            enviarCristais = false;
        }
    }
}
