using PlayFlow;
using PurrNet;
using Resoulnance.AvatarCustomization;
using Resoulnance.Telas.TelaMainMenu;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaLobby
{
    public class LobbyVisuals_Personalizada : MonoBehaviour
    {
        [System.Serializable]
        class PlayerLobbyObj
        {
            [Header("Id Player")]
            public string idPlayer;

            [Header("Objs")]
            public GameObject obj;
            public Text nick;
            public GameObject isHostObj;
            public AtribuirDados_AvatarCustom avatarCustom;
            public GameObject avisoObj;
        }

        [Header("Refs Script")]
        [SerializeField] Start_LobbyController startLobbyController;
        PlayFlowLobbyManagerV2 pfLobbyManager;

        [Header("UI")]
        [SerializeField] GameObject personalizadaPainel;
        [SerializeField] Button iniciarPartida_btn;

        [Header("Lista Players")]
        [SerializeField] PlayerLobbyObj[] playerLobbyObjs = new PlayerLobbyObj[8];

        private bool jogadorEstaPronto = false;
        private bool isConfiguredAsHost = false;
        int posicaoAtual = -1;

        public void StartSala()
        {
            int pos = 0;

            foreach (var playerObj in playerLobbyObjs)
            {
                playerObj.idPlayer = string.Empty;
                playerObj.isHostObj.SetActive(false);
                playerObj.avatarCustom.MostrarAvatar(false);
                playerObj.avisoObj.SetActive(false);

                int posicaoCorreta = pos;
                playerObj.obj.GetComponent<Button>().onClick.RemoveAllListeners();
                playerObj.obj.GetComponent<Button>().onClick.AddListener(() => MudarPosicao(posicaoCorreta));
                pos++;
            }

            personalizadaPainel.SetActive(true);

            pfLobbyManager = PlayFlowLobbyManagerV2.Instance;
            pfLobbyManager.Events.OnLobbyUpdated.AddListener(AtualizarPlayersNaSala);

            jogadorEstaPronto = true;
            isConfiguredAsHost = false;
            iniciarPartida_btn.onClick.RemoveAllListeners();
            iniciarPartida_btn.GetComponentInChildren<Text>().text = "Esperar";
            iniciarPartida_btn.onClick.AddListener(OnBotaoClienteClick);
        }

        public void AtualizarPlayersNaSala(Lobby lobby)
        {
            if (pfLobbyManager == null) pfLobbyManager = PlayFlowLobbyManagerV2.Instance;

            bool souHost = lobby.host == pfLobbyManager.PlayerId;

            var posicaoParaPlayerId = new Dictionary<int, string>();
            var playersSemPosicao = new List<string>();

            foreach (var playerId in lobby.players)
            {
                if (lobby.lobbyStateRealTime.TryGetValue(playerId, out var dadosPlayer))
                {
                    var state = dadosPlayer as Dictionary<string, object>;
                    if (state != null && state.TryGetValue("posicao", out var posObj))
                    {
                        int posicao = Convert.ToInt32(posObj);

                        if (posicao >= 0 && posicao < playerLobbyObjs.Length)
                        {
                            // Jogador já tem uma posição válida, adiciona ao mapa.
                            posicaoParaPlayerId[posicao] = playerId;
                        }
                        else
                        {
                            // Jogador é novo (posicao == -1), adiciona à lista de espera.
                            playersSemPosicao.Add(playerId);
                        }
                    }
                }
            }

            if (souHost && playersSemPosicao.Count > 0)
            {
                // Itera por todos os slots possíveis (0 a 7)
                for (int i = 0; i < playerLobbyObjs.Length; i++)
                {                    
                    if (playersSemPosicao.Count == 0) break; // Se não há mais jogadores para posicionar, podemos parar.

                    // O slot 'i' está vago? (Verificamos se não está no nosso mapa)
                    if (!posicaoParaPlayerId.ContainsKey(i))
                    {
                        // Sim, está vago! Pegamos o primeiro jogador da lista de espera.
                        string playerParaAtribuir = playersSemPosicao[0];
                        playersSemPosicao.RemoveAt(0);

                        var newState = new Dictionary<string, object> { { "posicao", i } };

                        pfLobbyManager.GetComponentInChildren<LobbyManager>().AtualizarOutroPlayer(playerParaAtribuir, i);
                        posicaoParaPlayerId[i] = playerParaAtribuir;
                    }
                }
            }

            for (int i = 0; i < playerLobbyObjs.Length; i++)
            {
                var slot = playerLobbyObjs[i];

                if (posicaoParaPlayerId.TryGetValue(i, out string novoPlayerId))
                {
                    // Pega os dados do jogador que deve estar neste slot.
                    var dadosNovoPlayer = lobby.lobbyStateRealTime[novoPlayerId] as Dictionary<string, object>;

                    if (slot.idPlayer != novoPlayerId)
                    {
                        slot.idPlayer = novoPlayerId;
                        slot.isHostObj.SetActive(lobby.host == novoPlayerId);

                        if (dadosNovoPlayer.TryGetValue("avatar", out var avatarObj))
                        {
                            string avatarJson = avatarObj.ToString();
                            AvatarCustom playerCustom = ListAvatarCustom.Instance.DesserializarAvatarCustom(avatarJson);
                            slot.avatarCustom.MostrarAvatar(true);
                            slot.avatarCustom.AtribuirDados(playerCustom);
                        }
                    }
                    else
                    {
                        slot.isHostObj.SetActive(lobby.host == novoPlayerId);
                    }

                    if (dadosNovoPlayer.TryGetValue("isReady", out var readyObj))
                    {
                        bool isReady = Convert.ToBoolean(readyObj);
                        slot.avisoObj.SetActive(!isReady);
                    }                    
                }
                else
                {
                    slot.avatarCustom.AtribuirDados(ListAvatarCustom.Instance.NovoAvatarCustom());
                    slot.avatarCustom.MostrarAvatar(false);
                    slot.idPlayer = string.Empty;
                    slot.isHostObj.SetActive(false);
                    slot.avisoObj.SetActive(false);
                }
            }

            AtualizarBotao(souHost);

            Dictionary<string, object> dadosLobby = lobby.settings;
            if (dadosLobby.TryGetValue("OnStart", out object onStart))
            {
                bool isReady = Convert.ToBoolean(onStart);
                if (isReady)
                {
                    HostIniciouPartida(lobby);
                }
            }
        }

        void MudarPosicao(int novaPos)
        {
            posicaoAtual = novaPos;
            PlayFlowLobbyManagerV2.Instance.GetComponentInChildren<LobbyManager>().AtualizarPlayer(novaPos, jogadorEstaPronto);
        }

        void AtualizarBotao(bool isHost)
        {
            if (isConfiguredAsHost == isHost)
                return;

            isConfiguredAsHost = isHost;

            iniciarPartida_btn.onClick.RemoveAllListeners();
            var btnTxt = iniciarPartida_btn.GetComponentInChildren<Text>();

            if (isHost)
            {
                btnTxt.text = "Iniciar partida";
                iniciarPartida_btn.onClick.AddListener(() => IniciarPartida());
            }
            else
            {
                iniciarPartida_btn.onClick.AddListener(OnBotaoClienteClick);
            }               
        }

        void OnBotaoClienteClick()
        {
            jogadorEstaPronto = !jogadorEstaPronto;

            var lobbyManager = PlayFlowLobbyManagerV2.Instance.GetComponentInChildren<LobbyManager>();
            lobbyManager.AtualizarPlayer(posicaoAtual, jogadorEstaPronto);

            var btnTxt = iniciarPartida_btn.GetComponentInChildren<Text>();
            if (jogadorEstaPronto)
            {
                btnTxt.text = "Esperar";
            }
            else
            {
                btnTxt.text = "Pronto";
            }
        }

        void IniciarPartida()
        {
            bool todosEstaoProntos = true;
            bool temJogadorTimeAzul = false;
            bool temJogadorTimeVermelho = false;

            var lobby = pfLobbyManager.CurrentLobby;
            foreach (var playerId in lobby.players)
            {
                if (lobby.lobbyStateRealTime.TryGetValue(playerId, out var dadosPlayer))
                {
                    var state = dadosPlayer as Dictionary<string, object>;
                    if (state == null) continue;

                    if (state.TryGetValue("isReady", out var prontoObj))
                    {
                        bool ready = Convert.ToBoolean(prontoObj);
                        if (!ready)
                        {
                            todosEstaoProntos = false;
                        }
                    }

                    if (state.TryGetValue("posicao", out var posObj))
                    {
                        int posicao = Convert.ToInt32(posObj);
                                               
                        if (posicao >= 0 && posicao <= 3)  // Slots 0, 1, 2, 3 = Time Azul
                        {
                            temJogadorTimeAzul = true;
                        }                        
                        else if (posicao >= 4 && posicao <= 7) // Slots 4, 5, 6, 7 = Time Vermelho
                        {
                            temJogadorTimeVermelho = true;
                        }
                    }
                }
            }

            if (!todosEstaoProntos)
            {
                NotificarPainel.Instance.AtivarErro(true, 8);
            }
            else if (!temJogadorTimeAzul || !temJogadorTimeVermelho)
            {
                NotificarPainel.Instance.AtivarErro(true, 9);
            }
            else
            {
                startLobbyController.IniciarPartidaPersonalizada();
            }
        }

        public void DesativarInscricoes()
        {
            pfLobbyManager.Events.OnLobbyUpdated.RemoveListener(AtualizarPlayersNaSala);
            personalizadaPainel.SetActive(false);
        }

        void HostIniciouPartida(Lobby lobby)
        {
            foreach (var player in lobby.players)
            {
                var state = lobby.lobbyStateRealTime[player] as Dictionary<string, object>;

                string idAuth = "";
                string nick = "";
                Team teamPlayer = Team.Nenhum;
                AvatarCustom novoAvatar = new AvatarCustom();

                if (state.TryGetValue("idAuth", out var _idAuth))
                {
                    idAuth = _idAuth.ToString();
                }

                if (state.TryGetValue("nickname", out var _nick))
                {
                    nick = _nick.ToString();
                }

                if (state.TryGetValue("posicao", out var posObj))
                {
                    int posicao = Convert.ToInt32(posObj);

                    if (posicao >= 0 && posicao <= 3)  // Slots 0, 1, 2, 3 = Time Azul
                    {
                        teamPlayer = Team.Blue;
                    }
                    else if (posicao >= 4 && posicao <= 7) // Slots 4, 5, 6, 7 = Time Vermelho
                    {
                        teamPlayer = Team.Red;
                    }
                }

                if (state.TryGetValue("avatar", out var avatarObj))
                {
                    string avatarJson = avatarObj.ToString();
                    novoAvatar = ListAvatarCustom.Instance.DesserializarAvatarCustom(avatarJson);
                }

                Jogador jogador = new Jogador();
                jogador.authId = idAuth;
                jogador.nickname = nick;
                jogador.team = teamPlayer;
                jogador.avatarCustom = novoAvatar;

                ListTeamController.Instance.JogadoresConfig.Add(jogador);
            }

            TelaDeCarregamento.Instance.IniciarCarregamentoAvatar();

            DesativarInscricoes();
        }
    }
}

