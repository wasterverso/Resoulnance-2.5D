using PlayFlow;
using PurrNet;
using PurrNet.Transports;
using Resoulnance.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayFlow_LobbyController : MonoBehaviour
{
    
    PlayFlowLobbyManagerV2 lobbyManager;

    [Header("Referências")]
    [SerializeField] PlayFlow_LobbyEvents eventosDoLobby;

    [Tooltip("Nome padrão para lobbies criados por este script. Pode ser substituído ao criar um lobby.")]
    [SerializeField] string defaultLobbyName = "MyTestLobby";

    [Tooltip("Número máximo padrão de jogadores permitidos em lobbies criados por este script.")]
    [Range(1, 16)]
    [SerializeField] int defaultMaxPlayers = 1;

    [Tooltip("Define se os lobbies criados por este script devem ser privados (exigem código de convite) ou públicos.")]
    [SerializeField] bool defaultIsPrivate = false;

    void Start()
    {
        lobbyManager = PlayFlowLobbyManagerV2.Instance;

        try
        {
            // Gera um ID de jogador relativamente único para esta sessão
            //_uniquePlayerId = "LobbyUser" + "_" + Guid.NewGuid().ToString().Substring(0, 8);

            string playerIdAuth = PlayerConfigData.Instance.idAuth;

            lobbyManager.Initialize(playerIdAuth, OnManagerReady);
            //Debug.Log($"[LobbyManager] Inicializado com o ID do jogador: {_uniquePlayerId}");
        }
        catch (Exception ex)
        {
            string erro = $"[LobbyManager] Erro inesperado durante a inicialização: {ex.Message}\n{ex.StackTrace}";
            Debug.LogError(erro);
        }
    }

    void OnManagerReady()
    {
        // Agora você pode chamar outros métodos de lobby com segurança

        eventosDoLobby.InscreverNosEventos();

        if (ListTeamController.Instance.criouLobby)
            CriarLobby();
    }

    [ContextMenu("Criar novo Lobby")]
    public void CriarLobby()
    {
        if (lobbyManager.IsInLobby)
        {
            Debug.LogError("[LobbyManager] Já está em um lobby. Saia antes de tentar entrar em outro.");
            return;
        }

        // Crie configuracoes customizadas para o lobby
        var custom_Settings = new Dictionary<string, object>
        {
            ["gameMode"] = "1v1",
            ["matchType"] = "ranked"
        };

        lobbyManager.CreateLobby(
            name: defaultLobbyName,
            maxPlayers: defaultMaxPlayers,
            isPrivate: defaultIsPrivate,
            allowLateJoin: false,
            region: "south-america-brazil",
            customSettings: custom_Settings,
            onSuccess: (lobby) => {

                bool isHost = PlayFlowLobbyManagerV2.Instance.IsHost;
                string feed = $"Lobby criado com sucesso: {lobby.name} (Regiao: {lobby.region}) (Host: {isHost} : {lobby.host})";
                Debug.Log(feed);

                if (lobby.isPrivate && !string.IsNullOrEmpty(lobby.inviteCode))
                {
                    Debug.Log($"[LobbyHelloWorld] Código de convite: {lobby.inviteCode}");
                }

            },
            onError: (error) => {

                string feed = $"Falha ao criar o lobby: {error}";
                Debug.LogError(feed);
            }
        );
    }

    public void EntrarNoPrimeiroLobbyEncontrado()
    {
        if (lobbyManager.IsInLobby)
        {
            return;
        }

        lobbyManager.GetAvailableLobbies(
            onSuccess: (lobbies) => {
                var availableLobby = lobbies.Find(l => !l.isPrivate && l.currentPlayers < l.maxPlayers);

                if (availableLobby != null)
                {
                    lobbyManager.JoinLobby(availableLobby.id,
                        onSuccess: (lobby) => {
                            string feed = $"Entrou no lobby com sucesso: {lobby.name}";
                            Debug.Log(feed);
                        },
                        onError: (error) => {
                            string feed = $"Falha ao entrar no lobby: {error}";
                            Debug.LogError(feed);
                        }
                    );
                }
                else
                {
                    string feed = "Nenhum lobby público disponível foi encontrado.";
                    Debug.LogWarning(feed);
                }
            },
            onError: (error) => {
                string feed = $"Falha ao obter os lobbies: {error}";
                Debug.LogError(feed);
            }
        );
    }

    public bool SairDoLobby()
    {
        if (lobbyManager.CurrentLobby == null)
        {
            Debug.LogWarning("[LobbyManager] Não está em um lobby!");
            return true;
        }

        Debug.Log("[LobbyManager] Tentando sair do lobby atual...");

        bool saiu = false;
        lobbyManager.LeaveLobby(
            onSuccess: () => {
                Debug.Log("[LobbyManager] Saiu do lobby com sucesso");
                saiu = true;
            },
            onError: (error) => {
                Debug.LogError($"[LobbyManager] Falha ao sair do lobby: {error}");
                saiu = false;
            }
        );

        return saiu;
    }

    [ContextMenu("Iniciar Partida")]
    public void IniciarPartida()
    {
        var lobbyMan = PlayFlowLobbyManagerV2.Instance;

        if (lobbyMan.CurrentLobby == null)
        {
            Debug.LogWarning("[LobbyController] Não é possível iniciar a partida: você precisa estar em um lobby");
            return;
        }
        
        if (!lobbyMan.IsHost)
        {
            var host = lobbyMan.CurrentLobby.host;
            var meuId = lobbyMan.PlayerId;
            Debug.LogWarning($"[LobbyController] Não é possível iniciar a partida: você não é o host. (Host: {host} | Voce {meuId})");
            return;
        }

        Debug.Log("[LobbyManager] Iniciando partida...");

        lobbyManager.StartMatch(
            onSuccess: (lobby) => {
                string feed = $"[LobbyManager] Partida iniciada com sucesso. Status: {lobby.status}";
                Debug.Log(feed);
            },
            onError: (error) => {
                string feed = $"[LobbyManager] Falha ao iniciar a partida: {error}";
                Debug.LogError(feed);
            }
        );
    }

    public void ConectarServidor()
    {
        if (lobbyManager.CurrentLobby?.status != "in_game")
        {
            Debug.LogWarning("[LobbyController] Não é possível obter informações de conexão: não está em uma partida em andamento.");
            return;
        }

        ConnectionInfo? connectionInfo = PlayFlowLobbyManagerV2.Instance.GetGameServerConnectionInfo();

        if (connectionInfo.HasValue)
        {
            //Debug.Log($"[LobbyController] Informações do servidor de jogo: IP = {connectionInfo.Value.Ip}, Porta = {connectionInfo.Value.Port}");
            // Aqui você usaria essas informações para conectar seu cliente do jogo (ex: usando Netcode, Mirror, etc.)

            UDPTransport transp = NetworkManager.main.GetComponent<UDPTransport>();

            if (transp != null)
            {
                transp.address = connectionInfo.Value.Ip;
                transp.serverPort = (ushort)connectionInfo.Value.Port;
            }

            //await Task.Delay(5000); // espera 5 segundos sem travar o jogo

            NetworkManager.main.StartClient();
        }
        else
        {
            Debug.LogError("[LobbyController] Falha ao obter informações de conexão, embora a partida esteja em execução. Os dados do servidor de jogo podem estar faltando.");
        }
    }

    public void FinalizarPartida()
    {
        if (lobbyManager.CurrentLobby == null || !lobbyManager.IsHost)
        {
            Debug.LogWarning("[LobbyManager] Não é possível finalizar a partida: você precisa estar em um lobby e ser o host.");
            return;
        }

        Debug.Log("[LobbyManager] Finalizando partida...");

        PlayFlowLobbyManagerV2.Instance.EndMatch(
            onSuccess: (lobby) => {
                Debug.Log($"[LobbyManager] Partida finalizada com sucesso. Status: {lobby.status}");
            },
            onError: (error) => {
                Debug.LogError($"[LobbyManager] Falha ao finalizar a partida: {error}");
            }
        );
    }

    public void AtualizarDadosDoPlayer()
    {
        var myCustomData = new Dictionary<string, object>
        {
            { "ready", true },
            { "character", "Wizard" },
            { "skin", "blue_robe" }
        };

        PlayFlowLobbyManagerV2.Instance.UpdatePlayerState(myCustomData);
    }
}
