using Resoulnance.Scene_Login.Start;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine;

namespace Resoulnance.Scene_Login.Controles
{
    public class AuthLogin : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] LoginController loginController;

        [Header("Referências")]
        [SerializeField] GameObject authPainel;

        public void IniciarVerificacaoDeLogin()
        {
            // AVISO: JA ESTA CONECTADO (FOI CHAMADO O AuthenticationService.Instance.SignInAnonymouslyAsync)
            // verifique o PlayerPrefs pra saber se tem login no cache

            loginController.avisoPrincipal_txt.text = "Verificando Login";
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            bool hasLoggedIn = PlayerPrefs.GetInt("temLogin", 0) == 1;
            if (hasLoggedIn)
            {
                Debug.Log("Recuperou ultimo acesso!");
                FinalizarAuthenticacao();
            }
            else
            {
                CriarLogin();
            }
        }

        void CriarLogin()
        {
            loginController.avisoPrincipal_txt.gameObject.SetActive(false);
            authPainel.gameObject.SetActive(true);
        }

        public void FinalizarAuthenticacao()
        {
            PlayerPrefs.SetInt("temLogin", 1);
            PlayerPrefs.Save();

            authPainel.SetActive(false);
            loginController.avisoPrincipal_txt.gameObject.SetActive(true);
            loginController.avisoPrincipal_txt.text = "Login feito com sucesso";

            loginController.VerificarTermos();
        }

        public async void EntrarComContaVinculada()
        {
            try
            {
                bool temSave = await VerificarContaJogador();

                if (!temSave)
                {
                    await AuthenticationService.Instance.DeleteAccountAsync();
                    Debug.Log($"Contas anonima deletada: {AuthenticationService.Instance.PlayerId} || Tem save? {temSave}");
                }

                // Sai da conta atual (anônima)
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    AuthenticationService.Instance.SignOut();
                }

                // Agora faz login com a conta já vinculada
                var accessToken = PlayerAccountService.Instance.AccessToken;
                await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

                Debug.Log("Entrou com a conta já vinculada.");
                FinalizarAuthenticacao();
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro ao entrar com a conta vinculada:");
                Debug.LogException(ex);
            }
        }

        public void ContinuarComContaAnonima()
        {
            Debug.Log($"Entrou como convidado - PlayerID: {AuthenticationService.Instance.PlayerId}");

            FinalizarAuthenticacao();
        }


        async Task<bool> VerificarContaJogador()
        {
            bool temSave = false;
            var keys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync(
                new ListAllKeysOptions(new PublicReadAccessClassOptions()));
            for (int i = 0; i < keys.Count; i++)
            {
                Debug.Log(keys[i].Key);
                temSave = true;
            }

            return temSave;
        }
    }
}

