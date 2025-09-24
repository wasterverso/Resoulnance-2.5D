using PlayFlow;
using Resoulnance.Scene_Preparation.Inicialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resoulnance.Scene_Preparation.Dados
{
    public class LeituraDadosServer_Preparacao : MonoBehaviour
    {
        [SerializeField] PreparacaoStart_Server prepStartServer;

        public TiposDeSalas TipoDePartidaDoLobby()
        {
            var config = PlayFlowServerConfig.LoadConfig(Application.platform == RuntimePlatform.WindowsEditor);
            if (config == null || config.custom_data == null)
            {
                prepStartServer.DebugServidor("Config do PlayFlow ou custom_data não encontrados para ler o tipo de partida.");
                return TiposDeSalas.Nenhum;
            }

            // Tenta obter o 'gameMode' do 'lobby_settings' no nível superior (para partidas de matchmaking)
            if (config.custom_data.TryGetValue("lobby_settings", out object lobbySettingsObj))
            {
                if (lobbySettingsObj is Dictionary<string, object> lobbySettingsDict &&
                    lobbySettingsDict.TryGetValue("gameMode", out object gameModeObj))
                {
                    if (gameModeObj is string gameModeString && Enum.TryParse(gameModeString, out TiposDeSalas tipoPartidaEnum))
                    {
                        prepStartServer.DebugServidor($"Modo Matchmaking: Tipo de partida do lobby: {tipoPartidaEnum}");
                        return tipoPartidaEnum;
                    }
                }
            }

            // Se a tentativa acima falhar, tenta obter 'gameMode' diretamente de custom_data (para partidas personalizadas)
            if (config.custom_data.TryGetValue("gameMode", out object gameModeObjDirect))
            {
                if (gameModeObjDirect is string gameModeStringDirect && Enum.TryParse(gameModeStringDirect, out TiposDeSalas tipoPartidaEnumDirect))
                {
                    prepStartServer.DebugServidor($"Modo Personalizado: Tipo de partida do lobby: {tipoPartidaEnumDirect}");
                    return tipoPartidaEnumDirect;
                }
            }

            prepStartServer.DebugServidor("Nenhum tipo de partida encontrado no custom_data.");
            return TiposDeSalas.Nenhum;
        }

        public int QuantidadeDePlayersNoLobby()
        {
            var config = PlayFlowServerConfig.LoadConfig(Application.platform == RuntimePlatform.WindowsEditor);
            if (config == null || config.custom_data == null)
            {
                prepStartServer.DebugServidor("Config do PlayFlow ou custom_data não encontrados.");
                return 0;
            }

            // Tenta encontrar a chave 'all_players' (para partidas de matchmaking)
            if (config.custom_data.TryGetValue("all_players", out object allPlayersObj))
            {
                if (allPlayersObj is IEnumerable<object> allPlayersEnumerable)
                {
                    int quantJogadores = allPlayersEnumerable.Count();
                    prepStartServer.DebugServidor($"Modo Matchmaking: Quantidade de jogadores no lobby: {quantJogadores}");
                    return quantJogadores;
                }
            }

            // Se 'all_players' não existir, tenta a chave 'players' (para partidas personalizadas)
            if (config.custom_data.TryGetValue("players", out object playersObj))
            {
                if (playersObj is IEnumerable<object> playersEnumerable)
                {
                    int quantJogadores = playersEnumerable.Count();
                    prepStartServer.DebugServidor($"Modo Personalizado: Quantidade de jogadores no lobby: {quantJogadores}");
                    return quantJogadores;
                }
            }

            prepStartServer.DebugServidor("Nenhuma lista de jogadores encontrada no custom_data.");
            return 0;
        }

        void LeituraServerMatchmaking()
        {
            /*{
            "instance_id": "18128be3-8dd8-4e07-9b9b-5c5092b34e22",
            "region": "south-america-brazil",
            "custom_data": 
                {
                    "match_id": "match_1758250556122_1ecfsg2",
                    "lobby_ids": 
                        [
                        "e8845b6e-0969-421f-aef4-b2e7a4d38582",
                        "1630b7ba-c9e4-4c0a-b3e0-0b0ab9041e1d"
                        ],
                    "lobby_id": "e8845b6e-0969-421f-aef4-b2e7a4d38582",
                    "matchmaking_mode": "1v1",
                    "teams": [
                        {
                            "team_id": 0,
                            "lobbies": [
                                {
                                    "lobby_id": "e8845b6e-0969-421f-aef4-b2e7a4d38582",
                                    "lobby_name": "lobby_UpbeatImprovingBuzzard",
                                    "players": 
                                        [
                                            "M93lHJKGvKDDp3vXEpW4ER1ynXmv"
                                        ],
                                        "player_count": 1,
                                        "host": "M93lHJKGvKDDp3vXEpW4ER1ynXmv",
                                        "lobby_settings": 
                                            {
                                                "gameMode": "Normal1v1"
                                            },
                                        "player_states": 
                                            {
                                                "M93lHJKGvKDDp3vXEpW4ER1ynXmv": 
                                                    {
                                                        "avatar": "{\"acessoriosId\":0,\"cabelosId\":0,\"cabelosCor\":\"FFFFFFFF\",\"rostosId\":0,\"corpoId\":0,\"corpoCor\":\"FFFFFFFF\",\"roupasId\":0,\"pesId\":0,\"pesCor\":\"FFFFFFFF\"}",
                                                        "isReady": true,
                                                        "posicao": -1
                                                    }
                                            }
                                        }
                                    ]
                                },
                            {
                                "team_id": 1,
                                "lobbies": 
                                [
                                    {
                                        "lobby_id": "1630b7ba-c9e4-4c0a-b3e0-0b0ab9041e1d",
                                        "lobby_name": "lobby_SullenAlluringHusky",
                                        "players": 
                                            [
                                                "uzItAcNH9Pz1yIk9N1rcTzXtkptX"
                                            ],
                                        "player_count": 1,
                                        "host": "uzItAcNH9Pz1yIk9N1rcTzXtkptX",
                                        "lobby_settings": 
                                            {
                                                "gameMode": "Normal1v1"
                                            },
                                        "player_states": 
                                            {
                                                "uzItAcNH9Pz1yIk9N1rcTzXtkptX": 
                                            {
                                                "avatar": "{\"acessoriosId\":0,\"cabelosId\":0,\"cabelosCor\":\"FFFFFFFF\",\"rostosId\":0,\"corpoId\":0,\"corpoCor\":\"FFFFFFFF\",\"roupasId\":0,\"pesId\":0,\"pesCor\":\"FFFFFFFF\"}",
                                                "isReady": true,
                                                "posicao": -1
                                            }
                                        }
                                    }
                                ]
                            }
                        ],
                    "all_players": 
                        [
                            "M93lHJKGvKDDp3vXEpW4ER1ynXmv",
                            "uzItAcNH9Pz1yIk9N1rcTzXtkptX"
                        ],
                    "match_configuration": 
                        {
                            "teams_count": 2,
                            "players_per_team": 1,
                            "min_players_per_team": 1,
                            "timeout": 300
                        },
                    "lobby_settings": 
                            {
                                "gameMode": "Normal1v1"
                            }
                        }
                    }
            */
        }
    }
}

