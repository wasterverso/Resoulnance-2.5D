using Newtonsoft.Json;
using PlayFlow;
using Resoulnance.Telas.TelaMainMenu;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaLobby
{
    public class Start_LobbyController : MonoBehaviour
    {
        PlayFlowLobbyManagerV2 pfLobbyManager;

        [Header("UI")]
        [SerializeField] LobbyVisuals_Padrao lobbyVisuals;
        [SerializeField] LobbyVisuals_Personalizada salaPersonalizada;

        [Header("UI")]
        [SerializeField] Text tipoSala_txt;
        [SerializeField] Text idSala_txt;

        [Header("Canvas/Painel")]
        [SerializeField] GameObject lobbyPainel;

        string idSala;

        public void StartSala(string codigo)
        {
            pfLobbyManager = PlayFlowLobbyManagerV2.Instance;

            idSala = codigo;
            idSala_txt.text = idSala;

            Dictionary<string, object> dadosLobby = pfLobbyManager.CurrentLobby.settings;
            TiposDeSalas tipoSala = TiposDeSalas.Nenhum;

            if (dadosLobby.TryGetValue("gameMode", out object gameModeObj))
            {
                string gameModeString = gameModeObj.ToString();

                if (Enum.TryParse(gameModeString, out tipoSala))
                {
                    ListTeamController.Instance.tipoSalaAtual = tipoSala;
                    Debug.Log("Tipo da sala carregado com sucesso: " + tipoSala);
                }
            }

            if (tipoSala == TiposDeSalas.Personalizada)
            {
                salaPersonalizada.StartSala();
            }
            else
            {
                lobbyVisuals.EntrouNoLobby();
            }

            lobbyPainel.SetActive(true);

            pfLobbyManager.Events.OnMatchRunning.AddListener(PartidaFoiIniciada);
            pfLobbyManager.Events.OnMatchFound.AddListener(OnMatchFound);

            NotificarPainel.Instance.AtivarCarregandoPainel(false);
        }

        public async void SairDoLobby()
        {
            NotificarPainel.Instance.AtivarCarregandoPainel(true);
            bool saiu = await pfLobbyManager.GetComponentInChildren<LobbyManager>().SairDoLobby();
            if (saiu)
            {
                lobbyPainel.SetActive(false);
                NotificarPainel.Instance.AtivarCarregandoPainel(false);
            }
            else
                Debug.LogWarning("Jogador não conseguiu sair do lobby");

            salaPersonalizada.DesativarInscricoes();
        }

        public async void IniciarPartida()
        {
            bool iniciou = await pfLobbyManager.GetComponentInChildren<LobbyManager>().StartMatchmaking();                      
        }

        public async void IniciarPartidaPersonalizada()
        {
            var lobbyManager = pfLobbyManager.GetComponentInChildren<LobbyManager>();

            lobbyManager.AtualizarConfiguracoesDoLobby(true);

            bool iniciou = await lobbyManager.IniciarPartida();
        }

        void PartidaFoiIniciada(ConnectionInfo connectionInfo)
        {
            var listTeamController = ListTeamController.Instance;
            listTeamController.address = connectionInfo.Ip;
            listTeamController.port = connectionInfo.Port;
            listTeamController.networkMode = NetworkMode.Cliente;

            SceneManager.LoadScene("Preparacao", LoadSceneMode.Single);
        }

        void OnMatchFound(Lobby lobby)
        {
            Debug.Log($"startLobbyController Partida encontrada! Status: {lobby.status}. Servidor do jogo está sendo iniciado...");

            if (lobby.matchmakingData != null)
            {
                Debug.Log($"startLobbyController Dados de matchmaking: {JsonConvert.SerializeObject(lobby.matchmakingData)}");
            }
        }
    }
}

