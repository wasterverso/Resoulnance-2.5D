using Resoulnance.Scene_Login.Start;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Resoulnance.Scene_Login.Controles
{
    public class TermosCondicoes : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] LoginController loginController;
        [SerializeField] NicknameLogin nickLogin;

        [Header("Links")]
        [SerializeField] Links_Data linksData;

        [Header("Canvas")]
        [SerializeField] GameObject termosPainel;

        public async void IniciarTermosCondicoes()
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "TermosCondicoes" });

            if (playerData.ContainsKey("TermosCondicoes"))
            {
                FinalizarContinuar();
            }
            else
            {
                loginController.avisoPrincipal_txt.gameObject.SetActive(false);
                termosPainel.SetActive(true);
            }
        }

        public void CarregarPrivacidade()
        {
            Application.OpenURL(linksData.polPrivacidade);
        }

        public void CarregarSeguranca()
        {
            Application.OpenURL(linksData.polSeguranca);
        }

        public void CarregarServico()
        {
            Application.OpenURL(linksData.polServico);
        }

        public async void AceitarTermos()
        {
            var playerData = new Dictionary<string, object> { { "TermosCondicoes", "termosAceitos" } };

            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);

                FinalizarContinuar();
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao salvar cartas: {e.Message}");
            }
        }

        void FinalizarContinuar()
        {
            termosPainel.SetActive(false);
            loginController.avisoPrincipal_txt.gameObject.SetActive(true);
            loginController.ProximaEtapa();
        }

        public async void Recusar()
        {
            await AuthenticationService.Instance.DeleteAccountAsync();

            Application.Quit();
        }
    }
}
