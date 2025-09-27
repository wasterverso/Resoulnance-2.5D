using Resoulnance.Scene_Login.Start;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Login.Controles
{
    public class NicknameLogin : MonoBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] LoginController loginController;

        [Header("Login Nickname")]
        [SerializeField] InputField nick_input;
        [SerializeField] GameObject nickPainel;

        public async void IniciarPainenlNick()
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "Nickname" }, new LoadOptions(new PublicReadAccessClassOptions()));

            if (playerData.ContainsKey("Nickname"))
            {
                var nickname = playerData["Nickname"].Value.GetAs<string>();
                Debug.Log($"Seu nickname: {nickname}");

                Finalizar();
            }
            else
            {
                nickPainel.SetActive(true);
            }
        }

        public async void SalvarNickname()
        {
            string nicknameTxt = nick_input.text.Trim().Replace(" ", "");

            if (!VerificarNickValido(nicknameTxt))
            {
                Debug.LogError("Nickname inválido. Deve ter entre 5 e 15 caracteres e conter apenas letras, números, '.', '_', '-', '@'.");
                loginController.avisoPrincipal_txt.text = "Nickname inválido. Use entre 5 e 15 caracteres: letras, números, '.', '_', '-', '@'.";
                return;
            }

            loginController.avisoPrincipal_txt.text = "Carregando...";
            nickPainel.SetActive(false);

            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(nicknameTxt);
                Debug.Log("Nome atualizado com sucesso no Authentication Service!");

                string playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
                var playerData = new Dictionary<string, object> { { "Nickname", playerName } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData, new SaveOptions(new PublicWriteAccessClassOptions()));
                Debug.Log($"Nickname salvo: " + playerName);

                loginController.ProximaEtapa();
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao salvar o nickname: {e.Message}");
                loginController.avisoPrincipal_txt.text = $"Erro ao salvar o nickname. Por favor, tente novamente. Erro: {e.Message}";
                nickPainel.SetActive(true);
            }
        }

        private bool VerificarNickValido(string nickname)
        {
            // Verifica se o comprimento está entre 5 e 15 caracteres
            if (nickname.Length < 5 || nickname.Length > 15)
                return false;

            // Regex para permitir apenas letras, números, '.', '_', '-', '@'
            string pattern = @"^[a-zA-Z0-9._\-@]+$";
            return Regex.IsMatch(nickname, pattern);
        }

        void Finalizar()
        {
            loginController.ProximaEtapa();
        }
    }
}

