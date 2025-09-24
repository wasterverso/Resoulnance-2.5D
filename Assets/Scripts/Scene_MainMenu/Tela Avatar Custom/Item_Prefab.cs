using Resoulnance.AvatarCustomization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaAvatarCustom
{
    public class Item_Prefab : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] Button button;
        [SerializeField] Image item_img;
        [SerializeField] Image fundo_img;

        [Header("Cabelo e rosto")]
        [SerializeField] Sprite cabecaFundo;

        [Header("Roupas")]
        [SerializeField] Sprite roupaFundo;

        public void ReceberItem(Item_AvatarCustom item, AvatarTipoItem tipo, Action<Item_AvatarCustom, AvatarTipoItem> onSelecionado)
        {
            item_img.sprite = item.padrao_Sprite;

            if (tipo == AvatarTipoItem.Roupa)
            {
                fundo_img.sprite = roupaFundo;
            }

            if (tipo == AvatarTipoItem.Cabelo || tipo == AvatarTipoItem.Rosto || tipo == AvatarTipoItem.Acessorio)
            {
                fundo_img.sprite = cabecaFundo;
            }

            if (tipo == AvatarTipoItem.Pes || tipo == AvatarTipoItem.Corpo)
            {
                fundo_img.enabled = false;
            }

            button.onClick.AddListener(() => onSelecionado?.Invoke(item, tipo));
        }
    }
}

