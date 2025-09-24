using PurrNet;
using PurrNet.Prediction;
using Resoulnance.Cartas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resoulnance.Scene_Arena.Config
{
    public class PlayerSpawn_Arena : StatelessPredictedIdentity
    {
        [SerializeField] ArenaReferences arenaReferences;
        [SerializeField] GameStart_Inicial gameStartInicial;
        [SerializeField] GameStart_Treinamento gameStartTreinamento;
        [SerializeField] Transform spawnPoint;
        ListTeamController listTeamController;

        readonly Dictionary<PlayerID, PredictedObjectID> _players = new();

        protected override void LateAwake()
        {
            _players.Clear();

            predictionManager.players.onPlayerAdded += IniciarSpawn;
        }

        public void IniciarSpawn(PlayerID playerId)
        {
            if (_players.ContainsKey(playerId))
                return;

            if (listTeamController == null)
            {
                listTeamController = ListTeamController.Instance;
            }

            GameObject _playerPrefab = null;

            if (gameStartTreinamento.EstaTestandoNaCena() || listTeamController.tipoSalaAtual == TiposDeSalas.Treinamento)
            {
                _playerPrefab = gameStartTreinamento.GetPersonagem().prefabPersonagem;
            }
            else
            {
                Jogador jogador = listTeamController.JogadoresConfig.FirstOrDefault(c => c.playerID.id == playerId.id);
                _playerPrefab = arenaReferences.flyerData.personagens.FirstOrDefault(c => c.id == jogador.idFlyer).prefabPersonagem;
            }

            PredictedObjectID? newPlayer;

            newPlayer = predictionManager.hierarchy.Create(_playerPrefab, spawnPoint.position, Quaternion.identity, owner: playerId);

            if (!newPlayer.HasValue)
                return;

            _players[playerId] = newPlayer.Value;
            predictionManager.SetOwnership(newPlayer, playerId);
        }
    }
}

