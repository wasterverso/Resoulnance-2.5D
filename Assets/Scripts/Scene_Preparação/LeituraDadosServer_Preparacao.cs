using Newtonsoft.Json.Linq;
using PlayFlow;
using Resoulnance.Scene_Preparation.Inicialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resoulnance.Scene_Preparation.Dados
{
    [Serializable]
    public class LobbySettings
    {
        public bool OnStart;
        public string gameMode;
    }

    public class LeituraDadosServer_Preparacao : MonoBehaviour
    {
        [SerializeField] PreparacaoStart_Server prepStartServer;

        public TiposDeSalas TipoDePartidaDoLobby()
        {
            PlayFlowServerConfig config = PlayFlowServerConfig.LoadConfig(Application.platform == RuntimePlatform.WindowsEditor);
            if (config == null)
            {
                prepStartServer.DebugServidor("Config do PlayFlow ou custom_data não encontrados para ler o tipo de partida.");
                return TiposDeSalas.Nenhum;
            }

            JObject lobbyConfig = config.GetLobbySettings();

            if (lobbyConfig.TryGetValue("gameMode", out var gameModeToken))
            {
                string gameModeString = gameModeToken.ToString();
                if (Enum.TryParse(gameModeString, out TiposDeSalas tipoPartidaEnum))
                {
                    prepStartServer.DebugServidor($"Tipo de partida (matchmaking): {tipoPartidaEnum}");
                    return tipoPartidaEnum;
                }
            }

            prepStartServer.DebugServidor("GameMode não encontrado em custom_data.lobby_settings.");
            return TiposDeSalas.Nenhum;
        }

        public int QuantidadeDePlayersNoLobby()
        {
            var listTeamController = ListTeamController.Instance;
            listTeamController.JogadoresConfig.Clear();

            var config = PlayFlowServerConfig.LoadConfig(Application.platform == RuntimePlatform.WindowsEditor);
            if (config == null)
            {
                prepStartServer.DebugServidor("Config do PlayFlow ou custom_data não encontrados.");
                return 0;
            }

            JArray teams = config.GetTeams();
            int quantPlayers = 0;

            foreach (JObject team in teams)
            {
                int teamId = team.Value<int>("team_id"); // 0 = azul, 1 = vermelho
                JArray lobbies = team.Value<JArray>("lobbies");

                foreach (JObject lobby in lobbies)
                {
                    JObject playerStates = lobby.Value<JObject>("player_states");
                    foreach (var playerProperty in playerStates.Properties())
                    {
                        JObject playerToken = (JObject)playerProperty.Value;

                        var novoPlayer = new Jogador();
                        novoPlayer.authId = playerToken.Value<string>("idAuth");
                        novoPlayer.nickname = playerToken.Value<string>("nickname");

                        if (teamId == 0)
                            novoPlayer.team = Team.Blue;
                        else if (teamId == 1)
                            novoPlayer.team = Team.Red;

                        listTeamController.JogadoresConfig.Add(novoPlayer);
                        quantPlayers++;
                    }
                }
            }

            prepStartServer.DebugServidor($"Players encontrados{quantPlayers}");
            return quantPlayers;
        }
    }
}

