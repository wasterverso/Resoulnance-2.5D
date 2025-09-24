using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaJogar
{
    public class CalculadoraSala : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] Config_TelaJogar telaJogarConfig;
        [SerializeField] Button[] numeros_btn;
        [SerializeField] Text painelNumeros_txt;

        [Header("Codigo")]
        [SerializeField] string numerosApertados = "";

        void Start()
        {
            for (int i = 0; i < numeros_btn.Length; i++)
            {
                int index = i; // Captura o índice atual para uso no lambda
                numeros_btn[i].onClick.AddListener(() => OnButtonClick(index));
            }
        }

        public void OnButtonClick(int index)
        {
            if (index >= 0 && index <= 9)
            {
                if (numerosApertados.Length < 4)
                {
                    numerosApertados += index.ToString();
                    painelNumeros_txt.text = numerosApertados;
                }
            }
            else if (index == 10)
            {
                // Apaga o último número
                if (numerosApertados.Length > 0)
                {
                    numerosApertados = numerosApertados.Substring(0, numerosApertados.Length - 1);
                    painelNumeros_txt.text = numerosApertados;
                }
            }
            else if (index == 11)
            {
                // Limpa o painel
                numerosApertados = "";
                painelNumeros_txt.text = numerosApertados;
            }
        }

        public void ProcurarSala()
        {
            telaJogarConfig.EntrarNoLobbyPorCodigo(numerosApertados);

            numerosApertados = "";
            painelNumeros_txt.text = numerosApertados;
        }
    }
}

