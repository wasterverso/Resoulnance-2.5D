using Resoulnance.Scene_Login.Controles;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Login.Authentication
{
    public class LoginWebUnity : MonoBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] AuthLogin authLogin;
        [SerializeField] Text erro_txt;

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
                await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
                Debug.Log("Logou com sucesso");

                authLogin.FinalizarAuthenticacao();
                erro_txt.text = "";
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
                erro_txt.text = ex.Message;
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                erro_txt.text = ex.Message;
            }

            PlayerAccountService.Instance.SignedIn -= FezLoginPelaWeb;
        }
    }
}

