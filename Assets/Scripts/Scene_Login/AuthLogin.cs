using Resoulnance.Scene_Login.Start;
using System;
using System.Collections;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Login.Controles
{
    public class AuthLogin : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] LoginController loginController;

        [Header("Referências")]
        [SerializeField] GameObject authPainel;

        [Header("Erro  UI")]
        [SerializeField] Text avisoErro_txt;

        Coroutine avisoCoroutine;

        public async void IniciarVerificacaoDeLogin()
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.SessionTokenExists)
            {
                CriarLogin();
            }
            else
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("Recuperou ultimo acesso!");
                    Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

                    FinalizarAuthenticacao();
                }
                catch (AuthenticationException ex)
                {
                    CriarLogin();
                    Debug.LogException(ex);
                }
                catch (RequestFailedException ex)
                {
                    CriarLogin();
                    Debug.LogException(ex);
                }
            }
        }

        void CriarLogin()
        {
            AuthenticationService.Instance.SignOut();

            loginController.avisoPrincipal_txt.gameObject.SetActive(false);
            authPainel.gameObject.SetActive(true);
        }

        public void FinalizarAuthenticacao()
        {
            authPainel.SetActive(false);

            loginController.avisoPrincipal_txt.gameObject.SetActive(true);
            loginController.avisoPrincipal_txt.text = "Login feito com sucesso";

            loginController.ProximaEtapa();
        }

        public async void EntrarAnonimo()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Acessou como Anonimo!");
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

                FinalizarAuthenticacao();
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
                MostrarAviso(ex.Message);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                MostrarAviso(ex.Message);
            }
        }

        public void MostrarAviso(string aviso)
        {
            if (avisoCoroutine != null)
            {
                StopCoroutine(avisoCoroutine);
            }
            avisoCoroutine = StartCoroutine(FadeAvisoCoroutine(aviso));
        }

        IEnumerator FadeAvisoCoroutine(string aviso)
        {
            avisoErro_txt.gameObject.SetActive(true);
            avisoErro_txt.text = aviso;
            avisoErro_txt.color = Color.white;

            // Mostra o aviso por 5 segundos
            yield return new WaitForSeconds(5f);

            float duration = 1f; // 1 segundo para o fade
            float elapsed = 0f;

            Color originalColor = avisoErro_txt.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                avisoErro_txt.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            avisoErro_txt.gameObject.SetActive(false);
        }
    }
}

