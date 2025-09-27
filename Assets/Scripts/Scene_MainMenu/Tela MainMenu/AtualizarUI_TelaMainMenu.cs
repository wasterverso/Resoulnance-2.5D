using Resoulnance.AvatarCustomization;
using Resoulnance.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaMainMenu
{
    public class AtualizarUI_TelaMainMenu : MonoBehaviour
    {
        [SerializeField] AtribuirDados_AvatarCustom[] avatares = new AtribuirDados_AvatarCustom[0];
        [SerializeField] Text[] nicknames = new Text[0];
        [SerializeField] Text[] nickAuth = new Text[0];

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

        public void AtualizarInfos()
        {
            string nickname = PlayerConfigData.Instance.Nickname;
            string nickAuthenti = PlayerConfigData.Instance.NicknameAuth;

            foreach (var nick in nicknames)
            {
                nick.text = nickname;
            }

            foreach (var nick in nickAuth)
            {
                nick.text = nickAuthenti;
            }
        }
    }
}

