using Newtonsoft.Json;
using PlayFlow;
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

            // Eventos de matchmaking
            events.OnMatchmakingStarted.AddListener(OnMatchmakingStarted);
            events.OnMatchmakingCancelled.AddListener(OnMatchmakingCancelled);
            events.OnMatchFound.AddListener(OnMatchFound);

            // Eventos de jogador
            events.OnPlayerJoined.AddListener(OnPlayerJoined);
            events.OnPlayerLeft.AddListener(OnPlayerLeft);

            // Eventos de sistema
            events.OnError.AddListener(OnError);

            // Mudanças no estado da sessão
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
            Debug.Log($"{TT} Início da partida acionado para o lobby {lobby.name}. Aguardando o servidor...");
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
            Debug.Log($"{TT} Partida encontrada! Status: {lobby.status}. Servidor do jogo está sendo iniciado...");

            if (lobby.matchmakingData != null)
            {
                Debug.Log($"{TT} Dados de matchmaking: {JsonConvert.SerializeObject(lobby.matchmakingData)}");
            }
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

        #region (Sessão)
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

