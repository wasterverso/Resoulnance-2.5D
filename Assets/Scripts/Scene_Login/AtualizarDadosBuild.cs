using Resoulnance.Scene_Login.Start;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Resoulnance.Scene_Login.Controles
{
    public class AtualizarDadosBuild : MonoBehaviour
    {
        [Header("Ref Script")]
        [SerializeField] LoginController loginController;
        [SerializeField] NicknameLogin startLogin;

        [Header("Paineis")]
        [SerializeField] GameObject verificarDados_Painel;
        [SerializeField] GameObject temDownload_Painel;
        [SerializeField] GameObject baixando_Painel;
        [SerializeField] GameObject erro_Painel;

        [Header("UI")]
        [SerializeField] Text infoDownloa_txt;
        [SerializeField] Text progressValue_txt;
        [SerializeField] Slider progressBar;

        [Header("Download Status")]
        [SerializeField] AssetLabelReference _assetLabel;

        private long downloadSizeBytes; // Armazena o tamanho do download verificado

        public void ComecarChecagem()
        {
            loginController.avisoPrincipal_txt.text = "Verificando atualizações...";
            StartCoroutine(CheckForUpdates());
        }

        IEnumerator CheckForUpdates()
        {
            verificarDados_Painel.SetActive(true);
            loginController.avisoPrincipal_txt.text = "Verificando dados...";

            AsyncOperationHandle<long> downloadSizeHandler = Addressables.GetDownloadSizeAsync(_assetLabel.labelString);
            yield return downloadSizeHandler;

            if (!downloadSizeHandler.IsValid() || downloadSizeHandler.Status != AsyncOperationStatus.Succeeded)
            {
                loginController.avisoPrincipal_txt.text = "Erro ao verificar dados. Handle de downloadSizeHandler é inválido ou a operação falhou.";
                Debug.LogError("Handle de downloadSizeHandler é inválido ou a operação falhou.");

                Addressables.Release(downloadSizeHandler); // Libera memória após o uso
                yield break;
            }

            downloadSizeBytes = downloadSizeHandler.Result;
            float downloadSizeMB = downloadSizeBytes / (1024f * 1024f); // Converte bytes para megabytes

            Addressables.Release(downloadSizeHandler); // Libera memória após o uso

            if (downloadSizeBytes > 0)
            {
                // Exibe o painel perguntando ao jogador se ele quer baixar os dados
                loginController.avisoPrincipal_txt.gameObject.SetActive(false);
                temDownload_Painel.SetActive(true);
                infoDownloa_txt.text = $"Nova atualização detectada! Tamanho: {downloadSizeMB:F2} MB \n Deseja baixar agora?";
            }
            else
            {
                loginController.avisoPrincipal_txt.text = "Nenhum download adicional necessário.";
                FecharFinalizar();
            }
        }
        public void IniciarDownload()
        {
            StartCoroutine(BaixarDados());
        }

        public IEnumerator BaixarDados()
        {
            loginController.avisoPrincipal_txt.gameObject.SetActive(false);
            temDownload_Painel.SetActive(false);
            baixando_Painel.SetActive(true);

            progressBar.gameObject.SetActive(true);
            progressValue_txt.gameObject.SetActive(true);

            float downloadSizeMB = downloadSizeBytes / (1024f * 1024f); // Converte bytes para megabytes
            progressValue_txt.text = $"Limpar cachê e baixar dados adicionais... Tamanho: {downloadSizeMB:F2} MB";

            var handle = Addressables.ClearDependencyCacheAsync(_assetLabel.labelString, false); // Limpa o cache
            yield return handle;

            AsyncOperationHandle downloadHandler = Addressables.DownloadDependenciesAsync(_assetLabel.labelString);
            while (!downloadHandler.IsDone)
            {
                float progress = downloadHandler.PercentComplete * 100;
                progressValue_txt.text = $"Baixando dados adicionais: {progress:F2}% / {downloadSizeMB:F2} MB";
                progressBar.value = downloadHandler.PercentComplete;
                yield return null;
            }

            if (downloadHandler.Status != AsyncOperationStatus.Succeeded)
            {
                baixando_Painel.SetActive(false);
                erro_Painel.gameObject.SetActive(true);
                Addressables.Release(downloadHandler); // Libera memória
                yield break;
            }

            progressBar.value = 1;
            progressValue_txt.gameObject.SetActive(false);
            loginController.avisoPrincipal_txt.gameObject.SetActive(true);
            loginController.avisoPrincipal_txt.text = "Download completo!";
            Addressables.Release(downloadHandler); // Libera memória após o uso

            FecharFinalizar();
        }

        void FecharFinalizar()
        {
            erro_Painel.SetActive(false);
            baixando_Painel.SetActive(false);
            temDownload_Painel.SetActive(false);
            verificarDados_Painel.SetActive(false);
            loginController.avisoPrincipal_txt.gameObject.SetActive(true);

            loginController.ProximaEtapa();
        }
    }
}

