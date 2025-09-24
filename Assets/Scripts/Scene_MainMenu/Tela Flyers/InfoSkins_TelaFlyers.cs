using Resoulnance.Flyers;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaFlyers
{
    public class InfoSkins_TelaFlyers : MonoBehaviour
    {
        [Header("Controle de dados")]
        [SerializeField] Flyer_Data flyerData;

        [Header("Referências")]
        [SerializeField] GameObject skinsPrefab;
        [SerializeField] Transform gridLayout;

        [Header("Flyer")]
        [SerializeField] Transform flyerUi_Trans;
        Personagem_Data flyerEscolhido;
        int amostraSkin = 0;
        int skinEscolhida;

        public void PersonagemSelecionado(Personagem_Data personagem)
        {
            foreach (Transform b in gridLayout)
            {
                Destroy(b.gameObject);
            }

            GameObject botaoInicial = null;
            flyerEscolhido = flyerData.personagens.Find(c => c.id == personagem.id);
            foreach (Skin s in flyerEscolhido.skins)
            {
                GameObject botao = Instantiate(skinsPrefab, gridLayout);
                botao.GetComponent<Button>().onClick.AddListener(() => AtribuirSkin(s.id, botao, false));

                botao.GetComponent<Image>().sprite = s.quadroSprite;

                if (s.id == 0)
                {
                    botaoInicial = botao;
                }
            }

            AtribuirSkin(0, botaoInicial, false);
        }

        public void AtribuirSkin(int idSkin, GameObject btn, bool isToggle)
        {
            skinEscolhida = idSkin;

            foreach (Transform b in gridLayout)
            {
                if (!isToggle)
                {
                    if (b.gameObject == btn)
                    {
                        b.GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        b.GetComponent<Image>().color = Color.grey;
                    }
                }
            }

            foreach (Transform fl in flyerUi_Trans)
            {
                Destroy(fl.gameObject);
            }

            Skin skin = flyerEscolhido.skins[idSkin];
            GameObject flyerObj = Instantiate(skin.prefab_UI, flyerUi_Trans);
            FlyerUI flyierUI = flyerObj.GetComponent<FlyerUI>();
            flyierUI.AtivarSplash(true);
        }

        public void DefinirToogle(int valor)
        {
            amostraSkin = valor;
            AtribuirSkin(skinEscolhida, null, true);
        }
    }
}

