using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaMainMenu
{
    public class NotificarPainel : MonoBehaviour
    {
        public static NotificarPainel Instance { get; private set; }

        [Header("Painel Error")]
        [SerializeField] GameObject painel_error;
        [SerializeField] Text conteudo_txt;
        [SerializeField] Text subTexto_txt;
        [SerializeField] GameObject fechar_btn;

        [Header("Painel Loading")]
        [SerializeField] GameObject carregandoPainel;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void AtivarErro(bool podeFechar, int valorErro, string erro = "")
        {
            //AudioController.Instance.PlaySfx(errorPainel_Audio);

            painel_error.SetActive(true);
            fechar_btn.SetActive(podeFechar);

            subTexto_txt.gameObject.SetActive(!string.IsNullOrEmpty(erro));
            if (!string.IsNullOrEmpty(erro))
            {
                subTexto_txt.text = erro;
            }

            switch (valorErro)
            {
                case 0:
                    conteudo_txt.text = "Erro Desconhecido.";
                    break;
                case 1:
                    conteudo_txt.text = "Erro no login automático.";
                    break;
                case 2:
                    conteudo_txt.text = "Cartas insuficientes. (Obrigatório 3 cartas no 'Deck Universal 1' para jogar).";
                    break;
                case 3:
                    conteudo_txt.text = "Erro na sala:";
                    break;
                case 4:
                    conteudo_txt.text = "Sala não encontrada!";
                    break;
                case 5:
                    conteudo_txt.text = "Sala está cheia!";
                    break;
                case 6:
                    conteudo_txt.text = "Já está na sala.";
                    break;
                case 7:
                    conteudo_txt.text = "Jogador não encontrado.";
                    break;
                case 8:
                    conteudo_txt.text = "Jogadores não estão prontos.";
                    break;
                case 9:
                    conteudo_txt.text = "Não é possível iniciar: um ou ambos os times não possuem jogadores.";
                    break;
            }

        }

        public void AtivarCarregandoPainel(bool ativar)
        {
            carregandoPainel.SetActive(ativar);
        }
    }
}

