using PurrNet.Prediction;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Player
{
    public class PlayerReferences : PredictedIdentity<PlayerReferences.State>
    {
        public struct State : IPredictedData<State>
        {
            public string nickname;
            public Team team;
            public ulong playerId;

            public void Dispose() { }

            public override string ToString()
            {
                string result = $"Team: {team}";
                result += $"\n Nickname: {nickname}";
                result += $"\n PlayerId: {playerId}";

                return result;
            }
        }

        [Header("Refs")]
        public PlayerController playerController;
        public PlayerMovement playerMovement;
        public PlayerVida playerVida;
        public PlayerAnimation playerAnimation;
        public SkillDirectionPlayer skillDirectionPlayer;
        public PlayerArma playerArma;
        public PlayerAtk playerAtk;
        public PlayerCristals playerCristals;
        public PlayerMostrarCartas playerMostrarCartas;
        public PlayerItensAtivaveis playerItensAtivaveis;

        [Header("Skills")]
        public PlayerSkills playerSkill_1;
        public PlayerSkills playerSkill_2;
        public PlayerSkills playerSkill_3;

        //------------------------------ Player Infos ------------------------------
       
        protected override State GetInitialState()
        {
            return new State()
            {
                team = Team.Nenhum,
                nickname = "Nickname",
                playerId = 0,
            };
        }

        protected override void LateAwake()
        {
            if (isOwner)
            {
                ArenaReferences.Instance.playerReferences = this;
            }
            
            if (isServer)
            {
                Atualizar();
            }
        }

        async void Atualizar()
        {
            ulong meuId = owner.Value.id.value;
            Team meuTime = await ArenaReferences.Instance.gameStartConfig.PlayerEstaPronto(meuId);
            currentState.team = meuTime;
            currentState.playerId = meuId;

            ArenaReferences.Instance.gameStartConfig.SincronizarPlayers();
        }

        public void Set_Nickname(string nick)
        {
            currentState.nickname = nick;
        }
    }
}

