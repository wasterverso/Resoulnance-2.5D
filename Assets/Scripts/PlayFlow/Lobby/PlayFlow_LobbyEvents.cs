using Newtonsoft.Json;
using PlayFlow;
using Resoulnance.Telas.TelaLobby;
using UnityEngine;

public class PlayFlow_LobbyEvents : MonoBehaviour
{
    public static PlayFlow_LobbyEvents Instance;

    [Header("Referências")]
    [Tooltip("Atribua aqui sua instância do PlayFlowLobbyManagerV2. Isso é necessário para que o script funcione.")]
    [SerializeField] PlayFlowLobbyManagerV2 lobbyManager;
    [SerializeField] PlayFlow_LobbyController lobbyController;
    [SerializeField] LobbyVisuals_Padrao lobbyVisuals;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void InscreverNosEventos()
    {
        var events = lobbyManager.Events;

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
        lobbyManager.Events.OnStateChanged.AddListener(OnStateChanged);
    }

    #region (Eventos de lobby)
    void OnLobbyCreated(Lobby lobby)
    {
        Debug.Log($"<color=#00FF00>[Eventos do Lobby]</color> Lobby criado - {lobby.name}");

        lobbyVisuals.EntrouNoLobby();
        //lobbyVisuals.AtualizouLobby();
    }

    void OnLobbyJoined(Lobby lobby)
    {
        Debug.Log($"<color=#00FF00>[Eventos do Lobby]</color> Entrou no lobby - {lobby.name}");
        Debug.Log($"  Jogadores: {string.Join(", ", lobby.players)}");
        Debug.Log($"  Host: {lobby.host}");

        lobbyVisuals.EntrouNoLobby();
        //lobbyVisuals.AtualizouLobby();
    }

    void OnLobbyUpdated(Lobby lobby)
    {
        Debug.Log($"<color=#00FF00>[Eventos do Lobby]</color> Lobby atualizado - {lobby.name} ({lobby.currentPlayers}/{lobby.maxPlayers})");
        //lobbyVisuals.AtualizouLobby();
    }

    void OnLobbyLeft()
    {
        Debug.Log("<color=#00FF00>[Eventos do Lobby]</color> Saiu do lobby");
    }
    #endregion

    #region (Eventos de Partida)
    void OnMatchStarted(Lobby lobby)
    {
        Debug.Log($"<color=#1E90FF>[Eventos do Lobby]</color> Início da partida acionado para o lobby {lobby.name}. Aguardando o servidor...");
    }

    void OnMatchRunning(ConnectionInfo connectionInfo)
    {
        Debug.Log($"<color=#1E90FF>[Eventos do Lobby]</color> Servidor pronto! IP: {connectionInfo.Ip}, Porta: {connectionInfo.Port}");
        
        //lobbyVisuals.IniciarPartida(connectionInfo.Ip, connectionInfo.Port);
    }

    void OnMatchEnded(Lobby lobby)
    {
        Debug.Log($"<color=#1E90FF>[Eventos do Lobby]</color> Partida encerrada no lobby {lobby.name}. Retornando ao estado 'aguardando'.");
    }
    #endregion

    #region (Eventos de Matchmaking)
    void OnMatchmakingStarted(Lobby lobby)
    {
        Debug.Log($"<color=#FFD700>[Eventos do Lobby]</color> Matchmaking iniciado! Modo: {lobby.matchmakingMode}, Status: {lobby.status}");
        Debug.Log($"<color=#FFD700>[Eventos do Lobby]</color> Procurando oponentes com MMR semelhante...");
    }

    void OnMatchmakingCancelled(Lobby lobby)
    {
        Debug.Log($"<color=#FFD700>[Eventos do Lobby]</color> Matchmaking cancelado. Status: {lobby.status}");
    }

    void OnMatchFound(Lobby lobby)
    {
        Debug.Log($"<color=#FFD700>[Eventos do Lobby]</color> Partida encontrada! Status: {lobby.status}");
        Debug.Log($"<color=#FFD700>[Eventos do Lobby]</color> Servidor do jogo está sendo iniciado...");

        if (lobby.matchmakingData != null)
        {
            Debug.Log($"<color=#FFD700>[Eventos do Lobby]</color> Dados de matchmaking: {JsonConvert.SerializeObject(lobby.matchmakingData)}");
        }
    }
    #endregion

    #region (Eventos de Jogador)
    void OnPlayerJoined(PlayerAction action)
    {
        Debug.Log($"<color=#ADFF2F>[Eventos do Lobby]</color> Jogador entrou - {action.PlayerId}");
        //lobbyVisuals.AtualizouLobby();
        lobbyController.AtualizarDadosDoPlayer();
    }

    void OnPlayerLeft(PlayerAction action)
    {
        Debug.Log($"<color=#ADFF2F>[Eventos do Lobby]</color> Jogador saiu - {action.PlayerId}");
    }
    #endregion

    #region (Eventos De Sistema)
    void OnError(string error)
    {
        Debug.LogError($"<color=#FF4500>[Eventos do Lobby]</color> Erro - {error}");
    }
    #endregion

    #region (Sessão)
    void OnStateChanged(LobbyState oldState, LobbyState newState)
    {
        Debug.Log($"<color=#BA55D3>[Eventos do Lobby]</color> Estado alterado: {oldState} -> {newState}");
    }
    #endregion

    void OnDestroy()
    {
        // Limpeza
        if (lobbyManager != null)
        {
            // Se o jogador estiver em um lobby, saia graciosamente
            if (lobbyManager.IsInLobby)
            {
                // Essa chamada é "fire-and-forget", pois a aplicação provavelmente será encerrada
                lobbyManager.LeaveLobby();
            }

            lobbyManager.Disconnect();

            var events = lobbyManager.Events;
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
    }
}
