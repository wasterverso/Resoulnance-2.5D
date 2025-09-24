using PurrNet;
using PurrNet.Transports;
using UnityEngine;


namespace Resoulnance.Scene_Arena
{
    public class GameStart_Client : NetworkBehaviour
    {
        ListTeamController listTeamController;

        public void IniciarMultiplayerClient()
        {
            listTeamController = ListTeamController.Instance;

            if (listTeamController.port != 0 && !string.IsNullOrEmpty(listTeamController.address))
            {
                UDPTransport udpTransport = NetworkManager.main.GetComponent<UDPTransport>();
                if (udpTransport != null)
                {
                    udpTransport.address = listTeamController.address;
                    udpTransport.serverPort = (ushort)listTeamController.port;
                    udpTransport.StartClient();
                }
            }
        }


    }
}

