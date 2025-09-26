using PlayFlow;
using PurrNet;
using Resoulnance.Scene_Preparation.Controles;
using Resoulnance.Scene_Preparation.Dados;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Resoulnance.Scene_Preparation.Inicialize
{
    public class PreparacaoStart_Server : NetworkBehaviour
    {
        [SerializeField] PreparacaoStart_Inicial prepStartInicial;
        [SerializeField] LeituraDadosServer_Preparacao leituraServer;
        [SerializeField] Cronometro_Preparacao cronometro;
        ListTeamController listTeamController;

        public SyncVar<int> numDePlayers = new SyncVar<int>();
        public SyncVar<int> playersConnectados = new SyncVar<int>();
        public SyncVar<TiposDeSalas> tipoDePartida = new SyncVar<TiposDeSalas>();

        private WaitForSeconds waitForOneSecond = new WaitForSeconds(1f);

        protected override void OnSpawned()
        {
            if (!isServer) return;

            listTeamController = ListTeamController.Instance;
            
            StartCoroutine(EsperarPlayers());
        }

        IEnumerator EsperarPlayers()
        {
            tipoDePartida.value = leituraServer.TipoDePartidaDoLobby();

            numDePlayers.value = leituraServer.QuantidadeDePlayersNoLobby();

            if (numDePlayers.value == 0)
            {
                DebugServidor("Numeros de players é 0 ou teve problema ao encontrar a quantidade de players");
                yield break;
            }

            yield return EsperarPelaCondicao(() => playersConnectados.value == numDePlayers.value);

            cronometro.IniciarCronometro();
        }

        [ServerRpc(requireOwnership: false)]
        public void PlayerPronto_ServerRpc(PlayerID playerID, string nick, string idAuth)
        {
            if (!isServer) return;

            playersConnectados.value++;

            var jogadorExistente = listTeamController.JogadoresConfig.FirstOrDefault(j => j.authId == idAuth);

            bool jaTinha = false;

            if (jogadorExistente != null)
            {
                jogadorExistente.playerID = playerID;
                jogadorExistente.nickname = nick;

                jaTinha = true;
            }

            DebugServidor($"Players Conectados: {playersConnectados.value}, jogador adicionado. Ja tinha: {jaTinha}");
        }

        private IEnumerator EsperarPelaCondicao(Func<bool> condition)
        {
            while (!condition())
            {
                yield return waitForOneSecond;
            }
        }

        [ObserversRpc(requireServer: false, runLocally: true)]
        public void DebugServidor(string textDebug)
        {
            Debug.Log($"[Debug Server] {textDebug}");
        }

    }
}

