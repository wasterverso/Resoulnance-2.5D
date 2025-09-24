using PurrNet;
using PurrNet.Prediction;
using Resoulnance.Cartas;
using System;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerController : PredictedIdentity<PlayerController.State>
    {
        public struct State : IPredictedData<State>
        {
            public PlayerState playerState;

            public float tempoParaRessucitar;

            public void Dispose() { }
        }

        [Header("Refs script")]
        [SerializeField] PlayerReferences playerReferences;
        ArenaReferences _arenaReferences;
        
        [Header("Refs Others")]
        [SerializeField] GameObject hud_Object;

        Vector3 spawnPosition;

        protected override void LateAwake()
        {
            _arenaReferences = ArenaReferences.Instance;

            if (isOwner)
            {
                _arenaReferences.cameraFollow.SetTarget(this.transform);                
            }

            spawnPosition = transform.position;
        }

        protected override State GetInitialState()
        {
            return new State
            {
                playerState = PlayerState.Idle,
            };
        }

        public void SetPlayerState(PlayerState newState)
        {
            currentState.playerState = newState;
        }

        public void EstaVivo(bool estaVivo)
        {
            hud_Object.SetActive(estaVivo);
            playerReferences.playerAnimation.PlayerEstaVivo(estaVivo);

            if (estaVivo)
            {
                playerReferences.playerVida.Ressucitou();
                transform.position = spawnPosition;
            }
            else
            {
                currentState.tempoParaRessucitar = 15;

                if (isServer)
                {
                    ListCristaisController.Instance.PlayerMorreu(playerReferences.currentState.playerId);
                }

                if (isOwner)
                {
                    _arenaReferences.morteController.MeuPlayerMorreu(this);
                }
            }
        }

        protected override void Simulate(ref State state, float delta)
        {
            if (state.playerState == PlayerState.Dead)
            {
                state.tempoParaRessucitar -= delta;
                if (state.tempoParaRessucitar <= 0)
                {
                    state.playerState = PlayerState.Idle;
                    EstaVivo(true);
                }
            }
        }

        public void AtualizarPersonagem()
        {
            ulong playerId = owner.Value.id.value;
            Team meuTime = playerReferences.currentState.team;

            if (isOwner)
            {
                _arenaReferences.team = meuTime;
            }

            _arenaReferences.Set_PlayerObj(playerId, this.gameObject);

            playerReferences.playerVida.AtualizarTipoBarraDeVida(meuTime);

            Collider meuCollider = GetComponent<Collider>();
            Collider paredeCollider = (meuTime == Team.Blue) ? 
                _arenaReferences.paredeBlue : _arenaReferences.paredeRed;

            Physics.IgnoreCollision(paredeCollider, meuCollider, true);
        }
    }
}

