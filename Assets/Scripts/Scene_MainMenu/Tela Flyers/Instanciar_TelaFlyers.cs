using Resoulnance.Flyers;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaFlyers
{
    public class Instanciar_TelaFlyers : MonoBehaviour
    {
        [Header("Controle De dados")]
        [SerializeField] Flyer_Data flyerData;
        [SerializeField] Icons_Data iconsData;

        [Header("Refs Script")]
        [SerializeField] Info_TelaFlyers infoTelaFlyers;

        [Header("Refs UI")]
        [SerializeField] Transform gridLayout;

        [Header("Refs Obj")]
        [SerializeField] GameObject prefabInstanciar;

        [Header("Painel")]
        [SerializeField] GameObject listFlyersPainel;
        [SerializeField] GameObject infoPainel;

        [Header("Botoes Filtros")]
        [SerializeField] Button todosBtn;
        [SerializeField] Button coletorBtn;
        [SerializeField] Button guerreiroBtn;
        [SerializeField] Button suporteBtn;
        [SerializeField] Button tanqueBtn;

        void Start()
        {
            foreach (Transform t in gridLayout)
            {
                Destroy(t.gameObject);
            }

            InstanciarFlyers();

            todosBtn.onClick.AddListener(() => FiltrarPersonagens(Classes.Nenhum));
            coletorBtn.onClick.AddListener(() => FiltrarPersonagens(Classes.Coletor));
            guerreiroBtn.onClick.AddListener(() => FiltrarPersonagens(Classes.Guerreiro));
            suporteBtn.onClick.AddListener(() => FiltrarPersonagens(Classes.Suporte));
            tanqueBtn.onClick.AddListener(() => FiltrarPersonagens(Classes.Tanque));
        }

        public void InstanciarFlyers()
        {
            foreach (Personagem_Data personagem in flyerData.personagens)
            {
                if (personagem.Ativo)
                {
                    GameObject flyerObj = Instantiate(prefabInstanciar, gridLayout);

                    flyerObj.GetComponentInChildren<Text>().text = personagem.nome;

                    Image[] images = flyerObj.GetComponentsInChildren<Image>();
                    if (images.Length > 1)
                    {
                        images[2].sprite = personagem.skins[0].SplashSprite;

                        SpriteClass spriteClass = iconsData.spriteClasses.Find(sc => sc.nome == personagem.classe.ToString());
                        images[4].sprite = spriteClass.sprite;
                    }

                    // Adicione um listener ao botão do flyer
                    Button flyerButton = flyerObj.GetComponent<Button>();
                    if (flyerButton != null)
                    {
                        flyerButton.onClick.AddListener(() => AbrirTelaInformacoes(personagem));
                    }
                }
            }
        }

        public void FiltrarPersonagens(Classes classe)
        {
            foreach (Transform child in gridLayout)
            {
                GameObject flyerObj = child.gameObject;
                Personagem_Data personagem = flyerData.personagens.Find(p => p.nome == flyerObj.GetComponentInChildren<Text>().text);

                if (classe == Classes.Nenhum || personagem.classe == classe)
                {
                    flyerObj.SetActive(true);
                }
                else
                {
                    flyerObj.SetActive(false);
                }
            }
        }

        public void AbrirTelaInformacoes(Personagem_Data personagem)
        {
            listFlyersPainel.SetActive(false);
            infoPainel.SetActive(true);
            infoTelaFlyers.AtualizarInfoPersonagem(personagem);
        }
    }

}
