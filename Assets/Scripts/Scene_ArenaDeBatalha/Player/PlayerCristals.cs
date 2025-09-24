using PurrNet;
using PurrNet.Prediction;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerCristals : PredictedIdentity<PlayerCristals.Input, PlayerCristals.State>
    {
        public struct State : IPredictedData<State>
        {
            public bool estaGuardando;

            public float tempoGuardarCristal;

            public void Dispose() { }
        }

        public struct Input : IPredictedData<Input>
        {
            public bool guardarCristais;

            public void Dispose() { }
        }

        ListCristaisController listCristaisController;

        [Header("Refs")]
        [SerializeField] PlayerReferences playerReferences;
        [SerializeField] Text contagem_txt;

        ulong meuId = 0;
        bool guardarCristal;

        private void Start()
        {
            listCristaisController = ListCristaisController.Instance;
            listCristaisController.OnListChanged += AtualizarCristais;
        }

        public void ChamarGuardarCristais()
        {
            guardarCristal = true;
        }

        protected override void UpdateInput(ref Input input)
        {
            if (guardarCristal)
            {
                guardarCristal = false;
                input.guardarCristais = true;
            }
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            if (input.guardarCristais)
            {
                input.guardarCristais = false;

                if (listCristaisController.TemCristaisNaMao(playerReferences.currentState.playerId))
                {
                    state.estaGuardando = true;
                    state.tempoGuardarCristal = 1;
                    playerReferences.playerController.SetPlayerState(PlayerState.Depositar);
                }
            }

            if (state.estaGuardando)
            {
                PlayerState playerEstado = playerReferences.playerController.currentState.playerState;
                if (playerEstado == PlayerState.Depositar)
                {
                    state.tempoGuardarCristal -= delta;
                    if (state.tempoGuardarCristal <= 0)
                    {
                        if (listCristaisController.TemCristaisNaMao(playerReferences.currentState.playerId))
                        {
                            GuardarCristalNoGuardiao();
                            state.tempoGuardarCristal = 1f;
                        }
                        else
                        {
                            state.estaGuardando = false;
                            if ( playerEstado == PlayerState.Depositar)
                            {
                                playerReferences.playerController.SetPlayerState(PlayerState.Idle);
                            }
                        }
                    }
                }
                else
                {
                    state.estaGuardando = false;
                }
            }
        }

        void GuardarCristalNoGuardiao()
        {
            listCristaisController.PlayerGuardouCristal(meuId, playerReferences.currentState.team);
        }

        void AtualizarCristais(CristalInfo change)
        {
            if (meuId == 0)
            {
                meuId = playerReferences.currentState.playerId;
            }

            ulong cont = 0;

            foreach (var cris in listCristaisController.currentState.cristaisList)
            {
                if (cris.idPlayer == meuId)
                {
                    cont++;
                }
            }

            contagem_txt.text = cont.ToString();
        }

        void OnDisable()
        {
            if (listCristaisController == null) return;

            listCristaisController.OnListChanged -= AtualizarCristais;
        }
    }
}


