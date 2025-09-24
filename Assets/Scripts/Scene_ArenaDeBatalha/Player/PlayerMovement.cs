using PurrNet.Prediction;
using Resoulnance.Scene_Arena;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Resoulnance.Scene_Arena.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PredictedRigidbody))]
    public class PlayerMovement : PredictedIdentity<PlayerMovement.Input, PlayerMovement.State>
    {
        public struct State : IPredictedData<State>
        {
            public float velAtual;
            public float velBasica;

            public bool is_VelocidadeEstaDiferente => (tempo_MudancaVelocidade > 0 || velBasica != velAtual);
            public float valor_MudancaVelocidade;
            public float tempo_MudancaVelocidade;

            public Vector3 moveDirection;

            public void Dispose() { }
        }

        public struct Input : IPredictedData<Input>
        {
            public Vector2 direction;

            public bool chamouDash;

            public void Dispose() { }
        }

        [Header("Refs")]
        [SerializeField] PlayerController playerController;
        [SerializeField] PlayerDash playerDash;
        [SerializeField] PredictedRigidbody _rigidbody;

        [Header("Config")]
        [SerializeField] float _velocidade = 5;
        [SerializeField] float _valorDash = 3;
        [SerializeField] float _tempoDash = 2;

        [Header("Controles")]
        public bool podeMover = true;

        Joystick _joystick;
        bool deuDash = false;
        float velAtualRef;

        public event Action<VelocidadeRefs> OnVelocidadeEstaDiferente;

        protected override void LateAwake()
        {
            _joystick = ArenaReferences.Instance.joystick;
            velAtualRef = _velocidade;
        }

        //Iniciar velocidade atual
        protected override State GetInitialState()
        {
            return new State
            {
                velBasica = _velocidade,
                velAtual = _velocidade,
                valor_MudancaVelocidade = 0f,
            };
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            if (!podeMover)
            {
                NaoPodeMover();
            }

            PlayerState stateAtual = playerController.currentState.playerState;
            if (stateAtual == PlayerState.Dead || stateAtual == PlayerState.Stun || stateAtual == PlayerState.Atk)
            {
                NaoPodeMover();
                return;
            }

            if(input.direction.sqrMagnitude < 0.0001f)
            {
                NaoPodeMover();
                return;
            }

            UpdateDashInfo(input, ref state, delta); //Atualizar infos de Dash

            state.moveDirection = new Vector3(input.direction.x, 0, input.direction.y);
            state.moveDirection.Normalize();

            state.velAtual = state.is_VelocidadeEstaDiferente ? (state.velBasica + state.valor_MudancaVelocidade) : state.velBasica;

            if (state.velAtual != velAtualRef)
            {
                velAtualRef = state.velAtual;
                VelocidadeRefs newVel = new VelocidadeRefs
                {
                    velAtual = state.velAtual,
                    velBasica = state.velBasica,
                };

                OnVelocidadeEstaDiferente.Invoke(newVel);
            }

            _rigidbody.linearVelocity = state.moveDirection * state.velAtual;
        }

        void UpdateDashInfo(Input input, ref State state, float delta)
        {
            if (input.chamouDash && !state.is_VelocidadeEstaDiferente)
            {
                state.tempo_MudancaVelocidade = _tempoDash;
                state.valor_MudancaVelocidade = _valorDash;
                playerDash.ChamouDash();
            }

            // Atualiza o timer do dash se estiver ativo
            if (state.is_VelocidadeEstaDiferente)
            {
                state.tempo_MudancaVelocidade -= delta;
                if (state.tempo_MudancaVelocidade <= 0f)
                {
                    state.valor_MudancaVelocidade = 0f;

                    if (playerController.currentState.playerState == PlayerState.Dash)
                    {
                        playerController.SetPlayerState(PlayerState.Idle);
                    }
                }
            }
        }

        public void NaoPodeMover()
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }

        protected override void UpdateInput(ref Input input)
        {
            if (deuDash)
            {
                deuDash = false;
                input.chamouDash = true;
            }
        }

        //Usado para inputs que clicam por um longo tempo
        protected override void GetFinalInput(ref Input input)
        {
            Vector2 inputJoy = new Vector2(_joystick.Horizontal, _joystick.Vertical);

            if (inputJoy == Vector2.zero)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                Vector2 inputPc = new Vector2(InputPCTeste.Instance._horizontal, InputPCTeste.Instance._vertical);
                inputJoy = inputPc;
#endif
            }

            input.direction = inputJoy;
        }

        protected override void SanitizeInput(ref Input input)
        {
            input.direction.Normalize();
        }

        public void ChamarDash()
        {
            if (playerDash.currentState.cargasAtual == 0) return;

            PlayerState stateAtual = playerController.currentState.playerState;

            if (stateAtual == PlayerState.Idle || stateAtual == PlayerState.Move)
            {
                deuDash = true;
            }
        }

        public void MudarVelocidadeTemporariamente(float vel, float tempo)
        {
            if (playerController.currentState.playerState == PlayerState.Dead) return;

            currentState.valor_MudancaVelocidade = vel;
            currentState.tempo_MudancaVelocidade = tempo;
        }

        public void CancelarMudancaVelocidade()
        {
            if (currentState.is_VelocidadeEstaDiferente)
            {
                currentState.tempo_MudancaVelocidade = 0;
            }
        }
    }
}

public struct VelocidadeRefs
{
    public float velAtual;
    public float velBasica;
}