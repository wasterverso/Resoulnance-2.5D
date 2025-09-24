using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerArma : PredictedIdentity<PlayerArma.State>
    {
        [Header("Ref scripts")]
        [SerializeField] PlayerAnimation _playerAnimation;
        [SerializeField] PlayerController _playerController;
        [SerializeField] PlayerMovement _playerMovement;

        [Header("Ref Internos")]
        [SerializeField] SpriteRenderer arma_sprite;
        [SerializeField] Transform armaTransform;

        protected override void LateAwake()
        {
            if (!isOwner) return;
        }

        protected override State GetInitialState()
        {
            return new State
            {
                rotation = 0,
                podeMoverArma = true,
            };
        }

        protected override void Simulate(ref State state, float delta)
        {
            if (_playerController == null || _playerMovement == null) return;

            PlayerState atualStatePlayer = _playerController.currentState.playerState;

            if (!state.podeMoverArma || atualStatePlayer == PlayerState.Atk || atualStatePlayer == PlayerState.Dead) return;

            if (atualStatePlayer == PlayerState.Move)
            {
                Vector2 direction = _playerMovement.currentInput.direction;
                if (direction != Vector2.zero)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    state.rotation = angle;
                }
            }
            else if (atualStatePlayer == PlayerState.Dash)
            {
                state.rotation = -33f;
                /*
                if (playerAnimation.currentState.isEsquerda)
                {
                    armaTransform.localRotation = Quaternion.Euler(0f, 180f, -33f);
                }
                else
                {
                    armaTransform.localRotation = Quaternion.Euler(0f, 0f, -33f);
                }
                */
            }

            armaTransform.localRotation = Quaternion.Euler(0f, 0f, state.rotation);
        }

        public void Set_PodeMoverArma(bool pode)
        {
            ref State stateAtual = ref currentState;
            stateAtual.podeMoverArma = pode;
        }

        public void DefinirDirecao(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            currentState.rotation = angle;
            armaTransform.localRotation = Quaternion.Euler(0f, 0f, angle);

            _playerAnimation.currentState.isEsquerda = (angle > -90 && angle < 90);
        }

        public void Set_MostrarArma(bool mostrar)
        {
            armaTransform.gameObject.SetActive(mostrar);
        }

        public struct State : IPredictedData<State>
        {
            public float rotation;
            public bool podeMoverArma;

            public void Dispose() { }
        }
    }
}

