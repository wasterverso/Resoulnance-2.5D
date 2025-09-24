using PurrNet;
using System;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class ControleDeTreinamento : MonoBehaviour
{
    [Header("Refs UI")]
    [SerializeField] ListCristaisController listCristaisController;

    [Header("Refs UI")]
    [SerializeField] GameObject buttonAbrirPainel;
    [SerializeField] GameObject controlePainel;

    private void Start()
    {
        NetworkManager.main.onPlayerJoined += VerificarButton;        
    }

    private void VerificarButton(PlayerID player, bool isReconnect, bool asServer)
    {
        buttonAbrirPainel.SetActive(NetworkManager.main.isHost);
    }

    public void AdicionarCristaisNoGuardiaoInimigo()
    {
        int adicionados = 0;

        for (int i = 0; i < listCristaisController.currentState.cristaisList.Count; i++)
        {
            if (adicionados >= 10)
                break;

            CristalInfo cristal = listCristaisController.currentState.cristaisList[i];
            if (cristal.estaAtivado)
            {
                // modifica
                cristal.idPlayer = 0;
                cristal.team = Team.Red;
                cristal.estaAtivado = false;
                cristal.estaNoGuardiao = true;

                // devolve na lista
                listCristaisController.currentState.cristaisList[i] = cristal;

                adicionados++;
            }
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.main != null)
            NetworkManager.main.onPlayerJoined -= VerificarButton;
    }
}
