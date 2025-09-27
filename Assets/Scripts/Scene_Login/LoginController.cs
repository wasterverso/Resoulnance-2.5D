using Resoulnance.Scene_Login.Controles;
using Resoulnance.Sistema;
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
        [SerializeField] AtualizarDadosBuild attDadosBuild;
        [SerializeField] AuthLogin authLogin;
        [SerializeField] NicknameLogin nicknameLogin;

        [Header("Textos")]
        [SerializeField] Text versao_txt;
        public Text avisoPrincipal_txt;

        [Header("Paineis")]
        public GameObject semConexaoPainel;

        int etapaAtual = 0;

        private void Start()
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

            ProximaEtapa();
        }


        public void ProximaEtapa()
        {
            etapaAtual++;

            switch (etapaAtual)
            {
                case 1:
                    FazerVerificaoDeInternet();
                    break;

                case 2:
                    VerificarVersao();
                    break;

                case 3:
                    VerificarAssetsBuild();
                    break;

                case 4:
                    AuthenticarLogin();
                    break;

                case 5:
                    VerificarTermos();
                    break;

                case 6:
                    CarregarNickname();
                    break;

                case 7:
                    CarregarMenu();
                    break;

                default:
                    break;
            }
        }

        async void FazerVerificaoDeInternet()
        {
            avisoPrincipal_txt.text = "Verificando Conexão...";

            bool temNet = await VerificarInternet.IniciarVerificacao();

            if (temNet)
            {
                avisoPrincipal_txt.text = "Conexão estabelecida";
                VerificarVersao();
            }
            else
            {
                avisoPrincipal_txt.gameObject.SetActive(false);
                semConexaoPainel.SetActive(true);
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
            ProximaEtapa();
            //remoteConfigController.StartarRemoteConfig();
        }

        void VerificarAssetsBuild()
        {
            ProximaEtapa();

            //attDadosBuild.ComecarChecagem();
        }

        void AuthenticarLogin()
        {
            authLogin.IniciarVerificacaoDeLogin();
        }

        void VerificarTermos()
        {
            CarregarNickname();
            //termosCondicoes.IniciarTermosCondicoes();
        }

        void CarregarNickname()
        {
            nicknameLogin.IniciarPainenlNick();
        }

        void CarregarMenu()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        public void SairDoJogo()
        {
            Application.Quit();
        }
    }
}

