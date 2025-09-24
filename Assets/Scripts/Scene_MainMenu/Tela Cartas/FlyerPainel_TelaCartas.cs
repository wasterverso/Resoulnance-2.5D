using Resoulnance.Flyers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaCartas
{
    public class FlyerPainel_TelaCartas : MonoBehaviour
    {
        [Header("Controle De dados")]
        [SerializeField] Flyer_Data flyerData;
        [SerializeField] Icons_Data iconsData;

        [Header("Referencias")]
        [SerializeField] StartTelaCartas startTelaCartas;
        [SerializeField] DeckPainel_TelaCartas deckTelaCartas;
        [SerializeField] TabBotoesUI botoesSelecionadosScript;

        [Header("Flyer")]
        public Personagem_Data flyerEscolhido;
        public bool isPadrao = false;

        [Header("Flyer Painel")]
        [SerializeField] GameObject prefabFlyerQuadrado;
        [SerializeField] GameObject trocarFlyersPainel;
        [SerializeField] Transform gridLayoutGroup;

        [Header("Referencias Flyer")]
        [SerializeField] Text nomeFlyer_txt;
        [SerializeField] Image flyersImage_btn;
        [SerializeField] Image afinidade_1;
        [SerializeField] Image afinidade_2;
        [SerializeField] Image naoAfinidade_1;
        [SerializeField] Image naoAfinidade_2;

        [Header("Deck Universal")]
        [SerializeField] Sprite deckUniversal_sprite;

        public void IniciarComDeckPadrao()
        {
            flyersImage_btn.sprite = deckUniversal_sprite;

            isPadrao = true;

            AtualizarPainel(false, "Universal");
        }

        public void TrocarFlyer()
        {
            trocarFlyersPainel.SetActive(true);

            foreach (Transform child in gridLayoutGroup)
            {
                Destroy(child.gameObject);
            }

            foreach (Personagem_Data personagem in ListaFlyers.Instance.meusPersonagens)
            {
                GameObject flyerObj = Instantiate(prefabFlyerQuadrado, gridLayoutGroup);

                flyerObj.GetComponent<Image>().sprite = personagem.quadradoSprite;

                // Adicione um listener ao botão do flyer
                Button flyerButton = flyerObj.GetComponent<Button>();
                if (flyerButton != null)
                {
                    flyerButton.onClick.AddListener(() => AtualizarInfoTrocarFlyer(personagem));
                }
            }
        }

        public void TrocarParaPadrao()
        {
            IniciarComDeckPadrao();
            deckTelaCartas.EditarDeckTemporario(true);
            startTelaCartas.ButtonDecks(0);
            botoesSelecionadosScript.OnButtonClick(botoesSelecionadosScript.botoesList[0].botao);
            trocarFlyersPainel.SetActive(false);
        }

        public void AtualizarInfoTrocarFlyer(Personagem_Data personagem)
        {
            trocarFlyersPainel.SetActive(false);

            flyerEscolhido = personagem;

            flyersImage_btn.sprite = flyerEscolhido.quadradoSprite;

            // Encontrar as imagens correspondentes
            afinidade_1.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == flyerEscolhido.Afinidade1).SpriteAfinidade;
            afinidade_2.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == flyerEscolhido.Afinidade2).SpriteAfinidade;
            naoAfinidade_1.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == flyerEscolhido.NaoAfinidade1).SpriteAfinidade;
            naoAfinidade_2.sprite = iconsData.iconAfinidade.Find(icon => icon.Afinidade == flyerEscolhido.NaoAfinidade2).SpriteAfinidade;

            deckTelaCartas.EditarDeckTemporario(false);

            isPadrao = false;

            AtualizarPainel(true, flyerEscolhido.nome);

            startTelaCartas.ButtonDecks(0);
            botoesSelecionadosScript.OnButtonClick(botoesSelecionadosScript.botoesList[0].botao);
        }

        public void AtualizarPainel(bool ativo, string nome)
        {
            nomeFlyer_txt.text = nome;
            afinidade_1.enabled = ativo;
            afinidade_2.enabled = ativo;
            naoAfinidade_1.enabled = ativo;
            naoAfinidade_2.enabled = ativo;
        }

    }
}

