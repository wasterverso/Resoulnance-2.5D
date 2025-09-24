using Resoulnance.Cartas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaCartas
{
    public class CartaCodicePrefab : MonoBehaviour
    {
        [SerializeField] Icons_Data iconsData;
        [SerializeField] Image fundo_img;
        [SerializeField] Image icon_img;
        [SerializeField] Image carta_img;
        [SerializeField] Image frente_img;
        [SerializeField] Text aviso_txt;

        Carta carta;
        CodiceCartasConfig codiceConfig;

        public void ConfigurarCarta(Carta card, bool tem, CodiceCartasConfig codiceScript)
        {
            carta = card;
            codiceConfig = codiceScript;

            IconAfinidade afinidade = iconsData.iconAfinidade.Find(c => c.Afinidade == carta.tipo);

            carta_img.sprite = carta.splashSprite;
            icon_img.sprite = afinidade.SpriteAfinidade;
            fundo_img.sprite = afinidade.background;

            if (tem)
            {
                frente_img.gameObject.SetActive(false);
                aviso_txt.gameObject.SetActive(false);
            }
        }

        public void MostrarInfoCarta()
        {
            codiceConfig.MostrarInfoCarta(carta);
        }
    }
}

