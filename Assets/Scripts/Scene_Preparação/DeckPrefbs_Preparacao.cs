using Resoulnance.Cartas;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Preparation.Prefabs
{
    public class DeckPrefbs_Preparacao : MonoBehaviour
    {
        [SerializeField] Text nomeDeck_txt;
        [SerializeField] AtribuirCartas atribuirCarta_1;
        [SerializeField] AtribuirCartas atribuirCarta_2;
        [SerializeField] AtribuirCartas atribuirCarta_3;
        [SerializeField] Image itemAtivavel_img;
        [SerializeField] Button usarDeck_btn;

        public void ReceberDeck(Action<bool, Deck> metodo, bool universal, Deck deck)
        {
            nomeDeck_txt.text = deck.nomeDeck;

            atribuirCarta_1.ReceberCarta(deck.cartas[0]);
            atribuirCarta_2.ReceberCarta(deck.cartas[1]);
            atribuirCarta_3.ReceberCarta(deck.cartas[2]);

            itemAtivavel_img.sprite = 
                ListaDecks.Instance.itensData.itensAtivaveis.FirstOrDefault(c => c.id == deck.itemAtivavel.id).iconSprite;

            usarDeck_btn.onClick.AddListener(() => metodo(universal, deck));
        }

        public void ReceberDeck(Deck deck)
        {
            nomeDeck_txt.text = deck.nomeDeck;

            atribuirCarta_1.ReceberCarta(deck.cartas[0]);
            atribuirCarta_2.ReceberCarta(deck.cartas[1]);
            atribuirCarta_3.ReceberCarta(deck.cartas[2]);

            itemAtivavel_img.sprite =
                ListaDecks.Instance.itensData.itensAtivaveis.FirstOrDefault(c => c.id == deck.itemAtivavel.id).iconSprite;
        }
    }
}
