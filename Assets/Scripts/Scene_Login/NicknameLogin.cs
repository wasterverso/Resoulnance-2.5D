using Resoulnance.Scene_Login.Start;
using System;
using System.Text.RegularExpressions;
using Unity.Services.Authentication;
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

        public void IniciarPainenlNick()
        {
            nickPainel.SetActive(true);
        }

        public async void SalvarNickname()
        {
            string nicknameTxt = nick_input.text.Trim().Replace(" ", "");

            if (!VerificarNickValido(nicknameTxt))
            {
                Debug.LogError("Nickname inv�lido. Deve ter entre 5 e 15 caracteres e conter apenas letras, n�meros, '.', '_', '-', '@'.");
                loginController.avisoPrincipal_txt.text = "Nickname inv�lido. Use entre 5 e 15 caracteres: letras, n�meros, '.', '_', '-', '@'.";
                return;
            }

            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(nicknameTxt);
                Debug.Log("Nome atualizado com sucesso no Authentication Service!");

                loginController.CarregarMenu();
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao salvar o nickname: {e.Message}");
            }
        }

        private bool VerificarNickValido(string nickname)
        {
            // Verifica se o comprimento est� entre 5 e 15 caracteres
            if (nickname.Length < 5 || nickname.Length > 15)
                return false;

            // Regex para permitir apenas letras, n�meros, '.', '_', '-', '@'
            string pattern = @"^[a-zA-Z0-9._\-@]+$";
            return Regex.IsMatch(nickname, pattern);
        }
    }
}

