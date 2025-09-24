using PurrNet;
using Resoulnance.Flyers;
using Resoulnance.Player;
using Resoulnance.Scene_Preparation.Controles;
using Resoulnance.Scene_Preparation.Prefabs;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Preparation.Visuals
{
    public class PreparacaoVisuals : MonoBehaviour
    {
        [Header("List Data")]
        [SerializeField] Icons_Data iconsData;
        PlayerConfigData playerConfigData;
        ListaFlyers listFlyers;
        ListaCartas listCartas;
        ListaDecks listDecks;

        [Header("Refs Script")]
        [SerializeField] PreparacaoController prepController;

        [Header("UI")]
        [SerializeField] Button pronto_btn;
        [SerializeField] Transform gridPersonagens;
        [SerializeField] Text selecioneFlyerMsg_txt;
        [SerializeField] Transform gridSkins;
        [SerializeField] Transform gridDecks_UniversalPainel_Transform;
        [SerializeField] Transform gridDecks_FlyerPainel_Transform;

        [Header("Prefabs")]
        [SerializeField] GameObject personagensPrefab;
        [SerializeField] GameObject skinsPrefab;
        [SerializeField] GameObject deckPrefab;

        [Header("Paineis")]
        [SerializeField] GameObject escolherPersonagem_painel;
        [SerializeField] GameObject personalizacao_Painel;

        [Header("UI Personagem")]
        [SerializeField] Text nomeFlyer_txt;
        [SerializeField] Image iconClasseFlyer_img;
        [SerializeField] Transform flyerUI_Transform;

        [Header("Botoes")]
        [SerializeField] Button todos_btn;
        [SerializeField] Button guerreiro_btn;
        [SerializeField] Button coletor_btn;
        [SerializeField] Button tank_btn;
        [SerializeField] Button suporte_btn;

        [Header("Decks")]
        [SerializeField] GameObject deckPainel;
        [SerializeField] DeckPrefbs_Preparacao deckPrefb_btn;

        Personagem_Data flyerEscolhido;
        Deck deckEscolhido;
        bool estaPronto = false;
        int idSkinAtual = 0;
        bool painelDeck_FoiAberto = false;

        void Start()
        {
            NetworkManager.main.onLocalPlayerReceivedID += IniciarCena;

            escolherPersonagem_painel.SetActive(true);
            personalizacao_Painel.SetActive(false);
        }

        void IniciarCena(PlayerID playerId)
        {
            listFlyers = ListaFlyers.Instance;
            listCartas = ListaCartas.Instance;
            listDecks = ListaDecks.Instance;
            playerConfigData = PlayerConfigData.Instance;

            foreach (Transform f in gridPersonagens)
                Destroy(f.gameObject);

            foreach (Transform f in flyerUI_Transform)
                Destroy(f.gameObject);

            selecioneFlyerMsg_txt.gameObject.SetActive(true);
            nomeFlyer_txt.text = "";
            iconClasseFlyer_img.enabled = false;

            todos_btn.onClick.AddListener(() => FiltrarGrid(Classes.Nenhum));
            guerreiro_btn.onClick.AddListener(() => FiltrarGrid(Classes.Guerreiro));
            coletor_btn.onClick.AddListener(() => FiltrarGrid(Classes.Coletor));
            tank_btn.onClick.AddListener(() => FiltrarGrid(Classes.Tanque));
            suporte_btn.onClick.AddListener(() => FiltrarGrid(Classes.Suporte));

            InstanciarPlayersGrid();
        }

        void InstanciarPlayersGrid()
        {
            foreach (var fff in listFlyers.flyerData.personagens)
            {
                GameObject personaObj = Instantiate(personagensPrefab, gridPersonagens);

                FlyerGrid_Prefab scriptPrefab = personaObj.GetComponent<FlyerGrid_Prefab>();
                scriptPrefab.personagemData = fff;
                scriptPrefab.spriteFlyer_img.sprite = fff.quadradoSprite;
                scriptPrefab.classe_img.sprite = iconsData.spriteClasses.Find(c => c.classe_ == fff.classe).sprite;
                scriptPrefab.nome_txt.text = fff.nome;

                bool possuiPersonagem = ListaFlyers.Instance.meusPersonagens.Exists(f => f.id == fff.id);

                if (possuiPersonagem)
                {
                    personaObj.GetComponent<Button>().onClick.AddListener(() => SelecionouPersonagem(fff));
                }
                else
                {
                    scriptPrefab.spriteFlyer_img.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }
        }

        void FiltrarGrid(Classes tipo)
        {
            foreach (Transform p in gridPersonagens)
            {
                FlyerGrid_Prefab scriptPrefab = p.GetComponent<FlyerGrid_Prefab>();

                bool deveMostrar = (tipo == Classes.Nenhum || scriptPrefab.personagemData.classe == tipo);
                p.gameObject.SetActive(deveMostrar);
            }
        }

        void SelecionouPersonagem(Personagem_Data flyerClicado)
        {
            selecioneFlyerMsg_txt.gameObject.SetActive(false);
            nomeFlyer_txt.text = flyerClicado.nome;
            iconClasseFlyer_img.enabled = true;
            iconClasseFlyer_img.sprite = iconsData.spriteClasses.Find(c => c.classe_ == flyerClicado.classe).sprite;

            foreach (Transform f in flyerUI_Transform)
                Destroy(f.gameObject);

            var flyerUi = Instantiate(flyerClicado.skins[0].prefab_UI, flyerUI_Transform);

            flyerEscolhido = flyerClicado;

            prepController.SelecionouPersonagem(flyerEscolhido.id, playerConfigData.idAuth);
        }

        public Personagem_Data GetPersonagemEscolhido()
        {
            if (flyerEscolhido)
                return flyerEscolhido;
            else
                return listFlyers.flyerData.personagens[1];
        }

        public void ConfirmarButton()
        {
            if (estaPronto) return;

            estaPronto = true;

            if (flyerEscolhido == null || flyerEscolhido == default)
            {
                flyerEscolhido = listFlyers.meusPersonagens[0];
                SelecionouPersonagem(flyerEscolhido);
            }

            escolherPersonagem_painel.SetActive(false);
            personalizacao_Painel.SetActive(true);

            InstanciarSkins();

            BotaoDeDecks();

            pronto_btn.interactable = false;
        }

        void BotaoDeDecks()
        {
            if (flyerEscolhido.deckFavorito.cartas.Count < 3)
            {
                if (flyerEscolhido.decks[0].cartas.Count == 3)
                {
                    deckEscolhido = flyerEscolhido.decks[0];
                }
                else
                {
                    deckEscolhido = listDecks.meusDecksUniversais[0];
                }
            }
            else
            {
                deckEscolhido = flyerEscolhido.deckFavorito;
            }

            deckPrefb_btn.ReceberDeck(deckEscolhido);
        }

        public void AbrirPainelDeck()
        {
            if (!painelDeck_FoiAberto)
            {
                painelDeck_FoiAberto = true;

                foreach (Transform iten in gridDecks_UniversalPainel_Transform)
                    Destroy(iten.gameObject);

                foreach (Transform iten in gridDecks_FlyerPainel_Transform)
                    Destroy(iten.gameObject);

                foreach (Deck deck in ListaDecks.Instance.meusDecksUniversais)
                {
                    if (deck.cartas.Count != 3) continue;

                    GameObject deckObj = Instantiate(deckPrefab, gridDecks_UniversalPainel_Transform);
                    var deckScript = deckObj.GetComponent<DeckPrefbs_Preparacao>();
                    deckScript.ReceberDeck(SelecionarDeck, true, deck);
                }

                foreach (Deck deck in flyerEscolhido.decks)
                {
                    if (deck.cartas.Count != 3) continue;

                    GameObject deckObj = Instantiate(deckPrefab, gridDecks_UniversalPainel_Transform);
                    var deckScript = deckObj.GetComponent<DeckPrefbs_Preparacao>();
                    deckScript.ReceberDeck(SelecionarDeck, true, deck);
                }
            }

            deckPainel.SetActive(true);
        }

        public void SelecionarDeck(bool universal, Deck deck)
        {
            deckPainel.SetActive(false);

            var flyer = ListaFlyers.Instance.meusPersonagens.FirstOrDefault(c => c.id == flyerEscolhido.id);

            if (flyer == null)
            {
                Debug.LogWarning($"Flyer {flyerEscolhido.id} não encontrado!");
                return;
            }

            deckEscolhido = deck;
            flyer.deckFavorito = deck;

            deckPrefb_btn.ReceberDeck(deckEscolhido);
        }

        public void InstanciarSkins()
        {
            foreach (Transform f in gridSkins) 
                Destroy(f.gameObject);

            GameObject botaoInicial = null;
            foreach (Skin s in flyerEscolhido.skins)
            {
                GameObject botao = Instantiate(skinsPrefab, gridSkins);
                botao.GetComponent<Image>().sprite = s.quadroSprite;
                botao.GetComponent<Button>().onClick.AddListener(() => AtribuirSkin(s.id, flyerEscolhido, botao));

                if (s.id == 0)
                {
                    botaoInicial = botao;
                }
            }

            AtribuirSkin(0, flyerEscolhido, botaoInicial);
        }

        public void AtribuirSkin(int idSkin, Personagem_Data p, GameObject btn)
        {
            idSkinAtual = idSkin;

            foreach (Transform t in flyerUI_Transform)
                Destroy(t.gameObject);

            foreach (Transform b in gridSkins)
            {
                var image = b.GetComponent<Image>();
                image.color = (b.gameObject == btn) ? Color.white : image.color = Color.grey;
            }

            Instantiate(p.skins[idSkin].prefab_UI, flyerUI_Transform);
        }

        public void ChamarFinal()
        {
            escolherPersonagem_painel.SetActive(false);
            personalizacao_Painel.SetActive(false);

            PlayerID meuId = NetworkManager.main.localPlayer;

            var playerConfig = PlayerConfigData.Instance;
            var nick = playerConfig.Nickname;
            var idAuth = playerConfig.idAuth;

            prepController.AtualizarInfoNoServer
                (
                idAuth,
                nick,
                meuId,
                flyerEscolhido.id,
                idSkinAtual,
                deckEscolhido.cartas[0].id,
                deckEscolhido.cartas[1].id,
                deckEscolhido.cartas[2].id,
                deckEscolhido.itemAtivavel.id
                );
        }
    }
}

