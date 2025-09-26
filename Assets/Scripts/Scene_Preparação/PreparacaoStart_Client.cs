using PurrNet;
using PurrNet.Modules;
using Resoulnance.Player;
using Resoulnance.Scene_Preparation.Controles;
using System;
using UnityEngine;

namespace Resoulnance.Scene_Preparation.Inicialize
{
    public class PreparacaoStart_Client : NetworkBehaviour
    {
        [SerializeField] PreparacaoStart_Server prepStartServer;
        [SerializeField] PreparacaoController prepController;

        protected override void OnSpawned()
        {
            if (isServer && ListTeamController.Instance.networkMode == NetworkMode.Host)
            {
                networkManager.onLocalPlayerReceivedID += ChamarInfos;
            }
            else
            {
                ChamarInfos(default);
            }
        }

        void ChamarInfos(PlayerID player)
        {
            PlayerID meuId = NetworkManager.main.localPlayer;

            if (meuId.id != 0)
            {
                var playerConfig = PlayerConfigData.Instance;
                var nick = playerConfig.Nickname;
                var idAuth = playerConfig.idAuth;

                ListTeamController.Instance.meuId = meuId.id;

                prepStartServer.PlayerPronto_ServerRpc(meuId, nick, idAuth);
                prepController.IniciarAtualizacoes();
            }
        }

        protected override void OnDespawned()
        {
            if (isServer && ListTeamController.Instance.networkMode == NetworkMode.Host)
            {
                networkManager.onLocalPlayerReceivedID -= ChamarInfos;
            }
        }
    }
}

