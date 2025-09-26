using Resoulnance.Cartas;
using Resoulnance.Flyers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaCartas
{
    public class DeckPainel_TelaCartas : MonoBehaviour
    {
        [Header("Referencia Scripts")]
        [SerializeField] StartTelaCartas startTelaCartas;
        [SerializeField] FlyerPainel_TelaCartas flyerPainelCartas;
        [SerializeField] InfoCartaPainel infoCartaPainel;
        [SerializeField] TrocarItens_TelaCartas trocarItem;

        [Header("Deck Cartas")]
        [SerializeField] Sprite CartaPadrao;
        [SerializeField] CartaUIPrefab_TelaCartas Carta1;
        [SerializeField] GameObject carta1_Buff;
        [SerializeField] GameObject carta1_Debuff;
        [SerializeField] CartaUIPrefab_TelaCartas Carta2;
        [SerializeField] GameObject carta2_Buff;
        [SerializeField] GameObject carta2_Debuff;
        [SerializeField] CartaUIPrefab_TelaCartas Carta3;
        [SerializeField] GameObject carta3_Buff;
        [SerializeField] GameObject carta3_Debuff;

        [Header("Item")]
        [SerializeField] Image item_img;

        [Header("Carta Selecionada")]
        [SerializeField] Carta cartaSelecionada;

        [Header("Decks")]
        public List<Deck> decksTemporarios = new List<Deck>();
        [SerializeField] InputField nomeDeck_input;

        [Header("Audio")]
        [SerializeField] AudioClip adicionarCarta_Audio;

        bool editouDeck = false;

        public void EditarDeckTemporario(bool padrao)
        {
            if (padrao)
            {
                decksTemporarios = ListaDecks.Instance.meusDecksUniversais;
            }
            else
            {
                Personagem_Data personagem = ListaFlyers.Instance.meusPersonagens.Find(c => c.id == flyerPainelCartas.flyerEscolhido.id);
                if (personagem != null)
                {
                    decksTemporarios = personagem.decks;
                }
            }
        }

        public void AdicionarCartaAoDeckAtual()
        {
            editouDeck = true;

            var deckAtual = decksTemporarios[startTelaCartas.deckAtual];

            if (deckAtual.cartas.Count < 3)
            {
                // Verificar se a carta já está no deck
                if (!deckAtual.cartas.Contains(cartaSelecionada))
                {
                    // Verificar se é uma carta passiva
                    if (cartaSelecionada.cartaAtiva == CartaAtivaPassiva.Passiva)
                    {
                        // Inserir como primeira no deck
                        deckAtual.cartas.Insert(0, cartaSelecionada);
                    }
                    else
                    {
                        // Adicionar ao final do deck
                        deckAtual.cartas.Add(cartaSelecionada);
                    }
                }
                else
                {
                    Debug.Log("A carta já está no deck.");
                }
            }
            else
            {
                Debug.Log("O deck está cheio. Não é possível adicionar mais cartas.");
            }

            AtualizarImagensDoDeck();
        }

        public void AtualizarImagensDoDeck()
        {
            Carta1.ReceberCartaUI(null);
            Carta2.ReceberCartaUI(null);
            Carta3.ReceberCartaUI(null);

            carta1_Buff.SetActive(false);
            carta1_Debuff.SetActive(false);
            carta2_Buff.SetActive(false);
            carta2_Debuff.SetActive(false);
            carta3_Buff.SetActive(false);
            carta3_Debuff.SetActive(false);

            var cartasDoDeck = decksTemporarios[startTelaCartas.deckAtual].cartas;

            nomeDeck_input.text = decksTemporarios[startTelaCartas.deckAtual].nomeDeck;

            Personagem_Data flyerEscolhido = flyerPainelCartas.flyerEscolhido;

            if (cartasDoDeck.Count > 0)
            {
                Carta1.ReceberCartaUI(cartasDoDeck[0], this);

                if (!flyerPainelCartas.isPadrao && flyerEscolhido != null)
                {
                    if (cartasDoDeck[0].tipo == flyerEscolhido.Afinidade1 || cartasDoDeck[0].tipo == flyerEscolhido.Afinidade2)
                    {
                        carta1_Buff.SetActive(true);
                    }

                    if (cartasDoDeck[0].tipo == flyerEscolhido.NaoAfinidade1 || cartasDoDeck[0].tipo == flyerEscolhido.NaoAfinidade2)
                    {
                        carta1_Debuff.SetActive(true);
                    }
                }
            }
            if (cartasDoDeck.Count > 1)
            {
                Carta2.ReceberCartaUI(cartasDoDeck[1], this);

                if (!flyerPainelCartas.isPadrao && flyerEscolhido != null)
                {
                    if (cartasDoDeck[1].tipo == flyerEscolhido.Afinidade1 || cartasDoDeck[1].tipo == flyerEscolhido.Afinidade2)
                    {
                        carta2_Buff.SetActive(true);
                    }

                    if (cartasDoDeck[1].tipo == flyerEscolhido.NaoAfinidade1 || cartasDoDeck[1].tipo == flyerEscolhido.NaoAfinidade2)
                    {
                        carta2_Debuff.SetActive(true);
                    }
                }
            }
            if (cartasDoDeck.Count > 2)
            {
                Carta3.ReceberCartaUI(cartasDoDeck[2], this);

                if (!flyerPainelCartas.isPadrao && flyerEscolhido != null)
                {
                    if (cartasDoDeck[2].tipo == flyerEscolhido.Afinidade1 || cartasDoDeck[2].tipo == flyerEscolhido.Afinidade2)
                    {
                        carta3_Buff.SetActive(true);
                    }

                    if (cartasDoDeck[2].tipo == flyerEscolhido.NaoAfinidade1 || cartasDoDeck[2].tipo == flyerEscolhido.NaoAfinidade2)
                    {
                        carta3_Debuff.SetActive(true);
                    }
                }
            }

            if (decksTemporarios[startTelaCartas.deckAtual].itemAtivavel.nomeItem != string.Empty)
                item_img.sprite = decksTemporarios[startTelaCartas.deckAtual].itemAtivavel.iconSprite;
            else
            {
                decksTemporarios[startTelaCartas.deckAtual].itemAtivavel = ListaDecks.Instance.itensData.itensAtivaveis[0];
                item_img.sprite = decksTemporarios[startTelaCartas.deckAtual].itemAtivavel.iconSprite;
            }
        }

        public void LimparDeckAtual()
        {
            editouDeck = true;

            var cartasDoDeck = decksTemporarios[startTelaCartas.deckAtual];
            cartasDoDeck.cartas.Clear();

            AtualizarImagensDoDeck();
        }

        public void RemoverUltimaCarta()
        {
            editouDeck = true;

            var cartasDoDeck = decksTemporarios[startTelaCartas.deckAtual];
            if (cartasDoDeck.cartas.Count > 0)
            {
                cartasDoDeck.cartas.RemoveAt(cartasDoDeck.cartas.Count - 1);
            }
            else
            {
                Debug.Log("O deck está vazio. Não há cartas para remover.");
            }

            AtualizarImagensDoDeck();
        }

        public void RemoverCartaSelecionada()
        {
            editouDeck = true;

            var deckAtual = decksTemporarios[startTelaCartas.deckAtual];

            var carta_ = deckAtual.cartas.Find(c => c.id == cartaSelecionada.id);
            if (carta_ != null)
            {
                decksTemporarios[startTelaCartas.deckAtual].cartas.Remove(carta_);

                AtualizarImagensDoDeck();
            }
        }

        public void SalvarAlteracoes()
        {
            if (editouDeck)
            {
                editouDeck = false;

                if (flyerPainelCartas.isPadrao)
                {
                    ListaDecks.Instance.meusDecksUniversais = decksTemporarios;
                    //ListaDecks.Instance.SalvarDecks();
                }
                else
                {
                    int f_Id = ListaFlyers.Instance.meusPersonagens.FindIndex(c => c.id == flyerPainelCartas.flyerEscolhido.id);
                    //decksTemporarios = ListaFlyers.Instance.meusPersonagens[f_Id].decks;
                    //ListaFlyers.Instance.SalvarPersonagens();
                }
            }
        }

        public void AtualizarNomeDeck()
        {
            editouDeck = true;

            string nome = nomeDeck_input.text;

            var deck_ = decksTemporarios[startTelaCartas.deckAtual];

            deck_.nomeDeck = nome;
        }

        public void AbrirDetalhesDaCarta(Carta cartaClicada)
        {
            cartaSelecionada = cartaClicada;
            var carta_ = decksTemporarios[startTelaCartas.deckAtual].cartas.Find(c => c.id == cartaClicada.id);

            infoCartaPainel.AtribuirInfoCarta(cartaClicada, !(carta_ != null), true);
        }

        public void TrocarItem()
        {
            editouDeck = true;

            trocarItem.AbrirPainelItens(decksTemporarios[startTelaCartas.deckAtual].itemAtivavel);
        }
    }
}


