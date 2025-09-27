using Newtonsoft.Json;
using PlayFlow;
using System.Collections.Generic;
using UnityEngine;

namespace Resoulnance.Telas.TelaLobby
{
    public class LobbyEvents : MonoBehaviour
    {
        [SerializeField] PlayFlowLobbyManagerV2 playFlowLobbyManagerV2;

        string TT = "<color=#ADFF2F>[Eventos do Lobby]</color>";

        public void InscreverNosEventos()
        {
            var events = playFlowLobbyManagerV2.Events;

            // Eventos de lobby
            events.OnLobbyCreated.AddListener(OnLobbyCreated);
            events.OnLobbyJoined.AddListener(OnLobbyJoined);
            events.OnLobbyUpdated.AddListener(OnLobbyUpdated);
            events.OnLobbyLeft.AddListener(OnLobbyLeft);

            // Eventos de partida
            events.OnMatchStarted.AddListener(OnMatchStarted);
            events.OnMatchRunning.AddListener(OnMatchRunning);
            events.OnMatchEnded.AddListener(OnMatchEnded);
            events.OnMatchServerDetailsReady.AddListener(OnMatchServerDetailsReady);

            // Eventos de matchmaking
            events.OnMatchmakingStarted.AddListener(OnMatchmakingStarted);
            events.OnMatchmakingCancelled.AddListener(OnMatchmakingCancelled);
            events.OnMatchFound.AddListener(OnMatchFound);

            // Eventos de jogador
            events.OnPlayerJoined.AddListener(OnPlayerJoined);
            events.OnPlayerLeft.AddListener(OnPlayerLeft);

            // Eventos de sistema
            events.OnError.AddListener(OnError);

            // Mudan�as no estado da sess�o
            playFlowLobbyManagerV2.Events.OnStateChanged.AddListener(OnStateChanged);
        }


        #region (Eventos de lobby)
        void OnLobbyCreated(Lobby lobby)
        {
            Debug.Log($"{TT} Lobby criado - {lobby.name}");
        }

        void OnLobbyJoined(Lobby lobby)
        {
            Debug.Log($"{TT} Entrou no lobby - {lobby.name}|| Host: {lobby.host} || Jogadores: {string.Join(", ", lobby.players)}");
        }

        void OnLobbyUpdated(Lobby lobby)
        {
            Debug.Log($"{TT} Lobby atualizado - Status: {lobby.status} || ({lobby.currentPlayers}/{lobby.maxPlayers})");
        }

        void OnLobbyLeft()
        {
            Debug.Log($"{TT} Saiu do lobby");
        }
        #endregion

        #region (Eventos de Partida)
        void OnMatchStarted(Lobby lobby)
        {
            Debug.Log($"{TT} In�cio da partida acionado para o lobby {lobby.name}. Aguardando o servidor...");
        }

        void OnMatchRunning(ConnectionInfo connectionInfo)
        {
            Debug.Log($"{TT} Servidor pronto! IP: {connectionInfo.Ip}, Porta: {connectionInfo.Port}");
        }

        void OnMatchEnded(Lobby lobby)
        {
            Debug.Log($"{TT} Partida encerrada no lobby {lobby.name}. Retornando ao estado 'aguardando'.");
        }
        #endregion

        #region (Eventos de Matchmaking)
        void OnMatchmakingStarted(Lobby lobby)
        {
            Debug.Log($"{TT} Matchmaking iniciado! Modo: {lobby.matchmakingMode}, Status: {lobby.status}");
        }

        void OnMatchmakingCancelled(Lobby lobby)
        {
            Debug.Log($"{TT} Matchmaking cancelado. Status: {lobby.status}");
        }

        void OnMatchFound(Lobby lobby)
        {
            Debug.Log($"{TT} Partida encontrada! Status: {lobby.status}. Servidor do jogo est� sendo iniciado...");

            if (lobby.matchmakingData != null)
            {
                //Debug.Log($"{TT} Dados de matchmaking: {JsonConvert.SerializeObject(lobby.matchmakingData)}");
            }
            
        }

        private void OnMatchServerDetailsReady(List<PortMappingInfo> portMappings)
        {
            Debug.Log("Detalhes completos do servidor recebidos.");

            /*
            var lobbyAtual = playFlowLobbyManagerV2.CurrentLobby;
            if (lobbyAtual?.gameServer?.custom_data != null)
            {
                var dadosCustomizados = lobbyAtual.gameServer.custom_data;

                // Acessa informa��es dos times para partidas baseadas em pap�is
                if (dadosCustomizados.ContainsKey("teams"))
                {
                    var times = dadosCustomizados["teams"] as List<object>;
                    foreach (dynamic time in times)
                    {
                        Debug.Log($"Time {time.team_id}:");
                        foreach (dynamic informacoesLobby in time.lobbies)
                        {
                            foreach (var entradaJogador in informacoesLobby.player_states)
                            {
                                string idJogador = entradaJogador.Key;
                                dynamic estadoJogador = entradaJogador.Value;
                                Debug.Log($"  Jogador {idJogador}: Fun��o = {estadoJogador.role}");
                            }
                        }
                    }
                }

                // Verifica em qual time voc� est�
                var meuIdJogador = playFlowLobbyManagerV2.PlayerId;
                Debug.Log($"Meu ID de jogador: {meuIdJogador}");
            }

            */
        }
        #endregion

        #region (Eventos de Jogador)
        void OnPlayerJoined(PlayerAction action)
        {
            Debug.Log($"{TT} Jogador entrou - {action.PlayerId}");
            //lobbyVisuals.AtualizouLobby();
            //lobbyController.AtualizarDadosDoPlayer();
        }

        void OnPlayerLeft(PlayerAction action)
        {
            Debug.Log($"{TT} Jogador saiu - {action.PlayerId}");
        }
        #endregion

        #region (Eventos De Sistema)
        void OnError(string error)
        {
            Debug.LogError($"{TT} Erro - {error}");
        }
        #endregion

        #region (Sess�o)
        void OnStateChanged(LobbyState oldState, LobbyState newState)
        {
            Debug.Log($"{TT} Estado alterado: {oldState} -> {newState}");
        }
        #endregion

        void OnDestroy()
        {
            if (playFlowLobbyManagerV2 != null)
            {
                if (playFlowLobbyManagerV2.IsInLobby)
                {
                    playFlowLobbyManagerV2.LeaveLobby();
                }

                playFlowLobbyManagerV2.Disconnect();

                var events = playFlowLobbyManagerV2.Events;
                events.OnLobbyCreated.RemoveAllListeners();
                events.OnLobbyJoined.RemoveAllListeners();
                events.OnLobbyUpdated.RemoveAllListeners();
                events.OnLobbyLeft.RemoveAllListeners();
                events.OnPlayerJoined.RemoveAllListeners();
                events.OnPlayerLeft.RemoveAllListeners();
                events.OnMatchStarted.RemoveAllListeners();
                events.OnMatchEnded.RemoveAllListeners();
                events.OnMatchRunning.RemoveAllListeners();
                events.OnMatchServerDetailsReady.RemoveAllListeners();

                events.OnMatchmakingStarted.RemoveAllListeners();
                events.OnMatchmakingCancelled.RemoveAllListeners();
                events.OnMatchFound.RemoveAllListeners();
                events.OnError.RemoveAllListeners();
            }
            else
            {
                Debug.Log($"Nao tinha PlayFlowManager: {playFlowLobbyManagerV2}");
            }
        }
    }
}

