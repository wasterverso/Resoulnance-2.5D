using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Cartas
{
    public class AtribuirCartas : MonoBehaviour
    {
        [SerializeField] Image fundo_img;
        [SerializeField] Image sprite_img;
        [SerializeField] Image icon_img;

        public void ReceberCarta(Carta carta)
        {
            icon_img.sprite =  ListaCartas.Instance.iconsData.iconAfinidade.Find(c => c.Afinidade == carta.tipo).SpriteAfinidade;
            fundo_img.sprite = ListaCartas.Instance.iconsData.iconAfinidade.Find(c => c.Afinidade == carta.tipo).background;
            sprite_img.sprite = carta.splashSprite;
        }
    }
}

