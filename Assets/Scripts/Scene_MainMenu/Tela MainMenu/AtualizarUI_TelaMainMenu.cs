using Resoulnance.AvatarCustomization;
using UnityEngine;

namespace Resoulnance.Telas.TelaMainMenu
{
    public class AtualizarUI_TelaMainMenu : MonoBehaviour
    {
        [SerializeField] AtribuirDados_AvatarCustom[] avatares = new AtribuirDados_AvatarCustom[0];

        private void Start()
        {
            ListAvatarCustom.Instance.OnAvatarCustomAtualizado += AtualizarAvatarCustom;
        }
        private void OnDestroy()
        {
            ListAvatarCustom.Instance.OnAvatarCustomAtualizado -= AtualizarAvatarCustom;
        }

        void AtualizarAvatarCustom(AvatarCustom avatCustom)
        {
            foreach (var avat in avatares)
            {
                avat.AtribuirDados(avatCustom);
            }
        }
    }
}

