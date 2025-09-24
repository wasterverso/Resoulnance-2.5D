using PurrNet.Prediction;
using Resoulnance.Cartas;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerMostrarCartas : PredictedIdentity<PlayerMostrarCartas.Input, PlayerMostrarCartas.State>
    {
        public struct State : IPredictedData<State>
        {
            public float tempoMostrandoCarta;
            public bool estaMostrando;

            public void Dispose() { }
        }

        public struct Input : IPredictedData<Input>
        {
            public bool mostrarCarta;
            public int idCarta;

            public void Dispose() { }
        }

        ArenaReferences arenaReferences;
        public GameObject cartaObj;

        [Header("UI Image")]
        [SerializeField] Image fundo;
        [SerializeField] Image spriteCarta;
        [SerializeField] Image icon;

        InputMostrarCarta inputMostrarCarta = new InputMostrarCarta();

        protected override void LateAwake()
        {
            arenaReferences = ArenaReferences.Instance;
        }

        public void ChamarMostrarCarta(int idCarta)
        {
            inputMostrarCarta.idCarta = idCarta;
            inputMostrarCarta.mostrarCarta = true;
        }

        protected override void UpdateInput(ref Input input)
        {
            if (inputMostrarCarta.mostrarCarta)
            {
                inputMostrarCarta.mostrarCarta = false;

                input.mostrarCarta = true;
                input.idCarta = inputMostrarCarta.idCarta;
            }
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            if (input.mostrarCarta)
            {
                input.mostrarCarta = false;

                state.tempoMostrandoCarta = 1.8f;
                MostrandoCarta(input.idCarta);
                state.estaMostrando = true;
            }

            if (state.estaMostrando)
            {
                state.tempoMostrandoCarta -= delta;
                if (state.tempoMostrandoCarta <= 0)
                {
                    state.estaMostrando = false;
                    cartaObj.SetActive(false);
                }
            }
        }

        void MostrandoCarta(int idCarta)
        {
            Carta carta = arenaReferences.cartasData.cartas.Find(p => p.id == idCarta);
            ReceberCartaUI(carta);
            cartaObj.SetActive(true);
        }

        public void ReceberCartaUI(Carta card)
        {
            IconAfinidade afinidade = arenaReferences.iconsData.iconAfinidade.Find(c => c.Afinidade == card.tipo);

            fundo.sprite = afinidade.background;
            icon.sprite = afinidade.SpriteAfinidade;

            spriteCarta.sprite = card.splashSprite;
        }

        struct InputMostrarCarta
        {
            public bool mostrarCarta;
            public int idCarta;
        }
    }
}

