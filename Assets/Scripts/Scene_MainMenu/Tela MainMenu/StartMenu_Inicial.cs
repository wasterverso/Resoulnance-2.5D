using PlayFlow;
using Resoulnance.AvatarCustomization;
using Resoulnance.Player;
using Resoulnance.Telas.TelaLobby;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Resoulnance.Telas.TelaMainMenu
{
    public class StartMenu_Inicial : MonoBehaviour
    {
        string TT = "<color=blue>[Start Menu inicial]</color>";

        [Header("Canvas/Paineis")]
        [SerializeField] GameObject menuPrincipalCanvas;
        [SerializeField] GameObject carregandoPainel;

        private async void Awake()
        {
            carregandoPainel.SetActive(true);

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                try
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log($"{TT} Unity UGS Services inicializados.");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.Log($"{TT} Unity UGS Services já estão inicializados.");
            }

            ConfigurarEventosDeLogin();

            bool logou = await VerificarLoginAuth();

            if (logou)
                CarregarGameData();
        }

        void ConfigurarEventosDeLogin()
        {
            AuthenticationService.Instance.SignedIn += () => 
            {
                //Debug.Log($"PlayerAuthID: {AuthenticationService.Instance.PlayerId}");
                //Debug.Log($"Token de acesso: {AuthenticationService.Instance.AccessToken}");
            };

            AuthenticationService.Instance.SignInFailed += (err) => {
                Debug.LogError($"{TT} Falha ao fazer login: {err}");
            };

            AuthenticationService.Instance.SignedOut += () => {
                Debug.Log($"{TT} Jogador desconectado.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log($"{TT} A sessão do jogador não pôde ser renovada e expirou.");
            };
        }

        private async Task<bool> VerificarLoginAuth()
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log($"{TT} Já está logado. PlayerID: {AuthenticationService.Instance.PlayerId}");
                return true;
            }

            if (AuthenticationService.Instance.SessionTokenExists)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"{TT} Login automático bem-sucedido. PlayerID: {AuthenticationService.Instance.PlayerId}");
                    return true;
                }
                catch (AuthenticationException ex)
                {
                    Debug.LogError($"{TT} Erro no login automático: {ex.Message}");
                    //NotificarPainel.Instance.AtivarErro(false, 0, $"{ex.Message}");
                    return false;
                }
            }

            Debug.LogError($"{TT} Nenhum login anterior encontrado.");
            //NotificarPainel.Instance.AtivarErro(false, 0, "Nenhum login anterior encontrado.");
            return false;
        }

        private async void CarregarGameData()
        {
            PlayerConfigData playerConfig = PlayerConfigData.Instance;

            if (string.IsNullOrEmpty(playerConfig.idAuth))
            {
                playerConfig.idAuth = AuthenticationService.Instance.PlayerId;
            }

            if (string.IsNullOrEmpty(playerConfig.NicknameAuth))
            {
                playerConfig.NicknameAuth = await AuthenticationService.Instance.GetPlayerNameAsync();
            }

            if (string.IsNullOrEmpty(playerConfig.Nickname))
            {
                playerConfig.Nickname = playerConfig.NicknameAuth.Split('#')[0];
            }

            PlayFlowLobbyManagerV2.Instance.GetComponentInChildren<LobbyManager>().IniciarLobbyManager();

            var initializationTasks = new List<Task>()
            {
                ListAvatarCustom.Instance.CarregarAvatarCloud(),
            };

            await Task.WhenAll(initializationTasks);

            menuPrincipalCanvas.SetActive(true);

            carregandoPainel.SetActive(false);
        }
    }
}

