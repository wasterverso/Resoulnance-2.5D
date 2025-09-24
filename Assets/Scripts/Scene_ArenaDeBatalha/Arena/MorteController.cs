using PurrNet;
using Resoulnance.Scene_Arena;
using Resoulnance.Scene_Arena.Camera;
using Resoulnance.Scene_Arena.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MorteController : NetworkBehaviour
{
    [Header("Refs Scripts")]
    [SerializeField] ArenaReferences arenaReferences;
    CameraFollow cameraFollowScript;

    [Header("Refs Others")]
    [SerializeField] GameObject deadPainel;
    [SerializeField] Text contagem_txt;
    [SerializeField] Image barraContagem_img;

    [Header("Listas")]
    [SerializeField] Button[] bluePlayers_btn;
    [SerializeField] Button[] redPlayers_btn;

    private void Start()
    {
        cameraFollowScript = arenaReferences.cameraFollow;
    }

    public void MeuPlayerMorreu(PlayerController playerCC )
    {
        deadPainel.SetActive(true);
        StartCoroutine(Contagem(playerCC));
    }

    private IEnumerator Contagem(PlayerController playerScript)
    {
        var tempo = playerScript.currentState.tempoParaRessucitar;
        while (tempo >= 0)
        {
            contagem_txt.text = tempo.ToString();
            barraContagem_img.fillAmount = tempo / 15f;
            yield return new WaitForSeconds(1);
            tempo--;
        }

        deadPainel.SetActive(false);
        cameraFollowScript.SetTarget(playerScript.transform);
    }
}
