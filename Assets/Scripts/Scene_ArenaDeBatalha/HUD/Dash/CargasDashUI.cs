using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.UI
{
    public class CargasDashUI : MonoBehaviour
    {
        [Header("Cargas UI")]
        [SerializeField] Color corPadrao;
        [SerializeField] Image[] cargas_img = new Image[6];

        [Header("Iniciar Anim")]
        [SerializeField] float duracao = 1f;
        [SerializeField] Vector3 escalaFinal = new Vector3(1.2f, 1.2f, 1f);

        [Header("Recarregar Anim")]
        [SerializeField] Color inicialColor;
        [SerializeField] Color finalColor;

        [Header("Fundo UI")]
        [SerializeField] Image fundo_img;
        [SerializeField] Sprite fundoComCargas;
        [SerializeField] Sprite fundoSemCargas;

        Coroutine animAtual;

        public void ChamarDash(int cargas)
        {
            int cargaObj = cargas + 1;

            if (animAtual != null)
                StopCoroutine(animAtual);

            // Ativar as primeiras 'cargas' imagens
            for (int i = 0; i < cargas_img.Length; i++)
            {
                cargas_img[i].gameObject.SetActive(i < cargaObj);
                cargas_img[i].fillAmount = 1f;
            }

            // Procura a última imagem ativa
            for (int i = cargas_img.Length - 1; i >= 0; i--)
            {
                if (cargas_img[i].gameObject.activeSelf)
                {
                    animAtual = StartCoroutine(AnimarCarga(cargas_img[i], cargas));
                    break;
                }
            }            
        }

        public void RecuperarCarga(int cargas)
        {
            if (cargas == 1)
            {
                fundo_img.sprite = fundoComCargas;
            }

            if (animAtual != null)
                StopCoroutine(animAtual);

            // Ativar as primeiras 'cargas' imagens
            for (int i = 0; i < cargas_img.Length; i++)
            {
                cargas_img[i].gameObject.SetActive(i < cargas);
                cargas_img[i].fillAmount = 1f;
            }

            // Se houver ao menos uma carga, animar o último ativado
            if (cargas > 0 && cargas <= cargas_img.Length)
            {
                Image imagemParaAnimar = cargas_img[cargas - 1];
                animAtual = StartCoroutine(AnimarRecuperacaoDeCarga(imagemParaAnimar));
            }
        }

        private IEnumerator AnimarCarga(Image imagem, int cargas)
        {
            imagem.fillAmount = 1;

            imagem.color = Color.white;
            imagem.transform.localScale = Vector3.one;

            float tempo = 0f;
            Vector3 escalaInicial = Vector3.one;

            Color corInicial = Color.white;
            Color corFinal = new Color(1f, 1f, 1f, 0f);

            while (tempo < duracao)
            {
                tempo += Time.deltaTime;
                float t = tempo / duracao;

                //Tamanho da imagem
                imagem.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);

                //Opacidade
                imagem.color = Color.Lerp(corInicial, corFinal, t);

                yield return null;
            }

            imagem.gameObject.SetActive(false);
            animAtual = null;

            imagem.color = corPadrao;
            imagem.transform.localScale = Vector3.one;

            if (cargas == 0)
            {
                fundo_img.sprite = fundoSemCargas;
                animAtual = StartCoroutine(OscilarOpacidade(fundo_img));
            }
        }

        private IEnumerator AnimarRecuperacaoDeCarga(Image imagem)
        {
            fundo_img.color = Color.white;

            imagem.fillAmount = 0f;
            imagem.transform.localScale = Vector3.one;

            // Começa com vermelho transparente
            Color corInicial = inicialColor;   // Vermelho sem alpha
            Color corFinal = finalColor;   // Amarelo com alpha 1

            imagem.color = corInicial;

            float tempo = 0f;

            while (tempo < duracao)
            {
                tempo += Time.deltaTime;
                float t = tempo / duracao;

                imagem.fillAmount = Mathf.Lerp(0f, 1f, t);
                imagem.color = Color.Lerp(corInicial, corFinal, t);

                yield return null;
            }

            imagem.fillAmount = 1f;
            imagem.color = corPadrao;
            animAtual = null;
        }

        private IEnumerator OscilarOpacidade(Image imagem)
        {
            float tempo = 0f;
            float duracao = 0.7f;

            Color corBase = imagem.color;

            while (true)
            {
                tempo += Time.deltaTime;
                float t = Mathf.PingPong(tempo, duracao) / duracao;

                // Opacidade de 0.5 até 1
                float alpha = Mathf.Lerp(0.5f, 1f, t);
                imagem.color = new Color(corBase.r, corBase.g, corBase.b, alpha);

                yield return null;
            }
        }

    }
}

