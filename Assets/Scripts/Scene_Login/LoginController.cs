using Resoulnance.Scene_Login.Controles;
using System.Collections;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Resoulnance.Scene_Login.Start
{
    public class LoginController : MonoBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] RemoteConfigController remoteConfigController;
        [SerializeField] AuthLogin authLogin;
        [SerializeField] NicknameLogin nicknameLogin;

        [Header("Textos")]
        [SerializeField] Text versao_txt;
        public Text avisoPrincipal_txt;

        [Header("Paineis")]
        public GameObject semConexaoPainel;

        private void Awake()
        {
            if (Application.platform == RuntimePlatform.LinuxServer)
            {
                SceneManager.LoadScene("Preparacao", LoadSceneMode.Single);
                return;
            }

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // Impede que a tela desligue apenas em dispositivos móveis
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            versao_txt.text = $"v {Application.version}";

            avisoPrincipal_txt.gameObject.SetActive(true);
            avisoPrincipal_txt.text = "Carregando...";

            VerificarInternet();
        }

        void VerificarInternet()
        {
            avisoPrincipal_txt.text = "Verificando Conexão...";

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("Sem conexão Wifi ou 3G, 4G, 5G.");

                avisoPrincipal_txt.gameObject.SetActive(false);
                semConexaoPainel.SetActive(true);

                return;
            }
            else
            {
                StartCoroutine(CheckInternetConnection((isConnected) =>
                {
                    if (isConnected)
                    {
                        avisoPrincipal_txt.text = "Conexão estabelecida";
                        StartGame();
                    }
                    else
                    {
                        Debug.LogError("Sem conexão com a internet.");
                        avisoPrincipal_txt.gameObject.SetActive(false);
                        semConexaoPainel.SetActive(true);
                    }
                }));
            }
        }

        public IEnumerator CheckInternetConnection(System.Action<bool> onResult)
        {
            using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
            {
                request.timeout = 5;

                // Envia a requisição e espera a resposta.
                yield return request.SendWebRequest();

                // Verifica se houve erro na requisição.
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Conexão bem-sucedida.
                    onResult?.Invoke(true);
                }
                else
                {
                    // Falha na conexão.
                    onResult?.Invoke(false);
                }
            }
        }

        async void StartGame()
        {
            await UnityServices.InitializeAsync();

            // opções podem ser passadas no inicializador, por exemplo, se você quiser definir o AnalyticsUserId ou um EnvironmentName, use as linhas abaixo:
            // var options = new InitializationOptions()
            // .SetEnvironmentName("testing")
            // .SetAnalyticsUserId("test-user-id-12345");
            // await UnityServices.InitializeAsync(options);

            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.InvalidSessionToken)
            {
                avisoPrincipal_txt.text = "Token inválido ou expirado. Resetando sessão...";

                AuthenticationService.Instance.SignOut();

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Erro inesperado ao tentar autenticar:");
                Debug.LogException(ex);
            }

            VerificarVersao();
        }

        void VerificarVersao()
        {
            remoteConfigController.StartarRemoteConfig();
        }

        public void VerificarAssetsBuild()
        {
            AuthenticarLogin(); // temp

            //attDadosBuild.ComecarChecagem();
        }

        public void AuthenticarLogin()
        {
            authLogin.IniciarVerificacaoDeLogin();
        }

        public void VerificarTermos()
        {
            CarregarNickname();
            //termosCondicoes.IniciarTermosCondicoes();
        }

        public void CarregarNickname()
        {
            nicknameLogin.IniciarPainenlNick();
        }

        public void CarregarMenu()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        public void SairDoJogo()
        {
            Application.Quit();
        }
    }
}

