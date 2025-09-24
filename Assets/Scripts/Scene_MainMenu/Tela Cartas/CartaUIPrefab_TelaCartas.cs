using Resoulnance.Cartas;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaCartas
{
    public class CartaUIPrefab_TelaCartas : MonoBehaviour
    {
        [Header("Dados icon")]
        [SerializeField] Icons_Data iconsData;

        [Header("Refs Script")]
        DeckPainel_TelaCartas decksTelaCartas;

        [Header("UI Image")]
        [SerializeField] Image fundo;
        [SerializeField] Image spriteCarta;
        [SerializeField] Image icon;
        [SerializeField] GameObject spritePadrao_img;

        [Header("Carta")]
        Carta carta;

        public void ReceberCartaUI(Carta card, DeckPainel_TelaCartas telaCartas = null)
        {
            decksTelaCartas = telaCartas;

            if (card != null)
            {
                carta = card;

                IconAfinidade afinidade = iconsData.iconAfinidade.Find(c => c.Afinidade == carta.tipo);

                fundo.sprite = afinidade.background;
                icon.sprite = afinidade.SpriteAfinidade;

                spriteCarta.sprite = carta.splashSprite;

                if (spritePadrao_img != null) spritePadrao_img.SetActive(false);
            }
            else
            {
                carta = null;
                if (spritePadrao_img != null) spritePadrao_img.SetActive(true);
            }
        }

        public Carta GetCarta()
        {
            return carta;
        }

        public void SelecionarCarta()
        {
            if (decksTelaCartas != null)
            {
                decksTelaCartas.AbrirDetalhesDaCarta(carta);
            }
        }
    }
}

