using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.AvatarCustomization
{
    public class AtribuirDados_AvatarCustom : MonoBehaviour
    {
        [Header("Controle de dados")]
        [SerializeField] AvatarCustom_Data customData;

        [Header("Refs Image")]
        [SerializeField] Image acessorio_img;
        [SerializeField] Image cabelo_img;
        [SerializeField] Image rosto_img;
        [SerializeField] Image corpo_img;
        [SerializeField] Image roupa_img;
        [SerializeField] Image pesE_img;
        [SerializeField] Image pesD_img;
        [SerializeField] Image cabeca_img;
        [SerializeField] Image maosE_img;
        [SerializeField] Image maosD_img;

        public void AtribuirDados(AvatarCustom player)
        {
            corpo_img.sprite = (player.corpo != null) ? player.corpo?.frames?[0] : customData.corpo[0]?.frames?[0];
            cabelo_img.sprite = (player.cabelos != null) ? player.cabelos?.frames?[0] : customData.cabelos[0]?.frames?[0];
            acessorio_img.sprite = (player.acessorios != null) ? player.acessorios?.frames?[0] : customData.acessorios[0]?.frames?[0];
            rosto_img.sprite = (player.rostos != null) ? player.rostos?.frames?[0] : customData.rostos[0]?.frames?[0];
            roupa_img.sprite = (player.roupas != null) ? player.roupas?.frames?[0] : customData.roupas[0]?.frames?[0];

            if (player.pes != null && player.pes.frames.Count > 0)
            {
                pesD_img.sprite = player.pes.frames[0];
                pesE_img.sprite = player.pes.frames[0];
            }
            else
            {
                pesD_img.sprite = customData.pes[0].frames[0];
                pesE_img.sprite = customData.pes[0].frames[0];
            }


            // Configurar cores
            ConfigurarCor(cabeca_img, player.corpo?.cor);
            ConfigurarCor(corpo_img, player.corpo?.cor);
            ConfigurarCor(maosD_img, player.corpo?.cor);
            ConfigurarCor(maosE_img, player.corpo?.cor);
            ConfigurarCor(cabelo_img, player.cabelos?.cor);
            ConfigurarCor(pesD_img, player.pes?.cor);
            ConfigurarCor(pesE_img, player.pes?.cor);
        }

        private void ConfigurarCor(Image image, Color? cor)
        {
            Color finalColor = cor ?? Color.white;

            if (image != null)
            {
                image.color = finalColor;
            }
        }

        public void MostrarAvatar(bool mostrar)
        {
            Color cor;
            if (mostrar)
                cor = Color.white;
            else
                cor = Color.black;

            cabeca_img.color = cor;
            corpo_img.color = cor;
            cabelo_img.color = cor;
            acessorio_img.color = cor;
            rosto_img.color = cor;
            roupa_img.color = cor;
            maosD_img.color = cor;
            maosE_img.color = cor;
            pesD_img.color = cor;
            pesE_img.color = cor;
        }
    }
}

