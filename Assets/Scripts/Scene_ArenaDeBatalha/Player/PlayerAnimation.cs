using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using System;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerAnimation : PredictedIdentity<PlayerAnimation.State>
    {
        [Header("Ref Script")]
        [SerializeField] PlayerReferences playerReferences;

        [Header("Sprite Skin Animator")]
        [SerializeField] Animator animator;
        [SerializeField] SpriteRenderer animSkim_SpriteRenderer;

        [Header("Efeitos")]
        [SerializeField] GameObject rastroEfeito;
        [SerializeField] GameObject lentidaoEfeito;

        PlayerState _playerStateAtual;

        protected override void LateAwake()
        {
            playerReferences.playerMovement.OnVelocidadeEstaDiferente += VerificarEfeitosVelocidade;
        }

        protected override State GetInitialState()
        {
            return new State
            {
                isEsquerda = true,
            };
        }

        protected override void Simulate(ref State state, float delta)
        {
            PlayerState atualStatePlayer = playerReferences.playerController.currentState.playerState;

            if (atualStatePlayer == PlayerState.Dead || atualStatePlayer == PlayerState.Stun || atualStatePlayer == PlayerState.Atk) return;

            Vector2 inputDirection = playerReferences.playerMovement.currentInput.direction;
            if (inputDirection.x != 0)
            {
                state.isEsquerda = inputDirection.x > 0;
            }

            if (atualStatePlayer != PlayerState.Dash)
            {
                if (inputDirection != Vector2.zero)
                    playerReferences.playerController.currentState.playerState = PlayerState.Move;
                else
                {
                    if (atualStatePlayer != PlayerState.Depositar)
                    {
                        playerReferences.playerController.currentState.playerState = PlayerState.Idle;
                    }
                }
            }
        }

        protected override void UpdateView(State interpolatedState, State? verified)
        {
            PlayerState atualStatePlayer = playerReferences.playerController.currentState.playerState;

            if (atualStatePlayer != _playerStateAtual)
            {
                _playerStateAtual = atualStatePlayer;
                AtualizarAnimacao(_playerStateAtual);
            }

            animSkim_SpriteRenderer.flipX = interpolatedState.isEsquerda;
        }

        void AtualizarAnimacao(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    animator.Play("Idle");
                    break;
                case PlayerState.Move:
                    animator.Play("Move");
                    break;
                case PlayerState.Dash:
                    animator.Play("Dash");
                    break;
                case PlayerState.Stun:
                    animator.Play("Stun");
                    break;
                case PlayerState.Atk:
                    animator.Play("Idle");
                    break;
                case PlayerState.Depositar:
                    animator.Play("Idle");
                    break;
                case PlayerState.Dead:
                    animator.Play("Dead");
                    break;
            }

            playerReferences.playerMovement.podeMover = (state != PlayerState.Dead || state != PlayerState.Stun);
            playerReferences.playerArma.Set_MostrarArma(state != PlayerState.Dead);

        }

        void VerificarEfeitosVelocidade(VelocidadeRefs refsVel)
        {
            lentidaoEfeito.SetActive(refsVel.velAtual < refsVel.velBasica);

            rastroEfeito.SetActive(playerReferences.playerController.currentState.playerState == PlayerState.Dash);
        }

        public void PlayerEstaVivo(bool estaVivo)
        {
            if (estaVivo)
            {
                animSkim_SpriteRenderer.sortingOrder = 3;
            }
            else
            {
                animSkim_SpriteRenderer.sortingOrder = 0;
            }
        }

        public SpriteRenderer Get_SpriteAnim()
        {
            return animSkim_SpriteRenderer;
        }

        public struct State : IPredictedData<State>
        {
            public bool isEsquerda;

            public void Dispose() { }
        }

        protected override void Destroyed()
        {
            playerReferences.playerMovement.OnVelocidadeEstaDiferente -= VerificarEfeitosVelocidade;
        }
    }
}


