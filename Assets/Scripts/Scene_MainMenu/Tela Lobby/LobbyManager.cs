using PlayFlow;
using PurrNet;
using Resoulnance.AvatarCustomization;
using Resoulnance.Player;
using Resoulnance.Telas.TelaMainMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Resoulnance.Telas.TelaLobby
{
    public class LobbyManager : MonoBehaviour
    {
        string TT = "[LobbyManager]";

        [Header("PlayFlowManager")]
        [SerializeField] PlayFlowLobbyManagerV2 pfLobbyManager;

        [Header("Refs")]
        [SerializeField] LobbyEvents lobbyEvents;

        public void IniciarLobbyManager()
        {
            try
            {
                string playerIdAuth = PlayerConfigData.Instance.idAuth;
                pfLobbyManager.Initialize(playerIdAuth, LobbyManagerConfigurado);
            }
            catch (Exception ex)
            {
                string erro = $" {TT} Erro inesperado durante a inicialização: {ex.Message}\n{ex.StackTrace}";
                Debug.LogError(erro);
                NotificarPainel.Instance.AtivarErro(true, 0, erro);
            }
        }

        void LobbyManagerConfigurado()
        {
            lobbyEvents.InscreverNosEventos();
        }

        public async Task<string> CriarLobby(TiposDeSalas gameMode, string nomeLobby, int quantPlayers, bool isPrivate)
        {
            if (pfLobbyManager.IsInLobby)
            {
                string erro = $"{TT} Já está em um lobby. Saia antes de tentar entrar em outro.";
                Debug.LogError(erro);
                NotificarPainel.Instance.AtivarErro(true, 6, erro);
                return "InLobby";
            }

            var custom_Settings = new Dictionary<string, object>
            {
                ["gameMode"] = gameMode.ToString(),
                ["OnStart"] = false,
                //["matchType"] = "ranked"
            };

            // encapsula a espera do callback em Task
            var tcs = new TaskCompletionSource<string>();

            pfLobbyManager.CreateLobby(
                name: nomeLobby,
                maxPlayers: quantPlayers,
                isPrivate: isPrivate,
                allowLateJoin: false,
                region: "south-america-brazil",
                customSettings: custom_Settings,
                onSuccess: lobby =>
                {
                    //Debug.Log($"{TT} Lobby criado com sucesso: {lobby.name} (Host: {pfLobbyManager.IsHost})");

                    AtualizarPlayer(-1, true);

                    if (lobby.isPrivate && !string.IsNullOrEmpty(lobby.inviteCode))
                        tcs.TrySetResult(lobby.inviteCode);
                    else
                        tcs.TrySetResult("sucess");
                },
                onError: error =>
                {
                    Debug.LogError($"{TT} Falha ao criar o lobby: {error}");
                    NotificarPainel.Instance.AtivarErro(true, 3, $"Falha ao criar o lobby: {error}");
                    tcs.TrySetResult("error");
                }
            );

            // aqui o código só continua quando o callback chamar tcs.SetResult
            return await tcs.Task;
        }

        public async Task<string> EntrarNoLobbyPeloCodigo(string codigo)
        {
            var tcs = new TaskCompletionSource<string>();

            pfLobbyManager.JoinLobbyByCode(codigo,
                    onSuccess: (lobby) =>
                    {
                        Debug.Log($"{TT} Entrou no lobby com sucesso: {lobby.name}");
                        AtualizarPlayer(-1, true);
                        tcs.TrySetResult("success");
                    },
                    onError: (error) =>
                    {
                        string feed = $"{TT} Falha ao entrar no lobby: {error}";
                        Debug.LogError(feed);
                        if (error.Contains("404 Not Found"))
                        {
                            tcs.TrySetResult("notFound");
                        }
                        else
                        {
                            tcs.TrySetResult("error");
                        }
                    }
            );

            return await tcs.Task;
        }

        public void AtualizarPlayer(int posicao, bool pronto)
        {
            if (pfLobbyManager.CurrentLobby == null)
            {
                Debug.LogWarning($"{TT} Não esta em um Lobby!");
                return;
            }

            AvatarCustomSerializable playerCustomData = ListAvatarCustom.Instance.SerializarAvatarCustom();
            string customDataJson = JsonUtility.ToJson(playerCustomData);

            var playerConfig = PlayerConfigData.Instance; 

            var testState = new Dictionary<string, object>
            {
                ["isReady"] = pronto,
                ["avatar"] = customDataJson,
                ["posicao"] = posicao,
                ["nickname"] = playerConfig.Nickname,
                ["idAuth"] = playerConfig.idAuth,
            };

            pfLobbyManager.UpdatePlayerState(testState,
                onSuccess: (lobby) => {
                    //Debug.Log($"{TT} Player atualizado com sucesso");
                },
                onError: (error) => {
                    Debug.LogError($"{TT} Falha ao atualizar player: {error}");
                }
            );
        }

        public void AtualizarOutroPlayer(string targetPlayerId, int posicao)
        {
            if (!pfLobbyManager.IsInLobby)
            {
                string erro = $"{TT} Voce nao esta em um lobby";
                Debug.LogError(erro);
            }

            if (!pfLobbyManager.IsHost)
            {
                Debug.LogError("Apenas o host pode atualizar outros jogadores!");
                return;
            }

            var currentLobby = pfLobbyManager.CurrentLobby;
            if (!currentLobby.players.Contains(targetPlayerId))
            {
                Debug.LogError($"Player target {targetPlayerId} nao esta no lobby lobby!");
                return;
            }

            var targetPlayerState = new Dictionary<string, object>
            {
                ["posicao"] = posicao,
            };

            PlayFlowLobbyManagerV2.Instance.UpdateStateForPlayer(targetPlayerId, targetPlayerState,
                onSuccess: (lobby) => {
                    //Debug.Log($"Host atualizu com sucesso o player: {targetPlayerId}");

                    if (lobby.lobbyStateRealTime.TryGetValue(targetPlayerId, out var updatedState))
                    {
                        //Debug.Log($"Player alvo atualizado: {JsonConvert.SerializeObject(updatedState)}");
                    }
                },
                onError: (error) => {
                    Debug.LogError($"Falha ao atualizar o player {targetPlayerId}. state: {error}");
                }
            );
        }

        public async Task<bool> SairDoLobby()
        {
            if (pfLobbyManager.CurrentLobby == null)
            {
                Debug.LogWarning($"{TT} Não está em um lobby!");
                return false;
            }

            var tcs = new TaskCompletionSource<bool>();

            try
            {
                pfLobbyManager.LeaveLobby(
                    onSuccess: () => {
                        //Debug.Log($"{TT} Saiu do lobby!");
                        tcs.TrySetResult(true);
                    },
                    onError: (error) => {
                        Debug.LogError($"{TT} Falha ao sair do lobby: {error}");
                        tcs.TrySetResult(false);
                    }
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro ao sair do lobby: {ex.Message}");
                tcs.TrySetResult(false);
            }

            return await tcs.Task;
        }

        public async Task<bool> IniciarPartida()
        {
            if (pfLobbyManager.CurrentLobby == null)
            {
                Debug.LogWarning($"{TT} Não é possível iniciar a partida: você precisa estar em um lobby");
                return false;
            }

            if (!pfLobbyManager.IsHost)
            {
                var host = pfLobbyManager.CurrentLobby.host;
                var meuId = pfLobbyManager.PlayerId;
                Debug.LogWarning($"{TT} Não é possível iniciar a partida: você não é o host. (Host: {host} | Você: {meuId})");
                return false;
            }

            Debug.Log($"{TT} Iniciando partida...");

            var tcs = new TaskCompletionSource<bool>();

            try
            {
                pfLobbyManager.StartMatch(
                    onSuccess: (lobby) =>
                    {
                        string feed = $"{TT} Partida iniciada com sucesso. Status: {lobby.status}";
                        Debug.Log(feed);
                        tcs.TrySetResult(true);
                    },
                    onError: (error) =>
                    {
                        string feed = $"{TT} Falha ao iniciar a partida: {error}";
                        Debug.LogError(feed);
                        tcs.TrySetResult(false);
                    }
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro ao iniciar partida: {ex.Message}");
                tcs.TrySetResult(false);
            }

            return await tcs.Task;
        }

        public async Task<bool> StartMatchmaking()
        {
            if (!pfLobbyManager.IsInLobby || !pfLobbyManager.IsHost)
            {
                Debug.LogWarning($"{TT} É preciso ser o host de um lobby para iniciar o matchmaking!");
                return false;
            }

            var tcs = new TaskCompletionSource<bool>();

            try
            {
                pfLobbyManager.FindMatch("1v1",
                    onSuccess: (lobby) =>
                    {
                        Debug.Log($"{TT} Matchmaking iniciado! Status: {lobby.status}. Modo de matchmaking: {lobby.matchmakingMode}");
                        Debug.Log($"{TT} ID do ticket: {lobby.matchmakingTicketId}");
                        tcs.TrySetResult(true);
                    },
                    onError: (error) =>
                    {
                        Debug.LogError($"{TT} Falha ao iniciar o matchmaking: {error}");
                        tcs.TrySetResult(false);
                    }
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro ao iniciar matchmaking: {ex.Message}");
                tcs.TrySetResult(false);
            }

            return await tcs.Task;
        }

        public void AtualizarConfiguracoesDoLobby(bool iniciou)
        {
            var custom_Settings = new Dictionary<string, object>
            {
                ["OnStart"] = iniciou,
            };

            pfLobbyManager.UpdateLobbySettings(custom_Settings,
                onSuccess: lobby =>
                {
                    Debug.Log($"{TT} Configuracoes do lobby atualizadas com sucesso!");
                },
                onError: error =>
                {
                    Debug.LogError($"{TT} Falha ao atualizar cofiguracoes do lobby: {error}");
                });
        }
    }
}

