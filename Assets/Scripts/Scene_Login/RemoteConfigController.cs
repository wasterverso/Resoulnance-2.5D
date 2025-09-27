using Resoulnance.Scene_Login.Start;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Resoulnance.Scene_Login.Controles
{
    public class RemoteConfigController : MonoBehaviour
    {
        public struct User_Atributos { }

        public struct App_Atributos { }

        RemoteConfigService ConfigManager;

        [Header("Versao")]
        [SerializeField] LoginController loginController;

        [Header("Versao")]
        [SerializeField] int versao;

        [Header("UI")]
        [SerializeField] GameObject versaoPainel;

        public async void StartarRemoteConfig()
        {
            string version = Application.version;
            string versionWithoutDots = version.Replace(".", "");
            int versionNumber;
            if (int.TryParse(versionWithoutDots, out versionNumber))
            {
                versao = versionNumber;
            }

            ConfigManager = RemoteConfigService.Instance;

            try
            {
                await RemoteConfigService.Instance.FetchConfigsAsync<User_Atributos, App_Atributos>(new User_Atributos(), new App_Atributos());

                if (ConfigManager.appConfig.HasKey("AppVersion"))
                {
                    int novaVersao = ConfigManager.appConfig.GetInt("AppVersion");

                    if (novaVersao != versao)
                    {
                        Debug.Log("RemoteConfig: Tem nova versao!");
                        loginController.avisoPrincipal_txt.text = "Tem nova versao!";
                        versaoPainel.SetActive(true);
                    }
                    else
                    {
                        loginController.avisoPrincipal_txt.text = "Versão Atualizada!";
                        loginController.ProximaEtapa();
                    }
                }
                else
                {
                    Debug.LogWarning("RemoteConfig: Chave 'AppVersion' não encontrada.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"RemoteConfig: Erro ao buscar as configurações: {e.Message}");
            }
        }

        public void AtualizarJogo()
        {
            string packageName = "com.Resoulverse.Resoulnance";
            string playStoreUrl = $"market://details?id={packageName}";
            string webUrl = $"https://play.google.com/store/apps/details?id={packageName}";

            try
            {
                Application.OpenURL(playStoreUrl);
            }
            catch
            {
                Application.OpenURL(webUrl);
            }
        }
    }
}
