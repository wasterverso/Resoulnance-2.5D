using PurrNet.Prediction;
using Resoulnance.Scene_Arena.Config;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerAtk : PredictedIdentity<PlayerAtk.Input, PlayerAtk.State>
    {
        public struct Input : IPredictedData<Input>
        {
            public bool atacou;
            public bool arrastouAtk;
            public Vector2 directionAtk;

            public bool usouSuprema;
            public bool arrastouSuprema;
            public Vector2 directionSuprema;

            public bool actionOnPlayerAtacou;
            public bool actionOnPlayerSkill;

            public void Dispose() { }
        }

        public struct State : IPredictedData<State>
        {
            public float tempoParaPoderAtacar;
            public bool podeAtacar => (tempoParaPoderAtacar <= 0);

            public float tempoFinalizarAtk;
            public bool estaAtacando;

            public void Dispose() { }
        }

        [Header("Ref Script")]
        [SerializeField] PlayerReferences playerReferences;
        PlayerController playerController;
        PlayerArma playerArma;
        IAttack attackScript;

        [Header("Config")]
        [SerializeField] float _tempoVelDeAtk = 1f;
        [SerializeField] float _tempoDuranteAtk = 0.5f;

        [Header("Mostrar Gizmo")]
        [SerializeField] bool mostrarGizmo = false;
        [SerializeField] float range = 5;

        [Header("Controle Int")]
        bool _atacar = false;
        bool _arrastouAtk = false;
        Vector2 _direcaoAtk;
        bool _usouSuprema = false;
        bool _arrastouSuprema = false;
        Vector2 _direcaoSuprema;
                
        protected override void LateAwake()
        {
            playerController = playerReferences.playerController;
            playerArma = playerReferences.playerArma;
            attackScript = GetComponent<IAttack>();         
        }

        protected override State GetInitialState()
        {
            return new State()
            {
                tempoFinalizarAtk = 0,
                tempoParaPoderAtacar = 0,
            };
        }

        public void AtkBasico(bool arrastou, Vector2 directionAngle)
        {
            _atacar = true;
            _arrastouAtk = arrastou;
            _direcaoAtk = directionAngle;
        }

        public void ChamarSuprema(bool arrastou, Vector2 directionAngle)
        {
            _usouSuprema = true;
            _arrastouSuprema = arrastou;
            _direcaoSuprema = directionAngle;           
        }

        public void PlayerUsouSkill()
        {
            attackScript.PlayerUsouSkill();
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            //Ataque basico
            if (!state.estaAtacando && state.podeAtacar && input.atacou)
            {
                PlayerState stateAtual = playerController.currentState.playerState;
                if (stateAtual != PlayerState.Dead && stateAtual != PlayerState.Stun)
                {
                    GameObject alvo = null;
                    bool vaiAtacar = input.arrastouAtk;

                    if (!input.arrastouAtk)
                    {
                        alvo = VerificarRange_System.GetAlvoAutomatico(
                            ArenaConfig.Instance.alvo, playerController.transform, range, playerReferences.currentState.team);

                        if (alvo == null)
                        {
                            alvo = VerificarRange_System.GetAlvoGuardiao(playerController.transform, range, playerReferences.currentState.team);
                        }

                        vaiAtacar = (alvo != null);
                    }

                    if (vaiAtacar)
                    {
                        if (attackScript != null)
                        {
                            Vector2 direction;

                            if (alvo != null)
                            {
                                Vector3 dir3D = alvo.transform.position - transform.position;
                                direction = new Vector2(dir3D.x, dir3D.z).normalized;
                            }
                            else
                            {
                                direction = input.directionAtk;
                            }

                            EstaEmAtaque(true);
                            playerArma.DefinirDirecao(direction);
                            attackScript.ExecutarAttack(input.arrastouAtk, direction, alvo);
                            attackScript.PlayerAtacou();
                        }
                        else
                        {
                            Debug.Log("AttackScript nao encontrado");
                        }
                    }
                }
            }

            if (!state.podeAtacar)
                state.tempoParaPoderAtacar -= delta;

            if (state.estaAtacando)
            {
                state.tempoFinalizarAtk -= delta;

                if (state.tempoFinalizarAtk <= 0f)
                {
                    EstaEmAtaque(false);
                }
            }

            //Suprema
            if (input.usouSuprema)
            {
                input.usouSuprema = false;

                if (attackScript != null)
                {
                    attackScript.ExecutarSuprema(input.arrastouSuprema, input.directionSuprema);
                }
            }
        }

        public void EstaEmAtaque(bool atk)
        {
            if (atk)
            {
                currentState.tempoFinalizarAtk = _tempoDuranteAtk;
                currentState.tempoParaPoderAtacar = _tempoVelDeAtk;
                currentState.estaAtacando = true;
                playerController.SetPlayerState(PlayerState.Atk);
            }
            else
            {
                currentState.estaAtacando = false;
                if (playerController.currentState.playerState == PlayerState.Atk)
                {
                    playerController.SetPlayerState(PlayerState.Idle);
                }
            }
        }

        protected override void UpdateInput(ref Input input)
        {
            //input.atacou |= Keyboard.current.spaceKey.wasPressedThisFrame;

            if (_atacar)
            {
                _atacar = false;
                input.atacou = true;

                input.arrastouAtk = _arrastouAtk;
                _arrastouAtk = false;
            }

            if (_usouSuprema)
            {
                _usouSuprema = false;
                input.usouSuprema = true;
                input.arrastouSuprema = _arrastouSuprema;
                input.directionSuprema = _direcaoSuprema;
            }
        }

        protected override void GetFinalInput(ref Input input)
        {
            input.directionAtk = _direcaoAtk;
        }

        protected override void SanitizeInput(ref Input input)
        {
            input.directionAtk.Normalize();
        }

        private void OnDrawGizmos()
        {
            if (mostrarGizmo)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, range);
            }
        }
    }
}
