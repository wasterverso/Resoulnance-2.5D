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

        [Header("Refs Scripts")]
        [SerializeField] LobbyVisuals_Padrao lobbyVisuals;
        [SerializeField] LobbyVisuals_Personalizada salaPersonalizada;
        [SerializeField] Contador_Lobby contadorLobby;
        [SerializeField] Carregamento_AvatarGameServer avatarGameServer;

        [Header("UI")]
        [SerializeField] Text tipoSala_txt;
        [SerializeField] Text idSala_txt;

        [Header("Canvas/Painel")]
        [SerializeField] GameObject lobbyPainel;

        [TextArea(5, 15)]
        [SerializeField] string dadosMatch_Debug;

        string idSala;
        TiposDeSalas tipoSala;

        public void StartSala(string codigo)
        {
            pfLobbyManager = PlayFlowLobbyManagerV2.Instance;

            idSala = codigo;
            idSala_txt.text = idSala;

            Dictionary<string, object> dadosLobby = pfLobbyManager.CurrentLobby.settings;
            tipoSala = TiposDeSalas.Nenhum;
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
                pfLobbyManager.Events.OnMatchFound.AddListener(OnMatchFound);
            }

            tipoSala_txt.text = tipoSala.ToString();
            lobbyPainel.SetActive(true);

            pfLobbyManager.Events.OnMatchRunning.AddListener(PartidaFoiIniciada);            

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

            if (tipoSala == TiposDeSalas.Personalizada)
            {
                salaPersonalizada.DesativarInscricoes();
            }
            else
            {
                lobbyVisuals.DesativarInscricoes();
            }
        }

        public async void IniciarPartida()
        {
            bool iniciou = await pfLobbyManager.GetComponentInChildren<LobbyManager>().StartMatchmaking();  
            if (iniciou)
            {
                contadorLobby.Iniciar();
            }
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
            contadorLobby.Parar();

            TelaDeCarregamento.Instance.CarregamentoAchouPartida(true);

            if (lobby.matchmakingData != null)
            {                
                Debug.Log($"startLobbyController Dados de matchmaking: {JsonConvert.SerializeObject(lobby.matchmakingData)}");
            }

            dadosMatch_Debug = JsonConvert.SerializeObject(lobby.gameServer);

            avatarGameServer.LoadAvatars(dadosMatch_Debug);
        }

        public void CancelarFila()
        {
            pfLobbyManager.CancelMatchmaking();
        }
    }
}

