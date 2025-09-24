using UnityEngine;

namespace Resoulnance.Telas.TelaCartas
{
    public class StartTelaCartas : MonoBehaviour
    {
        [Header("Scripts Referencia")]
        [SerializeField] Instanciar_TelaCartas instanciaCartasTelaCartas;
        [SerializeField] DeckPainel_TelaCartas deckTelaCartas;
        [SerializeField] FlyerPainel_TelaCartas flyerPainelCartas;

        [Header("Decks")]
        public int deckAtual = 0;

        void Start()
        {
            flyerPainelCartas.IniciarComDeckPadrao();
            deckTelaCartas.EditarDeckTemporario(true);
            ButtonDecks(0);
        }

        private void OnEnable()
        {
            instanciaCartasTelaCartas.InstanciarCartas();
        }

        public void ButtonDecks(int valor)
        {
            deckAtual = valor;
            deckTelaCartas.AtualizarImagensDoDeck();
        }
    }
}

