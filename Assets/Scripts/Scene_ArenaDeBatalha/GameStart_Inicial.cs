using PlayFlow;
using PurrNet;
using PurrNet.Prediction;
using PurrNet.Transports;
using Resoulnance.Flyers;
using Resoulnance.Scene_Arena.Config;
using Resoulnance.Scene_Arena.HUD;
using Resoulnance.Scene_Arena.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Scene_Arena
{
    public class GameStart_Inicial : NetworkBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] GameStart_Treinamento gameStartTreinamento;
        [SerializeField] PlayerSpawn_Arena playerSpawnArena;
        [SerializeField] HudController hudController;
        ListTeamController listTeamController;

        [Header("Refs Obj")]
        [SerializeField] int players = 2;

        [Header("Refs Script")]
        [SerializeField] Flyer_Data flyerData;

        [Header("Iniciar apenas Cena Atual")]
        public bool testarNaCena;
        
        int _playerProntos = 0;

        private void Awake()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor) return;

            Unity.Collections.NativeLeakDetection.Mode = Unity.Collections.NativeLeakDetectionMode.Enabled;
        }

        protected override void OnSpawned()
        {
            if (isServer) return;

            listTeamController = ListTeamController.Instance;

            if (listTeamController != null && listTeamController.networkMode == NetworkMode.Cliente)
            {
                ulong playerId = networkManager.localPlayer.id;
                Jogador jogador = listTeamController.JogadoresConfig.FirstOrDefault(c => c.playerID.id == playerId);

                int idCarta1_Test = jogador.idCarta1;
                int idCarta2_Test = jogador.idCarta2;
                int idCarta3_Test = jogador.idCarta3;
                int idItem = jogador.idItemAtivavel;

                hudController.AtribuirCartas(idCarta1_Test, idCarta2_Test, idCarta3_Test, idItem);

                Personagem_Data flyer = ArenaReferences.Instance.flyerData.personagens.FirstOrDefault(c => c.id == jogador.idFlyer);
                hudController.AtribuirSupremaFlyer(flyer);
            }            
        }

        [ServerRpc(requireOwnership: false)]
        public async Task<Team> PlayerEstaPronto(ulong playerId)
        {
            if (!isServer) return Team.Nenhum;

            await Task.Yield();

            _playerProntos++;

            Team teamRef = Team.Nenhum;

            if (playerId == 1)
                teamRef = Team.Blue;
            else if (playerId == 2)
                teamRef = Team.Red;

            return teamRef;
        }

        public void SincronizarPlayers()
        {
            SincronizarDadosPlayer_Rpc();
        }

        [ObserversRpc]
        void SincronizarDadosPlayer_Rpc()
        {
            PlayerController[] playerControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

            foreach (PlayerController controller in playerControllers)
            {
                controller.AtualizarPersonagem();
            }
        }

        [ObserversRpc(runLocally: true)]
        public void ReceberDebugDoServidor(string textoTxt)
        {
            Debug.Log($"[Servidor Debug] {textoTxt}");
        }
    }

}
