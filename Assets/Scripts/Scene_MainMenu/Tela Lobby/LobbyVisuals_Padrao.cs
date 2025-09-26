using PlayFlow;
using Resoulnance.AvatarCustomization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resoulnance.Telas.TelaLobby
{
    public class LobbyVisuals_Padrao : MonoBehaviour
    {
        [Header("Refs Script")]
        [SerializeField] Start_LobbyController startLobbyController;
        PlayFlowLobbyManagerV2 pfLobbyManager;

        [Header("Refs UI")]
        [SerializeField] GameObject padraoPainel;
        [SerializeField] Button iniciarPartida_btn;

        [Header("Lista")]
        [SerializeField] List<PlayerLobbyObj> listaPlayersLobbyObj = new List<PlayerLobbyObj>();

        public void EntrouNoLobby()
        {
            pfLobbyManager = PlayFlowLobbyManagerV2.Instance;
            pfLobbyManager.Events.OnLobbyUpdated.AddListener(AtualizouLobby);

            iniciarPartida_btn.onClick.RemoveAllListeners();
            iniciarPartida_btn.onClick.AddListener(() => IniciarPartida());
            padraoPainel.SetActive(true);
        }

        void AtualizouLobby(Lobby lobby)
        {
            // Primeiro, desativa tudo
            foreach (var playerObj in listaPlayersLobbyObj)
            {
                playerObj.obj.SetActive(false);
                playerObj.isHostObj.SetActive(false);
            }

            for (int i = 0; i < lobby.currentPlayers; i++)
            {
                var playerId = lobby.players[i]; // ID do jogador
                listaPlayersLobbyObj[i].idPlayer = playerId;
                listaPlayersLobbyObj[i].obj.SetActive(true);
                listaPlayersLobbyObj[i].isHostObj.SetActive(lobby.host == playerId);

                if (lobby.lobbyStateRealTime.TryGetValue(playerId, out var dadosPlayer))
                {
                    var state = dadosPlayer as Dictionary<string, object>;
                    if (state.TryGetValue("isReady", out var readyObj))
                    {
                        bool isReady = Convert.ToBoolean(readyObj);
                        if (!isReady)
                            listaPlayersLobbyObj[i].avisoObj.SetActive(true);
                        else
                            listaPlayersLobbyObj[i].avisoObj.SetActive(false);
                    }

                    if (state.TryGetValue("avatar", out var avatarObj))
                    {
                        string avatarJson = avatarObj.ToString();

                        AvatarCustom playerCustom = ListAvatarCustom.Instance.DesserializarAvatarCustom(avatarJson);

                        listaPlayersLobbyObj[i].avatarCustom.MostrarAvatar(true);
                        listaPlayersLobbyObj[i].avatarCustom.AtribuirDados(playerCustom);
                    }
                }
            }
        }

        void IniciarPartida()
        {
            startLobbyController.IniciarPartida();
        }

        public void DesativarInscricoes()
        {
            pfLobbyManager.Events.OnLobbyUpdated.RemoveListener(AtualizouLobby);
            padraoPainel.SetActive(false);
        }

        [System.Serializable]
        class PlayerLobbyObj
        {
            public string idPlayer;
            public GameObject obj;
            public Text nick;
            public GameObject isHostObj;
            public AtribuirDados_AvatarCustom avatarCustom;
            public GameObject avisoObj;
        }
    }
}

