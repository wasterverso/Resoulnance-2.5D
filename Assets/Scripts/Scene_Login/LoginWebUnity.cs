using Resoulnance.Scene_Login.Controles;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

namespace Resoulnance.Scene_Login.Authentication
{
    public class LoginWebUnity : MonoBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] AuthLogin authLogin;

        [Header("Refs Script")]
        [SerializeField] GameObject vinculoPainel;

        public async void LoginNavegador()
        {
            PlayerAccountService.Instance.SignedIn += FezLoginPelaWeb;

            if (PlayerAccountService.Instance.IsSignedIn)
            {
                PlayerAccountService.Instance.SignOut();
            }

            await PlayerAccountService.Instance.StartSignInAsync();
        }

        private async void FezLoginPelaWeb()
        {
            try
            {
                var accessToken = PlayerAccountService.Instance.AccessToken;
                await SignInWithUnityAsync(accessToken);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private async Task SignInWithUnityAsync(string accessToken)
        {
            try
            {
                //await AuthenticationService.Instance.SignInWithUnityAsync(accessToken); // Entrar com Unity

                await AuthenticationService.Instance.LinkWithUnityAsync(accessToken); // Vincular conta anônima com conta Unity

                Debug.Log("Conta anônima vinculada com Unity com sucesso");

                authLogin.FinalizarAuthenticacao();
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                Debug.LogError("Este usuário já está vinculado a outra conta. Faça login em vez disso.");
                vinculoPainel.SetActive(true);
            }
            catch (AuthenticationException ex)
            {
                // Comparar o código de erro com AuthenticationErrorCodes
                // Notificar o jogador com a mensagem de erro apropriada
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Comparar o código de erro com CommonErrorCodes
                // Notificar o jogador com a mensagem de erro apropriada
                Debug.LogException(ex);
            }

            PlayerAccountService.Instance.SignedIn -= FezLoginPelaWeb;
        }
    }
}

