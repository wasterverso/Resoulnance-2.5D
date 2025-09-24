using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Lobby = Unity.Services.Lobbies.Models.Lobby;

public class Services_LobbyController : MonoBehaviour
{
    string TT = "[Services Lobby Controller]";

    [Header("Config")]
    [SerializeField] Services_LobbyUpdate servicesLobbyUpdate;

    [Header("Config")]
    [SerializeField] string lobbyNameDefault = "NewLobby";

    [Tooltip("Número máximo padrão de jogadores permitidos em lobbies criados por este script.")]
    [Range(1, 16)]
    [SerializeField] int defaultMaxPlayers = 1;

    public Lobby hostLobby, joinnedLobby;

    ConcurrentQueue<string> createdLobbyIds = new ConcurrentQueue<string>();
    Coroutine heartBeat_Coroutine;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    public async void CriarLobby()
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {"StartGame", new DataObject(DataObject.VisibilityOptions.Member, "0" )},
                    {"Address", new DataObject(DataObject.VisibilityOptions.Member, "0", DataObject.IndexOptions.S1 )}, // String
                    {"Port", new DataObject(DataObject.VisibilityOptions.Member, "0", DataObject.IndexOptions.N1) } // Numero
                }
            };

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyNameDefault, defaultMaxPlayers, options);
            joinnedLobby = hostLobby;
            servicesLobbyUpdate.InscreverNaAtualizacaoDoLobby();

            Debug.Log($"{TT} Lobby criado com sucesso: {hostLobby.Name} (ID: {hostLobby.Id})");

            createdLobbyIds.Enqueue(hostLobby.Id);

            heartBeat_Coroutine = StartCoroutine(HeartbeatLobbyCoroutine(hostLobby.Id, 20));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError($"{TT} Erro ao criar o lobby: {ex.Reason} - {ex.Message}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"{TT} Erro inesperado: {ex.Message}");
        }
    }

    public async void EntradaRapidaNoLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            options.Filter = new List<QueryFilter>()
            { 
                new QueryFilter(
                    field: QueryFilter.FieldOptions.MaxPlayers, 
                    op: QueryFilter.OpOptions.GE, 
                    value: $"{defaultMaxPlayers}")
            };

            joinnedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            Debug.Log($"{TT} Entrou em um lobby");

            servicesLobbyUpdate.InscreverNaAtualizacaoDoLobby();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task SairDoLobby()
    {
        string playerId = AuthenticationService.Instance.PlayerId;

        try
        {
            if (joinnedLobby.HostId == playerId && joinnedLobby.Players.Count == 1)
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinnedLobby.Id);
            }
            else
            {
                await LobbyService.Instance.RemovePlayerAsync(joinnedLobby.Id, playerId);
            }

            joinnedLobby = null;
            hostLobby = null;

            StopAllCoroutines();

            Debug.Log("Saiu do Lobby");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao sair do lobby: {ex.Message}");
        }
    }

    public async Task<bool> AtualizarCodigoServidorNoLobby(string address, int port)
    {
        try
        {
            if (joinnedLobby != null)
            {
                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinnedLobby.Id, new UpdateLobbyOptions{
                        Data = new Dictionary<string, DataObject>{
                                { "StartGame", new DataObject(DataObject.VisibilityOptions.Member, "1")},
                                {"Address", new DataObject(DataObject.VisibilityOptions.Member, address, DataObject.IndexOptions.S1 )}, // String
                                {"Port", new DataObject(DataObject.VisibilityOptions.Member, $"{port}", DataObject.IndexOptions.N1) }
                        }
                });

                joinnedLobby = lobby;

                return true;
            }
            else
            {
                Debug.LogError($"{TT} Erro: Nenhum lobby encontrado.");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{TT} Erro ao atualizar código Relay no lobby: {e.Message}");
            return false;
        }
    }

    #region (HeartBeat)
    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    #endregion

    async void OnApplicationQuit()
    {
        StopAllCoroutines();

        while (createdLobbyIds.TryDequeue(out var lobbyId))
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Erro ao deletar lobby {lobbyId}: {ex.Message}");
            }
        }
    }
}
