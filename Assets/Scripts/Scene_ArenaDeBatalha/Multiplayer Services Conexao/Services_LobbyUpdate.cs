using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class Services_LobbyUpdate : MonoBehaviour
{
    string TT = "[Services Lobby Update]";

    [SerializeField] Services_LobbyController servicesLobbyController;
    [SerializeField] Services_PartidaController servicesPartidaController;

    ILobbyEvents mLobbyEventos;

    public async void InscreverNaAtualizacaoDoLobby()
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += AoAlterarLobby;
        callbacks.KickedFromLobby += AoSerExpulsoDoLobby;
        callbacks.LobbyEventConnectionStateChanged += AoMudarEstadoConexao;
        callbacks.DataChanged += AlterarDadosDoLobby;

        try
        {
            mLobbyEventos = await LobbyService.Instance.SubscribeToLobbyEventsAsync(servicesLobbyController.joinnedLobby.Id, callbacks);
        }
        catch (LobbyServiceException ex)
        {
            switch (ex.Reason)
            {
                case LobbyExceptionReason.AlreadySubscribedToLobby:
                    Debug.LogWarning($"{TT} Já inscrito no lobby[{servicesLobbyController.joinnedLobby.Name}]. Não precisamos tentar novamente. Mensagem da exceção: {ex.Message}");
                    break;
                case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy:
                    Debug.LogError($"{TT} A inscrição nos eventos do lobby foi perdida enquanto estava ocupado tentando se inscrever. Mensagem da exceção: {ex.Message}");
                    throw;
                case LobbyExceptionReason.LobbyEventServiceConnectionError:
                    Debug.LogError($"{TT} Falha ao conectar aos eventos do lobby. Mensagem da exceção: {ex.Message}");
                    throw;
                default:
                    throw;
            }
        }
    }

    private void AlterarDadosDoLobby(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dictionary)
    {
        Debug.Log("Dados do lobby alterados:");

        bool iniciouGame = false;
        int port = 0;
        string address = "0";

        foreach (var kvp in dictionary)
        {
            string chave = kvp.Key;
            var valorAlterado = kvp.Value.Value.Value;

            Debug.Log($"Campo '{chave}' alterado para: {valorAlterado}");

            if(chave == "StartGame")
            {
                if (valorAlterado == "1")
                {
                    iniciouGame = true;
                }
            }

            if (chave == "Port")
            {
                port = int.Parse(valorAlterado);
            }

            if(chave == "Address")
            {
                address = valorAlterado;
            }
        }

        if (iniciouGame)
        {
            servicesPartidaController.ConectarNoServidor(address, port);
        }
    }

    void AoAlterarLobby(ILobbyChanges mudancas)
    {
        if (mudancas.LobbyDeleted)
        {
            Debug.LogWarning($"{TT} O lobby foi deletado. Não é possível aplicar mudanças.");
            return;
        }

        mudancas.ApplyToLobby(servicesLobbyController.joinnedLobby);

        Debug.Log($"Lobby Alterado - Players: {servicesLobbyController.joinnedLobby.Players.Count}");
    }

    private void AoSerExpulsoDoLobby()
    {
        mLobbyEventos = null;
    }

    private void AoMudarEstadoConexao(LobbyEventConnectionState estado)
    {
        switch (estado)
        {
            case LobbyEventConnectionState.Unsubscribed:
                Debug.Log($"{TT} Desinscrito dos eventos do lobby.");
                mLobbyEventos = null;
                break;
            case LobbyEventConnectionState.Subscribing:
                Debug.Log($"{TT} Tentando se inscrever nos eventos do lobby...");
                break;
            case LobbyEventConnectionState.Subscribed:
                Debug.Log($"{TT} Inscrição bem-sucedida nos eventos do lobby.");
                break;
            case LobbyEventConnectionState.Unsynced:
                Debug.LogWarning($"{TT} Problemas de sincronização com os eventos do lobby. Tentando reconectar automaticamente...");
                break;
            case LobbyEventConnectionState.Error:
                Debug.LogError($"{TT} Erro crítico na conexão com os eventos do lobby. Não será feita nova tentativa de reconexão.");
                break;
        }
    }
}
